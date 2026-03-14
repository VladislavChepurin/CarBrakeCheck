using System.IO.Ports;
using Microsoft.Extensions.Logging;
using TechSto.Core.Interfaces;
using TechSto.Core.Models;

namespace TechSto.Infrastructure.Services;

public class Ft232BrakeTesterService : IBrakeTesterService, IDisposable
{
    private readonly ILogger<Ft232BrakeTesterService> _logger;

    private SerialPort _port;
    private CancellationTokenSource _readCts;
    private Task _readTask;

    private readonly SemaphoreSlim _ioLock = new(1, 1);

    private bool _disposed;

    private const byte CMD_INIT = 0xA5;
    private const byte CMD_CALIBRATION = 0x47;

    private const int MEASUREMENT_SIZE = 18;
    private const int CALIBRATION_SIZE = 32;

    public bool IsConnected => _port?.IsOpen == true;

    public event EventHandler<DeviceConnectionEventArgs> ConnectionStateChanged;
    public event EventHandler<MeasurementReceivedEventArgs> MeasurementReceived;
    public event EventHandler<DeviceErrorEventArgs> ErrorOccurred;

    public Ft232BrakeTesterService(ILogger<Ft232BrakeTesterService> logger)
    {
        _logger = logger;
    }

    #region CONNECT

    public async Task<bool> ConnectAsync(string portName, CancellationToken ct = default)
    {
        try
        {
            if (IsConnected)
                return true;

            _port = new SerialPort(portName, 460800, Parity.None, 8, StopBits.One)
            {
                ReadTimeout = 1000,
                WriteTimeout = 1000
            };

            _port.Open();

            _logger.LogInformation("FT232 connected on {port}", portName);

            OnConnectionChanged(true, portName);

            await InitializeAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Connection failed");

            RaiseError("Connection error", ex, ErrorSeverity.Error);

            return false;
        }
    }

    private async Task InitializeAsync(CancellationToken ct)
    {
        await SendCommandAsync(CMD_INIT, ct);

        var data = await ReadExactAsync(MEASUREMENT_SIZE, ct);

        if (!ValidateChecksum(data))
            throw new Exception("Invalid init checksum");

        var measurement = ParseMeasurement(data);

        OnMeasurement(measurement, data);

        _logger.LogInformation("Device initialized");
    }

    #endregion

    #region CONTINUOUS READING

    public async Task<bool> RequestMeasurementAsync(CancellationToken ct = default) 
    {
        if (!IsConnected) throw new InvalidOperationException("Device not connected");
        try 
        {
            await SendCommandAsync(CMD_INIT, ct);
            var data = await ReadExactAsync(MEASUREMENT_SIZE, ct);
            if (!ValidateChecksum(data))
            {
                _logger.LogWarning("Invalid checksum in measurement");
                return false; 
            } 
            var measurement = ParseMeasurement(data);
            OnMeasurement(measurement, data);
            return true; } catch (Exception ex) { _logger.LogError(ex, "Measurement request failed");
            RaiseError("Measurement error", ex, ErrorSeverity.Error); return false; 
        }
    }

    public Task StartContinuousReadingAsync(CancellationToken ct = default)
    {
        if (!IsConnected)
            throw new InvalidOperationException("Device not connected");

        _readCts = CancellationTokenSource.CreateLinkedTokenSource(ct);

        _readTask = Task.Run(() => ReadLoopAsync(_readCts.Token));

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
            _readTask?.Wait();
        }
        catch { }

        _readCts.Dispose();
        _readCts = null;

        _logger.LogInformation("Continuous reading stopped");
    }

    private async Task ReadLoopAsync(CancellationToken ct)
    {
        var buffer = new byte[MEASUREMENT_SIZE];

        while (!ct.IsCancellationRequested)
        {
            try
            {
                await SendCommandAsync(CMD_INIT, ct);

                await ReadExactAsync(buffer, ct);

                if (!ValidateChecksum(buffer))
                    continue;

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

                await Task.Delay(200, ct);
            }
        }
    }

    #endregion

    #region IO

    private async Task SendCommandAsync(byte command, CancellationToken ct)
    {
        await _ioLock.WaitAsync(ct);

        try
        {
            var data = new[] { command };

            await _port.BaseStream.WriteAsync(data, 0, 1, ct);
        }
        finally
        {
            _ioLock.Release();
        }
    }

    private async Task<byte[]> ReadExactAsync(int size, CancellationToken ct)
    {
        var buffer = new byte[size];

        await ReadExactAsync(buffer, ct);

        return buffer;
    }

    private async Task ReadExactAsync(byte[] buffer, CancellationToken ct)
    {
        int offset = 0;

        while (offset < buffer.Length)
        {
            int read = await _port.BaseStream.ReadAsync(
                buffer,
                offset,
                buffer.Length - offset,
                ct);

            if (read == 0)
                throw new IOException("Device disconnected");

            offset += read;
        }
    }

    #endregion

    #region CHECKSUM

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

    #endregion

    #region PARSE

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

    #endregion

    #region CALIBRATION

    public async Task<CalibrationData> RequestCalibrationDataAsync(CancellationToken ct = default)
    {
        await SendCommandAsync(CMD_CALIBRATION, ct);

        var data = await ReadExactAsync(CALIBRATION_SIZE, ct);

        return new CalibrationData
        {
            RawData = data,
            IsValid = true
        };
    }

    #endregion

    #region DISCONNECT

    public async Task DisconnectAsync(CancellationToken ct = default)
    {
        StopContinuousReading();

        if (_port != null)
        {
            await Task.Run(() => _port.Close(), ct);

            _port.Dispose();
            _port = null;
        }

        OnConnectionChanged(false, null);
    }

    #endregion

    #region EVENTS

    private void OnConnectionChanged(bool connected, string port)
    {
        ConnectionStateChanged?.Invoke(this,
            new DeviceConnectionEventArgs
            {
                IsConnected = connected,
                PortName = port,
                Timestamp = DateTime.Now
            });
    }

    private void OnMeasurement(BrakeMeasurement measurement, byte[] raw)
    {
        MeasurementReceived?.Invoke(this,
            new MeasurementReceivedEventArgs
            {
                Measurement = measurement,
                RawData = raw,
                Timestamp = DateTime.Now
            });
    }

    private void RaiseError(string msg, Exception ex, ErrorSeverity severity)
    {
        ErrorOccurred?.Invoke(this,
            new DeviceErrorEventArgs
            {
                ErrorMessage = msg,
                Exception = ex,
                Severity = severity,
                Timestamp = DateTime.Now
            });
    }



    #endregion

    #region DISPOSE

    public void Dispose()
    {
        if (_disposed)
            return;

        StopContinuousReading();

        _port?.Dispose();

        _ioLock.Dispose();

        _disposed = true;
    }

    #endregion
}