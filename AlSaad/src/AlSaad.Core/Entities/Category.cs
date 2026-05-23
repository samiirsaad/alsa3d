using AlSaad.Core.Common;

namespace AlSaad.Core.Entities;

/// <summary>
/// تصنيف المنتجات
/// </summary>
public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // التصنيف الأب
    public int? ParentId { get; set; }
    public virtual Category? Parent { get; set; }
    
    // حالة التصنيف
    public bool IsActive { get; set; } = true;
    
    // خصائص التنقل
    public virtual ICollection<Category>? Children { get; set; }
    public virtual ICollection<Product>? Products { get; set; }
}
