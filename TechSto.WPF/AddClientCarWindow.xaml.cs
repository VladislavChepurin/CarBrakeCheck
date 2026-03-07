using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TechSto.Core.Interfaces;
using TechSto.WPF.ViewModels;

namespace TechSto.WPF
{
    public partial class AddClientCarWindow : Window
    {
        private readonly bool _isEditMode;

        private readonly AddClientCarViewModel _viewModel;

        public AddClientCarWindow(AddClientCarViewModel viewModel, ILocalizationService localizationService)
        {
            InitializeComponent();
            _viewModel = viewModel;          
            DataContext = _viewModel;
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
