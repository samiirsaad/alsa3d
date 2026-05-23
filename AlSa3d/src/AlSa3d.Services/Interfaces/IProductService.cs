using AlSa3d.Core;
using AlSa3d.Core.DTOs;
using AlSa3d.Core.Entities;

namespace AlSa3d.Services.Interfaces;

public interface IProductService
{
    Task<Result<IEnumerable<Product>>> GetAllProductsAsync();
    Task<Result<Product>> GetProductByIdAsync(int id);
    Task<Result<Product>> CreateProductAsync(CreateProductDto dto);
    Task<Result<Product>> UpdateProductAsync(int id, UpdateProductDto dto);
    Task<Result<bool>> DeleteProductAsync(int id);
    Task<Result<Category>> CreateCategoryAsync(CreateCategoryDto dto);
    Task<Result<Warehouse>> CreateWarehouseAsync(CreateWarehouseDto dto);
    Task<Result<int>> GetProductStockAsync(int productId, int warehouseId);
    Task<Result<bool>> UpdateStockAsync(int productId, int warehouseId, int quantityChange);
    Task<Result<IEnumerable<Product>>> GetLowStockProductsAsync(int threshold = 10);
    Task<Result<PricingRule>> CreatePricingRuleAsync(CreatePricingRuleDto dto);

}
