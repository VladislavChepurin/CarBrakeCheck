using System.Windows.Input;
using TechSto.Core.Interfaces;
using TechSto.WPF.Services;

namespace TechSto.WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {              
        public SettingsViewModel SettingsVM { get; }
        public MeasurementsViewModel MeasurementsVM { get; }
        public ReportsViewModel ReportsVM { get; }
        public HelpViewModel HelpVM { get; }
        public AboutViewModel AboutVM { get; }

        private bool _isDeviceConnected;
        public bool IsDeviceConnected
        {
            get => _isDeviceConnected;
            set { _isDeviceConnected = value; OnPropertyChanged(); }
        }

        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public ICommand ShowSettingsCommand { get; }
        public ICommand ShowMeasurementsCommand { get; }
        public ICommand ShowReportsCommand { get; }
        public ICommand ShowHelpCommand { get; }
        public ICommand ShowAboutCommand { get; }


        private readonly ILocalizationService _localizationService;

        public LocalizationProvider LocalizationProvider { get; }

        public MainViewModel(
            SettingsViewModel settingsVM,
            MeasurementsViewModel measurementsVM,
            ReportsViewModel reportsVM,
            HelpViewModel helpVM,
            AboutViewModel aboutVM,
            ILocalizationService localizationService,
            LocalizationProvider localizationProvider)
        {
            SettingsVM = settingsVM;
            MeasurementsVM = measurementsVM;
            ReportsVM = reportsVM;
            HelpVM = helpVM;
            AboutVM = aboutVM;

            _localizationService = localizationService;
            LocalizationProvider = localizationProvider;

            OnConnectionStateChanged(false);

            ShowSettingsCommand = new RelayCommand(_ => CurrentViewModel = SettingsVM);
            ShowMeasurementsCommand = new RelayCommand(_ => CurrentViewModel = MeasurementsVM);
            ShowReportsCommand = new RelayCommand(_ => CurrentViewModel = ReportsVM);
            ShowHelpCommand = new RelayCommand(_ => CurrentViewModel = HelpVM);
            ShowAboutCommand = new RelayCommand(_ => CurrentViewModel = AboutVM);

            CurrentViewModel = SettingsVM;
        }

        private void OnConnectionStateChanged(bool isConnected)
        {
            IsDeviceConnected = isConnected;
        }
    }
}
