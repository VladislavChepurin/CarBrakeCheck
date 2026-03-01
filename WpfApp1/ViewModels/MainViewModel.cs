using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TechSto.WPF.BusinessLayer;
using TechSto.WPF.SecondWindow;
using TechSto.WPF.DataBase.Entity;
namespace TechSto.WPF.ViewModels
{
    class MainViewModel: ViewModelBase
    {
        private readonly MainContext _context;
    
        private bool _isDeviceConnected;
        private Visibility _brandsVisibility = Visibility.Collapsed;
        private ObservableCollection<ClientRecordDto> _clientRecords = [];
        public ICommand OpenAddClientCommand { get; }

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

        public MainViewModel(MainContext context)
        {
            _context = context;        
            OpenAddClientCommand = new RelayCommand(OpenAddClientWindow);
                       
            //if (DeviceConnectionService.Instance != null)
            //{
            //    DeviceConnectionService.Instance.ConnectionStateChanged += OnConnectionStateChanged;
            //    IsDeviceConnected = DeviceConnectionService.Instance.IsConnected;
            //}

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
