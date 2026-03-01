using System.Globalization;

namespace TechSto.Core.Interfaces
{
    public interface ILocalizationService
    {
        CultureInfo CurrentCulture { get; }
        event EventHandler LanguageChanged;
        void SetLanguage(string cultureCode);
    }
}
