using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace TechSto.Converters
{
    public class DisplayMemberMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || values[0] == null || values[1] == null)
                return null;

            object item = values[0];
            string propertyName = values[1].ToString();
            PropertyInfo prop = item.GetType().GetProperty(propertyName);
            return prop?.GetValue(item);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}