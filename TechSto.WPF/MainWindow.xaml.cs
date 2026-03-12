using System.Windows;
using System.Windows.Controls;
using TechSto.WPF.ViewModels;

namespace TechSto.WPF
{
    public partial class MainWindow : Window
    {  
        private Dictionary<string, (Border tab, FrameworkElement content)>? _tabMapping;
        private readonly MainViewModel _viewModel;

        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;           
            DataContext = _viewModel;                                       
        }        
    }
}