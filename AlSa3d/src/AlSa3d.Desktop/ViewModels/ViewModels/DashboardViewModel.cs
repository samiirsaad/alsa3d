using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlSa3d.Core.DTOs;
using AlSa3d.Services.Interfaces;

namespace AlSa3d.Desktop.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;

        [ObservableProperty]
        private decimal _totalSales;

        [ObservableProperty]
        private int _salesCount;

        [ObservableProperty]
        private int _totalCustomers;

        [ObservableProperty]
        private int _totalProducts;

        [ObservableProperty]
        private int _lowStockCount;

        [ObservableProperty]
        private ObservableCollection<InvoiceDto> _recentInvoices = new();

        public DashboardViewModel(
            IInvoiceService invoiceService,
            ICustomerService customerService,
            IProductService productService)
        {
            _invoiceService = invoiceService;
            _customerService = customerService;
            _productService = productService;
            
            LoadDashboardData();
        }

        private async void LoadDashboardData()
        {
            try
            {
                var stats = await _invoiceService.GetDashboardStatsAsync();
                if (stats.Success)
                {
                    TotalSales = stats.Data.TotalRevenue;
                    SalesCount = stats.Data.TotalInvoices;
                }
                
                var customers = await _customerService.GetAllCustomersAsync();
                if (customers.Success)
                    TotalCustomers = customers.Data.Count();
                
                var products = await _productService.GetAllProductsAsync();
                if (products.Success)
                    TotalProducts = products.Data.Count();
                
                var lowStock = await _productService.GetLowStockProductsAsync();
                if (lowStock.Success)
                    LowStockCount = lowStock.Data.Count();
                
                var invoices = await _invoiceService.GetRecentInvoicesAsync(10);
                if (invoices.Success)
                    RecentInvoices = new ObservableCollection<InvoiceDto>(invoices.Data);
            }
            catch (Exception ex)
            {
            }
        }

        [RelayCommand]
        private void ViewAllInvoices()
        {
        }
    }
}
