using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TechSto.Core.Interfaces;
using TechSto.WPF.ViewModels;

namespace TechSto.WPF
{
    public partial class MainWindow : Window
    {
        private Border? _selectedTab;       
        private Dictionary<string, (Border tab, FrameworkElement content)>? _tabMapping;
        private readonly MainViewModel _viewModel;
        private readonly IAppSettingsService _appSettingsService;
        private readonly ILocalizationService _localizationService;

        public MainWindow(MainViewModel viewModel, IAppSettingsService appSettingsService, ILocalizationService localizationService)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _appSettingsService = appSettingsService;
            _localizationService = localizationService;
            DataContext = _viewModel;

            // Остальная инициализация
            InitializeTabMapping();
            SubscribeToEvents();
            InitializeLanguage();

            Loaded += OnLoaded;
            Closed += OnClosed;
        }           

        private void InitializeTabMapping()
        {
            _tabMapping = new Dictionary<string, (Border, FrameworkElement)>
            {
                ["Settings"] = (SettingsTab, SettingsContent),
                ["Measurements"] = (MeasurementsTab, MeasurementsContent),
                ["Reports"] = (ReportsTab, ReportsContent),
                ["Help"] = (HelpTab, HelpContent),
                ["About"] = (AboutTab, AboutContent)
            };
        }


        private void SubscribeToEvents()
        {
            _localizationService.LanguageChanged += OnLanguageChanged;
        }

