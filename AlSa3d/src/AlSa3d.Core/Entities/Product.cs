using AlSa3d.Core.Common;

namespace AlSa3d.Core.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Barcode { get; set; }
    public int CategoryId { get; set; }
    public decimal CostPrice { get; set; } = 0;
    public decimal SellingPrice { get; set; } = 0;
    public decimal MinStockLevel { get; set; } = 10;
    public bool IsActive { get; set; } = true;
    public string? Unit { get; set; } = "قطعة";
    
    // Navigation Properties
    public virtual Category? Category { get; set; }
    public virtual ICollection<ProductWarehouse> ProductWarehouses { get; set; } = new List<ProductWarehouse>();
}

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }
    
    public virtual Category? ParentCategory { get; set; }
    public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

public class Warehouse : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;
    
    public virtual ICollection<ProductWarehouse> ProductWarehouses { get; set; } = new List<ProductWarehouse>();
}

public class ProductWarehouse : BaseEntity
{
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public decimal Quantity { get; set; } = 0;
    public decimal ReservedQuantity { get; set; } = 0;
    
    public virtual Product? Product { get; set; }
    public virtual Warehouse? Warehouse { get; set; }
}
