using System.Windows;
using AlSa3d.Desktop.ViewModels;

namespace AlSa3d.Desktop.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.NavigateCommand.Execute("Dashboard");
        }
    }
}
