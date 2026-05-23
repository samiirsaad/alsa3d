using System;
using AlSaad.Core.Common;
using AlSaad.Core.Enums;

namespace AlSaad.Core.Entities;

/// <summary>
/// المستخدم
/// </summary>
public class User : BaseEntity
{
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    
    // معلومات إضافية
    public string? ImagePath { get; set; }
    public DateTime? LastLoginDate { get; set; }
    
    // الحالة
    public bool IsActive { get; set; } = true;
    public bool MustChangePassword { get; set; }
    public DateTime? PasswordExpiryDate { get; set; }
    
    // إعدادات العرض
    public string? Language { get; set; } = "ar-EG";
    public string? Theme { get; set; } = "Light";
    
    // خصائص التنقل
    public virtual ICollection<UserRole>? Roles { get; set; }
    public virtual ICollection<UserPermission>? Permissions { get; set; }
}

/// <summary>
/// الدور/الصلاحية المجمعة
/// </summary>
public class Role : BaseEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool IsSystem { get; set; } // لا يمكن حذفه
    public bool IsActive { get; set; } = true;
    
    // خصائص التنقل
    public virtual ICollection<RolePermission>? Permissions { get; set; }
    public virtual ICollection<UserRole>? Users { get; set; }
}

/// <summary>
/// الصلاحية
/// </summary>
public class Permission : BaseEntity
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Module { get; set; }
    public PermissionType Type { get; set; }
    public bool IsSystem { get; set; }
    
    // خصائص التنقل
    public virtual ICollection<RolePermission>? Roles { get; set; }
    public virtual ICollection<UserPermission>? Users { get; set; }
}

/// <summary>
/// ربط المستخدمين بالأدوار
/// </summary>
public class UserRole : BaseEntity
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public DateTime AssignedDate { get; set; }
    public int? AssignedBy { get; set; }
    
    // خصائص التنقل
    public virtual User? User { get; set; }
    public virtual Role? Role { get; set; }
}

/// <summary>
/// ربط الأدوار بالصلاحيات
/// </summary>
public class RolePermission : BaseEntity
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
    
    // خصائص التنقل
    public virtual Role? Role { get; set; }
    public virtual Permission? Permission { get; set; }
}

/// <summary>
/// ربط المستخدمين بالصلاحيات المباشرة
/// </summary>
public class UserPermission : BaseEntity
{
    public int UserId { get; set; }
    public int PermissionId { get; set; }
    public bool IsDenied { get; set; } // رفض الصلاحية صراحة
    
    // خصائص التنقل
    public virtual User? User { get; set; }
    public virtual Permission? Permission { get; set; }
}

/// <summary>
/// نوع الصلاحية
/// </summary>
public enum PermissionType
{
    View = 0,       // عرض
    Create = 1,     // إنشاء
    Edit = 2,       // تعديل
    Delete = 3,     // حذف
    Print = 4,      // طباعة
    Export = 5,     // تصدير
    Approve = 6,    // اعتماد
    Void = 7        // إلغاء
}

/// <summary>
/// سجل التدقيق
/// </summary>
public class AuditLog : BaseEntity
{
    public int? UserId { get; set; }
    public string Action { get; set; }
    public string EntityType { get; set; }
    public int? EntityId { get; set; }
    public string? OldValues { get; set; } // JSON
    public string? NewValues { get; set; } // JSON
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public DateTime Timestamp { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    
    // خصائص التنقل
    public virtual User? User { get; set; }
}

/// <summary>
/// إعدادات النظام
/// </summary>
public class Setting : BaseEntity
{
    public string Key { get; set; }
    public string Value { get; set; }
    public string Type { get; set; }
    public string? Description { get; set; }
    public string Module { get; set; }
    public bool IsEncrypted { get; set; }
    
    public T GetValue<T>()
    {
        if (Value == null) return default!;
        
        if (typeof(T) == typeof(bool))
            return (T)(object)(Value.ToLower() == "true" || Value == "1");
        
        if (typeof(T) == typeof(int))
            return (T)(object)int.Parse(Value);
        
        if (typeof(T) == typeof(decimal))
            return (T)(object)decimal.Parse(Value);
        
        if (typeof(T) == typeof(DateTime))
            return (T)(object)DateTime.Parse(Value);
        
        return (T)Convert.ChangeType(Value, typeof(T));
    }
}

/// <summary>
/// البنك
/// </summary>
public class Bank : BaseEntity
{
    public string Name { get; set; }
    public string? Code { get; set; }
    public string? SwiftCode { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;
    
    // خصائص التنقل
    public virtual ICollection<BankAccount>? Accounts { get; set; }
}

/// <summary>
/// الحساب البنكي
/// </summary>
public class BankAccount : BaseEntity
{
    public int BankId { get; set; }
    public string AccountName { get; set; }
    public string AccountNumber { get; set; }
    public string? Iban { get; set; }
    public string Currency { get; set; } = "EGP";
    public decimal Balance { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsMain { get; set; }
    
    // خصائص التنقل
    public virtual Bank? Bank { get; set; }
    public virtual ICollection<Check>? Checks { get; set; }
}

/// <summary>
/// الشيك
/// </summary>
public class Check : BaseEntity
{
    public string CheckNumber { get; set; }
    public int? BankAccountId { get; set; }
    public int? CustomerId { get; set; }
    
    public CheckType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    
    public CheckStatus Status { get; set; }
    
    public string? Notes { get; set; }
    public DateTime? CashedDate { get; set; }
    public int? CashedBy { get; set; }
    
    // خصائص التنقل
    public virtual BankAccount? BankAccount { get; set; }
    public virtual Customer? Customer { get; set; }
}

/// <summary>
/// نوع الشيك
/// </summary>
public enum CheckType
{
    Received = 0,  // شيك مورد
    Paid = 1       // شيك صادر
}
