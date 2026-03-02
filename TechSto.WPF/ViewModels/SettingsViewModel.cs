using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TechSto.Core.Interfaces;
using TechSto.Core.Models;
using TechSto.Infrastructure.Data;
using TechSto.WPF.BusinessLayer;
using TechSto.WPF.SecondWindow;
using TechSto.WPF.Services;
namespace TechSto.WPF.ViewModels
{
    public class SettingsViewModel: ViewModelBase
    {            
        private bool _isDeviceConnected;
        private Visibility _brandsVisibility = Visibility.Collapsed;
        private ObservableCollection<ClientRecordDto> _clientRecords = [];
        public ICommand OpenAddClientCommand { get; }

        private readonly IAppSettingsService _appSettingsService;
        private readonly ILocalizationService _localizationService;
        private readonly MainContext _context;
        private AppSettings _settings;

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

        public SettingsViewModel(IAppSettingsService appSettingsService, ILocalizationService localizationService, MainContext context)
        {
           
            _appSettingsService = appSettingsService;
            _localizationService = localizationService;
            _context = context;

            _localizationService = localizationService;
            LocalizationProvider = new LocalizationProvider(_localizationService);


            OpenAddClientCommand = new RelayCommand(OpenAddClientWindow);

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
            var service = new ClientRecordService(_context);
            var list = service.LoadClientRecords();
            ClientRecords = new ObservableCollection<ClientRecordDto>(list);
        }

        private void OnConnectionStateChanged(bool isConnected)
        {
            IsDeviceConnected = isConnected;
        }

        private void OpenAddClientWindow()
        {
            var addWindow = new ClientWindow(_context);
            if (addWindow.ShowDialog() == true)
            {
                LoadData(); // Обновляем таблицу после закрытия окна
            }
        }          
    }
}
