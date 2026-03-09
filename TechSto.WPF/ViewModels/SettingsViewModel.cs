using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TechSto.Core.DTOs;
using TechSto.Core.Interfaces;
using TechSto.Core.Models;
using TechSto.WPF.Services;
namespace TechSto.WPF.ViewModels
{
    public class SettingsViewModel: ViewModelBase, IDisposable
    {            
        private bool _isDeviceConnected;     
        private ClientRecordDto _selectedClientRecord;
        private Visibility _brandsVisibility = Visibility.Collapsed;
        private ObservableCollection<ClientRecordDto> _clientRecords = [];
        private ObservableCollection<ClientRecordDto> _allClientRecords = [];
        private List<ClientRecordDto> _filteredList = [];
        private readonly IAppSettingsService _appSettingsService;
        private readonly ILocalizationService _localizationService;
        private readonly IClientRecordService _clientRecordService;
        private readonly IServiceProvider _serviceProvider;
        private AppSettings _settings;
        private string _searchText = string.Empty;
        private System.Timers.Timer _searchTimer;

        public ClientRecordDto SelectedClientRecord
        {
            get => _selectedClientRecord;
            set
            {
                _selectedClientRecord = value;
                OnPropertyChanged();
                OnSelectedClientRecordChanged();

                // Обновляем состояние команды Start
                _startCommand?.RaiseCanExecuteChanged();

                // Если есть другие команды, зависящие от выбора
                (_editClientCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (_deleteClientCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (_allCheckClientCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        // Свойства для правой панели
        public string SelectedCarDisplay =>
            SelectedClientRecord != null
                ? $"{SelectedClientRecord.BrandName} {SelectedClientRecord.Model}"
                : "";

        private RelayCommand? _addClientCommand;
        public ICommand AddClientCommand => _addClientCommand ??= new RelayCommand(OpenAddClientWindow);

        private RelayCommand? _editClientCommand;
        public ICommand EditClientCommand => _editClientCommand ??= new RelayCommand(OpenEditClientWindow, CanExecuteCommand);

        private RelayCommand? _deleteClientCommand;
        public ICommand DeleteClientCommand => _deleteClientCommand ??= new RelayCommand(DeleteClient, CanExecuteCommand);

        private RelayCommand? _allCheckClientCommand;
        public ICommand AllCheckClientCommand => _allCheckClientCommand ??= new RelayCommand(OpenAllCheckClient, CanExecuteCommand);

        private RelayCommand? _startCommand;
        public ICommand StartCommand => _startCommand ??= new RelayCommand(ExecuteStart, CanExecuteCommand);

        // Команды для режимов (не зависят от выбора)
        private RelayCommand? _manualModeCommand;
        public ICommand ManualModeCommand => _manualModeCommand ??= new RelayCommand(_ => SelectedMeasurementMode = false);

        private RelayCommand? _autoModeCommand;
        public ICommand AutoModeCommand => _autoModeCommand ??= new RelayCommand(_ => SelectedMeasurementMode = true);

        public string OwnerName => SelectedClientRecord?.OwnerName ?? "";
        public string OwnerSurname => SelectedClientRecord?.OwnerSurname ?? "";
        public string VinNumber => SelectedClientRecord?.VinCode ?? "";
        public string GosNumber => SelectedClientRecord?.GosNumber ?? "";
        public string CarCategory => SelectedClientRecord?.CategoryName ?? "";
        public int AxlesCount => SelectedClientRecord?.AxlesCount ?? 0;
        public int CurbMass => SelectedClientRecord?.CurbMass ?? 0;
        public int MaxMass => SelectedClientRecord?.MaxMass ?? 0;


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

        private bool _isRelativeDifference;
        public bool IsRelativeDifference
        {
            get => _isRelativeDifference;
            set => SetProperty(ref _isRelativeDifference, value);
        }

        private ObservableCollection<string> _axleItems = new();
        public ObservableCollection<string> AxleItems
        {
            get => _axleItems;
            private set => SetProperty(ref _axleItems, value);
        }

        private string _selectedAxle;
        public string SelectedAxle
        {
            get => _selectedAxle;
            set => SetProperty(ref _selectedAxle, value);
        }

     // Ползунок Авто-Ручной
        private bool _selectedMeasurementMode;
        public bool SelectedMeasurementMode
        {
            get => _selectedMeasurementMode;
            set
            {
                _selectedMeasurementMode = value;
                OnPropertyChanged();
            }
        }

        private int _selectedMeasurementType; // для радиокнопок: 0 - въезд/просушка, 1 - полная загрузка и т.д.

        public int SelectedMeasurementType
        {
            get => _selectedMeasurementType;
            set => SetProperty(ref _selectedMeasurementType, value);
        }

        public bool IsDeviceConnected
        {
            get => _isDeviceConnected;
            set { _isDeviceConnected = value; OnPropertyChanged(); }
        }

        public Visibility BrandsVisibility
        {
            get => _brandsVisibility;
            set { _brandsVisibility = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ClientRecordDto> ClientRecords
        {
            get => _clientRecords;
            set { _clientRecords = value; OnPropertyChanged(); }
        }

        public AppSettings SettingsModel
        {
            get => _settings;
            private set => SetProperty(ref _settings, value);
        }

        public LocalizationProvider LocalizationProvider { get; }

        public SettingsViewModel(IAppSettingsService appSettingsService, ILocalizationService localizationService, 
            IClientRecordService clientRecordService, IServiceProvider serviceProvider, 
            LocalizationProvider localizationProvider)
        {           
            _appSettingsService = appSettingsService;
            _localizationService = localizationService;
            _clientRecordService = clientRecordService;
            _serviceProvider = serviceProvider;           
            LocalizationProvider = localizationProvider;
            // Загружаем настройки
            SettingsModel = _appSettingsService.Load();
            // Устанавливаем язык из настроек
            _localizationService.SetLanguage(SettingsModel.Language);

            _searchTimer = new System.Timers.Timer(300);
            _searchTimer.AutoReset = false;
            _searchTimer.Elapsed += (s, e) =>
            {
                Application.Current.Dispatcher.Invoke(() => ApplyFilter());
            };
            LoadData();
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

            // Обновляем коллекцию только если изменилось количество или порядок
            if (!_filteredList.SequenceEqual(filtered))
            {
                _filteredList = filtered;
                ClientRecords = new ObservableCollection<ClientRecordDto>(_filteredList);
            }
        }

        private void OnConnectionStateChanged(bool isConnected)
        {
            IsDeviceConnected = isConnected;
        }

        private void OpenAddClientWindow(object e)
        {
            var window = _serviceProvider.GetRequiredService<AddClientCarWindow>();
            window.Owner = Application.Current.MainWindow;

            if (window.ShowDialog() == true)
            {
                LoadData(); // перезагрузка данных после успешного сохранения
            }
        }

        private void OpenEditClientWindow(object e)
        {
            if (SelectedClientRecord == null) return;

            // Создаём отдельный scope для изоляции контекста
            using var scope = _serviceProvider.CreateScope();
            var sp = scope.ServiceProvider;

            // Загружаем полный объект автомобиля через сервис
            var carService = sp.GetRequiredService<ITheCarService>();
            var car = carService.GetById(SelectedClientRecord.CarId);

            if (car == null) return;

            var viewModel = ActivatorUtilities.CreateInstance<AddClientCarViewModel>(sp, car.Owner, car);

            // Создаём окно вручную (не через DI, чтобы передать нашу ViewModel)
            var window = new AddClientCarWindow(viewModel, sp.GetRequiredService<ILocalizationService>())
            {
                Owner = Application.Current.MainWindow
            };

            if (window.ShowDialog() == true)
            {
                LoadData(); // перезагрузка данных после успешного сохранения               
            }          
        }

        private void DeleteClient(object e)
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
                // Предполагаем, что в сервисе есть метод удаления по идентификатору автомобиля
                _clientRecordService.DeleteClientRecord(SelectedClientRecord.CarId);

                // Перезагружаем данные
                LoadData();

                // Сбрасываем выделение, так как удалённой записи больше нет
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

        private void OpenAllCheckClient(object e)
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

            // Проверяем наличие проверок через сервис
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

            // Формируем информацию об автомобиле
            string carInfo = $"{SelectedClientRecord.BrandName} {SelectedClientRecord.Model}".Trim();

            // Получаем фабрику или используем ActivatorUtilities для создания ViewModel с параметрами
            var viewModel = ActivatorUtilities.CreateInstance<ChecksWindowViewModel>(
                sp,
                checkService,
                LocalizationProvider,
                SelectedClientRecord.CarId,
                SelectedClientRecord.OwnerName ?? "",
                SelectedClientRecord.OwnerSurname ?? "",
                SelectedClientRecord.GosNumber ?? "",
                carInfo
            );

            // Получаем окно из DI и устанавливаем ViewModel
            var window = sp.GetRequiredService<ChecksWindow>();
            window.DataContext = viewModel;
            window.Owner = Application.Current.MainWindow;   
            window.Closed += (s, args) => SetFocusReturn();
            window.ShowDialog();
        }
                
        private void OnSelectedClientRecordChanged()
        {
            // Обновляем список осей
            AxleItems.Clear();
            if (SelectedClientRecord != null)
            {
                for (int i = 1; i <= SelectedClientRecord.AxlesCount; i++)
                {
                    AxleItems.Add($"Ось {i}");
                }
            }
            // Сбросить выбранную ось, если их количество изменилось
            SelectedAxle = AxleItems.FirstOrDefault();
            // Обновить свойства, от которых зависит интерфейс
            OnPropertyChanged(nameof(SelectedCarDisplay));
            OnPropertyChanged(nameof(GosNumber));
            OnPropertyChanged(nameof(VinNumber));
            OnPropertyChanged(nameof(CarCategory));
            OnPropertyChanged(nameof(AxlesCount));
            OnPropertyChanged(nameof(OwnerName));
            OnPropertyChanged(nameof(OwnerSurname));
            OnPropertyChanged(nameof(CurbMass));
            OnPropertyChanged(nameof(MaxMass));
        }
        private bool CanExecuteCommand(object? parameter)
        {
            return SelectedClientRecord != null;
        }

        private void SetFocusReturn()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow?.MainTableWithClientData?.Focus();
            }));
        }

        private void ExecuteStart(object? parameter)
        {
            
        }

        public void Dispose()
        {
            _searchTimer?.Dispose();
        }

    }
}
