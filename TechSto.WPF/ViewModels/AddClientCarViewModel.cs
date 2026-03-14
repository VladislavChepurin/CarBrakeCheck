using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TechSto.Core.DTOs;
using TechSto.Core.Entities;
using TechSto.Core.Interfaces;
using TechSto.Core.Models;
using TechSto.WPF.Services;

namespace TechSto.WPF.ViewModels
{
    // Вспомогательный класс для отображения локализованных значений enum
    public class EnumDisplay<T>
    {
        public T? Value { get; set; }
        public string Display { get; set; } = string.Empty;
    }
    public class PendingCarItem
    {
        public string GosNumber { get; set; } = "";
        public string Vin { get; set; } = "";
        public int CarModelId { get; set; }
        public string Display { get; set; } = "";
    }

    public class AddClientCarViewModel : ViewModelBase
    {
        private readonly IAppSettingsService _appSettingsService;
        private readonly ILocalizationService _localizationService;
        private AppSettings _settings;

        private readonly IOwnerService _ownerService;
        private readonly ICarBrandService _brandService;
        private readonly ICarModelService _modelService;
        private readonly ICarCategoryService _categoryService;
        private readonly IAddClientCarService _addClientCarService;
        private readonly int? _editingCarId;

        public event EventHandler? DataSaved;

        // Состояние владельца
        private bool _isNewOwner = true;
        private string _ownerName = "";
        private string _ownerSurname = "";
        private Owner? _selectedExistingOwner;
        private ObservableCollection<Owner> _owners = new();

        // Данные автомобиля
        private string _gosNumber = "";
        private string _vin = "";
        private CarModel? _selectedCarModel;
        private ObservableCollection<CarModel> _carModels = new();

        // Новая модель (расширенная форма)
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

        // Справочники
        private ObservableCollection<CarBrand> _brands = new();
        private ObservableCollection<CarСategory> _categories = new();

        // Коллекции для временных автомобилей (при добавлении нескольких)
        private ObservableCollection<PendingCarItem> _pendingCars = new();

        // Локализованные коллекции для enum
        private ObservableCollection<EnumDisplay<ParkingBrakeType?>> _parkingBrakeChoices;
        private ObservableCollection<EnumDisplay<ReserveBrakeSystem?>> _reserveBrakeChoices;
        private ObservableCollection<EnumDisplay<RotationDirection?>> _rotationDirectionChoices;
        private ObservableCollection<EnumDisplay<BrakeType?>> _brakeTypeChoices;

        // Команды
        public ICommand SaveCommand { get; }
        public ICommand AddAnotherCarCommand { get; }
        public ICommand CreateBrandCommand { get; }
        public ICommand CreateModelCommand { get; }
        public ICommand ToggleNewModelCommand { get; }
        public ICommand ToggleNewBrandCommand { get; }
        public ICommand IncreaseAxleCountCommand { get; }
        public ICommand DecreaseAxleCountCommand { get; }

        public LocalizationProvider LocalizationProvider { get; }

        // ========== Свойства ==========

        public AppSettings SettingsModel
        {
            get => _settings;
            private set => SetProperty(ref _settings, value);
        }

        // Владелец
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

