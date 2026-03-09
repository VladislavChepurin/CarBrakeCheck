using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using System.IO;
using System.Windows;
using TechSto.Core.Interfaces;
using TechSto.Infrastructure.Data;
using TechSto.Infrastructure.Services;
using TechSto.WPF.Services;
using TechSto.WPF.ViewModels;

namespace TechSto.WPF
{
    public partial class App : Application
    {
        private readonly IHost _host;
        private ILocalizationService _localizationService;

        public App()
        {
            _host = Host.CreateDefaultBuilder()

                .ConfigureServices((context, services) =>
                {
                    // Регистрация конфигурации из appsettings.json
                    services.AddSingleton<IConfiguration>(provider =>
                    {
                        var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                        return builder.Build();
                    });

                    // DbContext
                    services.AddDbContext<MainContext>(options =>
                        options.UseSqlite("Data Source=DataBase.db")
                        .UseLazyLoadingProxies());

                    // Репозитории
                    //services.AddScoped<IClientRecordRepository, ClientRecordRepository>();

                    // Сервисы приложения
                    services.AddScoped<IClientRecordService, ClientRecordService>();
                    services.AddScoped<ICarBrandService, CarBrandService>();
                    services.AddScoped<ICarModelService, CarModelService>();
                    services.AddScoped<ICarCategoryService, CarCategoryService>();
                    services.AddScoped<IOwnerService, OwnerService>();
                    services.AddScoped<ITheCarService, TheCarService>();
                    services.AddScoped<IAddClientCarService, AddClientCarService>();
                    services.AddScoped<ICheckService, CheckService>();

                    //services.AddScoped<ICarDetailsService, CarDetailsService>(); // если есть

                    // Сервисы инфраструктуры
                    services.AddSingleton<ILocalizationService, LocalizationService>();
                    services.AddSingleton<IAppSettingsService, AppSettingsService>();
                    services.AddSingleton<LocalizationProvider>();

                    // Сервис выбора (для связи между вкладками)
                    //services.AddSingleton<ISelectionService, SelectionService>();

                    //// ViewModel                    
                    services.AddTransient<MeasurementsViewModel>();
                    services.AddTransient<ReportsViewModel>();
                    services.AddTransient<HelpViewModel>();
                    services.AddTransient<AboutViewModel>();
                    services.AddTransient<SettingsViewModel>();
                    services.AddTransient<ClientViewModel>();
                    services.AddTransient<AddClientCarViewModel>();
                    services.AddTransient<ChecksWindowViewModel>();

                    // Окна
                    services.AddTransient<MainWindow>();
                    services.AddTransient<AddClientCarWindow>();
                    services.AddTransient<ChecksWindow>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();                        

            // Подписка на изменение языка для обновления ресурсов
            _localizationService = _host.Services.GetRequiredService<ILocalizationService>();
            _localizationService.LanguageChanged += OnLanguageChanged!;
            ApplyResourcesCulture(_localizationService.CurrentCulture);

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
            base.OnStartup(e);
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            ApplyResourcesCulture(_localizationService.CurrentCulture);
        }

        private void ApplyResourcesCulture(CultureInfo culture)
        {
            WPF.Properties.Resources.Culture = culture;
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            _localizationService.LanguageChanged -= OnLanguageChanged!;
            await _host.StopAsync();
            _host.Dispose();
            base.OnExit(e);
        }
    }
}