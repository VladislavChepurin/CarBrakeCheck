using System.Diagnostics;
using System.Windows;

namespace TechSto.WPF.SecondWindow
{
    /// <summary>
    /// Логика взаимодействия для lientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {        
      
        public ClientWindow()
        {
            InitializeComponent();           
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
