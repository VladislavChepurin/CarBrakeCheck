using System.Globalization;
using System.Windows.Data;

namespace TechSto.WPF.Converters
{
    public class BoolToCheckResultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isPassed)
            {
                return isPassed
                    ? Properties.Resources.CheckPassed
                    : Properties.Resources.CheckFailed;
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
