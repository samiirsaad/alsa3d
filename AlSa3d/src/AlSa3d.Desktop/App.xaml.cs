using System;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AlSa3d.Core.Entities;
using AlSa3d.Core.Interfaces;
using AlSa3d.Services.Interfaces;
using AlSa3d.Services.Implementations;
using AlSa3d.Infrastructure.Data;
using AlSa3d.Infrastructure.Data.Repositories;
using AlSa3d.Desktop.ViewModels;
using AlSa3d.Desktop.Views;
using AlSa3d.Desktop.Services;

namespace AlSa3d.Desktop;

    public partial class App : Application
{
    private readonly IHost _host;
    private static IServiceScope? _appScope;

    public static IServiceProvider ServiceProvider => _appScope?.ServiceProvider!;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(AppContext.BaseDirectory);
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                config.AddEnvironmentVariables();
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddDebug();
            })
            .ConfigureServices((context, services) =>
            {
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlite(context.Configuration.GetConnectionString("DefaultConnection")));

                services.AddScoped<IInvoiceService, InvoiceService>();
                services.AddScoped<ICustomerService, CustomerService>();
                services.AddScoped<IEmployeeService, EmployeeService>();
                services.AddScoped<IFinancialService, FinancialService>();
                services.AddScoped<IProductService, ProductService>();
                services.AddScoped<IUserService, UserService>();

                services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
                services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

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

                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<IDialogService, DialogService>();
                services.AddSingleton<INotificationService, NotificationService>();

                services.AddSingleton<MainWindow>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();
        _appScope = _host.Services.CreateScope();

        using (var scope = _host.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        }

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        _appScope?.Dispose();
        await _host.StopAsync();
        _host.Dispose();

        base.OnExit(e);
    }
}
