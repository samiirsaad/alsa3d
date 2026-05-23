using AlSa3d.Core;
using AlSa3d.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlSa3d.Core.Entities;
using AlSa3d.Core.Interfaces;
using AlSa3d.Core.DTOs;

namespace AlSa3d.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Warehouse> _warehouseRepository;
        private readonly IRepository<WarehouseProduct> _warehouseProductRepository;
        private readonly IRepository<PricingRule> _pricingRuleRepository;

        public ProductService(
            IRepository<Product> productRepository,
            IRepository<Category> categoryRepository,
            IRepository<Warehouse> warehouseRepository,
            IRepository<WarehouseProduct> warehouseProductRepository,
            IRepository<PricingRule> pricingRuleRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _warehouseRepository = warehouseRepository;
            _warehouseProductRepository = warehouseProductRepository;
            _pricingRuleRepository = pricingRuleRepository;
        }

        public async Task<Result<IEnumerable<Product>>> GetAllProductsAsync()
        {
            try
            {
                var products = await _productRepository.GetAllAsync(
                    p => p.Category,
                    p => p.WarehouseProducts);
                
                return Result.Ok(products.Where(p => !p.IsDeleted).OrderBy((p => p.Name)).AsEnumerable());
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<Product>>($"فشل في جلب المنتجات: {ex.Message}");
            }
        }

        public async Task<Result<Product>> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id,
                    p => p.Category,
                    p => p.WarehouseProducts);

                if (product == null || product.IsDeleted)
                    return Result.Failure<Product>("المنتج غير موجود");

                return Result.Ok(product);
            }
            catch (Exception ex)
            {
                return Result.Failure<Product>($"فشل في جلب المنتج: {ex.Message}");
            }
        }

        public async Task<Result<Product>> CreateProductAsync(CreateProductDto dto)
        {
            try
            {
                // التحقق من عدم التكرار
                var existing = await _productRepository.GetAsync(p => p.Barcode == dto.Barcode && !p.IsDeleted);
                if (existing != null)
                    return Result.Failure<Product>("الباركود مسجل بالفعل");

                var product = new Product
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Barcode = dto.Barcode,
                    CategoryId = dto.CategoryId,
                    Unit = dto.Unit ?? "قطعة",
                    CostPrice = dto.CostPrice ?? 0,
                    SellingPrice = dto.SellingPrice,
                    TaxRate = dto.TaxRate ?? 14,
                    MinStockLevel = dto.MinStockLevel ?? 10,
                    MaxStockLevel = dto.MaxStockLevel ?? 1000,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                var result = await _productRepository.AddAsync(product);
                if (!result.Success)
                    return result;

                // إضافة المخزون الأولي
                if (dto.InitialStock.HasValue && dto.WarehouseId.HasValue)
                {
                    var warehouseProduct = new WarehouseProduct
                    {
                        ProductId = product.Id,
                        WarehouseId = dto.WarehouseId.Value,
                        Quantity = dto.InitialStock.Value,
                        LastUpdated = DateTime.Now
                    };

                    await _warehouseProductRepository.AddAsync(warehouseProduct);
                }

                return Result.Ok(product);
            }
            catch (Exception ex)
            {
                return Result.Failure<Product>($"فشل في إضافة المنتج: {ex.Message}");
            }
        }

        public async Task<Result<Product>> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null || product.IsDeleted)
                    return Result.Failure<Product>("المنتج غير موجود");

                product.Name = dto.Name;
                product.Description = dto.Description;
                product.Barcode = dto.Barcode;
                product.CategoryId = dto.CategoryId;
                product.Unit = dto.Unit ?? product.Unit;
                product.CostPrice = dto.CostPrice ?? product.CostPrice;
                product.SellingPrice = dto.SellingPrice ?? product.SellingPrice;
                product.TaxRate = dto.TaxRate ?? product.TaxRate;
                product.MinStockLevel = dto.MinStockLevel ?? product.MinStockLevel;
                product.MaxStockLevel = dto.MaxStockLevel ?? product.MaxStockLevel;
                product.IsActive = dto.IsActive ?? product.IsActive;
                product.UpdatedAt = DateTime.Now;

                var result = await _productRepository.UpdateAsync(product);
                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<Product>($"فشل في تحديث المنتج: {ex.Message}");
            }
        }

        public async Task<Result<bool>> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null || product.IsDeleted)
                    return Result.Failure<bool>("المنتج غير موجود");

                product.IsDeleted = true;
                product.DeletedAt = DateTime.Now;

                var updateResult = await _productRepository.UpdateAsync(product);
                return Result.Ok(updateResult.Success);
            }
            catch (Exception ex)
            {
                return Result.Failure<bool>($"فشل في حذف المنتج: {ex.Message}");
            }
        }

        public async Task<Result<Category>> CreateCategoryAsync(CreateCategoryDto dto)
        {
            try
            {
                var category = new Category
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    ParentId = dto.ParentId,
                    CreatedAt = DateTime.Now
                };

                var result = await _categoryRepository.AddAsync(category);
                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<Category>($"فشل في إضافة التصنيف: {ex.Message}");
            }
        }

        public async Task<Result<Warehouse>> CreateWarehouseAsync(CreateWarehouseDto dto)
        {
            try
            {
                var warehouse = new Warehouse
                {
                    Name = dto.Name,
                    Address = dto.Address,
                    Phone = dto.Phone,
                    ManagerName = dto.ManagerName,
                    Capacity = dto.Capacity ?? 1000,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                var result = await _warehouseRepository.AddAsync(warehouse);
                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<Warehouse>($"فشل في إضافة المخزن: {ex.Message}");
            }
        }

        public async Task<Result<int>> GetProductStockAsync(int productId, int warehouseId)
        {
            try
            {
                var warehouseProduct = await _warehouseProductRepository.GetAsync(wp => 
                    wp.ProductId == productId && wp.WarehouseId == warehouseId);

                return Result.Ok((int)(warehouseProduct?.Quantity ?? 0));
            }
            catch (Exception ex)
            {
                return Result.Failure<int>($"فشل في جلب المخزون: {ex.Message}");
            }
        }

        public async Task<Result<bool>> UpdateStockAsync(int productId, int warehouseId, int quantityChange)
        {
            try
            {
                var warehouseProduct = await _warehouseProductRepository.GetAsync(wp => 
                    wp.ProductId == productId && wp.WarehouseId == warehouseId);

                if (warehouseProduct == null)
                {
                    warehouseProduct = new WarehouseProduct
                    {
                        ProductId = productId,
                        WarehouseId = warehouseId,
                        Quantity = 0,
                        LastUpdated = DateTime.Now
                    };

                    await _warehouseProductRepository.AddAsync(warehouseProduct);
                }

                warehouseProduct.Quantity += quantityChange;
                
                if (warehouseProduct.Quantity < 0)
                    return Result.Failure<bool>("الكمية لا يمكن أن تكون سالبة");

                warehouseProduct.LastUpdated = DateTime.Now;

                var updateResult = await _warehouseProductRepository.UpdateAsync(warehouseProduct);
                return Result.Ok(updateResult.Success);
            }
            catch (Exception ex)
            {
                return Result.Failure<bool>($"فشل في تحديث المخزون: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<Product>>> GetLowStockProductsAsync(int threshold = 10)
        {
            try
            {
                var products = await _productRepository.GetAllAsync(p => p.WarehouseProducts);
                
                var lowStock = products.Where(p => !p.IsDeleted && 
                    p.WarehouseProducts != null &&
                    p.WarehouseProducts.Sum(wp => wp.Quantity) <= threshold);

                return Result.Ok(lowStock);
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<Product>>($"فشل في جلب المنتجات منخفضة المخزون: {ex.Message}");
            }
        }

        public async Task<Result<PricingRule>> CreatePricingRuleAsync(CreatePricingRuleDto dto)
        {
            try
            {
                var rule = new PricingRule
                {
                    ProductId = dto.ProductId,
                    CustomerTypeId = dto.CustomerTypeId,
                    MinQuantity = dto.MinQuantity ?? 1,
                    MaxQuantity = dto.MaxQuantity,
                    DiscountPercent = dto.DiscountPercent ?? 0,
                    FixedPrice = dto.FixedPrice,
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
                return Result.Failure<PricingRule>($"فشل في إضافة قاعدة التسعير: {ex.Message}");
            }
        }
    }
}
