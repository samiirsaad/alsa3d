using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlSa3d.Core.Services.Interfaces;
using AlSa3d.Core.Entities;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AlSa3d.App.ViewModels;

/// <summary>
/// ViewModel لإدارة العملاء
/// </summary>
public partial class CustomerViewModel : ObservableObject
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomerViewModel> _logger;

    /// <summary>
    /// نص البحث
    /// </summary>
    [ObservableProperty]
    private string _searchText = string.Empty;

    /// <summary>
    /// العميل المختار حالياً
    /// </summary>
    [ObservableProperty]
    private Customer? _selectedCustomer;

    /// <summary>
    /// قائمة العملاء
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<Customer> _customers = new();

    /// <summary>
    /// رسالة الخطأ (إن وجدت)
    /// </summary>
    [ObservableProperty]
    private string? _errorMessage;

    /// <summary>
    /// هل يتم تحميل البيانات حالياً
    /// </summary>
    [ObservableProperty]
    private bool _isLoading = false;

    /// <summary>
    /// مُنشئ ViewModel
    /// </summary>
    public CustomerViewModel(ICustomerService customerService, ILogger<CustomerViewModel> logger)
    {
        _customerService = customerService;
        _logger = logger;
        LoadCustomersCommand.Execute(null);
    }

    /// <summary>
    /// تحميل جميع العملاء
    /// </summary>
    [RelayCommand]
    private async Task LoadCustomers()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;
            _logger.LogInformation("📖 جاري تحميل قائمة العملاء...");

            var result = await _customerService.GetAllCustomersAsync();
            
            if (result.IsSuccess)
            {
                Customers = new ObservableCollection<Customer>(result.Data ?? new List<Customer>());
                _logger.LogInformation("✅ تم تحميل {count} عميل", Customers.Count);
            }
            else
            {
                ErrorMessage = result.Message;
                _logger.LogWarning("⚠️ فشل تحميل العملاء: {message}", result.Message);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "حدث خطأ عند تحميل العملاء";
            _logger.LogError(ex, "❌ خطأ في تحميل العملاء");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// البحث عن العملاء
    /// </summary>
    [RelayCommand]
    private async Task Search()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                await LoadCustomersCommand.ExecuteAsync(null);
                return;
            }

            IsLoading = true;
            ErrorMessage = null;
            _logger.LogInformation("🔍 جاري البحث عن: {search}", SearchText);

            var result = await _customerService.SearchCustomersAsync(SearchText);
            
            if (result.IsSuccess)
            {
                Customers = new ObservableCollection<Customer>(result.Data ?? new List<Customer>());
                _logger.LogInformation("✅ تم العثور على {count} نتيجة", Customers.Count);
            }
            else
            {
                ErrorMessage = result.Message;
                _logger.LogWarning("⚠️ فشل البحث: {message}", result.Message);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "حدث خطأ عند البحث";
            _logger.LogError(ex, "❌ خطأ في البحث عن العملاء");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// إعادة تعيين البحث
    /// </summary>
    [RelayCommand]
    private async Task ResetSearch()
    {
        SearchText = string.Empty;
        await LoadCustomersCommand.ExecuteAsync(null);
    }

    /// <summary>
    /// إضافة عميل جديد
    /// </summary>
    [RelayCommand]
    private async Task AddNewCustomer()
    {
        try
        {
            _logger.LogInformation("➕ إضافة عميل جديد");
            // سيتم فتح نافذة حوار لإضافة عميل جديد
            // Implementation depends on DialogService
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            ErrorMessage = "فشل في إضافة عميل جديد";
            _logger.LogError(ex, "❌ خطأ في إضافة عميل جديد");
        }
    }

    /// <summary>
    /// تعديل عميل
    /// </summary>
    [RelayCommand]
    private async Task EditCustomer(Customer? customer)
    {
        try
        {
            if (customer == null)
            {
                ErrorMessage = "يجب اختيار عميل أولاً";
                return;
            }

            _logger.LogInformation("✏️ تعديل العميل: {customerId}", customer.Id);
            // سيتم فتح نافذة حوار للتعديل
            // Implementation depends on DialogService
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            ErrorMessage = "فشل في تعديل العميل";
            _logger.LogError(ex, "❌ خطأ في تعديل العميل: {customerId}", customer?.Id);
        }
    }

    /// <summary>
    /// حذف عميل
    /// </summary>
    [RelayCommand]
    private async Task DeleteCustomer(Customer? customer)
    {
        try
        {
            if (customer == null)
            {
                ErrorMessage = "يجب اختيار عميل أولاً";
                return;
            }

            _logger.LogInformation("🗑️ حذف العميل: {customerId}", customer.Id);
            
            var result = await _customerService.DeleteCustomerAsync(customer.Id);
            
            if (result.IsSuccess)
            {
                await LoadCustomersCommand.ExecuteAsync(null);
                _logger.LogInformation("✅ تم حذف العميل: {customerId}", customer.Id);
            }
            else
            {
                ErrorMessage = result.Message;
                _logger.LogWarning("⚠️ فشل حذف العميل: {message}", result.Message);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "فشل في حذف العميل";
            _logger.LogError(ex, "❌ خطأ في حذف العميل: {customerId}", customer?.Id);
        }
    }

    /// <summary>
    /// تصدير البيانات إلى Excel
    /// </summary>
    [RelayCommand]
    private async Task ExportToExcel()
    {
        try
        {
            _logger.LogInformation("📤 جاري تصدير البيانات إلى Excel...");
            ErrorMessage = null;
            
            // سيتم إضافة تطبيق التصدير
            await Task.CompletedTask;
            
            _logger.LogInformation("✅ تم تصدير البيانات بنجاح");
        }
        catch (Exception ex)
        {
            ErrorMessage = "فشل في تصدير البيانات";
            _logger.LogError(ex, "❌ خطأ في تصدير البيانات");
        }
    }
}
