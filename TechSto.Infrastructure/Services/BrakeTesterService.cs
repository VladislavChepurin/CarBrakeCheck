using System.IO.Ports;
using Microsoft.Extensions.Logging;
using TechSto.Core.Interfaces;
using TechSto.Core.Models;

namespace TechSto.Infrastructure.Services
{
    public class BrakeTesterService : IBrakeTesterService, IDisposable
    {
        private readonly ILogger<BrakeTesterService> _logger;
        private readonly SemaphoreSlim _ioLock = new(1, 1);

        private SerialPort? _port;
        private CancellationTokenSource? _readCts;
        private Task? _readTask;
        private bool _disposed;

        private const byte CMD_INIT = 0xA5;
        private const byte READ_CMD_CALIBRATION = 0x47;

        private const int BAUD_RATE = 410800;
        private const int MEASUREMENT_SIZE = 18;
        private const int CALIBRATION_SIZE = 32;
        private const int TIMEOUT = 1000;
        private const int AVAILABILITY_CHECK_COUNT = 5;

        public bool IsConnected => _port?.IsOpen == true;

        public event EventHandler<DeviceConnectionEventArgs>? ConnectionStateChanged;
        public event EventHandler<MeasurementReceivedEventArgs>? MeasurementReceived;
        public event EventHandler<DeviceErrorEventArgs>? ErrorOccurred;

        public BrakeTesterService(ILogger<BrakeTesterService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> ConnectAsync(string portName, CancellationToken ct = default)
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(portName))
                throw new ArgumentException("Port name is required.", nameof(portName));

            try
            {
                if (IsConnected)
                    return true;

                var port = CreatePort(portName);

                try
                {
                    port.Open();
                    _port = port;

                    await InitializeAsync(ct).ConfigureAwait(false);

                    _logger.LogInformation("FT232 connected on {Port}", portName);
                    OnConnectionChanged(true, portName);

                    return true;
                }
                catch
                {
                    try
                    {
                        if (port.IsOpen)
                            port.Close();
                    }
                    catch (Exception closeEx)
                    {
                        _logger.LogWarning(closeEx, "Error while closing port after failed initialization");
                    }

                    port.Dispose();

                    if (ReferenceEquals(_port, port))
                        _port = null;

                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Connection failed");
                RaiseError("Connection error", ex, ErrorSeverity.Error);
                return false;
            }
        }

        public async Task<bool> CheckAvailabilityAsync(string portName, CancellationToken ct = default)
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(portName))
                return false;

