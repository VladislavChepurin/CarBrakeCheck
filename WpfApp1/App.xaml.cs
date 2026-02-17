using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace WpfApp1
{
    public partial class App : Application
    {
        public static IConfiguration Configuration { get; private set; }

        // Событие для уведомления об изменении языка
        public static event EventHandler LanguageChanged;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();
        }

        // Статический метод для установки языка
        public static void SetLanguage(string cultureCode)
        {
            try
            {
                var culture = new CultureInfo(cultureCode);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;

                // ПРАВИЛЬНЫЙ СПОСОБ: используем Properties.Resources напрямую
                // (WpfApp1.Properties.Resources - это статический класс)
                WpfApp1.Properties.Resources.Culture = culture;

                // Уведомляем все окна об изменении языка
                LanguageChanged?.Invoke(null, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при установке языка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}