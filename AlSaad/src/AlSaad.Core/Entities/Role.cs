using AlSaad.Core.Common;

namespace AlSaad.Core.Entities;

/// <summary>
/// الدور/الصلاحية
/// </summary>
public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // حالة الدور
    public bool IsActive { get; set; } = true;
    public bool IsSystem { get; set; } = false; // أدوار النظام لا يمكن حذفها
    
    // خصائص التنقل
    public virtual ICollection<RolePermission>? RolePermissions { get; set; }
    public virtual ICollection<User>? Users { get; set; }
}

/// <summary>
/// الصلاحية
/// </summary>
public class Permission : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // مجموعة الصلاحية
    public string Group { get; set; } = string.Empty; // الفواتير، العملاء، إلخ
    public string Code { get; set; } = string.Empty; // CREATE_INVOICE, VIEW_CUSTOMER, إلخ
    
    // نوع الصلاحية
    public PermissionType Type { get; set; } = PermissionType.Access;
}

/// <summary>
/// ربط الأدوار بالصلاحيات
/// </summary>
public class RolePermission : BaseEntity
{
    public int RoleId { get; set; }
    public virtual Role? Role { get; set; }
    
    public int PermissionId { get; set; }
    public virtual Permission? Permission { get; set; }
    
    // هل الصلاحية مسموحة أم ممنوعة
    public bool IsAllowed { get; set; } = true;
}

public enum PermissionType
{
    Access,      // دخول للصفحة
    Create,      // إنشاء
    Read,        // عرض
    Update,      // تعديل
    Delete,      // حذف
    Print,       // طباعة
    Export,      // تصدير
    Approve      // اعتماد
}
