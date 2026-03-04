using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TechSto.WPF.Converters
{
    public class ModeToColorConverter : IValueConverter
    {
        // Цвет для активного режима (выбранного)
        public Brush ActiveBrush { get; set; } = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#80C1FF")); // синий
        // Цвет для неактивного режима
        public Brush InactiveBrush { get; set; } = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0")); // серый

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int selectedMode && parameter is string modeParam)
            {
                // Сравниваем значение с параметром (ожидается "0" или "1")
                if (selectedMode.ToString() == modeParam)
                    return ActiveBrush;
            }
            return InactiveBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
