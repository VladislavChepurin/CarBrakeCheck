using System.ComponentModel;
using TechSto.Core.Interfaces;
using TechSto.WPF.Properties; // для доступа к ресурсам

namespace TechSto.WPF.Services
{
    public class LocalizationProvider : INotifyPropertyChanged
    {
        private readonly ILocalizationService _localizationService;

        public LocalizationProvider(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
            _localizationService.LanguageChanged += OnLanguageChanged;
        }

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            // Уведомляем об изменении всех привязок к индексатору
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        }

        // Индексатор для доступа к локализованным строкам по ключу
        public string this[string key]
        {
            get
            {
                return Resources.ResourceManager.GetString(key, _localizationService.CurrentCulture) ?? $"[{key}]";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}