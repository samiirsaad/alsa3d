# 🔧 الحلول والكود المطلوب لإصلاح المشروع

---

## ✅ الحل #1: UserService - إضافة HashPassword و VerifyPassword

### الملف: `src/AlSa3d.Services/Implementations/UserService.cs`

#### أضف هذه Methods في آخر الكلاس:

```csharp
/// <summary>
/// تحويل كلمة المرور إلى hash آمن
/// </summary>
private string HashPassword(string password)
{
    using (var sha256 = System.Security.Cryptography.SHA256.Create())
    {
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
    // ملاحظة: يُفضل استخدام BCrypt:
    // return BCrypt.Net.BCrypt.HashPassword(password);
}

/// <summary>
/// التحقق من كلمة المرور
/// </summary>
private bool VerifyPassword(string password, string hash)
{
    using (var sha256 = System.Security.Cryptography.SHA256.Create())
    {
        var hashOfInput = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        var hashOfInputBase64 = Convert.ToBase64String(hashOfInput);
        return hashOfInputBase64.Equals(hash);
    }
    // أو باستخدام BCrypt:
    // return BCrypt.Net.BCrypt.Verify(password, hash);
}

/// <summary>
/// تسجيل العمليات في سجل التدقيق
/// </summary>
private async Task LogAuditAsync(int userId, string action, string details)
{
    try
    {
        var auditLog = new AuditLog
        {
            UserId = userId,
            Action = action,
            Details = details,
            Timestamp = DateTime.Now,
            IPAddress = GetClientIPAddress()
        };

        await _auditLogRepository.AddAsync(auditLog);
        _logger.LogInformation("✅ تم تسجيل العملية: {action} للمستخدم: {userId}", action, userId);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "❌ فشل في تسجيل العملية");
    }
}

/// <summary>
/// الحصول على عنوان IP للعميل
/// </summary>
private string GetClientIPAddress()
{
    return "127.0.0.1"; // يجب الحصول عليه من HttpContext في الـ Controller
}
```

---

## ✅ الحل #2: ProductService - إضافة SearchProductsAsync

### الملف: `src/AlSa3d.Services/Implementations/ProductService.cs`

#### أضف هذه Method:

