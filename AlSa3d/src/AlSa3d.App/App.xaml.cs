using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using AlSa3d.Services.Interfaces;
using AlSa3d.Services.Implementations;
using AlSa3d.Infrastructure.Data;
using AlSa3d.App.ViewModels;
using AlSa3d.App.Views;
using AlSa3d.App.Services;

namespace AlSa3d.App
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            // تعيين Culture للعربية
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ar-EG");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ar-EG");
        }

        private void ConfigureServices(ServiceCollection services)
        {
            // تسجيل DbContext
            services.AddDbContext<AppDbContext>();

            // تسجيل الخدمات (Services)
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IFinancialService, FinancialService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUserService, UserService>();

            // تسجيل خدمات التطبيق
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IDialogService, DialogService>();

            // تسجيل ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<CustomerViewModel>();
            services.AddTransient<InvoiceViewModel>();
            services.AddTransient<ProductViewModel>();
            services.AddTransient<EmployeeViewModel>();
            services.AddTransient<FinancialViewModel>();
            services.AddTransient<ReportsViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<MainViewModel>();

            // تسجيل الشاشات (Views)
            services.AddTransient<LoginView>();
            services.AddTransient<DashboardView>();
            services.AddTransient<CustomerView>();
            services.AddTransient<InvoiceView>();
            services.AddTransient<ProductView>();
            services.AddTransient<EmployeeView>();
            services.AddTransient<FinancialView>();
            services.AddTransient<ReportsView>();
            services.AddTransient<SettingsView>();
            services.AddTransient<MainWindow>();
        }
    }
}
