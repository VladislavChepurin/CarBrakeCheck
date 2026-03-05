using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TechSto.Core.DTOs;
using TechSto.Core.Interfaces;
using TechSto.Core.Models;

using TechSto.WPF.Services;
namespace TechSto.WPF.ViewModels
{
    public class SettingsViewModel: ViewModelBase
    {            
        private bool _isDeviceConnected;
        private ClientRecordDto _selectedClientRecord;
        private Visibility _brandsVisibility = Visibility.Collapsed;
        private ObservableCollection<ClientRecordDto> _clientRecords = [];
        public ICommand OpenAddClientCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand ManualModeCommand { get; }
        public ICommand AutoModeCommand { get; }


        private readonly IAppSettingsService _appSettingsService;
        private readonly ILocalizationService _localizationService;
        private readonly IClientRecordService _clientRecordService;       
        private AppSettings _settings;
        public ClientRecordDto SelectedClientRecord
        {
            get => _selectedClientRecord;
            set
            {
                SetProperty(ref _selectedClientRecord, value);
                OnSelectedClientRecordChanged(); // метод для обновления зависимых свойств
            }
        }

        // Свойства для правой панели
        public string SelectedCarDisplay =>
            SelectedClientRecord != null
                ? $"{SelectedClientRecord.StateNumber} ({SelectedClientRecord.BrandName} {SelectedClientRecord.Model})"
                : "";

        public string CarCategory => SelectedClientRecord?.CategoryName ?? "";

        public int AxlesCount => SelectedClientRecord?.AxlesCount ?? 0;

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

        private int _selectedMeasurementMode; // 0 - ручной, 1 - автомат (или enum)
        public int SelectedMeasurementMode
        {
            get => _selectedMeasurementMode;
            set => SetProperty(ref _selectedMeasurementMode, value);
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

        public SettingsViewModel(IAppSettingsService appSettingsService, ILocalizationService localizationService, IClientRecordService clientRecordService)
        {
           
            _appSettingsService = appSettingsService;
            _localizationService = localizationService;
            _clientRecordService = clientRecordService;          

            _localizationService = localizationService;
            LocalizationProvider = new LocalizationProvider(_localizationService);


            OpenAddClientCommand = new RelayCommand(OpenAddClientWindow);
            StartCommand = new RelayCommand(ExecuteStart, CanExecuteStart);
            //ManualModeCommand = new RelayCommand(() => SelectedMeasurementMode = 0);
            //AutoModeCommand = new RelayCommand(() => SelectedMeasurementMode = 1);

            //if (DeviceConnectionService.Instance != null)
            //{
            //    DeviceConnectionService.Instance.ConnectionStateChanged += OnConnectionStateChanged;
            //    IsDeviceConnected = DeviceConnectionService.Instance.IsConnected;
            //}

            // Загружаем настройки
            SettingsModel = _appSettingsService.Load();

            // Устанавливаем язык из настроек
            _localizationService.SetLanguage(SettingsModel.Language);
                        
            LoadData();
        }

        private void LoadData()
        {
            var records = _clientRecordService.LoadClientRecords();
            ClientRecords = new ObservableCollection<ClientRecordDto>(records);
        }

        private void OnConnectionStateChanged(bool isConnected)
        {
            IsDeviceConnected = isConnected;
        }

        private void OpenAddClientWindow(object e)
        {
            //var addWindow = new ClientWindow();
            //if (addWindow.ShowDialog() == true)
            //{
                LoadData(); // Обновляем таблицу после закрытия окна
            //}
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
            OnPropertyChanged(nameof(CarCategory));
            OnPropertyChanged(nameof(AxlesCount));
        }

        private void ExecuteStart(object? parameter)
        {
            // Ваша логика
        }

        private bool CanExecuteStart(object? parameter)
        {
            // Возвращаете true/false, параметр можно игнорировать, если не нужен
            return SelectedClientRecord != null;
        }
    }
}