```csharp
/// <summary>
/// البحث عن المنتجات
/// </summary>
public async Task<Result<IEnumerable<Product>>> SearchProductsAsync(string searchTerm)
{
    try
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Result.Ok(await _productRepository.GetAllAsync());

        var products = await _productRepository.GetAllAsync(
            p => p.Category);

        var filteredProducts = products
            .Where(p => !p.IsDeleted && (
                p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                p.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                p.Barcode?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                p.Category?.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true
            ))
            .OrderBy(p => p.Name)
            .AsEnumerable();

        return Result.Ok(filteredProducts);
    }
    catch (Exception ex)
    {
        return Result.Failure<IEnumerable<Product>>($"فشل في البحث: {ex.Message}");
    }
}

/// <summary>
/// الحصول على المنتجات قليلة المخزون
/// </summary>
public async Task<Result<IEnumerable<Product>>> GetLowStockProductsAsync(int threshold = 10)
{
    try
    {
        var products = await _productRepository.GetAllAsync(
            p => p.Category,
            p => p.WarehouseProducts);

        var lowStockProducts = products
            .Where(p => !p.IsDeleted && p.IsActive &&
                p.WarehouseProducts.Sum(wp => wp.Quantity) < threshold)
            .OrderBy(p => p.WarehouseProducts.Sum(wp => wp.Quantity))
            .AsEnumerable();

        return Result.Ok(lowStockProducts);
    }
    catch (Exception ex)
    {
        return Result.Failure<IEnumerable<Product>>($"فشل في جلب المنتجات قليلة المخزون: {ex.Message}");
    }
}

/// <summary>
/// الحصول على كمية المنتج في المستودع
/// </summary>
public async Task<Result<int>> GetProductStockAsync(int productId, int warehouseId)
{
    try
    {
        var product = await _productRepository.GetByIdAsync(productId,
            p => p.WarehouseProducts);

        if (product == null || product.IsDeleted)
            return Result.Failure<int>("المنتج غير موجود");

        var stock = product.WarehouseProducts
            .Where(wp => wp.WarehouseId == warehouseId && !wp.IsDeleted)
            .Sum(wp => (int)wp.Quantity);

        return Result.Ok(stock);
    }
    catch (Exception ex)
    {
        return Result.Failure<int>($"فشل في جلب المخزون: {ex.Message}");
    }
}

/// <summary>
/// تحديث المخزون
/// </summary>
public async Task<Result<bool>> UpdateStockAsync(int productId, int warehouseId, int quantityChange)
{
    try
    {
        var product = await _productRepository.GetByIdAsync(productId,
            p => p.WarehouseProducts);

        if (product == null || product.IsDeleted)
            return Result.Failure<bool>("المنتج غير موجود");

        var warehouseProduct = product.WarehouseProducts
            .FirstOrDefault(wp => wp.WarehouseId == warehouseId && !wp.IsDeleted);

        if (warehouseProduct == null)
        {
            warehouseProduct = new WarehouseProduct
            {
                ProductId = productId,
                WarehouseId = warehouseId,
                Quantity = quantityChange,
                CreatedAt = DateTime.Now
            };
            product.WarehouseProducts.Add(warehouseProduct);
        }
        else
        {
            warehouseProduct.Quantity += quantityChange;
            warehouseProduct.UpdatedAt = DateTime.Now;
        }

        var result = await _productRepository.UpdateAsync(product);
        return Result.Ok(result.Success);
    }
    catch (Exception ex)
    {
        return Result.Failure<bool>($"فشل في تحديث المخزون: {ex.Message}");
    }
}

/// <summary>
/// إنشاء قاعدة تسعير جديدة
/// </summary>
public async Task<Result<PricingRule>> CreatePricingRuleAsync(CreatePricingRuleDto dto)
{
    try
    {
        var rule = new PricingRule
        {
            ProductId = dto.ProductId,
            MinQuantity = dto.MinQuantity,
            MaxQuantity = dto.MaxQuantity,
            DiscountPercentage = dto.DiscountPercentage,
            DiscountType = dto.DiscountType ?? "Percentage",
            StartDate = dto.StartDate ?? DateTime.Now,
            EndDate = dto.EndDate,
            IsActive = true,
            CreatedAt = DateTime.Now
        };

        var result = await _pricingRuleRepository.AddAsync(rule);
        return result;
    }
    catch (Exception ex)
    {
        return Result.Failure<PricingRule>($"فشل في إنشاء قاعدة التسعير: {ex.Message}");
    }
}
```

---

## ✅ الحل #3: InvoiceService - إضافة SearchInvoicesAsync

### الملف: `src/AlSa3d.Services/Implementations/InvoiceService.cs`

#### أضف هذه Methods:

