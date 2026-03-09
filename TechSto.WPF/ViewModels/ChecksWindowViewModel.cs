using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TechSto.Core.DTOs;
using TechSto.Core.Interfaces;
using TechSto.WPF.Services;

namespace TechSto.WPF.ViewModels
{
    public class ChecksWindowViewModel : ViewModelBase
    {
        private readonly ICheckService _checkService;
        private ObservableCollection<CheckDto>? _checks;
        private string? _ownerInfo;
        private string? _carInfo;
        private string? _gosNumber;
        private bool _hasChecks;

        public ObservableCollection<CheckDto>? Checks
        {
            get => _checks;
            set => SetProperty(ref _checks, value);
        }

        public string? OwnerInfo
        {
            get => _ownerInfo;
            set => SetProperty(ref _ownerInfo, value);
        }

        public string? CarInfo
        {
            get => _carInfo;
            set => SetProperty(ref _carInfo, value);
        }

        public string? GosNumber
        {
            get => _gosNumber;
            set => SetProperty(ref _gosNumber, value);
        }

        public bool HasChecks
        {
            get => _hasChecks;
            set => SetProperty(ref _hasChecks, value);
        }

        public LocalizationProvider LocalizationProvider { get; }

        public ICommand CloseCommand { get; }

        public ChecksWindowViewModel(ICheckService checkService, LocalizationProvider localizationProvider, int carId, string ownerName, string ownerSurname, string gosNumber, string carInfo)
        {
            _checkService = checkService;
            LocalizationProvider = localizationProvider;
            OwnerInfo = $"{ownerSurname} {ownerName}".Trim();
            GosNumber = gosNumber;
            CarInfo = carInfo;
            CloseCommand = new RelayCommand(_ => CloseWindow());
            LoadChecks(carId);
        }

        private void LoadChecks(int carId)
        {
            var checksList = _checkService.GetChecksDtoByCarId(carId);
            HasChecks = checksList.Any();
            Checks = new ObservableCollection<CheckDto>(checksList);
        }

        private void CloseWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}