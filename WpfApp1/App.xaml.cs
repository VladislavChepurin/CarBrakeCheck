using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using System.IO;
using System.Windows;
using TechSto.WPF.DataBase.Entity;
using TechSto.WPF.ViewModels;

namespace TechSto.WPF
{
    public partial class App : Application
    {
        //private readonly IHost _host;
        //public App()
        //{
        //    _host = Host.CreateDefaultBuilder()
        //     .ConfigureServices((context, services) =>
        //     {
        //         // Регистрация контекста базы данных
        //         services.AddDbContext<MainContext>(options =>
        //             options.UseSqlite("Data Source=DataBase/DataBase.db")
        //                    .UseLazyLoadingProxies()); // если нужно

        //         // Регистрация главного окна
        //         services.AddSingleton<MainWindow>();

        //         // Регистрация ViewModel (если используете MVVM)
        //         services.AddTransient<MainViewModel>();
        //     })
        //     .Build();
        //}

        public static IConfiguration Configuration { get; private set; } = null!;

        public static event EventHandler? LanguageChanged;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();
        }

        public static void SetLanguage(string cultureCode)
        {
            try
            {
                var culture = new CultureInfo(cultureCode);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;

                TechSto.WPF.Properties.Resources.Culture = culture;

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