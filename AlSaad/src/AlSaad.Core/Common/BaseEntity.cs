using System;

namespace AlSaad.Core.Common;

/// <summary>
/// الكيان الأساسي الذي ترث منه جميع الكيانات الأخرى
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
    
    /// <summary>
    /// تاريخ الإنشاء
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    /// <summary>
    /// معرف المستخدم الذي أنشأ السجل
    /// </summary>
    public int? CreatedBy { get; set; }
    
    /// <summary>
    /// تاريخ آخر تعديل
    /// </summary>
    public DateTime? ModifiedAt { get; set; }
    
    /// <summary>
    /// معرف المستخدم الذي عدل السجل آخر مرة
    /// </summary>
    public int? ModifiedBy { get; set; }
    
    /// <summary>
    /// هل السجل محذوف منطقياً
    /// </summary>
    public bool IsDeleted { get; set; }
    
    /// <summary>
    /// تاريخ الحذف
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
