using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TechSto.WPF.ViewModels;

namespace TechSto.WPF.Converters
{
    public class TabStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Application.Current.Resources["NormalTabStyle"];

            string currentViewModelType = value.GetType().Name;
            string tabParameter = parameter.ToString();

            // Сопоставление параметров с типами ViewModel
            bool isSelected = tabParameter switch
            {
                "Settings" => currentViewModelType == nameof(SettingsViewModel),
                "Measurements" => currentViewModelType == nameof(MeasurementsViewModel),
                "Reports" => currentViewModelType == nameof(ReportsViewModel),
                "Help" => currentViewModelType == nameof(HelpViewModel),
                "About" => currentViewModelType == nameof(AboutViewModel),
                _ => false
            };

            // Возвращаем соответствующий стиль
            return isSelected
                ? Application.Current.Resources["SelectedTabStyleSet"]
                : Application.Current.Resources["NormalTabStyle"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
