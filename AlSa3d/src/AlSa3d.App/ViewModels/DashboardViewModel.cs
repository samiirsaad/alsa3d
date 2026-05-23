using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlSa3d.Services.Interfaces;

namespace AlSa3d.App.ViewModels
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
        private ObservableCollection<InvoiceDTO> _recentInvoices = new();

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
                // تحميل إحصائيات المبيعات
                var stats = await _invoiceService.GetDashboardStatsAsync();
                TotalSales = stats.TotalSales;
                SalesCount = stats.InvoiceCount;
                
                // تحميل عدد العملاء
                var customers = await _customerService.GetAllAsync();
                TotalCustomers = customers.Count;
                
                // تحميل المنتجات والمخزون
                var products = await _productService.GetAllAsync();
                TotalProducts = products.Count;
                LowStockCount = products.Count(p => p.StockQuantity < 10);
                
                // تحميل آخر الفواتير
                var invoices = await _invoiceService.GetRecentInvoicesAsync(10);
                RecentInvoices = new ObservableCollection<InvoiceDTO>(invoices);
            }
            catch (Exception ex)
            {
                // معالجة الأخطاء
            }
        }

        [RelayCommand]
        private void ViewAllInvoices()
        {
            // الانتقال لشاشة الفواتير
        }
    }
}
