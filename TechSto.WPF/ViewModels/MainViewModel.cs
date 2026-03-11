using TechSto.Core.Interfaces;
using TechSto.WPF.Services;

namespace TechSto.WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {

        private bool _isDeviceConnected;
        public SettingsViewModel SettingsVM { get; }
        public MeasurementsViewModel MeasurementsVM { get; }
        public ReportsViewModel ReportsVM { get; }
        public HelpViewModel HelpVM { get; }
        public AboutViewModel AboutVM { get; }

        public bool IsDeviceConnected
        {
            get => _isDeviceConnected;
            set { _isDeviceConnected = value; OnPropertyChanged(); }
        }

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
        }

        private void OnConnectionStateChanged(bool isConnected)
        {
            IsDeviceConnected = isConnected;
        }
    }
}
