using System;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using AlSa3d.Desktop.Services;
using AlSa3d.Desktop.Views;

namespace AlSa3d.Desktop.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        private string _currentUserName = "مدير النظام";

        [ObservableProperty]
        private string _currentRole = "مدير عام";

        [ObservableProperty]
        private DateTime _currentDate = DateTime.Now;

        [ObservableProperty]
        private object? _currentView;

        public ICommand NavigateCommand { get; }
        public ICommand LogoutCommand { get; }

        public MainViewModel(INavigationService navigationService, IDialogService dialogService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            NavigateCommand = new RelayCommand<string>(Navigate);
            LogoutCommand = new RelayCommand(Logout);
        }

        private void Navigate(string? viewName)
        {
            if (viewName == null) return;
            UserControl? view = viewName switch
            {
                "Dashboard" => _navigationService.CreateView<DashboardViewModel>(),
                "Customers" => _navigationService.CreateView<CustomerViewModel>(),
                "Invoices" => _navigationService.CreateView<InvoiceViewModel>(),
                "Products" => _navigationService.CreateView<ProductViewModel>(),
                "Employees" => _navigationService.CreateView<EmployeeViewModel>(),
                "Financial" => _navigationService.CreateView<FinancialViewModel>(),
                "Reports" => _navigationService.CreateView<ReportsViewModel>(),
                "Settings" => _navigationService.CreateView<SettingsViewModel>(),
                _ => null
            };
            if (view != null) CurrentView = view;
        }

        private void Logout()
        {
            if (!_dialogService.ShowConfirm("هل تريد تسجيل الخروج؟", "تأكيد")) return;

            var loginView = App.ServiceProvider.GetService<LoginView>();
            if (loginView != null)
            {
                var loginVm = App.ServiceProvider.GetService<LoginViewModel>();
                loginView.DataContext = loginVm;
                CurrentView = loginView;
            }
        }
    }
}
