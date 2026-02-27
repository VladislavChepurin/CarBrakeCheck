using System.Diagnostics;
using System.Windows;
using TechSto.DataBase.Entity;
using TechSto.ViewModels;

namespace TechSto.SecondWindow
{
    /// <summary>
    /// Логика взаимодействия для lientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        private readonly AppSettings? _settings;
        private readonly MainContext? _context;

        public ClientWindow(MainContext context)
        {
            InitializeComponent();

            try
            {
                _context = context;
                DataContext = new ClientViewModel(_context);
                _settings = AppSettings.Load() ?? new AppSettings();                                
                App.LanguageChanged += OnLanguageChanged;

                // Устанавливаем текущий язык и обновляем интерфейс
                UpdateAllTexts();
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации окна: {ex.Message}",
                      "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);      
                _context = null;   // явная инициализация null
                Application.Current.Shutdown();  // закрываем приложение, т.к. без контекста работать нельзя
                return;            // прерываем выполнение конструктора
            }
        }

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            UpdateAllTexts();
        }

        private void UpdateAllTexts()
        {
            try
            {
                Debug.WriteLine("Проверка события OnLanguageChanged");
            }
            catch (Exception ex)
            {

            }
        }
    }
}
