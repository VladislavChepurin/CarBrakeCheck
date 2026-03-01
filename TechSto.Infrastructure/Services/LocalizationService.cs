using Microsoft.Extensions.Configuration;
using System.Globalization;
using TechSto.Core.Interfaces;

namespace TechSto.Infrastructure.Services
{
    public class LocalizationService : ILocalizationService
    {
        private CultureInfo _currentCulture;
        public CultureInfo CurrentCulture => _currentCulture;
        public event EventHandler LanguageChanged;

        public LocalizationService(IConfiguration configuration)
        {
            var defaultLanguage = configuration["Language"] ?? "ru-RU";
            SetLanguage(defaultLanguage);
        }

        public void SetLanguage(string cultureCode)
        {
            var culture = new CultureInfo(cultureCode);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            _currentCulture = culture;
            LanguageChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
