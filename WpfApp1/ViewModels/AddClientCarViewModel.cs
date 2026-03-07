using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TechSto.BusinessLayer;
using TechSto.DataBase.Entity;

namespace TechSto.ViewModels
{
    public class PendingCarItem
    {
        public string GosNumber { get; set; } = "";
        public string Vin { get; set; } = "";
        public int CarModelId { get; set; }
        public string Display { get; set; } = "";
    }

    public class AddClientCarViewModel : ViewModelBase
    {
        private readonly AddClientCarService _addClientCarService;
        private readonly OwnerService _ownerService;
        private readonly CarBrandService _brandService;
        private readonly CarModelService _modelService;
        private readonly CarCategoryService _categoryService;
        private readonly int? _editingCarId;

        public event Action? DataSaved;

        private bool _isNewOwner = true;
        private string _ownerName = "";
        private string _stsNumber = "";
        private Owner? _selectedExistingOwner;
        private ObservableCollection<Owner> _owners = new();

        private string _gosNumber = "";
        private string _vin = "";
        private CarModel? _selectedCarModel;
        private ObservableCollection<CarModel> _carModels = new();

        private bool _isNewModelExpanded;
        private string _newModelName = "";
        private CarBrand? _selectedBrand;
        private bool _isNewBrandMode;
        private string _newBrandName = "";
        private CarСategory? _selectedCategory;
        private string _maxMass = "";
        private string _curbMass = "";
        private string _brakeForceDiff = "";
        private ParkingBrakeType? _selectedParkingBrake;
        private ReserveBrakeSystem? _selectedReserveBrake;
        private int _axleCount = 2;
        private ObservableCollection<AxleRowViewModel> _axles = new();
        private ObservableCollection<CarBrand> _brands = new();
        private ObservableCollection<CarСategory> _categories = new();
        private ObservableCollection<PendingCarItem> _pendingCars = new();

