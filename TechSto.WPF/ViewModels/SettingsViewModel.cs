using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TechSto.Core.DTOs;
using TechSto.Core.Interfaces;
using TechSto.Core.Messages;
using TechSto.Core.Models;
using TechSto.WPF.Services;

namespace TechSto.WPF.ViewModels
{
    public class SettingsViewModel : ViewModelBase, IDisposable
    {
        private readonly IAppSettingsService _appSettingsService;
        private readonly ILocalizationService _localizationService;
        private readonly IClientRecordService _clientRecordService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessageBus _messageBus;
        private readonly ISerialPortDiscoveryService _serialPortDiscoveryService;
        private readonly IBrakeTesterService _brakeTesterService;
        private readonly System.Timers.Timer _searchTimer;

        private CancellationTokenSource? _availabilityCts;

        private AppSettings _settings = new();
        private ClientRecordDto? _selectedClientRecord;

        private ObservableCollection<ClientRecordDto> _clientRecords = [];
        private ObservableCollection<ClientRecordDto> _allClientRecords = [];
        private List<ClientRecordDto> _filteredList = [];

        private ObservableCollection<SerialPortInfo> _availablePorts = [];
        private SerialPortInfo? _selectedPort;

        private ObservableCollection<string> _axleItems = [];
        private string? _selectedAxle;

        private string _selectedLanguage = string.Empty;
        private string _searchText = string.Empty;

        private bool _isRelativeDifference;
        private bool _selectedMeasurementMode;
        private bool _isDeviceAvailable;
        private MeasurementType _selectedMeasurementType = MeasurementType.EntryDrying;

        private RelayCommand? _addClientCommand;
        private RelayCommand? _editClientCommand;
        private RelayCommand? _deleteClientCommand;
        private RelayCommand? _allCheckClientCommand;
        private RelayCommand? _startCommand;
        private RelayCommand? _manualModeCommand;
        private RelayCommand? _autoModeCommand;

        public SettingsViewModel(
            IAppSettingsService appSettingsService,
            ILocalizationService localizationService,
            IClientRecordService clientRecordService,
            IServiceProvider serviceProvider,
            LocalizationProvider localizationProvider,
            IMessageBus messageBus,
            ISerialPortDiscoveryService serialPortDiscoveryService,
            IBrakeTesterService brakeTesterService)
        {
            _appSettingsService = appSettingsService;
            _localizationService = localizationService;
            _clientRecordService = clientRecordService;
            _serviceProvider = serviceProvider;
            _messageBus = messageBus;
            _serialPortDiscoveryService = serialPortDiscoveryService;
            _brakeTesterService = brakeTesterService;

            LocalizationProvider = localizationProvider;

            SettingsModel = _appSettingsService.Load();

            _selectedLanguage = SettingsModel.Language;
            _localizationService.SetLanguage(SettingsModel.Language);

            _searchTimer = new System.Timers.Timer(300)
            {
                AutoReset = false
            };
            _searchTimer.Elapsed += (_, _) =>
            {
                Application.Current.Dispatcher.Invoke(ApplyFilter);
            };

            LoadPorts();
            LoadData();

            _ = CheckSelectedPortAvailabilityAsync();
        }

        public LocalizationProvider LocalizationProvider { get; }

        public AppSettings SettingsModel
        {
            get => _settings;
            private set => SetProperty(ref _settings, value);
        }

        public ObservableCollection<ClientRecordDto> ClientRecords
        {
            get => _clientRecords;
            set => SetProperty(ref _clientRecords, value);
        }

        public ObservableCollection<SerialPortInfo> AvailablePorts
        {
            get => _availablePorts;
            set => SetProperty(ref _availablePorts, value);
        }

        public bool IsDeviceAvailable
        {
            get => _isDeviceAvailable;
            private set
            {
                if (SetProperty(ref _isDeviceAvailable, value))
                    _startCommand?.NotifyCanExecuteChanged();
            }
        }

