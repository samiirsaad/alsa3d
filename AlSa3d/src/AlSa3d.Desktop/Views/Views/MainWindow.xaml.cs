using System.Windows;
using System.Windows.Controls;
using AlSa3d.Desktop.ViewModels;
using AlSa3d.Desktop.Services;

namespace AlSa3d.Desktop.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow(MainViewModel viewModel, INavigationService navigationService)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            if (navigationService is NavigationService navService)
            {
                navService.SetMainWindow(this);
            }
        }
    }
}
