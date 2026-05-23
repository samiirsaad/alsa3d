using AlSa3d.Core.Common;

namespace AlSa3d.Core.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Barcode { get; set; }
    public int CategoryId { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal TaxRate { get; set; } = 14;
    public decimal MinStockLevel { get; set; } = 10;
    public decimal MaxStockLevel { get; set; } = 1000;
    public bool IsActive { get; set; } = true;
    public string? Unit { get; set; } = "قطعة";
    public DateTime? DeletedAt { get; set; }

    public virtual Category? Category { get; set; }
    public virtual ICollection<ProductWarehouse> ProductWarehouses { get; set; } = new List<ProductWarehouse>();
    public virtual ICollection<WarehouseProduct> WarehouseProducts { get; set; } = new List<WarehouseProduct>();
}

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }
    public int? ParentId { get; set; }

    public virtual Category? ParentCategory { get; set; }
    public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

public class Warehouse : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? ManagerName { get; set; }
    public decimal Capacity { get; set; } = 1000;
    public bool IsActive { get; set; } = true;

    public virtual ICollection<ProductWarehouse> ProductWarehouses { get; set; } = new List<ProductWarehouse>();
}

public class ProductWarehouse : BaseEntity
{
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public decimal Quantity { get; set; }
    public decimal ReservedQuantity { get; set; }
    public DateTime? LastUpdated { get; set; }

    public virtual Product? Product { get; set; }
    public virtual Warehouse? Warehouse { get; set; }
}

public class WarehouseProduct : BaseEntity
{
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public decimal Quantity { get; set; }
    public DateTime? LastUpdated { get; set; }

    public virtual Product? Product { get; set; }
    public virtual Warehouse? Warehouse { get; set; }
}

public class PricingRule : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public int? CustomerTypeId { get; set; }
    public decimal MinQuantity { get; set; } = 1;
    public decimal? MaxQuantity { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal? FixedPrice { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual Product? Product { get; set; }
}
