using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TechSto.BusinessLayer;
using TechSto.DataBase.Entity;
using TechSto.SecondWindow;

namespace TechSto.ViewModels
{
    class MainViewModel: ViewModelBase
    {
        private readonly MainContext _context;      
        private Visibility _brandsVisibility = Visibility.Collapsed;
        private ObservableCollection<ClientRecordDto> _clientRecords = [];
        public ICommand OpenAddClientCommand { get; }

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
            LoadData();
        }

        private void LoadData()
        {
            var service = new ClientRecordService(_context);
            var list = service.LoadClientRecords();
            ClientRecords = new ObservableCollection<ClientRecordDto>(list);
        }

        private void OpenAddClientWindow()
        {           
            var addWindow = new AddClientWindow();
            if (addWindow.ShowDialog() == true)
            {
                LoadData(); // Обновляем таблицу после закрытия окна
            }
        }      
    }
}
