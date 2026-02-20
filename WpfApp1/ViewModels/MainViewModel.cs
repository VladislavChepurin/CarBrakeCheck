using System.Windows.Input;
using WpfApp1.DataBase.Entity;
using WpfApp1.SettingsWindow;

namespace WpfApp1.ViewModels
{
    class MainViewModel: ViewModelBase
    {
        private readonly MainContext _context;
        public string Title { get; } = Properties.Resources.NameProgram;

        public ICommand OpenCarBrandsCommand { get; }
        public ICommand OpenCarModelsCommand { get; }


        public MainViewModel(MainContext context)
        {
            _context = context;
            OpenCarBrandsCommand = new RelayCommand(ExecuteOpenCarBrands);
            OpenCarModelsCommand = new RelayCommand(ExecuteOpenCarModels);
        }

        private void ExecuteOpenCarBrands(object parameter)
        {
            var window = new CarBrands(_context);
            window.ShowDialog();
        }

        private void ExecuteOpenCarModels(object parameter)
        {
            var window = new CarBrands(_context);
            window.ShowDialog();
        }
    }
}
