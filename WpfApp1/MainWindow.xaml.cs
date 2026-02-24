using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TechSto.DataBase.Entity;
using TechSto.ViewModels;

namespace TechSto
{
    public partial class MainWindow : Window
    {
        private Border _selectedTab;
        private AppSettings _settings;
        private Dictionary<string, (Border tab, FrameworkElement content)> _tabMapping;
        private readonly MainContext _context;
               
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                _context = new MainContext();
                DataContext = new MainViewModel(_context);
                // Инициализация в правильном порядке
                InitializeSettings();
                InitializeTabMapping();
                SubscribeToEvents();
                ApplyWindowSettings();
                InitializeLanguage();
               
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации окна: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }                 
        }

        private void InitializeSettings()
        {
            // Всегда получаем валидный объект настроек
            _settings = AppSettings.Load();

            // Дополнительная проверка (на всякий случай)
            if (_settings == null)
            {
                _settings = new AppSettings();
            }
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
            App.LanguageChanged += OnLanguageChanged;
        }

        private void ApplyWindowSettings()
        {
            if (_settings == null) return;

            this.Width = _settings.WindowWidth;
            this.Height = _settings.WindowHeight;
            this.Left = _settings.WindowLeft;
            this.Top = _settings.WindowTop;
            this.WindowState = _settings.IsMaximized ? WindowState.Maximized : WindowState.Normal;
        }

        private void InitializeLanguage()
        {
            if (_settings == null) return;

            App.SetLanguage(_settings.Language);
            SetSelectedLanguage();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_settings != null)
            {
                RestoreLastSelectedTab();
            }
        }              

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                if (_settings != null)
                {
                    SaveWindowSettings();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
            }
            finally
            {
                App.LanguageChanged -= OnLanguageChanged;
                _context?.Dispose();
            }
        }

        private void SaveWindowSettings()
        {
            if (_settings == null) return;

            try
            {
                if (this.WindowState == WindowState.Normal)
                {
                    _settings.WindowWidth = this.Width;
                    _settings.WindowHeight = this.Height;
                    _settings.WindowLeft = this.Left;
                    _settings.WindowTop = this.Top;
                }
                _settings.IsMaximized = this.WindowState == WindowState.Maximized;
                _settings.Save();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving window settings: {ex.Message}");
            }
        }

        // ========== МЕТОДЫ ДЛЯ РАБОТЫ С ЯЗЫКОМ ==========

        private void OnLanguageChanged(object sender, EventArgs e)
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

                if (SearchLabel != null)
                    SearchLabel.Text = Properties.Resources.SearchLabel;



                //if (BrandsManager != null)
                //{
                //    BrandsManager.Title = Properties.Resources.BrandsWindowTitle;
                //    BrandsManager.ButtonAdd = Properties.Resources.AddBth;
                //    BrandsManager.ButtonUpdate = Properties.Resources.UpdateBth;
                //    BrandsManager.ButtonDelete = Properties.Resources.DeleteBth;
                //    BrandsManager.ButtonChoose = Properties.Resources.ChooseBtn;
                //    BrandsManager.NameElement = Properties.Resources.NameElement;
                //    BrandsManager.EnterName = Properties.Resources.EnterName;
                //    BrandsManager.MessageWarning = Properties.Resources.MessageWarning;
                //    BrandsManager.SelectItemToEdit = Properties.Resources.SelectItemToEdit;
                //    BrandsManager.SelectItemToDelete = Properties.Resources.SelectItemToDelete;
                //    BrandsManager.DeleteSelectedItem = Properties.Resources.DeleteSelectedItem;
                //    BrandsManager.SelectItemToChoose = Properties.Resources.SelectItemToChoose;
                //}

                // Обновляем заголовок окна
                this.Title = Properties.Resources.NameProgram;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating texts: {ex.Message}");
            }
        }

        private void SetSelectedLanguage()
        {
            if (_settings == null || LanguageComboBox == null) return;

            try
            {
                foreach (ComboBoxItem item in LanguageComboBox.Items)
                {
                    if (item.Tag?.ToString() == _settings.Language)
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

        private void LanguageComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_settings == null) return;

            try
            {
                if (LanguageComboBox.SelectedItem is ComboBoxItem selectedItem)
                {
                    string cultureCode = selectedItem.Tag?.ToString() ?? "ru-RU";
                    _settings.Language = cultureCode;
                    _settings.Save();
                    App.SetLanguage(cultureCode);
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
            if (_settings == null || _tabMapping == null) return;

            try
            {
                if (_tabMapping.TryGetValue(_settings.LastSelectedTab, out var tabInfo))
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
            if (_settings == null || _tabMapping == null) return;

            try
            {
                if (_tabMapping.TryGetValue(tabName, out var tabInfo))
                {
                    SelectTab(tabInfo.tab, tabInfo.content);
                    _settings.LastSelectedTab = tabName;
                    _settings.Save();
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
                {
                    ResetTabStyle(_selectedTab);
                }

                ApplySelectedTabStyle(tab);
                HideAllContent();

                if (content != null)
                {
                    content.Visibility = Visibility.Visible;
                }

                _selectedTab = tab;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error selecting tab: {ex.Message}");
            }
        }

        private void ApplySelectedTabStyle(Border tab)
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

        private void ResetTabStyle(Border tab)
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