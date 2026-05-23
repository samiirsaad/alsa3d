using AlSaad.Core.Common;

namespace AlSaad.Core.Entities;

/// <summary>
/// المخزن
/// </summary>
public class Warehouse : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Address { get; set; }
    
    // المدير المسؤول
    public int? ManagerId { get; set; }
    public virtual Employee? Manager { get; set; }
    
    // حالة المخزن
    public bool IsActive { get; set; } = true;
    public bool IsMain { get; set; } = false;
    
    // ملاحظات
    public string? Notes { get; set; }
    
    // خصائص التنقل
    public virtual ICollection<WarehouseProduct>? WarehouseProducts { get; set; }
    public virtual ICollection<StockMovement>? StockMovements { get; set; }
}

/// <summary>
/// المنتج في المخزن
/// </summary>
public class WarehouseProduct : BaseEntity
{
    public int WarehouseId { get; set; }
    public virtual Warehouse? Warehouse { get; set; }
    
    public int ProductId { get; set; }
    public virtual Product? Product { get; set; }
    
    // الكميات
    public int QuantityOnHand { get; set; }
    public int QuantityReserved { get; set; }
    public int QuantityAvailable => QuantityOnHand - QuantityReserved;
    
    // آخر جرد
    public DateTime? LastStockCountDate { get; set; }
    public decimal? LastCostPrice { get; set; }
}

/// <summary>
/// حركة مخزنية
/// </summary>
public class StockMovement : BaseEntity
{
    public int WarehouseId { get; set; }
    public virtual Warehouse? Warehouse { get; set; }
    
    public int ProductId { get; set; }
    public virtual Product? Product { get; set; }
    
    // نوع الحركة
    public string MovementType { get; set; } = string.Empty; // وارد، صادر، تحويل، تسوية
    public int Quantity { get; set; }
    public int QuantityBefore { get; set; }
    public int QuantityAfter { get; set; }
    
    // مرجع الحركة
    public string? ReferenceType { get; set; } // فاتورة، أمر شراء، إلخ
    public int? ReferenceId { get; set; }
    
    // المستخدم الذي نفذ الحركة
    public int UserId { get; set; }
    public virtual User? User { get; set; }
    
    public DateTime MovementDate { get; set; } = DateTime.Now;
    public string? Notes { get; set; }
}
