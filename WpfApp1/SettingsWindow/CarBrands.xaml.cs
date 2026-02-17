using System.Windows;
using System.Windows.Data;
using WpfApp1.BusinessLayer;
using WpfApp1.DataBase.Entity;

namespace WpfApp1.SettingsWindow
{
    /// <summary>
    /// Логика взаимодействия для CarBrands.xaml
    /// </summary>
    public partial class CarBrands : Window
    {
        private MainContext _context;
        private CarBrandService _service;   // сервис для работы с марками

        public CarBrands(MainContext context)
        {
            InitializeComponent();
            _context = context;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Создаём экземпляр сервиса (контекст будет создан внутри)
            _service = new CarBrandService(_context);

            // Получаем CollectionViewSource из ресурсов
            var viewSource = (CollectionViewSource)FindResource("BrandsViewSource");

            // Устанавливаем источником данных локальную наблюдаемую коллекцию
            viewSource.Source = _service.GetLocalBrands();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // Освобождаем ресурсы контекста
            //_context?.Dispose(); //нужно сохранить контекст
        }
    }
}