        public string OwnerSurname
        {
            get => _ownerSurname;
            set { _ownerSurname = value; OnPropertyChanged(); }
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
                    OwnerSurname = _selectedExistingOwner.Surname ?? "";
                }
            }
        }

        public ObservableCollection<Owner> Owners
        {
            get => _owners;
            set { _owners = value; OnPropertyChanged(); }
        }

        // Автомобиль
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

        // Новая модель
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

        // Справочники
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

        // Временные автомобили
        public ObservableCollection<PendingCarItem> PendingCars
        {
            get => _pendingCars;
            set { _pendingCars = value; OnPropertyChanged(); }
        }

        // Режимы
        public bool IsEditMode => _editingCarId.HasValue;
        public bool IsNotEditMode => !IsEditMode;
        public bool CanEditOwnerFields => IsNewOwner || IsEditMode;

        // Локализованные коллекции enum
        public ObservableCollection<EnumDisplay<ParkingBrakeType?>> ParkingBrakeChoices
        {
            get => _parkingBrakeChoices;
            set => SetProperty(ref _parkingBrakeChoices, value);
        }

        public ObservableCollection<EnumDisplay<ReserveBrakeSystem?>> ReserveBrakeChoices
        {
            get => _reserveBrakeChoices;
            set => SetProperty(ref _reserveBrakeChoices, value);
        }

        public ObservableCollection<EnumDisplay<RotationDirection?>> RotationDirectionChoices
        {
            get => _rotationDirectionChoices;
            set => SetProperty(ref _rotationDirectionChoices, value);
        }

        public ObservableCollection<EnumDisplay<BrakeType?>> BrakeTypeChoices
        {
            get => _brakeTypeChoices;
            set => SetProperty(ref _brakeTypeChoices, value);
        }

        // ========== Конструктор ==========

        public AddClientCarViewModel(
            IAppSettingsService appSettingsService,
            ILocalizationService localizationService,
            LocalizationProvider localizationProvider,
            IOwnerService ownerService,
            ICarBrandService brandService,
            ICarModelService modelService,
            ICarCategoryService categoryService,
            IAddClientCarService addClientCarService,
            Owner? existingOwner = null,
            TheCar? existingCar = null)
        {
            _appSettingsService = appSettingsService;
            _localizationService = localizationService;
            LocalizationProvider = localizationProvider;
            _ownerService = ownerService;
            _brandService = brandService;
            _modelService = modelService;
            _categoryService = categoryService;
            _addClientCarService = addClientCarService;
            _editingCarId = existingCar?.Id;

            // Инициализация команд
            SaveCommand = new RelayCommand(ExecuteSave);
            AddAnotherCarCommand = new RelayCommand(ExecuteAddAnotherCar);
            CreateBrandCommand = new RelayCommand(ExecuteCreateBrand);
            CreateModelCommand = new RelayCommand(ExecuteCreateModel);
            ToggleNewModelCommand = new RelayCommand( () => IsNewModelExpanded = !IsNewModelExpanded);
            ToggleNewBrandCommand = new RelayCommand( () => IsNewBrandMode = !IsNewBrandMode);
            IncreaseAxleCountCommand = new RelayCommand( () => AxleCount++);
            DecreaseAxleCountCommand = new RelayCommand( () => AxleCount--);

            // Загружаем настройки и язык
            SettingsModel = _appSettingsService.Load();
            _localizationService.SetLanguage(SettingsModel.Language);

            // Инициализация локализованных коллекций enum
            InitializeLocalizedEnums();
            SelectedReserveBrake = ReserveBrakeSystem.None;
            // Загрузка справочников из БД
            LoadData();

            // Если редактирование существующего автомобиля
            if (existingCar != null)
            {
                IsNewOwner = false;
                SelectedExistingOwner = Owners.FirstOrDefault(o => o.Id == existingCar.OwnerId);
                OwnerName = SelectedExistingOwner?.Name ?? "";
                OwnerSurname = SelectedExistingOwner?.Surname ?? "";
                GosNumber = existingCar.GosNumber;
                Vin = existingCar.VinCode;
                SelectedCarModel = CarModels.FirstOrDefault(m => m.Id == existingCar.CarModelId);
            }
            else if (existingOwner != null)
            {
                IsNewOwner = false;
                SelectedExistingOwner = Owners.FirstOrDefault(o => o.Id == existingOwner.Id);
            }

            RebuildAxles();
        }

        // ========== Инициализация локализованных enum коллекций ==========

        private void InitializeLocalizedEnums()
        {
            // ParkingBrakeType
            ParkingBrakeChoices = new ObservableCollection<EnumDisplay<ParkingBrakeType?>>();
            foreach (ParkingBrakeType value in Enum.GetValues(typeof(ParkingBrakeType)))
            {
                string key = $"ParkingBrakeType_{value}";
                ParkingBrakeChoices.Add(new EnumDisplay<ParkingBrakeType?>
                {
                    Value = value,
                    Display = LocalizationProvider[key] ?? value.ToString()
                });
            }

            // ReserveBrakeSystem
            ReserveBrakeChoices = new ObservableCollection<EnumDisplay<ReserveBrakeSystem?>>();
            foreach (ReserveBrakeSystem value in Enum.GetValues(typeof(ReserveBrakeSystem)))
            {
                string key = $"ReserveBrakeSystem_{value}";
                ReserveBrakeChoices.Add(new EnumDisplay<ReserveBrakeSystem?>
                {
                    Value = value,
                    Display = LocalizationProvider[key] ?? value.ToString()
                });
            }

            // RotationDirection
            RotationDirectionChoices = new ObservableCollection<EnumDisplay<RotationDirection?>>();           
            foreach (RotationDirection value in Enum.GetValues(typeof(RotationDirection)))
            {
                string key = $"RotationDirection_{value}";
                RotationDirectionChoices.Add(new EnumDisplay<RotationDirection?>
                {
                    Value = value,
                    Display = LocalizationProvider[key] ?? value.ToString()
                });
            }

            // BrakeType
            BrakeTypeChoices = new ObservableCollection<EnumDisplay<BrakeType?>>();
            foreach (BrakeType value in Enum.GetValues(typeof(BrakeType)))
            {
                string key = $"BrakeType_{value}";
                BrakeTypeChoices.Add(new EnumDisplay<BrakeType?>
                {
                    Value = value,
                    Display = LocalizationProvider[key] ?? value.ToString()
                });
            }
        }

        // ========== Загрузка данных ==========

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
                MessageBox.Show(string.Format(Properties.Resources.ErrorLoadData, ex.Message),
                    Properties.Resources.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
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

        // ========== Команды ==========

        private void ExecuteCreateBrand()
        {
            if (string.IsNullOrWhiteSpace(NewBrandName))
            {
                MessageBox.Show(LocalizationProvider["WarnEnterBrandName"],
                    LocalizationProvider["MessageWarning"], MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var brand = new CarBrand { BrandName = NewBrandName };
            try
            {
                _brandService.Add(brand);
                Brands.Add(brand);
                SelectedBrand = brand;
                IsNewBrandMode = false;
                NewBrandName = "";
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                MessageBox.Show("Test"); // Заглушка, заменить на реальную обработку
            }
        }

        private void ExecuteCreateModel()
        {
            if (string.IsNullOrWhiteSpace(NewModelName))
            {
                MessageBox.Show(LocalizationProvider["WarnEnterModelName"],
                    LocalizationProvider["MessageWarning"], MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (SelectedBrand == null)
            {
                MessageBox.Show(LocalizationProvider["WarnSelectBrand"],
                    LocalizationProvider["MessageWarning"], MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show(string.Format(LocalizationProvider["ErrorCreateModel"], ex.Message),
                    LocalizationProvider["ErrorTitle"], MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void ExecuteAddAnotherCar()
        {
            if (string.IsNullOrWhiteSpace(GosNumber))
            {
                MessageBox.Show(LocalizationProvider["WarnEnterGosNumber"],
                    LocalizationProvider["MessageWarning"], MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (SelectedCarModel == null)
            {
                MessageBox.Show(LocalizationProvider["WarnSelectCarModel"],
                    LocalizationProvider["MessageWarning"], MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private void ExecuteSave()
        {
            // Валидация владельца
            if (IsNewOwner && string.IsNullOrWhiteSpace(OwnerName))
            {
                MessageBox.Show(LocalizationProvider["WarnEnterOwnerName"],
                    LocalizationProvider["MessageWarning"], MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!IsNewOwner && SelectedExistingOwner == null)
            {
                MessageBox.Show(LocalizationProvider["WarnSelectOwner"],
                    LocalizationProvider["MessageWarning"], MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Для режима редактирования проверяем поля автомобиля
            if (IsEditMode)
            {
                if (string.IsNullOrWhiteSpace(GosNumber))
                {
                    MessageBox.Show(LocalizationProvider["WarnEnterGosNumber"],
                        LocalizationProvider["MessageWarning"], MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (SelectedCarModel == null)
                {
                    MessageBox.Show(LocalizationProvider["WarnSelectCarModel"],
                        LocalizationProvider["MessageWarning"], MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else // Для добавления нового клиента с несколькими авто
            {
                // Добавляем текущий автомобиль, если он заполнен
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
                    MessageBox.Show(LocalizationProvider["WarnAddAtLeastOneCar"],
                        LocalizationProvider["MessageWarning"], MessageBoxButton.OK, MessageBoxImage.Warning);
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
                        VinCode = Vin ?? "",
                        CarModelId = SelectedCarModel!.Id,
                        OwnerId = SelectedExistingOwner?.Id,
                        OwnerName = OwnerName,
                        OwnerSurname = OwnerSurname ?? ""
                    });
                }
                else
                {
                    _addClientCarService.SaveNewClientWithCars(new SaveNewClientDto
                    {
                        IsNewOwner = IsNewOwner,
                        OwnerName = OwnerName,
                        OwnerSurname = OwnerSurname ?? "",
                        ExistingOwnerId = SelectedExistingOwner?.Id,
                        ExistingOwnerName = OwnerName,
                        ExistingOwnerSurname = OwnerSurname ?? "",
                        Cars = PendingCars.Select(p => new ClientCarItemDto
                        {
                            GosNumber = p.GosNumber,
                            VinCode = p.Vin ?? "",
                            CarModelId = p.CarModelId
                        }).ToList()
                    });
                }
                DataSaved?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(LocalizationProvider["ErrorSaveData"], ex.Message),
                    LocalizationProvider["ErrorTitle"], MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}