        public bool IsNewOwner
        {
            get => _isNewOwner;
            set
            {
                if (_isNewOwner == value) return;
                _isNewOwner = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsExistingOwner));
                OnPropertyChanged(nameof(CanEditOwnerFields));
            }
        }

        public bool IsExistingOwner
        {
            get => !_isNewOwner;
            set
            {
                if (_isNewOwner == !value) return;
                _isNewOwner = !value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNewOwner));
                OnPropertyChanged(nameof(CanEditOwnerFields));
            }
        }

        public string OwnerName
        {
            get => _ownerName;
            set { _ownerName = value; OnPropertyChanged(); }
        }

        public string STSNumber
        {
            get => _stsNumber;
            set { _stsNumber = value; OnPropertyChanged(); }
        }

        public Owner? SelectedExistingOwner
        {
            get => _selectedExistingOwner;
            set
            {
                _selectedExistingOwner = value;
                OnPropertyChanged();
                if (IsEditMode && _selectedExistingOwner != null)
                {
                    OwnerName = _selectedExistingOwner.Name ?? "";
                    STSNumber = _selectedExistingOwner.STSNumber ?? "";
                }
            }
        }

        public ObservableCollection<Owner> Owners
        {
            get => _owners;
            set { _owners = value; OnPropertyChanged(); }
        }

        public string GosNumber
        {
            get => _gosNumber;
            set { _gosNumber = value; OnPropertyChanged(); }
        }

        public string Vin
        {
            get => _vin;
            set { _vin = value; OnPropertyChanged(); }
        }

        public CarModel? SelectedCarModel
        {
            get => _selectedCarModel;
            set { _selectedCarModel = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasSelectedModel)); }
        }

        public bool HasSelectedModel => _selectedCarModel != null;

        public ObservableCollection<CarModel> CarModels
        {
            get => _carModels;
            set { _carModels = value; OnPropertyChanged(); }
        }

        public bool IsNewModelExpanded
        {
            get => _isNewModelExpanded;
            set { _isNewModelExpanded = value; OnPropertyChanged(); }
        }

        public string NewModelName
        {
            get => _newModelName;
            set { _newModelName = value; OnPropertyChanged(); }
        }

        public CarBrand? SelectedBrand
        {
            get => _selectedBrand;
            set { _selectedBrand = value; OnPropertyChanged(); }
        }

        public bool IsNewBrandMode
        {
            get => _isNewBrandMode;
            set { _isNewBrandMode = value; OnPropertyChanged(); }
        }

        public string NewBrandName
        {
            get => _newBrandName;
            set { _newBrandName = value; OnPropertyChanged(); }
        }

        public CarСategory? SelectedCategory
        {
            get => _selectedCategory;
            set { _selectedCategory = value; OnPropertyChanged(); }
        }

        public string MaxMass
        {
            get => _maxMass;
            set { _maxMass = value; OnPropertyChanged(); }
        }

        public string CurbMass
        {
            get => _curbMass;
            set { _curbMass = value; OnPropertyChanged(); }
        }

        public string BrakeForceDiff
        {
            get => _brakeForceDiff;
            set { _brakeForceDiff = value; OnPropertyChanged(); }
        }

        public ParkingBrakeType? SelectedParkingBrake
        {
            get => _selectedParkingBrake;
            set { _selectedParkingBrake = value; OnPropertyChanged(); }
        }

        public ReserveBrakeSystem? SelectedReserveBrake
        {
            get => _selectedReserveBrake;
            set { _selectedReserveBrake = value; OnPropertyChanged(); }
        }

        public int AxleCount
        {
            get => _axleCount;
            set
            {
                if (value < 1 || value > 6) return;
                _axleCount = value;
                OnPropertyChanged();
                RebuildAxles();
            }
        }

        public ObservableCollection<AxleRowViewModel> Axles
        {
            get => _axles;
            set { _axles = value; OnPropertyChanged(); }
        }

        public ObservableCollection<CarBrand> Brands
        {
            get => _brands;
            set { _brands = value; OnPropertyChanged(); }
        }

        public ObservableCollection<CarСategory> Categories
        {
            get => _categories;
            set { _categories = value; OnPropertyChanged(); }
        }

        public ObservableCollection<PendingCarItem> PendingCars
        {
            get => _pendingCars;
            set { _pendingCars = value; OnPropertyChanged(); }
        }

        public bool IsEditMode => _editingCarId.HasValue;
        public bool IsNotEditMode => !IsEditMode;
        public bool CanEditOwnerFields => IsNewOwner || IsEditMode;

        // enum-списки для комбобоксов в форме новой модели
        public IEnumerable<ParkingBrakeType?> ParkingBrakeChoices { get; } =
            new ParkingBrakeType?[] { null, ParkingBrakeType.Foot, ParkingBrakeType.Hand };

        public IEnumerable<ReserveBrakeSystem?> ReserveBrakeChoices { get; } =
            Enum.GetValues<ReserveBrakeSystem>().Cast<ReserveBrakeSystem?>().Prepend(null).ToArray();

        public IEnumerable<RotationDirection> RotationDirectionChoices { get; } =
            Enum.GetValues<RotationDirection>();

        public IEnumerable<BrakeType> BrakeTypeChoices { get; } =
            Enum.GetValues<BrakeType>();

        public ICommand SaveCommand { get; }
        public ICommand AddAnotherCarCommand { get; }
        public ICommand CreateBrandCommand { get; }
        public ICommand CreateModelCommand { get; }
        public ICommand ToggleNewModelCommand { get; }
        public ICommand ToggleNewBrandCommand { get; }
        public ICommand IncreaseAxleCountCommand { get; }
        public ICommand DecreaseAxleCountCommand { get; }

        public AddClientCarViewModel(MainContext context, Owner? existingOwner = null, TheCar? existingCar = null)
        {
            _addClientCarService = new AddClientCarService(context);
            _ownerService = new OwnerService(context);
            _brandService = new CarBrandService(context);
            _modelService = new CarModelService(context);
            _categoryService = new CarCategoryService(context);
            _editingCarId = existingCar?.Id;

            SaveCommand = new RelayCommand(ExecuteSave);
            AddAnotherCarCommand = new RelayCommand(ExecuteAddAnotherCar);
            CreateBrandCommand = new RelayCommand(ExecuteCreateBrand);
            CreateModelCommand = new RelayCommand(ExecuteCreateModel);
            ToggleNewModelCommand = new RelayCommand(_ => IsNewModelExpanded = !IsNewModelExpanded);
            ToggleNewBrandCommand = new RelayCommand(_ => IsNewBrandMode = !IsNewBrandMode);
            IncreaseAxleCountCommand = new RelayCommand(_ => AxleCount++);
            DecreaseAxleCountCommand = new RelayCommand(_ => AxleCount--);

            LoadData();

            if (existingCar != null)
            {
                IsNewOwner = false;
                SelectedExistingOwner = Owners.FirstOrDefault(o => o.Id == existingCar.OwnerId);
                OwnerName = SelectedExistingOwner?.Name ?? "";
                STSNumber = SelectedExistingOwner?.STSNumber ?? "";
                GosNumber = existingCar.GosNumber;
                Vin = existingCar.BodyNumber;
                SelectedCarModel = CarModels.FirstOrDefault(m => m.Id == existingCar.CarModelId);
            }
            else if (existingOwner != null)
            {
                IsNewOwner = false;
                SelectedExistingOwner = Owners.FirstOrDefault(o => o.Id == existingOwner.Id);
            }

            RebuildAxles();
        }

        private void LoadData()
        {
            try
            {
                Owners = new ObservableCollection<Owner>(_ownerService.GetAll().ToList());
                Brands = new ObservableCollection<CarBrand>(_brandService.GetAll());
                Categories = new ObservableCollection<CarСategory>(_categoryService.GetAll());
                LoadCarModels();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Properties.Resources.ErrorLoadData, ex.Message), Properties.Resources.ErrorTitle,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCarModels()
        {
            CarModels = new ObservableCollection<CarModel>(_modelService.GetAllWithBrands());
        }

        private void RebuildAxles()
        {
            var existing = Axles ?? new ObservableCollection<AxleRowViewModel>();
            var newAxles = new ObservableCollection<AxleRowViewModel>();
            for (int i = 1; i <= _axleCount; i++)
            {
                var prev = existing.FirstOrDefault(a => a.Order == i);
                newAxles.Add(prev ?? AxleRowViewModel.Default(i));
            }
            Axles = newAxles;
        }

        private void ExecuteCreateBrand(object? _)
        {
            if (string.IsNullOrWhiteSpace(NewBrandName))
            {
                MessageBox.Show(Properties.Resources.WarnEnterBrandName, Properties.Resources.MessageWarning, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var brand = new CarBrand { BrandName = NewBrandName };
            _brandService.Add(brand);
            Brands.Add(brand);
            SelectedBrand = brand;
            IsNewBrandMode = false;
            NewBrandName = "";
        }

        private void ExecuteCreateModel(object? _)
        {
            if (string.IsNullOrWhiteSpace(NewModelName))
            {
                MessageBox.Show(Properties.Resources.WarnEnterModelName, Properties.Resources.MessageWarning, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (SelectedBrand == null)
            {
                MessageBox.Show(Properties.Resources.WarnSelectBrand, Properties.Resources.MessageWarning, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var model = new CarModel
            {
                ModelName = NewModelName,
                CarBrandId = SelectedBrand.Id,
                CarCategoryId = SelectedCategory?.Id,
                MaxMass = int.TryParse(MaxMass, out var mm) ? mm : null,
                CurbMass = int.TryParse(CurbMass, out var cm) ? cm : null,
                BrakeForceDifference = int.TryParse(BrakeForceDiff, out var bd) ? bd : null,
                ParkingBrake = SelectedParkingBrake,
                ReserveBrake = SelectedReserveBrake
            };

            foreach (var axleVm in Axles)
                model.Axles.Add(axleVm.ToAxle());

            try
            {
                _modelService.AddModelWithAxles(model);
                LoadCarModels();
                SelectedCarModel = CarModels.FirstOrDefault(m => m.Id == model.Id);
                IsNewModelExpanded = false;
                ClearNewModelForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Properties.Resources.ErrorCreateModel, ex.Message), Properties.Resources.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearNewModelForm()
        {
            NewModelName = "";
            SelectedBrand = null;
            SelectedCategory = null;
            MaxMass = "";
            CurbMass = "";
            BrakeForceDiff = "";
            SelectedParkingBrake = null;
            SelectedReserveBrake = null;
            AxleCount = 2;
        }

        private void ExecuteAddAnotherCar(object? _)
        {
            if (string.IsNullOrWhiteSpace(GosNumber))
            {
                MessageBox.Show(Properties.Resources.WarnEnterGosNumber, Properties.Resources.MessageWarning, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (SelectedCarModel == null)
            {
                MessageBox.Show(Properties.Resources.WarnSelectCarModel, Properties.Resources.MessageWarning, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            PendingCars.Add(new PendingCarItem
            {
                GosNumber = GosNumber,
                Vin = Vin,
                CarModelId = SelectedCarModel.Id,
                Display = $"{GosNumber} — {SelectedCarModel.CarBrand?.BrandName} {SelectedCarModel.ModelName}"
            });

            GosNumber = "";
            Vin = "";
            SelectedCarModel = null;
        }

        private void ExecuteSave(object? _)
        {
            if (IsNewOwner && string.IsNullOrWhiteSpace(OwnerName))
            {
                MessageBox.Show(Properties.Resources.WarnEnterOwnerName, Properties.Resources.MessageWarning, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!IsNewOwner && SelectedExistingOwner == null)
            {
                MessageBox.Show(Properties.Resources.WarnSelectOwner, Properties.Resources.MessageWarning, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (IsEditMode)
            {
                if (string.IsNullOrWhiteSpace(GosNumber))
                {
                    MessageBox.Show(Properties.Resources.WarnEnterGosNumber, Properties.Resources.MessageWarning, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (SelectedCarModel == null)
                {
                    MessageBox.Show(Properties.Resources.WarnSelectCarModel, Properties.Resources.MessageWarning, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(GosNumber) && SelectedCarModel != null)
                {
                    PendingCars.Add(new PendingCarItem
                    {
                        GosNumber = GosNumber,
                        Vin = Vin,
                        CarModelId = SelectedCarModel.Id,
                        Display = $"{GosNumber} — {SelectedCarModel.CarBrand?.BrandName} {SelectedCarModel.ModelName}"
                    });
                }

                if (PendingCars.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.WarnAddAtLeastOneCar, Properties.Resources.MessageWarning, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            try
            {
                if (IsEditMode)
                {
                    _addClientCarService.UpdateClientCar(new UpdateClientCarDto
                    {
                        CarId = _editingCarId!.Value,
                        GosNumber = GosNumber,
                        BodyNumber = Vin ?? "",
                        CarModelId = SelectedCarModel!.Id,
                        OwnerId = SelectedExistingOwner?.Id,
                        OwnerName = OwnerName,
                        STSNumber = STSNumber ?? ""
                    });
                }
                else
                {
                    _addClientCarService.SaveNewClientWithCars(new SaveNewClientDto
                    {
                        IsNewOwner = IsNewOwner,
                        OwnerName = OwnerName,
                        STSNumber = STSNumber ?? "",
                        ExistingOwnerId = SelectedExistingOwner?.Id,
                        ExistingOwnerName = OwnerName,
                        ExistingOwnerSTS = STSNumber ?? "",
                        Cars = PendingCars.Select(p => new ClientCarItemDto
                        {
                            GosNumber = p.GosNumber,
                            BodyNumber = p.Vin ?? "",
                            CarModelId = p.CarModelId
                        }).ToList()
                    });
                }

                DataSaved?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Properties.Resources.ErrorSaveData, ex.Message), Properties.Resources.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
