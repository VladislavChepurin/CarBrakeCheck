using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TechSto.Core.Entities;
using TechSto.Infrastructure.Data;
using TechSto.WPF.ViewModels;

namespace TechSto.WPF
{
    public partial class AddClientCarWindow : Window
    {
        private readonly bool _isEditMode;

        public AddClientCarWindow(MainContext context, Owner? existingOwner = null, TheCar? existingCar = null)
        {
            InitializeComponent();
            var vm = new AddClientCarViewModel(context, existingOwner, existingCar);
            vm.DataSaved += Close;
            DataContext = vm;
            _isEditMode = existingCar != null;
            Title = _isEditMode ? $"{Properties.Resources.UpdateBth}: {Properties.Resources.AddClientWindowTitle}" : Properties.Resources.AddClientWindowTitle;

            //App.LanguageChanged += OnLangChanged;
            //Closed += (_, _) => App.LanguageChanged -= OnLangChanged;
        }

        private void OnLangChanged(object? sender, EventArgs e)
        {
            Title = _isEditMode ? $"{Properties.Resources.UpdateBth}: {Properties.Resources.AddClientWindowTitle}" : Properties.Resources.AddClientWindowTitle;
        }

        private void AxlesGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var cell = FindCell(e.OriginalSource as DependencyObject);
            if (cell != null && !cell.IsEditing && !cell.IsReadOnly)
            {
                cell.Focus();
                ((DataGrid)sender).BeginEdit(e);
            }
        }

        private static DataGridCell? FindCell(DependencyObject? obj)
        {
            while (obj != null)
            {
                if (obj is DataGridCell c) return c;
                obj = VisualTreeHelper.GetParent(obj);
            }
            return null;
        }
    }
}
