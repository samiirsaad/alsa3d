using System;
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
            switch (viewName)
            {
                case "Dashboard": _navigationService.NavigateTo<DashboardViewModel>(); break;
                case "Customers": _navigationService.NavigateTo<CustomerViewModel>(); break;
                case "Invoices": _navigationService.NavigateTo<InvoiceViewModel>(); break;
                case "Products": _navigationService.NavigateTo<ProductViewModel>(); break;
                case "Employees": _navigationService.NavigateTo<EmployeeViewModel>(); break;
                case "Financial": _navigationService.NavigateTo<FinancialViewModel>(); break;
                case "Reports": _navigationService.NavigateTo<ReportsViewModel>(); break;
                case "Settings": _navigationService.NavigateTo<SettingsViewModel>(); break;
            }
        }

        private void Logout()
        {
            if (_dialogService.ShowConfirm("هل تريد تسجيل الخروج؟", "تأكيد"))
            {
                var loginView = App.ServiceProvider.GetService<LoginView>();
                if (loginView != null)
                {
                    CurrentView = loginView;
                }
            }
        }
    }
}
