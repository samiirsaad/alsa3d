using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlSa3d.Core.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AlSa3d.App.ViewModels;

public partial class ProductViewModel : ObservableObject
{
    private readonly IProductService _productService;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Product> _products = new();

    public ProductViewModel(IProductService productService)
    {
        _productService = productService;
        LoadProductsCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadProducts()
    {
        var result = await _productService.GetAllProductsAsync();
        if (result.IsSuccess)
            Products = new ObservableCollection<Product>(result.Data);
    }

    [RelayCommand]
    private async Task Search()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            await LoadProductsCommand.ExecuteAsync(null);
            return;
        }
        var result = await _productService.SearchProductsAsync(SearchText);
        if (result.IsSuccess)
            Products = new ObservableCollection<Product>(result.Data);
    }

    [RelayCommand]
    private async Task AddNewProduct() => await Task.CompletedTask;

    [RelayCommand]
    private async Task EditProduct(Product? product)
    {
        if (product == null) return;
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task DeleteProduct(Product? product)
    {
        if (product == null) return;
        var result = await _productService.DeleteProductAsync(product.Id);
        if (result.IsSuccess)
            await LoadProductsCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task StockTake() => await Task.CompletedTask;
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public int Quantity { get; set; }
    public int MinQuantity { get; set; }
}
