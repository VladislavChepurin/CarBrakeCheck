using System.ComponentModel;
using System.Resources;
using TechSto.Core.Interfaces; // или где лежит ILocalizationService
using TechSto.WPF.Properties; // пространство имён ресурсов

namespace TechSto.WPF.Services
{
    public class LocalizationProvider : INotifyPropertyChanged
    {
        private readonly ILocalizationService _localizationService;
        private readonly ResourceManager _resourceManager;

        public LocalizationProvider(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
            _resourceManager = Resources.ResourceManager;

            // Подписка на событие смены языка
            _localizationService.LanguageChanged += (s, e) =>
                OnPropertyChanged(string.Empty); // обновить все привязки
        }

        // Индексатор для доступа по ключу ресурса
        public string this[string key] =>
            _resourceManager.GetString(key, _localizationService.CurrentCulture);

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}