        private void InitializeLanguage()
        {
            if (_viewModel.SettingsModel == null) return;

            // Устанавливаем язык из настроек через сервис локализации
            _localizationService.SetLanguage(_viewModel.SettingsModel.Language);
            SetSelectedLanguage();
        }    

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SettingsModel != null)
            {
                // Восстанавливаем размеры окна из настроек
                var settings = _viewModel.SettingsModel;
                //Width = settings.WindowWidth;
                //Height = settings.WindowHeight;
                //Left = settings.WindowLeft;
                //Top = settings.WindowTop;
                WindowState = settings.IsMaximized ? WindowState.Maximized : WindowState.Normal;

                // Восстанавливаем последнюю вкладку
                RestoreLastSelectedTab();
            }
        }

        private void OnClosed(object sender, EventArgs e)
        {
            try
            {
                if (_viewModel.SettingsModel != null)
                {
                    // Сохраняем размеры окна
                    var settings = _viewModel.SettingsModel;
                    if (WindowState == WindowState.Normal)
                    {
                        //settings.WindowWidth = Width;
                        //settings.WindowHeight = Height;
                        //settings.WindowLeft = Left;
                        //settings.WindowTop = Top;
                    }
                    settings.IsMaximized = WindowState == WindowState.Maximized;
                    _appSettingsService.Save(settings);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
            }
            finally
            {
                _localizationService.LanguageChanged -= OnLanguageChanged;
                //_viewModel.Dispose(); // если MainWindowViewModel реализует IDisposable
            }
        }

        // ========== МЕТОДЫ ДЛЯ РАБОТЫ С ЯЗЫКОМ ==========

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            try
            {
                UpdateAllTexts();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating texts: {ex.Message}");
            }
        }

        private void UpdateAllTexts()
        {
            try
            {
                // Обновляем текст вкладок
                if (SettingsTabText != null)
                    SettingsTabText.Text = Properties.Resources.Settings;

                if (MeasurementsTabText != null)
                    MeasurementsTabText.Text = Properties.Resources.Measurements;

                if (ReportsTabText != null)
                    ReportsTabText.Text = Properties.Resources.Reports;

                if (HelpTabText != null)
                    HelpTabText.Text = Properties.Resources.Help;

                if (AboutTabText != null)
                    AboutTabText.Text = Properties.Resources.About;

                // Обновляем текст в метке "Поиск"
                if (SearchLabel != null)
                    SearchLabel.Text = Properties.Resources.SearchLabel;

                // Обновляем текст в кнопках
                if (ButtonAddMain != null)
                    ButtonAddMain.Content = Properties.Resources.AddBth;

                if (ButtonUpdateMain != null)
                    ButtonUpdateMain.Content = Properties.Resources.UpdateBth;

                if (ButtonDeleteMain != null)
                    ButtonDeleteMain.Content = Properties.Resources.DeleteBth;

                // Обновляем заголовки колонок таблицы
                if (DataDridColumnOnwer != null)
                    DataDridColumnOnwer.Header = Properties.Resources.OnwerHeader;

                if (DataDridColumnCarNumber != null)
                    DataDridColumnCarNumber.Header = Properties.Resources.CarNumberHeader;

                if (DataDridColumnVinNumber != null)
                    DataDridColumnVinNumber.Header = Properties.Resources.VinNumberHeader;

                if (DataDridColumnCarBrand != null)
                    DataDridColumnCarBrand.Header = Properties.Resources.CarBrandHeader;

                if (DataDridColumnCarModel != null)
                    DataDridColumnCarModel.Header = Properties.Resources.CarModelHeader;

                if (DataDridColumnDateLastTest != null)
                    DataDridColumnDateLastTest.Header = Properties.Resources.DateLastTestHeader;

                // Обновляем заголовок окна
                Title = Properties.Resources.NameProgram;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating texts: {ex.Message}");
            }
        }

        private void SetSelectedLanguage()
        {
            if (_viewModel.SettingsModel == null || LanguageComboBox == null) return;

            try
            {
                foreach (ComboBoxItem item in LanguageComboBox.Items)
                {
                    if (item.Tag?.ToString() == _viewModel.SettingsModel.Language)
                    {
                        LanguageComboBox.SelectedItem = item;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting selected language: {ex.Message}");
            }
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel.SettingsModel == null) return;

            try
            {
                if (LanguageComboBox.SelectedItem is ComboBoxItem selectedItem)
                {
                    string cultureCode = selectedItem.Tag?.ToString() ?? "ru-RU";

                    // Сохраняем язык в настройках
                    _viewModel.SettingsModel.Language = cultureCode;
                    _appSettingsService.Save(_viewModel.SettingsModel);

                    // Меняем язык через сервис локализации
                    _localizationService.SetLanguage(cultureCode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при смене языка: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ========== МЕТОДЫ ДЛЯ РАБОТЫ С ВКЛАДКАМИ ==========

        private void RestoreLastSelectedTab()
        {
            if (_viewModel.SettingsModel == null || _tabMapping == null) return;

            try
            {
                if (_tabMapping.TryGetValue(_viewModel.SettingsModel.LastSelectedTab, out var tabInfo))
                {
                    SelectTab(tabInfo.tab, tabInfo.content);
                }
                else
                {
                    SelectTab(SettingsTab, SettingsContent);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error restoring tab: {ex.Message}");
                SelectTab(SettingsTab, SettingsContent);
            }
        }

        private void Tab_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border tab && tab.Tag is string tabName)
            {
                SwitchTab(tab, tabName);
            }
        }

        private void SwitchTab(Border clickedTab, string tabName)
        {
            if (_viewModel.SettingsModel == null || _tabMapping == null) return;

            try
            {
                if (_tabMapping.TryGetValue(tabName, out var tabInfo))
                {
                    SelectTab(tabInfo.tab, tabInfo.content);

                    // Сохраняем последнюю вкладку в настройках
                    _viewModel.SettingsModel.LastSelectedTab = tabName;
                    _appSettingsService.Save(_viewModel.SettingsModel);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error switching tab: {ex.Message}");
            }
        }

        private void SelectTab(Border tab, FrameworkElement content)
        {
            try
            {
                if (_selectedTab != null)
                    ResetTabStyle(_selectedTab);

                ApplySelectedTabStyle(tab);
                HideAllContent();

                if (content != null)
                    content.Visibility = Visibility.Visible;

                _selectedTab = tab;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error selecting tab: {ex.Message}");
            }
        }

        private static void ApplySelectedTabStyle(Border tab)
        {
            if (tab == null) return;

            try
            {
                var gradient = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 0)
                };

                gradient.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#4DA6FF"), 0));
                gradient.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#F0F8FF"), 0.8));

                tab.Background = gradient;
                tab.Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    ShadowDepth = 1,
                    Opacity = 0.2,
                    BlurRadius = 3
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying tab style: {ex.Message}");
            }
        }

        private static void ResetTabStyle(Border tab)
        {
            if (tab == null) return;

            try
            {
                tab.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E6F3FF"));
                tab.Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    ShadowDepth = 1,
                    Opacity = 0.1,
                    BlurRadius = 2
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error resetting tab style: {ex.Message}");
            }
        }

        private void HideAllContent()
        {
            try
            {
                if (SettingsContent != null) SettingsContent.Visibility = Visibility.Collapsed;
                if (MeasurementsContent != null) MeasurementsContent.Visibility = Visibility.Collapsed;
                if (ReportsContent != null) ReportsContent.Visibility = Visibility.Collapsed;
                if (HelpContent != null) HelpContent.Visibility = Visibility.Collapsed;
                if (AboutContent != null) AboutContent.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error hiding content: {ex.Message}");
            }
        }

       
    }
}