using System.Windows;
using System.Windows.Controls;
using AlSa3d.App.ViewModels;
using AlSa3d.App.Services;

namespace AlSa3d.App.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow(MainViewModel viewModel, INavigationService navigationService)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            // إعداد خدمة التنقل
            if (navigationService is NavigationService navService)
            {
                navService.ContentControl = this.FindName("PART_ContentHost") as ContentControl ?? 
                                            (this.Content as Grid)?.Children.OfType<Border>().FirstOrDefault()?
                                            .Child as ContentControl;
            }
        }
    }
}