        public SerialPortInfo? SelectedPort
        {
            get => _selectedPort;
            set
            {
                if (SetProperty(ref _selectedPort, value) && value != null)
                {
                    SettingsModel.LastSelectedComPort = value.PortName;
                    _appSettingsService.Save(SettingsModel);
                    _ = CheckSelectedPortAvailabilityAsync();
                }
            }
        }

        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (SetProperty(ref _selectedLanguage, value))
                {
                    SettingsModel.Language = value;
                    _appSettingsService.Save(SettingsModel);
                    _localizationService.SetLanguage(value);

                    LoadPorts();
                    _ = CheckSelectedPortAvailabilityAsync();
                }
            }
        }

        public ClientRecordDto? SelectedClientRecord
        {
            get => _selectedClientRecord;
            set
            {
                if (EqualityComparer<ClientRecordDto?>.Default.Equals(_selectedClientRecord, value))
                    return;

                _selectedClientRecord = value;
                OnPropertyChanged();

                OnSelectedClientRecordChanged();

                _startCommand?.NotifyCanExecuteChanged();
                _editClientCommand?.NotifyCanExecuteChanged();
                _deleteClientCommand?.NotifyCanExecuteChanged();
                _allCheckClientCommand?.NotifyCanExecuteChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    _searchTimer.Stop();
                    _searchTimer.Start();
                }
            }
        }

        public bool IsRelativeDifference
        {
            get => _isRelativeDifference;
            set => SetProperty(ref _isRelativeDifference, value);
        }

        public ObservableCollection<string> AxleItems
        {
            get => _axleItems;
            private set => SetProperty(ref _axleItems, value);
        }

        public string? SelectedAxle
        {
            get => _selectedAxle;
            set => SetProperty(ref _selectedAxle, value);
        }

        public bool SelectedMeasurementMode
        {
            get => _selectedMeasurementMode;
            set => SetProperty(ref _selectedMeasurementMode, value);
        }

        public MeasurementType SelectedMeasurementType
        {
            get => _selectedMeasurementType;
            set => SetProperty(ref _selectedMeasurementType, value);
        }

        public string CarName =>
            SelectedClientRecord != null
                ? $"{SelectedClientRecord.BrandName} {SelectedClientRecord.Model}"
                : string.Empty;

        public string OwnerName => SelectedClientRecord?.OwnerName ?? string.Empty;
        public string OwnerSurname => SelectedClientRecord?.OwnerSurname ?? string.Empty;
        public string VinNumber => SelectedClientRecord?.VinCode ?? string.Empty;
        public string GosNumber => SelectedClientRecord?.GosNumber ?? string.Empty;
        public string CarCategory => SelectedClientRecord?.CategoryName ?? string.Empty;
        public int AxlesCount => SelectedClientRecord?.AxlesCount ?? 0;
        public int CurbMass => SelectedClientRecord?.CurbMass ?? 0;
        public int MaxMass => SelectedClientRecord?.MaxMass ?? 0;

        public ICommand AddClientCommand =>
            _addClientCommand ??= new RelayCommand(OpenAddClientWindow);

        public ICommand EditClientCommand =>
            _editClientCommand ??= new RelayCommand(OpenEditClientWindow, CanExecuteCommand);

        public ICommand DeleteClientCommand =>
            _deleteClientCommand ??= new RelayCommand(DeleteClient, CanExecuteCommand);

        public ICommand AllCheckClientCommand =>
            _allCheckClientCommand ??= new RelayCommand(OpenAllCheckClient, CanExecuteCommand);

        public ICommand StartCommand =>
            _startCommand ??= new RelayCommand(ExecuteStart, CanStartCommand);

        public ICommand ManualModeCommand =>
            _manualModeCommand ??= new RelayCommand(() => SelectedMeasurementMode = false);

        public ICommand AutoModeCommand =>
            _autoModeCommand ??= new RelayCommand(() => SelectedMeasurementMode = true);

        private void LoadPorts()
        {
            var ports = _serialPortDiscoveryService.GetAvailablePorts().ToList();
            var savedPort = SettingsModel.LastSelectedComPort;

            if (!string.IsNullOrWhiteSpace(savedPort) &&
                ports.All(p => !string.Equals(p.PortName, savedPort, StringComparison.OrdinalIgnoreCase)))
            {
                ports.Insert(0, new SerialPortInfo
                {
                    PortName = savedPort,
                    DisplayName = $"{savedPort} ({LocalizationProvider["Unavailable"]})",
                    IsAvailable = false
                });
            }

            AvailablePorts = new ObservableCollection<SerialPortInfo>(ports);

            if (!string.IsNullOrWhiteSpace(savedPort))
                SelectedPort = AvailablePorts.FirstOrDefault(p => p.PortName == savedPort);

            SelectedPort ??= AvailablePorts.FirstOrDefault();
        }

        private async Task CheckSelectedPortAvailabilityAsync()
        {
            _availabilityCts?.Cancel();
            _availabilityCts?.Dispose();
            _availabilityCts = new CancellationTokenSource();

            var ct = _availabilityCts.Token;

            try
            {
                if (SelectedPort == null || !SelectedPort.IsAvailable)
                {
                    PublishAvailability(false, SelectedPort?.PortName);
                    return;
                }

                bool isAvailable = await _brakeTesterService.CheckAvailabilityAsync(SelectedPort.PortName, ct);

                if (ct.IsCancellationRequested)
                    return;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    PublishAvailability(isAvailable, SelectedPort.PortName);
                });
            }
            catch (OperationCanceledException)
            {
            }
            catch
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    PublishAvailability(false, SelectedPort?.PortName);
                });
            }
        }

        private void PublishAvailability(bool isAvailable, string? portName)
        {
            IsDeviceAvailable = isAvailable;

            _messageBus.Publish(new DeviceAvailabilityChangedMessage
            {
                IsAvailable = isAvailable,
                PortName = portName
            });
        }

        private void LoadData()
        {
            var records = _clientRecordService.LoadClientRecords();
            _allClientRecords = new ObservableCollection<ClientRecordDto>(records);
            ClientRecords = new ObservableCollection<ClientRecordDto>(records);
        }

        private void ApplyFilter()
        {
            List<ClientRecordDto> filtered;

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = _allClientRecords.ToList();
            }
            else
            {
                filtered = _allClientRecords.Where(record =>
                    (record.OwnerName?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (record.OwnerSurname?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (record.GosNumber?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (record.VinCode?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (record.BrandName?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (record.Model?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (record.CategoryName?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (record.LastTestDate?.ToString("dd.MM.yyyy")?.Contains(SearchText) ?? false)
                ).ToList();
            }

            if (!_filteredList.SequenceEqual(filtered))
            {
                _filteredList = filtered;
                ClientRecords = new ObservableCollection<ClientRecordDto>(_filteredList);
            }
        }

        private void OpenAddClientWindow()
        {
            var window = _serviceProvider.GetRequiredService<AddClientCarWindow>();
            window.Owner = Application.Current.MainWindow;

            if (window.ShowDialog() == true)
                LoadData();
        }

        private void OpenEditClientWindow()
        {
            if (SelectedClientRecord == null)
                return;

            using var scope = _serviceProvider.CreateScope();
            var sp = scope.ServiceProvider;

            var carService = sp.GetRequiredService<ITheCarService>();
            var car = carService.GetById(SelectedClientRecord.CarId);

            if (car == null)
                return;

            var viewModel = ActivatorUtilities.CreateInstance<AddClientCarViewModel>(sp, car.Owner, car);

            var window = new AddClientCarWindow(viewModel, sp.GetRequiredService<ILocalizationService>())
            {
                Owner = Application.Current.MainWindow
            };

            if (window.ShowDialog() == true)
                LoadData();
        }

        private void DeleteClient()
        {
            if (SelectedClientRecord == null)
            {
                MessageBox.Show(
                    LocalizationProvider["WarnSelectRowToDelete"],
                    LocalizationProvider["MessageWarning"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                string.Format(LocalizationProvider["ConfirmDeleteCar"], SelectedClientRecord.OwnerName),
                LocalizationProvider["MessageWarning"],
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes)
                return;

            try
            {
                _clientRecordService.DeleteClientRecord(SelectedClientRecord.CarId);
                LoadData();
                SelectedClientRecord = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(LocalizationProvider["ErrorCarDelete"], ex.Message),
                    LocalizationProvider["MessageError"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void OpenAllCheckClient()
        {
            if (SelectedClientRecord == null)
            {
                MessageBox.Show(
                    LocalizationProvider["WarnSelectRowToOpenCheck"],
                    LocalizationProvider["MessageWarning"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                SetFocusReturn();
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var checkService = sp.GetRequiredService<ICheckService>();

            var checks = checkService.GetChecksDtoByCarId(SelectedClientRecord.CarId);

            if (checks == null || !checks.Any())
            {
                MessageBox.Show(
                    string.Format(LocalizationProvider["NoChecksFound"], SelectedClientRecord.GosNumber),
                    LocalizationProvider["MessageInformation"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                SetFocusReturn();
                return;
            }

            string carInfo = $"{SelectedClientRecord.BrandName} {SelectedClientRecord.Model}".Trim();

            var viewModel = ActivatorUtilities.CreateInstance<ChecksWindowViewModel>(
                sp,
                checkService,
                LocalizationProvider,
                SelectedClientRecord.CarId,
                SelectedClientRecord.OwnerName ?? string.Empty,
                SelectedClientRecord.OwnerSurname ?? string.Empty,
                SelectedClientRecord.GosNumber ?? string.Empty,
                carInfo
            );

            var window = sp.GetRequiredService<ChecksWindow>();
            window.DataContext = viewModel;
            window.Owner = Application.Current.MainWindow;
            window.Closed += (_, _) => SetFocusReturn();
            window.ShowDialog();
        }

        private void OnSelectedClientRecordChanged()
        {
            AxleItems.Clear();

            if (SelectedClientRecord != null)
            {
                for (int i = 1; i <= SelectedClientRecord.AxlesCount; i++)
                    AxleItems.Add($"Ось {i}");
            }

            SelectedAxle = AxleItems.FirstOrDefault();

            OnPropertyChanged(nameof(CarName));
            OnPropertyChanged(nameof(GosNumber));
            OnPropertyChanged(nameof(VinNumber));
            OnPropertyChanged(nameof(CarCategory));
            OnPropertyChanged(nameof(AxlesCount));
            OnPropertyChanged(nameof(OwnerName));
            OnPropertyChanged(nameof(OwnerSurname));
            OnPropertyChanged(nameof(CurbMass));
            OnPropertyChanged(nameof(MaxMass));
        }

        private bool CanExecuteCommand()
        {
            return SelectedClientRecord != null;
        }

        private bool CanStartCommand()
        {
            return SelectedClientRecord != null && IsDeviceAvailable;
        }

        private void SetFocusReturn()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _ = Application.Current.MainWindow as MainWindow;
            }));
        }

        private void ExecuteStart()
        {
            _messageBus.Publish(new ClientRecordMessageDto
            {
                Start = true,
                AxlesCount = AxlesCount,
                CarName = CarName,
                GosNumber = GosNumber,
                CarCategory = CarCategory,
                IsRelativeDifference = IsRelativeDifference,
                SelectedMeasurementMode = SelectedMeasurementMode,
                SelectedMeasurementType = SelectedMeasurementType
            });
        }

        public void Dispose()
        {
            _availabilityCts?.Cancel();
            _availabilityCts?.Dispose();
            _searchTimer.Dispose();
        }
    }
}