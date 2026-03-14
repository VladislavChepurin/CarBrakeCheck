using System.Collections.ObjectModel;
using System.Windows.Input;
using TechSto.Core.Interfaces;
using TechSto.Core.Models;
using TechSto.WPF.Services;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace TechSto.WPF.ViewModels
{
    public class MeasurementsViewModel : ViewModelBase, IDisposable
    {
        private readonly IBrakeTesterService _device;
        private readonly LocalizationProvider _localization;
        private bool _isConnected;
        private string _connectionStatus;
        private ObservableCollection<BrakeMeasurement> _measurements = new();
        private BrakeMeasurement _lastMeasurement;
        private bool _isContinuousReading;
        private string _currentPort;

        // Свойства для привязки в UI
        public bool IsConnected
        {
            get => _isConnected;
            private set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    OnPropertyChanged();
                    UpdateConnectionStatus();
                }
            }
        }

        public string ConnectionStatus
        {
            get => _connectionStatus;
            private set => SetProperty(ref _connectionStatus, value);
        }

        public ObservableCollection<BrakeMeasurement> Measurements
        {
            get => _measurements;
            private set => SetProperty(ref _measurements, value);
        }

        public BrakeMeasurement LastMeasurement
        {
            get => _lastMeasurement;
            private set => SetProperty(ref _lastMeasurement, value);
        }

        public bool IsContinuousReading
        {
            get => _isContinuousReading;
            private set => SetProperty(ref _isContinuousReading, value);
        }

        public string CurrentPort
        {
            get => _currentPort;
            set
            {
                if (SetProperty(ref _currentPort, value))
                {
                    // При изменении порта обновляем состояние команды Connect
                    (ConnectCommand as AsyncRelayCommand<string>)?.NotifyCanExecuteChanged();
                }
            }
        }

        // Команды
        public ICommand ConnectCommand { get; }
        public ICommand DisconnectCommand { get; }
        public ICommand SingleMeasurementCommand { get; }
        public ICommand StartContinuousCommand { get; }
        public ICommand StopContinuousCommand { get; }
        public ICommand ClearMeasurementsCommand { get; }

        public MeasurementsViewModel(IBrakeTesterService brakeTester, LocalizationProvider localization)
        {
            _device = brakeTester;
            _localization = localization;

            // Инициализация команд с проверкой возможности выполнения
            ConnectCommand = new AsyncRelayCommand<string>(
                ExecuteConnectAsync,
                port => !IsConnected && !string.IsNullOrWhiteSpace(port)
            );

            DisconnectCommand = new AsyncRelayCommand(
                ExecuteDisconnectAsync,
                () => IsConnected
            );

            SingleMeasurementCommand = new AsyncRelayCommand(
                ExecuteSingleMeasurementAsync,
                () => IsConnected
            );

            StartContinuousCommand = new AsyncRelayCommand(
                ExecuteStartContinuousAsync,
                () => IsConnected && !IsContinuousReading
            );

            StopContinuousCommand = new RelayCommand(
                ExecuteStopContinuous,
                () => IsConnected && IsContinuousReading
            );

            ClearMeasurementsCommand = new RelayCommand(ClearMeasurements);

            // Подписка на события устройства
            _device.ConnectionStateChanged += OnConnectionStateChanged;
            _device.MeasurementReceived += OnMeasurementReceived;
            _device.ErrorOccurred += OnDeviceError;

            // Первоначальная проверка состояния
            _isConnected = _device.IsConnected;
            UpdateConnectionStatus();
        }

        #region Выполнение команд

        private async Task ExecuteConnectAsync(string port)
        {
            try
            {
                var success = await _device.ConnectAsync(port);
                if (!success)
                {
                    MessageBox.Show(
                        _localization["ConnectionFailed"],
                        _localization["ErrorTitle"],
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{_localization["ConnectionError"]}: {ex.Message}",
                    _localization["ErrorTitle"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async Task ExecuteDisconnectAsync()
        {
            try
            {
                await _device.DisconnectAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{_localization["DisconnectError"]}: {ex.Message}",
                    _localization["ErrorTitle"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async Task ExecuteSingleMeasurementAsync()
        {
            try
            {
                await _device.RequestMeasurementAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{_localization["MeasurementError"]}: {ex.Message}",
                    _localization["ErrorTitle"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async Task ExecuteStartContinuousAsync()
        {
            try
            {
                await _device.StartContinuousReadingAsync();
                IsContinuousReading = true;

                // Обновляем состояние команд
                (StartContinuousCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
                (StopContinuousCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{_localization["StartError"]}: {ex.Message}",
                    _localization["ErrorTitle"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ExecuteStopContinuous()
        {
            try
            {
                _device.StopContinuousReading();
                IsContinuousReading = false;

                // Обновляем состояние команд
                (StartContinuousCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
                (StopContinuousCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{_localization["StopError"]}: {ex.Message}",
                    _localization["ErrorTitle"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ClearMeasurements()
        {
            Measurements.Clear();
            LastMeasurement = null;
        }

        #endregion

        #region Обработчики событий устройства

        private void OnConnectionStateChanged(object sender, DeviceConnectionEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IsConnected = e.IsConnected;

                // При изменении состояния подключения сбрасываем режим чтения
                if (!IsConnected)
                {
                    IsContinuousReading = false;
                }

                // Команды обновятся автоматически благодаря вызову OnPropertyChanged для IsConnected
                // и привязанным к ним лямбда-выражениям
            });
        }

        private void OnMeasurementReceived(object sender, MeasurementReceivedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                LastMeasurement = e.Measurement;
                Measurements.Add(e.Measurement);

                // Ограничиваем количество записей
                if (Measurements.Count > 100)
                {
                    Measurements.RemoveAt(0);
                }
            });
        }

        private void OnDeviceError(object sender, DeviceErrorEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(
                    e.ErrorMessage,
                    _localization["DeviceError"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            });
        }

        #endregion

        #region Вспомогательные методы

        private void UpdateConnectionStatus()
        {
            ConnectionStatus = IsConnected
                ? _localization["DeviceConnected"]
                : _localization["DeviceDisconnected"];
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            _device.ConnectionStateChanged -= OnConnectionStateChanged;
            _device.MeasurementReceived -= OnMeasurementReceived;
            _device.ErrorOccurred -= OnDeviceError;
        }

        #endregion
    }
}