using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AlSa3d.Services.Interfaces;
using AlSa3d.Services.Implementations;
using AlSa3d.Infrastructure.Data;
using AlSa3d.App.ViewModels;
using AlSa3d.App.Views;
using AlSa3d.App.Services;
using Serilog;
using System;
using System.IO;

namespace AlSa3d.App
{
    /// <summary>
    /// فئة التطبيق الرئيسية - إعداد الـ DI والـ Logging
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;
        public static ILogger<App>? Logger { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // إعداد Serilog
            ConfigureLogging();
            Logger?.LogInformation("🚀 بدء تطبيق Al-Sa3d...");

            try
            {
                var services = new ServiceCollection();
                ConfigureServices(services);
                ServiceProvider = services.BuildServiceProvider();

                // تعيين Culture للعربية
                System.Threading.Thread.CurrentThread.CurrentCulture = 
                    new System.Globalization.CultureInfo("ar-EG");
                System.Threading.Thread.CurrentThread.CurrentUICulture = 
                    new System.Globalization.CultureInfo("ar-EG");

                Logger?.LogInformation("✅ التطبيق جاهز للاستخدام");
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "❌ خطأ حرج في بدء التطبيق");
                MessageBox.Show($"فشل بدء التطبيق: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Shutdown();
            }
        }

        /// <summary>
        /// إعداد نظام Logging
        /// </summary>
        private void ConfigureLogging()
        {
            string logPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "AlSa3d",
                "Logs"
            );

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(
                    path: Path.Combine(logPath, "AlSa3d-.txt"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] {Level:u3} - {Message:lj}{NewLine}{Exception}"
                )
                .WriteTo.File(
                    path: Path.Combine(logPath, "Errors-.txt"),
                    rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] {Level:u3} - {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();
        }

        /// <summary>
        /// إعداد خدمات الـ Dependency Injection
        /// </summary>
        private void ConfigureServices(ServiceCollection services)
        {
            // تسجيل Logging
            services.AddLogging(config =>
            {
                config.ClearProviders();
                config.AddSerilog();
            });

            // تسجيل DbContext
            services.AddDbContext<AppDbContext>();

            // تسجيل الخدمات الأساسية (Services)
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IFinancialService, FinancialService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUserService, UserService>();

            // تسجيل خدمات التطبيق المساعدة
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

            // تسجيل Views (Windows)
            services.AddSingleton<MainWindow>();
            services.AddTransient<LoginView>();
            services.AddTransient<DashboardView>();
            services.AddTransient<CustomerView>();
            services.AddTransient<InvoiceView>();
            services.AddTransient<ProductView>();
            services.AddTransient<EmployeeView>();
            services.AddTransient<FinancialView>();
            services.AddTransient<ReportsView>();
            services.AddTransient<SettingsView>();
        }
    }
}
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
