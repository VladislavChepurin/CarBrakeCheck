using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using TechSto.BusinessLayer;
using TechSto.DataBase.Entity;

namespace TechSto.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        private readonly MainContext _context;
        private CarBrandService _service;

        private ObservableCollection<CarRowViewModel> _cars = new();
        private ICollectionView? _carsView;
        private CarRowViewModel? _selectedCar;
        private bool _hasSelectedCar;
        private string _searchText = "";

        private string _previewOwner = "—";
        private string _previewOwnerSts = "—";
        private string _previewGos = "—";
        private string _previewVin = "—";
        private string _previewBrand = "—";
        private string _previewModel = "—";
        private string _previewCategory = "—";
        private string _previewMaxMass = "—";
        private string _previewCurbMass = "—";
        private string _previewBrakeDiff = "—";
        private string _previewParkingBrake = "—";
        private string _previewReserveBrake = "—";
        private string _previewLastTest = "—";
        private ObservableCollection<string> _previewAxles = new();

        public ObservableCollection<CarRowViewModel> Cars
        {
            get => _cars;
            set { _cars = value; OnPropertyChanged(); }
        }

        public CarRowViewModel? SelectedCar
        {
            get => _selectedCar;
            set
            {
                _selectedCar = value;
                OnPropertyChanged();
                HasSelectedCar = _selectedCar != null;
                LoadPreview();
            }
        }

        public bool HasSelectedCar
        {
            get => _hasSelectedCar;
            set { _hasSelectedCar = value; OnPropertyChanged(); }
        }

        public ICollectionView? CarsView
        {
            get => _carsView;
            set { _carsView = value; OnPropertyChanged(); }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value ?? "";
                OnPropertyChanged();
                CarsView?.Refresh();
            }
        }

        public string PreviewOwner { get => _previewOwner; set { _previewOwner = value; OnPropertyChanged(); } }
        public string PreviewOwnerSts { get => _previewOwnerSts; set { _previewOwnerSts = value; OnPropertyChanged(); } }
        public string PreviewGos { get => _previewGos; set { _previewGos = value; OnPropertyChanged(); } }
        public string PreviewVin { get => _previewVin; set { _previewVin = value; OnPropertyChanged(); } }
        public string PreviewBrand { get => _previewBrand; set { _previewBrand = value; OnPropertyChanged(); } }
        public string PreviewModel { get => _previewModel; set { _previewModel = value; OnPropertyChanged(); } }
        public string PreviewCategory { get => _previewCategory; set { _previewCategory = value; OnPropertyChanged(); } }
        public string PreviewMaxMass { get => _previewMaxMass; set { _previewMaxMass = value; OnPropertyChanged(); } }
        public string PreviewCurbMass { get => _previewCurbMass; set { _previewCurbMass = value; OnPropertyChanged(); } }
        public string PreviewBrakeDiff { get => _previewBrakeDiff; set { _previewBrakeDiff = value; OnPropertyChanged(); } }
        public string PreviewParkingBrake { get => _previewParkingBrake; set { _previewParkingBrake = value; OnPropertyChanged(); } }
        public string PreviewReserveBrake { get => _previewReserveBrake; set { _previewReserveBrake = value; OnPropertyChanged(); } }
        public string PreviewLastTest { get => _previewLastTest; set { _previewLastTest = value; OnPropertyChanged(); } }
        public ObservableCollection<string> PreviewAxles { get => _previewAxles; set { _previewAxles = value; OnPropertyChanged(); } }

        private static string DashIfEmpty(string? value) => string.IsNullOrWhiteSpace(value) ? "—" : value;

        private void ResetPreview()
        {
            PreviewOwner = "";
            PreviewOwnerSts = "";
            PreviewGos = "";
            PreviewVin = "";
            PreviewBrand = "";
            PreviewModel = "";
            PreviewCategory = "";
            PreviewMaxMass = "";
            PreviewCurbMass = "";
            PreviewBrakeDiff = "";
            PreviewParkingBrake = "";
            PreviewReserveBrake = "";
            PreviewLastTest = "";
            PreviewAxles = new ObservableCollection<string>();
        }

        private void LoadPreview()
        {
            if (SelectedCar == null)
            {
                ResetPreview();
                return;
            }

            var car = _context.TheCars
                .Include(c => c.Owner)
                .Include(c => c.CarModel).ThenInclude(m => m!.CarBrand)
                .Include(c => c.CarModel).ThenInclude(m => m!.CarСategory)
                .Include(c => c.CarModel).ThenInclude(m => m!.Axles)
                .Include(c => c.DataChecks)
                .FirstOrDefault(c => c.Id == SelectedCar.CarId);

            if (car == null)
            {
                ResetPreview();
                return;
            }

            PreviewOwner = DashIfEmpty(car.Owner?.Name);
            PreviewOwnerSts = DashIfEmpty(car.Owner?.STSNumber);
            PreviewGos = DashIfEmpty(car.GosNumber);
            PreviewVin = DashIfEmpty(car.BodyNumber);
            PreviewBrand = DashIfEmpty(car.CarModel?.CarBrand?.BrandName);
            PreviewModel = DashIfEmpty(car.CarModel?.ModelName);
            PreviewCategory = DashIfEmpty(car.CarModel?.CarСategory?.CategoryName);
            PreviewMaxMass = car.CarModel?.MaxMass?.ToString() ?? "—";
            PreviewCurbMass = car.CarModel?.CurbMass?.ToString() ?? "—";
            PreviewBrakeDiff = car.CarModel?.BrakeForceDifference?.ToString() ?? "—";
            PreviewParkingBrake = car.CarModel?.ParkingBrake?.ToString() ?? "—";
            PreviewReserveBrake = car.CarModel?.ReserveBrake?.ToString() ?? "—";

            var lastDate = car.DataChecks
                .Select(d => d.DataDateTime)
                .Where(d => d.HasValue)
                .OrderByDescending(d => d)
                .FirstOrDefault();
            PreviewLastTest = lastDate?.ToString("dd.MM.yyyy") ?? "—";

            var axleRows = car.CarModel?.Axles
                .OrderBy(a => a.Order)
                .Select(a => $"{a.Order}: {a.RotationDirection}, {a.BrakeType}, {(a.HasParkingBrake ? "+" : "-")} стоян., {(a.HasRegulator ? "+" : "-")} рег.")
                .ToList() ?? new List<string>();
            PreviewAxles = new ObservableCollection<string>(axleRows);
        }

        private bool FilterCar(object obj)
        {
            if (obj is not CarRowViewModel row) return false;
            if (string.IsNullOrWhiteSpace(SearchText)) return true;

            var q = SearchText.Trim().ToLowerInvariant();
            return (row.Owner ?? "").ToLowerInvariant().Contains(q)
                || (row.StateNumber ?? "").ToLowerInvariant().Contains(q)
                || (row.Vin ?? "").ToLowerInvariant().Contains(q)
                || (row.BrandName ?? "").ToLowerInvariant().Contains(q)
                || (row.Model ?? "").ToLowerInvariant().Contains(q);
        }

        private void RebuildCarsView()
        {
            CarsView = CollectionViewSource.GetDefaultView(Cars);
            if (CarsView != null)
                CarsView.Filter = FilterCar;
            CarsView?.Refresh();
        }

        public MainViewModel(MainContext context)
        {
            _context = context;
            _service = new CarBrandService(_context);
            LoadCars();
        }

        public void LoadCars()
        {
            try { LoadCarsInternal(); }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadCars failed: {ex}");
                Cars = new ObservableCollection<CarRowViewModel>();
                RebuildCarsView();
                ResetPreview();
            }
        }

        private void LoadCarsInternal()
        {
            var list = _context.TheCars
                .Include(c => c.Owner)
                .Include(c => c.CarModel).ThenInclude(m => m!.CarBrand)
                .Include(c => c.DataChecks)
                .AsEnumerable()
                .Select(c => new CarRowViewModel
                {
                    CarId = c.Id,
                    OwnerId = c.OwnerId,
                    Owner = c.Owner?.Name ?? "",
                    StateNumber = c.GosNumber,
                    Vin = c.BodyNumber,
                    BrandName = c.CarModel?.CarBrand?.BrandName ?? "",
                    Model = c.CarModel?.ModelName ?? "",
                    LastTestDate = c.DataChecks
                        .Select(d => d.DataDateTime)
                        .Where(d => d.HasValue)
                        .OrderByDescending(d => d)
                        .FirstOrDefault()
                })
                .ToList();

            Cars = new ObservableCollection<CarRowViewModel>(list);
            RebuildCarsView();

            if (SelectedCar != null)
                SelectedCar = Cars.FirstOrDefault(c => c.CarId == SelectedCar.CarId);
            else
                ResetPreview();
        }
    }
}

