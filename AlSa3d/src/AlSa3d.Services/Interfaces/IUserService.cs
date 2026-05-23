using AlSa3d.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlSa3d.Services.Interfaces
{
    /// <summary>
    /// واجهة خدمة المستخدمين والصلاحيات والأمان
    /// </summary>
    public interface IUserService
    {
        #region User Operations

        /// <summary>
        /// إنشاء مستخدم جديد
        /// </summary>
        Task<Result<User>> CreateUserAsync(User user);

        /// <summary>
        /// تحديث بيانات مستخدم
        /// </summary>
        Task<Result<User>> UpdateUserAsync(User user);

        /// <summary>
        /// حذف مستخدم (Soft Delete)
        /// </summary>
        Task<Result<bool>> DeleteUserAsync(Guid userId);

        /// <summary>
        /// الحصول على مستخدم بالمعرف
        /// </summary>
        Task<Result<User>> GetUserByIdAsync(Guid userId);

        /// <summary>
        /// الحصول على جميع المستخدمين
        /// </summary>
        Task<Result<List<User>>> GetAllUsersAsync();

        /// <summary>
        /// الحصول على مستخدم باسم المستخدم
        /// </summary>
        Task<Result<User>> GetUserByUsernameAsync(string username);

        /// <summary>
        /// الحصول على مستخدم بالإيميل
        /// </summary>
        Task<Result<User>> GetUserByEmailAsync(string email);

        /// <summary>
        /// تسجيل دخول مستخدم
        /// </summary>
        Task<Result<LoginResponse>> LoginAsync(string username, string password);

        /// <summary>
        /// تسجيل خروج مستخدم
        /// </summary>
        Task<Result<bool>> LogoutAsync(Guid userId);

        /// <summary>
        /// تغيير كلمة المرور
        /// </summary>
        Task<Result<bool>> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword);

        /// <summary>
        /// إعادة تعيين كلمة المرور
        /// </summary>
        Task<Result<bool>> ResetPasswordAsync(Guid userId, string newPassword);

        /// <summary>
        /// تفعيل/تعطيل مستخدم
        /// </summary>
        Task<Result<bool>> ToggleUserStatusAsync(Guid userId, bool isActive);

        #endregion

        #region Role Operations

        /// <summary>
        /// إنشاء دور جديد
        /// </summary>
        Task<Result<Role>> CreateRoleAsync(Role role);

        /// <summary>
        /// تحديث دور
        /// </summary>
        Task<Result<Role>> UpdateRoleAsync(Role role);

        /// <summary>
        /// حذف دور
        /// </summary>
        Task<Result<bool>> DeleteRoleAsync(Guid roleId);

        /// <summary>
        /// الحصول على جميع الأدوار
        /// </summary>
        Task<Result<List<Role>>> GetAllRolesAsync();

        /// <summary>
        /// الحصول على دور بالمعرف
        /// </summary>
        Task<Result<Role>> GetRoleByIdAsync(Guid roleId);

        /// <summary>
        /// إضافة مستخدم لدور
        /// </summary>
        Task<Result<bool>> AssignRoleToUserAsync(Guid userId, Guid roleId);

        /// <summary>
        /// إزالة مستخدم من دور
        /// </summary>
        Task<Result<bool>> RemoveRoleFromUserAsync(Guid userId, Guid roleId);

        /// <summary>
        /// الحصول على أدوار المستخدم
        /// </summary>
        Task<Result<List<Role>>> GetUserRolesAsync(Guid userId);

        #endregion

        #region Permission Operations

        /// <summary>
        /// إنشاء صلاحية جديدة
        /// </summary>
        Task<Result<Permission>> CreatePermissionAsync(Permission permission);

        /// <summary>
        /// تحديث صلاحية
        /// </summary>
        Task<Result<Permission>> UpdatePermissionAsync(Permission permission);

        /// <summary>
        /// حذف صلاحية
        /// </summary>
        Task<Result<bool>> DeletePermissionAsync(Guid permissionId);

        /// <summary>
        /// الحصول على جميع الصلاحيات
        /// </summary>
        Task<Result<List<Permission>>> GetAllPermissionsAsync();

        /// <summary>
        /// إضافة صلاحية لدور
        /// </summary>
        Task<Result<bool>> AssignPermissionToRoleAsync(Guid roleId, Guid permissionId);

        /// <summary>
        /// إزالة صلاحية من دور
        /// </summary>
        Task<Result<bool>> RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId);

        /// <summary>
        /// الحصول على صلاحيات الدور
        /// </summary>
        Task<Result<List<Permission>>> GetRolePermissionsAsync(Guid roleId);

        /// <summary>
        /// التحقق من صلاحية المستخدم
        /// </summary>
        Task<Result<bool>> HasPermissionAsync(Guid userId, string permissionName);

        /// <summary>
        /// الحصول على جميع صلاحيات المستخدم
        /// </summary>
        Task<Result<List<Permission>>> GetUserPermissionsAsync(Guid userId);

        #endregion

        #region Audit Log Operations

        /// <summary>
        /// تسجيل حدث في سجل التدقيق
        /// </summary>
        Task<Result<AuditLog>> LogAuditEventAsync(AuditLog auditLog);

        /// <summary>
        /// الحصول على سجل تدقيق المستخدم
        /// </summary>
        Task<Result<List<AuditLog>>> GetUserAuditLogsAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// الحصول على سجل تدقيق لنشاط معين
        /// </summary>
        Task<Result<List<AuditLog>>> GetAuditLogsByActionAsync(string action, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// الحصول على جميع سجلات التدقيق
        /// </summary>
        Task<Result<List<AuditLog>>> GetAllAuditLogsAsync(DateTime? startDate = null, DateTime? endDate = null);

        #endregion

        #region Security Operations

        /// <summary>
        /// توليد رمز تحقق ثنائي
        /// </summary>
        Task<Result<string>> GenerateTwoFactorCodeAsync(Guid userId);

        /// <summary>
        /// التحقق من رمز التحقق الثنائي
        /// </summary>
        Task<Result<bool>> VerifyTwoFactorCodeAsync(Guid userId, string code);

        /// <summary>
        /// تفعيل التحقق الثنائي
        /// </summary>
        Task<Result<bool>> EnableTwoFactorAuthAsync(Guid userId, string secretKey);

        /// <summary>
        /// تعطيل التحقق الثنائي
        /// </summary>
        Task<Result<bool>> DisableTwoFactorAuthAsync(Guid userId);

        /// <summary>
        /// تسجيل محاولة دخول فاشلة
        /// </summary>
        Task<Result<bool>> LogFailedLoginAttemptAsync(string username, string ipAddress);

        /// <summary>
        /// قفل الحساب بعد محاولات فاشلة
        /// </summary>
        Task<Result<bool>> LockAccountAsync(Guid userId, int lockDurationMinutes);

        /// <summary>
        /// فتح الحساب المقفل
        /// </summary>
        Task<Result<bool>> UnlockAccountAsync(Guid userId);

        #endregion

        #region Session Operations

        /// <summary>
        /// إنشاء جلسة مستخدم
        /// </summary>
        Task<Result<UserSession>> CreateSessionAsync(Guid userId, string ipAddress, string userAgent);

        /// <summary>
        /// الحصول على جلسة نشطة
        /// </summary>
        Task<Result<UserSession>> GetActiveSessionAsync(Guid sessionId);

        /// <summary>
        /// إنهاء جلسة
        /// </summary>
        Task<Result<bool>> EndSessionAsync(Guid sessionId);

        /// <summary>
        /// إنهاء جميع جلسات المستخدم
        /// </summary>
        Task<Result<bool>> EndAllUserSessionsAsync(Guid userId);

        /// <summary>
        /// الحصول على جلسات المستخدم النشطة
        /// </summary>
        Task<Result<List<UserSession>>> GetUserActiveSessionsAsync(Guid userId);

        #endregion

        #region Reports

        /// <summary>
        /// تقرير نشاط المستخدمين
        /// </summary>
        Task<Result<UserActivityReport>> GetUserActivityReportAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// تقرير محاولات الدخول
        /// </summary>
        Task<Result<LoginAttemptsReport>> GetLoginAttemptsReportAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// تقرير الصلاحيات
        /// </summary>
        Task<Result<PermissionsReport>> GetPermissionsReportAsync();

        #endregion
    }

    #region DTOs and Response Models

    public class LoginResponse
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; } = new();
        public List<string> Permissions { get; set; } = new();
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool RequiresTwoFactor { get; set; }
        public string Message { get; set; }
    }

    public class UserActivityReport
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int LockedUsers { get; set; }
        public int TotalLogins { get; set; }
        public int FailedLogins { get; set; }
        public List<UserActivitySummary> UserSummaries { get; set; } = new();
    }

    public class UserActivitySummary
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public int LoginCount { get; set; }
        public int FailedLoginCount { get; set; }
        public DateTime LastLoginDate { get; set; }
        public string LastIpAddress { get; set; }
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }
    }

    public class LoginAttemptsReport
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalAttempts { get; set; }
        public int SuccessfulLogins { get; set; }
        public int FailedLogins { get; set; }
        public double SuccessRate { get; set; }
        public List<LoginAttemptSummary> AttemptsByDate { get; set; } = new();
        public List<LoginAttemptSummary> AttemptsByUser { get; set; } = new();
        public List<LoginAttemptSummary> AttemptsByIp { get; set; } = new();
    }

    public class LoginAttemptSummary
    {
        public string Key { get; set; } // Date, Username, or IP
        public int TotalAttempts { get; set; }
        public int SuccessfulAttempts { get; set; }
        public int FailedAttempts { get; set; }
    }

    public class PermissionsReport
    {
        public DateTime ReportDate { get; set; }
        public int TotalRoles { get; set; }
        public int TotalPermissions { get; set; }
        public int TotalUsers { get; set; }
        public List<RolePermissionSummary> RoleSummaries { get; set; } = new();
    }

    public class RolePermissionSummary
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public int UserCount { get; set; }
        public int PermissionCount { get; set; }
        public List<string> Permissions { get; set; } = new();
    }

    #endregion
}
