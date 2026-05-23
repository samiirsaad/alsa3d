using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlSa3d.App.Services;

namespace AlSa3d.App.ViewModels
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

        public MainViewModel(INavigationService navigationService, IDialogService dialogService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            
            // الانتقال للوحة التحكم افتراضياً
            NavigateTo("Dashboard");
        }

        [RelayCommand]
        private void NavigateTo(string viewName)
        {
            _navigationService.NavigateTo(viewName);
        }

        [RelayCommand]
        private void Logout()
        {
            if (_dialogService.ShowConfirm("هل تريد تسجيل الخروج؟", "تأكيد"))
            {
                // العودة لشاشة تسجيل الدخول
                var loginView = App.ServiceProvider.GetService<LoginView>();
                if (loginView != null)
                {
                    loginView.Show();
                    var currentWindow = System.Windows.Application.Current.MainWindow;
                    currentWindow?.Close();
                    System.Windows.Application.Current.MainWindow = loginView;
                }
            }
        }
    }
}
