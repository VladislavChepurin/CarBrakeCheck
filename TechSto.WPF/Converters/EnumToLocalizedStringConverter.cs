using System.Globalization;
using System.Windows.Data;
using TechSto.WPF.Services;

namespace TechSto.WPF.Converters
{
    public class EnumToLocalizedStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return string.Empty;
            var enumValue = values[0];
            if (values[1] is not LocalizationProvider localizationProvider || enumValue == null) return string.Empty;

            var type = enumValue.GetType();
            if (!type.IsEnum) return enumValue.ToString()!;

            string key = $"{type.Name}_{enumValue}";
            return localizationProvider[key] ?? enumValue.ToString()!;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
