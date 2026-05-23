namespace AlSa3d.Core.DTOs;

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Barcode { get; set; }
    public int CategoryId { get; set; }
    public decimal? CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal? TaxRate { get; set; }
    public decimal? MinStockLevel { get; set; }
    public decimal? MaxStockLevel { get; set; }
    public string? Unit { get; set; } = "قطعة";
    public decimal? InitialStock { get; set; }
    public int? WarehouseId { get; set; }
}

public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Barcode { get; set; }
    public int CategoryId { get; set; }
    public decimal? CostPrice { get; set; }
    public decimal? SellingPrice { get; set; }
    public decimal? TaxRate { get; set; }
    public decimal? MinStockLevel { get; set; }
    public decimal? MaxStockLevel { get; set; }
    public string? Unit { get; set; }
    public bool? IsActive { get; set; }
}

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentId { get; set; }
}

public class CreateWarehouseDto
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? ManagerName { get; set; }
    public decimal? Capacity { get; set; }
}

public class StockAdjustmentDto
{
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public decimal QuantityChange { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class CreatePricingRuleDto
{
    public int ProductId { get; set; }
    public int? CustomerTypeId { get; set; }
    public decimal? MinQuantity { get; set; }
    public decimal? MaxQuantity { get; set; }
    public decimal? DiscountPercent { get; set; }
    public decimal? FixedPrice { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
