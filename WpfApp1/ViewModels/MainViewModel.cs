using System.Collections.ObjectModel;
using System.Drawing.Drawing2D;
using System.Windows;
using System.Windows.Input;
using TechSto.BusinessLayer;
using TechSto.Controls;
using TechSto.DataBase.Entity;
using TechSto.ViewModels;

namespace TechSto.ViewModels
{
    class MainViewModel: ViewModelBase
    {
        private readonly MainContext _context;       
        private CarBrandService _service;
        private Visibility _brandsVisibility = Visibility.Collapsed;
        private ObservableCollection<CarBrandViewModel> _brands;
        private CarBrandViewModel _selectedBrand;


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
        public ObservableCollection<CarBrandViewModel> Brands
        {
            get => _brands;
            set { _brands = value; OnPropertyChanged(); }
        }

        // Выбранный бренд (привязка TwoWay с EntityManager)
        public CarBrandViewModel SelectedBrand
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
            var brandsFromDb = _service.GetLocalBrands(); // List<CarBrand>
            Brands = new ObservableCollection<CarBrandViewModel>(
                brandsFromDb.Select(b => new CarBrandViewModel(b))
            );
            BrandsVisibility = Visibility.Visible;
        }

        private void AddBrand(object parameter)
        {
            if (parameter is string newName)
            {
                var brand = new CarBrand { BrandName = newName };
                _service.Add(brand);
                // Создаём ViewModel для нового бренда и добавляем в коллекцию
                var newBrandVm = new CarBrandViewModel(brand);
                Brands.Add(newBrandVm);

                // Опционально: выбрать новый элемент
                SelectedBrand = newBrandVm;
            }
        }

        private void EditBrand(object parameter)
        {
            if (SelectedBrand == null) return;
            if (parameter is EditCommandParameter param)
            {
                SelectedBrand.BrandName = param.NewName;
                _service.Update(SelectedBrand.Model);
            }
        }

        private void DeleteBrand(object parameter)
        {
            if (SelectedBrand == null) return;
            _service.Delete(SelectedBrand.Id);
            Brands.Remove(SelectedBrand);
            SelectedBrand = null!; // сбрасываем выделение (опционально)
        }

        private void ChooseBrand(object parameter)
        {
            if (SelectedBrand == null) return;

        }
    }
}
