using System.Windows;
using TechSto.WPF.ViewModels;

namespace TechSto.WPF
{
    public partial class MainWindow : Window
    {         
        private readonly MainViewModel _viewModel;

        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;           
            DataContext = _viewModel;                                       
        }        
    }
}