```csharp
/// <summary>
/// البحث عن الفواتير
/// </summary>
public async Task<Result<IEnumerable<Invoice>>> SearchInvoicesAsync(
    string searchTerm,
    InvoiceType? type = null,
    DateTime? fromDate = null,
    DateTime? toDate = null)
{
    try
    {
        var invoices = await _invoiceRepository.GetAllAsync(
            i => i.Customer,
            i => i.Items,
            i => i.CreatedBy);

        var query = invoices.Where(i => !i.IsDeleted);

        // البحث بالنص
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(i =>
                i.InvoiceNumber.Contains(searchTerm) ||
                i.Customer?.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                i.Notes?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true);
        }

        // تصفية حسب النوع
        if (type.HasValue)
            query = query.Where(i => i.Type == type.Value);

        // تصفية حسب التاريخ
        if (fromDate.HasValue)
            query = query.Where(i => i.Date >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(i => i.Date <= toDate.Value);

        var result = query.OrderByDescending(i => i.Date).AsEnumerable();
        return Result.Ok(result);
    }
    catch (Exception ex)
    {
        return Result.Failure<IEnumerable<Invoice>>($"فشل في البحث: {ex.Message}");
    }
}

/// <summary>
/// موافقة على الفاتورة
/// </summary>
public async Task<Result<Invoice>> ApproveInvoiceAsync(int id)
{
    try
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id);
        if (invoice == null || invoice.IsDeleted)
            return Result.Failure<Invoice>("الفاتورة غير موجودة");

        invoice.Status = InvoiceStatus.Approved;
        invoice.ApprovedAt = DateTime.Now;
        invoice.UpdatedAt = DateTime.Now;

        var result = await _invoiceRepository.UpdateAsync(invoice);
        return result;
    }
    catch (Exception ex)
    {
        return Result.Failure<Invoice>($"فشل في الموافقة: {ex.Message}");
    }
}

/// <summary>
/// إلغاء الفاتورة
/// </summary>
public async Task<Result<Invoice>> CancelInvoiceAsync(int id, string reason)
{
    try
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id);
        if (invoice == null || invoice.IsDeleted)
            return Result.Failure<Invoice>("الفاتورة غير موجودة");

        if (invoice.Status == InvoiceStatus.Paid || invoice.Status == InvoiceStatus.Cancelled)
            return Result.Failure<Invoice>("لا يمكن إلغاء فاتورة مدفوعة أو ملغاة");

        invoice.Status = InvoiceStatus.Cancelled;
        invoice.CancellationReason = reason;
        invoice.CancelledAt = DateTime.Now;
        invoice.UpdatedAt = DateTime.Now;

        var result = await _invoiceRepository.UpdateAsync(invoice);
        return result;
    }
    catch (Exception ex)
    {
        return Result.Failure<Invoice>($"فشل في الإلغاء: {ex.Message}");
    }
}

/// <summary>
/// إنشاء إرجاع
/// </summary>
public async Task<Result<Return>> CreateReturnAsync(CreateReturnDto dto)
{
    try
    {
        var invoice = await _invoiceRepository.GetByIdAsync(dto.InvoiceId);
        if (invoice == null || invoice.IsDeleted)
            return Result.Failure<Return>("الفاتورة غير موجودة");

        var returnRecord = new Return
        {
            ReturnNumber = GenerateReturnNumber(),
            InvoiceId = dto.InvoiceId,
            Date = DateTime.Now,
            Reason = dto.Reason,
            Status = ReturnStatus.Pending,
            CreatedByUserId = dto.CreatedByUserId,
            CreatedAt = DateTime.Now
        };

        // إضافة عناصر الإرجاع
        decimal totalAmount = 0;
        foreach (var item in dto.Items)
        {
            var invoiceItem = invoice.Items.FirstOrDefault(x => x.ProductId == item.ProductId);
            if (invoiceItem == null)
                continue;

            var returnItem = new ReturnItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = invoiceItem.UnitPrice,
                Total = item.Quantity * invoiceItem.UnitPrice,
                Reason = item.Reason,
                CreatedAt = DateTime.Now
            };

            returnRecord.Items.Add(returnItem);
            totalAmount += returnItem.Total;
        }

        returnRecord.TotalAmount = totalAmount;

        var result = await _returnRepository.AddAsync(returnRecord);
        return result;
    }
    catch (Exception ex)
    {
        return Result.Failure<Return>($"فشل في إنشاء الإرجاع: {ex.Message}");
    }
}

/// <summary>
/// الحصول على رصيد العميل
/// </summary>
public async Task<Result<decimal>> GetCustomerBalanceAsync(int customerId)
{
    try
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null || customer.IsDeleted)
            return Result.Failure<decimal>("العميل غير موجود");

        return Result.Ok(customer.Balance);
    }
    catch (Exception ex)
    {
        return Result.Failure<decimal>($"فشل في جلب الرصيد: {ex.Message}");
    }
}

/// <summary>
/// الحصول على إحصائيات لوحة التحكم
/// </summary>
public async Task<Result<InvoiceDashboardDto>> GetDashboardStatsAsync()
{
    try
    {
        var invoices = await _invoiceRepository.GetAllAsync();
        var activeInvoices = invoices.Where(i => !i.IsDeleted).ToList();

        var stats = new InvoiceDashboardDto
        {
            TotalSales = activeInvoices.Sum(i => i.Total),
            PaidAmount = activeInvoices.Sum(i => i.PaidAmount),
            PendingAmount = activeInvoices.Sum(i => i.Remaining),
            InvoiceCount = activeInvoices.Count(),
            PaidInvoiceCount = activeInvoices.Count(i => i.Status == InvoiceStatus.Paid),
            PendingInvoiceCount = activeInvoices.Count(i => i.Status == InvoiceStatus.Pending),
            AvgInvoiceAmount = activeInvoices.Any() ? activeInvoices.Average(i => i.Total) : 0
        };

        return Result.Ok(stats);
    }
    catch (Exception ex)
    {
        return Result.Failure<InvoiceDashboardDto>($"فشل في جلب الإحصائيات: {ex.Message}");
    }
}

private string GenerateReturnNumber()
{
    return $"RET-{DateTime.Now:yyyyMMddHHmmss}";
}
```