            try
            {
                using var probePort = CreatePort(portName);
                probePort.Open();

                for (int attempt = 0; attempt < AVAILABILITY_CHECK_COUNT; attempt++)
                {
                    ct.ThrowIfCancellationRequested();

                    byte[] commandBuffer = { CMD_INIT };
                    probePort.Write(commandBuffer, 0, 1);

                    var responseBuffer = new byte[MEASUREMENT_SIZE];
                    await ReadExactInternalAsync(probePort, responseBuffer, TIMEOUT, ct).ConfigureAwait(false);

                    if (!ValidateChecksum(responseBuffer))
                    {
                        _logger.LogWarning(
                            "Availability check failed on attempt {Attempt}: invalid checksum",
                            attempt + 1);

                        return false;
                    }
                }

                return true;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Availability check failed for port {Port}", portName);
                return false;
            }
        }

        public async Task<bool> RequestMeasurementAsync(CancellationToken ct = default)
        {
            ThrowIfDisposed();

            if (!IsConnected)
                throw new InvalidOperationException("Device not connected");

            try
            {
                var data = await ExchangeAsync(CMD_INIT, MEASUREMENT_SIZE, TIMEOUT, ct).ConfigureAwait(false);

                if (!ValidateChecksum(data))
                {
                    _logger.LogWarning("Invalid checksum in measurement");
                    return false;
                }

                var measurement = ParseMeasurement(data);
                OnMeasurement(measurement, data);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Measurement request failed");
                RaiseError("Measurement error", ex, ErrorSeverity.Error);
                return false;
            }
        }

        public Task StartContinuousReadingAsync(CancellationToken ct = default)
        {
            ThrowIfDisposed();

            if (!IsConnected)
                throw new InvalidOperationException("Device not connected");

            StopContinuousReading();

            _readCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            _readTask = Task.Run(() => ReadLoopAsync(_readCts.Token), _readCts.Token);

            _logger.LogInformation("Continuous reading started");
            return Task.CompletedTask;
        }

        public void StopContinuousReading()
        {
            if (_readCts == null)
                return;

            _readCts.Cancel();

            try
            {
                _readTask?.GetAwaiter().GetResult();
            }
            catch
            {
            }

            _readCts.Dispose();
            _readCts = null;
            _readTask = null;

            _logger.LogInformation("Continuous reading stopped");
        }

        public async Task<CalibrationData> RequestCalibrationDataAsync(CancellationToken ct = default)
        {
            ThrowIfDisposed();

            var data = await ExchangeAsync(READ_CMD_CALIBRATION, CALIBRATION_SIZE, TIMEOUT, ct).ConfigureAwait(false);

            return new CalibrationData
            {
                RawData = data,
                IsValid = true
            };
        }

        public async Task DisconnectAsync(CancellationToken ct = default)
        {
            ThrowIfDisposed();

            StopContinuousReading();

            if (_port != null)
            {
                var port = _port;
                _port = null;

                await Task.Run(() =>
                {
                    if (port.IsOpen)
                        port.Close();

                    port.Dispose();
                }, ct).ConfigureAwait(false);
            }

            OnConnectionChanged(false, null);
        }

        private async Task InitializeAsync(CancellationToken ct)
        {
            var data = await ExchangeAsync(CMD_INIT, MEASUREMENT_SIZE, TIMEOUT, ct).ConfigureAwait(false);

            if (!ValidateChecksum(data))
                throw new InvalidOperationException("Invalid init checksum");

            var measurement = ParseMeasurement(data);
            OnMeasurement(measurement, data);

            _logger.LogInformation("Device initialized");
        }

        private async Task<byte[]> ExchangeAsync(byte command, int responseSize, int timeoutMs, CancellationToken ct)
        {
            await _ioLock.WaitAsync(ct).ConfigureAwait(false);

            try
            {
                var port = _port ?? throw new InvalidOperationException("Device is not connected");

                byte[] commandBuffer = { command };
                port.Write(commandBuffer, 0, 1);

                var responseBuffer = new byte[responseSize];
                await ReadExactInternalAsync(port, responseBuffer, timeoutMs, ct).ConfigureAwait(false);

                return responseBuffer;
            }
            finally
            {
                _ioLock.Release();
            }
        }

        private async Task ReadLoopAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var buffer = await ExchangeAsync(CMD_INIT, MEASUREMENT_SIZE, TIMEOUT, ct).ConfigureAwait(false);

                    if (!ValidateChecksum(buffer))
                    {
                        _logger.LogWarning("Invalid checksum in continuous read");
                        continue;
                    }

                    var measurement = ParseMeasurement(buffer);
                    OnMeasurement(measurement, buffer);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Read loop error");
                    RaiseError("Read error", ex, ErrorSeverity.Error);

                    await Task.Delay(200, ct).ConfigureAwait(false);
                }
            }
        }

        private static SerialPort CreatePort(string portName)
        {
            return new SerialPort(portName, BAUD_RATE, Parity.None, 8, StopBits.One)
            {
                ReadTimeout = TIMEOUT,
                WriteTimeout = TIMEOUT
            };
        }

        private static async Task ReadExactInternalAsync(SerialPort port, byte[] buffer, int timeoutMs, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(buffer);

            await Task.Run(() =>
            {
                int offset = 0;
                int originalReadTimeout = port.ReadTimeout;

                try
                {
                    port.ReadTimeout = timeoutMs;

                    while (offset < buffer.Length)
                    {
                        ct.ThrowIfCancellationRequested();

                        int read = port.Read(buffer, offset, buffer.Length - offset);

                        if (read == 0)
                            throw new IOException("Device disconnected");

                        offset += read;
                    }
                }
                catch (TimeoutException)
                {
                    throw new TimeoutException($"Device not responding after {timeoutMs}ms");
                }
                finally
                {
                    port.ReadTimeout = originalReadTimeout;
                }
            }, ct).ConfigureAwait(false);
        }

        private bool ValidateChecksum(byte[] data)
        {
            uint sum = 0;

            for (int i = 0; i < 14; i++)
                sum += data[i];

            byte rx16 = data[16];
            byte rx17 = data[17];

            byte calc17 = 0;
            uint dat = sum;

            while (dat >= 256)
            {
                dat -= 255;
                dat -= 1;
                calc17++;
            }

            byte calc16 = (byte)dat;

            return rx16 == calc16 && rx17 == calc17;
        }

        private BrakeMeasurement ParseMeasurement(byte[] data)
        {
            return new BrakeMeasurement
            {
                RightWeight1 = data[0],
                RightWeight2 = data[1],
                LeftWeight1 = data[2],
                LeftWeight2 = data[3],
                PedalPressure = data[4],
                RightBrakeSensor = data[5] | (data[6] << 8),
                LeftBrakeSensor = data[7] | (data[8] << 8),
                DiscreteSignals = ParseDiscrete(data[13]),
                Timestamp = DateTime.Now,
                IsValid = true
            };
        }

        private DiscreteSignals ParseDiscrete(byte value)
        {
            return new DiscreteSignals
            {
                PedalConnected = (value & 0x80) != 0,
                LeftRunOverSensor = (value & 0x40) != 0,
                RightRunOverSensor = (value & 0x20) != 0,
                LeftSlipSensor = (value & 0x04) != 0,
                RightSlipSensor = (value & 0x02) != 0
            };
        }

        private void OnConnectionChanged(bool connected, string? port)
        {
            ConnectionStateChanged?.Invoke(this, new DeviceConnectionEventArgs
            {
                IsConnected = connected,
                PortName = port ?? string.Empty,
                Timestamp = DateTime.Now
            });
        }

        private void OnMeasurement(BrakeMeasurement measurement, byte[] raw)
        {
            MeasurementReceived?.Invoke(this, new MeasurementReceivedEventArgs
            {
                Measurement = measurement,
                RawData = raw,
                Timestamp = DateTime.Now
            });
        }

        private void RaiseError(string msg, Exception ex, ErrorSeverity severity)
        {
            ErrorOccurred?.Invoke(this, new DeviceErrorEventArgs
            {
                ErrorMessage = msg,
                Exception = ex,
                Severity = severity,
                Timestamp = DateTime.Now
            });
        }

        private void ThrowIfDisposed()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            StopContinuousReading();

            if (_port != null)
            {
                try
                {
                    if (_port.IsOpen)
                        _port.Close();
                }
                catch
                {
                }

                _port.Dispose();
                _port = null;
            }

            _ioLock.Dispose();
            _disposed = true;
        }
    }
}