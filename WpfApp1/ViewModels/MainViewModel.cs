using System.Collections.ObjectModel;
using System.Drawing.Drawing2D;
using System.Windows;
using System.Windows.Input;
using WpfApp1.BusinessLayer;
using WpfApp1.Controls;
using WpfApp1.DataBase.Entity;

namespace WpfApp1.ViewModels
{
    class MainViewModel: ViewModelBase
    {
        private readonly MainContext _context;       
        private CarBrandService _service;
        private Visibility _brandsVisibility = Visibility.Collapsed;
        private ObservableCollection<CarBrand> _brands;
        private CarBrand _selectedBrand;


        public ICommand OpenCarBrandsCommand { get; }
        public ICommand AddBrandCommand { get; }
        public ICommand EditBrandCommand { get; }
        public ICommand DeleteBrandCommand { get; }
        public ICommand ChooseBrandCommand { get; }


        public Visibility BrandsVisibility
        {
            get => _brandsVisibility;
            set { _brandsVisibility = value; OnPropertyChanged(); }
        }
        public ObservableCollection<CarBrand> Brands
        {
            get => _brands;
            set { _brands = value; OnPropertyChanged(); }
        }

        // Выбранный бренд (привязка TwoWay с EntityManager)
        public CarBrand SelectedBrand
        {
            get => _selectedBrand;
            set { _selectedBrand = value; OnPropertyChanged(); }
        }

        public MainViewModel(MainContext context)
        {
            _context = context;
            _service = new CarBrandService(_context);
            OpenCarBrandsCommand = new RelayCommand(ExecuteOpenCarBrands);
            AddBrandCommand = new RelayCommand(AddBrand);
            EditBrandCommand = new RelayCommand(EditBrand);
            DeleteBrandCommand = new RelayCommand(DeleteBrand);
            ChooseBrandCommand = new RelayCommand(ChooseBrand);
        }    

        private void ExecuteOpenCarBrands(object parameter)
        {
            Brands = _service.GetLocalBrands(); // ObservableCollection
            BrandsVisibility = Visibility.Visible;          
        }

        private void AddBrand(object parameter)
        {
            if (parameter is string newName)
            {
                var brand = new CarBrand { BrandName = newName };
                _service.Add(brand);
            }
        }

        private void EditBrand(object parameter)
        {
            if (parameter is EditCommandParameter param)
            {
                SelectedBrand.BrandName = param.NewName;
                _service.Update(SelectedBrand);
            }
        }

        private void DeleteBrand(object parameter)
        {
            if (SelectedBrand != null)
                _service.Delete(SelectedBrand.Id);            
        }

        private void ChooseBrand(object parameter)
        {

        }
    }
}