---

## ✅ الحل #4: إضافة DTOs الناقصة

### 1. UserDtos.cs - أضف هذه Classes:

```csharp
/// <summary>
/// DTO لإنشاء دور جديد
/// </summary>
public class CreateRoleDto
{
    [Required(ErrorMessage = "اسم الدور مطلوب")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "اسم الدور يجب أن يكون بين 2 و 100 حرف")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "الوصف لا يجب أن يتجاوز 500 حرف")]
    public string? Description { get; set; }
}

/// <summary>
/// DTO للرد على تسجيل الدخول
/// </summary>
public class LoginResponseDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public DateTime LoginTime { get; set; }
    public string? Token { get; set; }
}
```

### 2. ProductDtos.cs - أضف هذه Classes:

```csharp
public class CreateCategoryDto
{
    [Required(ErrorMessage = "اسم الفئة مطلوب")]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int? ParentId { get; set; }
}

public class UpdateCategoryDto
{
    [Required(ErrorMessage = "اسم الفئة مطلوب")]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int? ParentId { get; set; }

    public bool? IsActive { get; set; }
}

public class UpdateWarehouseDto
{
    [Required(ErrorMessage = "اسم المستودع مطلوب")]
    public string Name { get; set; } = string.Empty;

    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? ManagerName { get; set; }
    public decimal? Capacity { get; set; }
    public bool? IsActive { get; set; }
}

public class CreatePricingRuleDto
{
    [Required(ErrorMessage = "معرف المنتج مطلوب")]
    public int ProductId { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "الكمية الدنيا يجب أن تكون موجبة")]
    public int MinQuantity { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "الكمية العليا يجب أن تكون موجبة")]
    public int MaxQuantity { get; set; }

    [Range(0, 100, ErrorMessage = "الخصم يجب أن يكون بين 0 و 100")]
    public decimal DiscountPercentage { get; set; }

    public string? DiscountType { get; set; } = "Percentage";
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
```

### 3. InvoiceDtos.cs - أضف هذه Classes:

```csharp
public class InvoiceDto
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string? CustomerName { get; set; }
    public DateTime Date { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public string StatusName { get; set; } = string.Empty;
}

public class InvoiceDashboardDto
{
    public decimal TotalSales { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal PendingAmount { get; set; }
    public int InvoiceCount { get; set; }
    public int PaidInvoiceCount { get; set; }
    public int PendingInvoiceCount { get; set; }
    public decimal AvgInvoiceAmount { get; set; }
}

public class PayInvoiceDto
{
    [Required(ErrorMessage = "معرف الفاتورة مطلوب")]
    public int InvoiceId { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "المبلغ يجب أن يكون موجب")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "طريقة الدفع مطلوبة")]
    public string PaymentMethod { get; set; } = "Cash";

    public string? Reference { get; set; }
    public string? Notes { get; set; }
}
```

