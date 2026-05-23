using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlSa3d.Services.Interfaces;
using AlSa3d.Desktop.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using EntityProduct = AlSa3d.Core.Entities.Product;

namespace AlSa3d.Desktop.ViewModels;

public class ProductDisplayModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public int Quantity { get; set; }
    public int MinQuantity { get; set; }
}

public partial class ProductViewModel : ObservableObject
{
    private readonly IProductService _productService;
    private readonly IDialogService _dialogService;
    private readonly INotificationService _notificationService;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<ProductDisplayModel> _products = new();

    public ProductViewModel(IProductService productService, IDialogService dialogService, INotificationService notificationService)
    {
        _productService = productService;
        _dialogService = dialogService;
        _notificationService = notificationService;
        LoadProductsCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadProducts()
    {
        var result = await _productService.GetAllProductsAsync();
        if (result.Success)
            Products = new ObservableCollection<ProductDisplayModel>(
                result.Data.Select(p => new ProductDisplayModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    CategoryName = p.Category?.Name ?? "",
                    PurchasePrice = p.CostPrice,
                    SalePrice = p.SellingPrice,
                    Quantity = (int)(p.WarehouseProducts?.Sum(wp => wp.Quantity) ?? 0),
                    MinQuantity = (int)p.MinStockLevel
                }));
    }

    [RelayCommand]
    private async Task Search()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            await LoadProductsCommand.ExecuteAsync(null);
            return;
        }
        await LoadProductsCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task AddNewProduct()
    {
        _notificationService.ShowInfo("إضافة منتج جديد - قيد التطوير");
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task EditProduct(ProductDisplayModel? product)
    {
        if (product == null) return;
        _notificationService.ShowInfo("تعديل المنتج - قيد التطوير");
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task DeleteProduct(ProductDisplayModel? product)
    {
        if (product == null) return;
        if (!_dialogService.ShowConfirm($"هل أنت متأكد من حذف المنتج '{product.Name}'؟"))
            return;
        var result = await _productService.DeleteProductAsync(product.Id);
        if (result.Success)
        {
            _notificationService.ShowSuccess("تم حذف المنتج بنجاح");
            await LoadProductsCommand.ExecuteAsync(null);
        }
        else
            _dialogService.ShowMessage(result.Message ?? "فشل حذف المنتج", "خطأ");
    }

    [RelayCommand]
    private async Task StockTake()
    {
        _notificationService.ShowInfo("جرد المخزون - قيد التطوير");
        await Task.CompletedTask;
    }
}