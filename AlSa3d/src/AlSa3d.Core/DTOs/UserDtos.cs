using System.ComponentModel.DataAnnotations;

namespace AlSa3d.Core.DTOs;

/// <summary>
/// DTO لتسجيل مستخدم جديد
/// </summary>
public class RegisterUserDto
{
    /// <summary>
    /// اسم المستخدم (مطلوب ويجب أن يكون فريداً)
    /// </summary>
    [Required(ErrorMessage = "اسم المستخدم مطلوب")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "اسم المستخدم يجب أن يكون بين 3 و 50 حرف")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// كلمة المرور (مطلوبة وتحتاج على الأقل 6 أحرف)
    /// </summary>
    [Required(ErrorMessage = "كلمة المرور مطلوبة")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "كلمة المرور يجب أن تكون على الأقل 6 أحرف")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// الاسم الكامل للمستخدم
    /// </summary>
    [Required(ErrorMessage = "الاسم الكامل مطلوب")]
    [StringLength(100, ErrorMessage = "الاسم الكامل لا يجب أن يتجاوز 100 حرف")]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// بريد المستخدم الإلكتروني (اختياري)
    /// </summary>
    [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
    public string? Email { get; set; }

    /// <summary>
    /// رقم هاتف المستخدم (اختياري)
    /// </summary>
    [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
    public string? Phone { get; set; }

    /// <summary>
    /// معرف الدور (اختياري، القيمة الافتراضية هي 3 - مستخدم عام)
    /// </summary>
    public int? RoleId { get; set; }
}

/// <summary>
/// DTO لتحديث بيانات المستخدم
/// </summary>
public class UpdateUserDto
{
    /// <summary>
    /// الاسم الكامل للمستخدم
    /// </summary>
    [Required(ErrorMessage = "الاسم الكامل مطلوب")]
    [StringLength(100, ErrorMessage = "الاسم الكامل لا يجب أن يتجاوز 100 حرف")]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// بريد المستخدم الإلكتروني (اختياري)
    /// </summary>
    [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
    public string? Email { get; set; }

    /// <summary>
    /// رقم هاتف المستخدم (اختياري)
    /// </summary>
    [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
    public string? Phone { get; set; }

    /// <summary>
    /// معرف الدور (اختياري)
    /// </summary>
    public int? RoleId { get; set; }

    /// <summary>
    /// هل الحساب مفعل (اختياري)
    /// </summary>
    public bool? IsActive { get; set; }
}

/// <summary>
/// DTO لإنشاء دور جديد
/// </summary>
public class CreateRoleDto
{
    /// <summary>
    /// اسم الدور (مطلوب ويجب أن يكون فريداً)
    /// </summary>
    [Required(ErrorMessage = "اسم الدور مطلوب")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "اسم الدور يجب أن يكون بين 3 و 100 حرف")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// وصف الدور (اختياري)
    /// </summary>
    [StringLength(500, ErrorMessage = "الوصف لا يجب أن يتجاوز 500 حرف")]
    public string? Description { get; set; }
}