### 4. EmployeeDtos.cs - أضف:

```csharp
public class AttendanceDto
{
    public int EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public AttendanceType Type { get; set; }
    public string? Notes { get; set; }
}

public class ProcessSalaryDto
{
    [Required(ErrorMessage = "معرف الموظف مطلوب")]
    public int EmployeeId { get; set; }

    [Range(1, 12)]
    public int Month { get; set; }

    [Range(2020, 2100)]
    public int Year { get; set; }

    public decimal? Overtime { get; set; }
    public decimal? Bonus { get; set; }
    public decimal? Deductions { get; set; }
}
```

### 5. FinancialDtos.cs - أضف:

```csharp
public class CreateExchangeRateDto
{
    [Required]
    public int FromCurrencyId { get; set; }

    [Required]
    public int ToCurrencyId { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Rate { get; set; }

    public DateTime? Date { get; set; }
}

public class ConvertCurrencyDto
{
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    public int FromCurrencyId { get; set; }

    [Required]
    public int ToCurrencyId { get; set; }
}
```

---

## ✅ الحل #5: إصلاح DashboardViewModel

### الملف: `src/AlSa3d.App/ViewModels/DashboardViewModel.cs`

#### استبدل الـ Class بالكود التالي:

```csharp
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlSa3d.Core.DTOs;
using AlSa3d.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace AlSa3d.App.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly ILogger<DashboardViewModel> _logger;

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

        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private string? _errorMessage;

        public DashboardViewModel(
            IInvoiceService invoiceService,
            ICustomerService customerService,
            IProductService productService,
            ILogger<DashboardViewModel> logger)
        {
            _invoiceService = invoiceService;
            _customerService = customerService;
            _productService = productService;
            _logger = logger;
            
            LoadDashboardDataCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadDashboardData()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;
                _logger.LogInformation("جاري تحميل بيانات لوحة التحكم...");

                // تحميل إحصائيات الفواتير
                var statsResult = await _invoiceService.GetDashboardStatsAsync();
                if (statsResult.IsSuccess && statsResult.Data != null)
                {
                    TotalSales = statsResult.Data.TotalSales;
                    SalesCount = statsResult.Data.InvoiceCount;
                }
                else
                {
                    _logger.LogWarning("فشل في جلب إحصائيات الفواتير: {message}", statsResult.Message);
                }

                // تحميل عدد العملاء
                var customersResult = await _customerService.GetAllCustomersAsync();
                if (customersResult.IsSuccess && customersResult.Data != null)
                {
                    TotalCustomers = customersResult.Data.Count();
                }
                else
                {
                    _logger.LogWarning("فشل في جلب العملاء");
                }

                // تحميل المنتجات والمخزون
                var productsResult = await _productService.GetAllProductsAsync();
                if (productsResult.IsSuccess && productsResult.Data != null)
                {
                    var productsList = productsResult.Data.ToList();
                    TotalProducts = productsList.Count();
                    
                    // جلب المنتجات قليلة المخزون
                    var lowStockResult = await _productService.GetLowStockProductsAsync(10);
                    if (lowStockResult.IsSuccess && lowStockResult.Data != null)
                    {
                        LowStockCount = lowStockResult.Data.Count();
                    }
                }
                else
                {
                    _logger.LogWarning("فشل في جلب المنتجات");
                }

                // تحميل آخر الفواتير
                var recentInvoicesResult = await _invoiceService.GetRecentInvoicesAsync(10);
                if (recentInvoicesResult.IsSuccess && recentInvoicesResult.Data != null)
                {
                    RecentInvoices = new ObservableCollection<InvoiceDto>(recentInvoicesResult.Data);
                }
                else
                {
                    _logger.LogWarning("فشل في جلب آخر الفواتير");
                }

                _logger.LogInformation("✅ تم تحميل بيانات لوحة التحكم بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ خطأ في تحميل بيانات لوحة التحكم");
                ErrorMessage = "فشل في تحميل البيانات. يرجى المحاولة لاحقاً.";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void ViewAllInvoices()
        {
            // الانتقال لشاشة الفواتير
        }

        [RelayCommand]
        private void ViewLowStockProducts()
        {
            // الانتقال لشاشة المنتجات قليلة المخزون
        }
    }
}
```

