using System;
using AlSaad.Core.Common;

namespace AlSaad.Core.Entities;

/// <summary>
/// المنتج/الصنف
/// </summary>
public class Product : BaseEntity
{
    public string ProductCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // التصنيف
    public int CategoryId { get; set; }
    public virtual Category? Category { get; set; }
    
    // الوحدة
    public string Unit { get; set; } = "قطعة";
    
    // الأسعار
    public decimal CostPrice { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal? WholesalePrice { get; set; }
    
    // الضريبة
    public decimal TaxRate { get; set; } = 14.0m;
    public bool IsTaxable { get; set; } = true;
    
    // المخزون
    public int MinStockLevel { get; set; }
    public int MaxStockLevel { get; set; }
    public bool TrackInventory { get; set; } = true;
    
    // حالة المنتج
    public bool IsActive { get; set; } = true;
    public bool IsService { get; set; } = false;
    
    // باركود
    public string? Barcode { get; set; }
    public string? Manufacturer { get; set; }
    public string? Brand { get; set; }
    
    // ملاحظات
    public string? Notes { get; set; }
    
    // خصائص التنقل
    public virtual ICollection<InvoiceItem>? InvoiceItems { get; set; }
    public virtual ICollection<WarehouseProduct>? WarehouseProducts { get; set; }
}
