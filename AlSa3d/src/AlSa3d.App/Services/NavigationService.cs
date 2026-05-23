using System.Windows;
using System.Windows.Controls;

namespace AlSa3d.App.Services
{
    public interface INavigationService
    {
        void NavigateTo(string viewName);
        void NavigateTo<TViewModel>() where TViewModel : class;
    }

    public class NavigationService : INavigationService
    {
        private ContentControl? _contentControl;
        private readonly IServiceProvider _serviceProvider;

        public ContentControl? ContentControl
        {
            get => _contentControl;
            set => _contentControl = value;
        }

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void NavigateTo(string viewName)
        {
            if (_contentControl == null) return;

            var view = viewName switch
            {
                "Dashboard" => _serviceProvider.GetService<DashboardView>(),
                "Customers" => _serviceProvider.GetService<CustomerView>(),
                "Invoices" => _serviceProvider.GetService<InvoiceView>(),
                "Products" => _serviceProvider.GetService<ProductView>(),
                "Employees" => _serviceProvider.GetService<EmployeeView>(),
                "Financial" => _serviceProvider.GetService<FinancialView>(),
                "Reports" => _serviceProvider.GetService<ReportsView>(),
                "Settings" => _serviceProvider.GetService<SettingsView>(),
                _ => null
            };

            if (view != null)
                _contentControl.Content = view;
        }

        public void NavigateTo<TViewModel>() where TViewModel : class
        {
            // تنفيذ مستقبلي للتنقل عبر ViewModel
        }
    }
}