---

## ✅ الحل #6: إصلاح ProductViewModel

### الملف: `src/AlSa3d.App/ViewModels/ProductViewModel.cs`

#### استبدل الكود:

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlSa3d.Core.Entities;
using AlSa3d.Core.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AlSa3d.App.ViewModels;

public partial class ProductViewModel : ObservableObject
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductViewModel> _logger;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Product> _products = new();

    [ObservableProperty]
    private Product? _selectedProduct;

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private string? _errorMessage;

    public ProductViewModel(IProductService productService, ILogger<ProductViewModel> logger)
    {
        _productService = productService;
        _logger = logger;
        LoadProductsCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadProducts()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var result = await _productService.GetAllProductsAsync();
            if (result.IsSuccess && result.Data != null)
            {
                Products = new ObservableCollection<Product>(result.Data);
            }
            else
            {
                ErrorMessage = result.Message;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تحميل المنتجات");
            ErrorMessage = "فشل في تحميل المنتجات";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task Search()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                await LoadProductsCommand.ExecuteAsync(null);
                return;
            }

            var result = await _productService.SearchProductsAsync(SearchText);
            if (result.IsSuccess && result.Data != null)
            {
                Products = new ObservableCollection<Product>(result.Data);
            }
            else
            {
                ErrorMessage = result.Message;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في البحث");
            ErrorMessage = "فشل في البحث عن المنتجات";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AddNewProduct() 
    {
        // فتح dialog لإضافة منتج جديد
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task EditProduct(Product? product)
    {
        if (product == null) return;
        // فتح dialog للتعديل
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task DeleteProduct(Product? product)
    {
        if (product == null) return;
        
        var result = await _productService.DeleteProductAsync(product.Id);
        if (result.IsSuccess)
        {
            await LoadProductsCommand.ExecuteAsync(null);
        }
    }

    [RelayCommand]
    private async Task StockTake() 
    {
        // فتح شاشة الجرد
        await Task.CompletedTask;
    }
}
```

---

## ✅ الحل #7: إضافة Configuration Files

### 1. UserConfiguration.cs

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AlSa3d.Core.Entities;

namespace AlSa3d.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .HasMaxLength(100);

        builder.Property(u => u.Phone)
            .HasMaxLength(20);

        // Unique constraint على Username
        builder.HasIndex(u => u.Username)
            .IsUnique();

        // Foreign key إلى Role
        builder.HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Soft delete
        builder.Property(u => u.IsDeleted)
            .HasDefaultValue(false);
    }
}
```

### 2. RoleConfiguration.cs

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AlSa3d.Core.Entities;

namespace AlSa3d.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Description)
            .HasMaxLength(500);

        // Unique constraint على Name
        builder.HasIndex(r => r.Name)
            .IsUnique();

        // Relationship مع RolePermission
        builder.HasMany(r => r.RolePermissions)
            .WithOne(rp => rp.Role)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship مع Permissions (Many-to-Many)
        builder.HasMany(r => r.Permissions)
            .WithMany(p => p.RolePermissions.Select(rp => rp.Role))
            .UsingEntity(
                "RolePermission",
                l => l.HasOne(typeof(Permission)).WithMany(),
                r => r.HasOne(typeof(Role)).WithMany(),
                j => j.HasKey("RoleId", "PermissionId"));
    }
}
```

---

## ملاحظات مهمة:

1. **التحديثات على IRepository Interface** قد تكون مطلوبة
2. **Test Cases** يجب إضافتها لكل method جديد
3. **Migration في Entity Framework** قد تكون مطلوبة بعد إضافة Configurations
4. **سياسة الحذف (Soft Delete)** يجب تطبيقها بشكل متسق

---

**تم إعداد جميع الحلول والكود المطلوب لإصلاح المشروع.**
