using AlSa3d.Core;
using AlSa3d.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlSa3d.Core.Entities;
using AlSa3d.Core.Interfaces;
using AlSa3d.Core.DTOs;
using Microsoft.Extensions.Logging;
using BCrypt.Net;

namespace AlSa3d.Services.Implementations
{
    /// <summary>
    /// خدمة إدارة المستخدمين والمصادقة والصلاحيات
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<Permission> _permissionRepository;
        private readonly IRepository<AuditLog> _auditLogRepository;
        private readonly ILogger<UserService> _logger;

        /// <summary>
        /// مُنشئ الخدمة
        /// </summary>
        public UserService(
            IRepository<User> userRepository,
            IRepository<Role> roleRepository,
            IRepository<Permission> permissionRepository,
            IRepository<AuditLog> auditLogRepository,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _auditLogRepository = auditLogRepository;
            _logger = logger;
        }

        /// <summary>
        /// تسجيل دخول المستخدم
        /// </summary>
        /// <param name="username">اسم المستخدم</param>
        /// <param name="password">كلمة المرور</param>
        /// <returns>نتيجة تحتوي على بيانات المستخدم عند النجاح</returns>
        public async Task<Result<User>> LoginAsync(string username, string password)
        {
            try
            {
                _logger.LogInformation("📝 محاولة تسجيل دخول: {username}", username);

                // التحقق من المدخلات
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    _logger.LogWarning("⚠️ محاولة تسجيل دخول ببيانات فارغة");
                    return Result.Failure<User>("اسم المستخدم وكلمة المرور مطلوبة");
                }

                var user = await _userRepository.GetAsync(u => u.Username == username && !u.IsDeleted);

                if (user == null)
                {
                    _logger.LogWarning("⚠️ محاولة تسجيل دخول باسم مستخدم غير موجود: {username}", username);
                    return Result.Failure<User>("اسم المستخدم غير موجود");
                }

                if (!VerifyPassword(password, user.PasswordHash))
                {
                    _logger.LogWarning("⚠️ محاولة تسجيل دخول بكلمة مرور خاطئة: {username}", username);
                    return Result.Failure<User>("كلمة المرور غير صحيحة");
                }

                if (!user.IsActive)
                {
                    _logger.LogWarning("⚠️ محاولة تسجيل دخول حساب غير مفعل: {username}", username);
                    return Result.Failure<User>("الحساب غير مفعل");
                }

                // تحديث آخر تسجيل دخول
                user.LastLoginAt = DateTime.Now;
                user.LoginCount++;
                await _userRepository.UpdateAsync(user);

                // تسجيل العملية
                await LogAuditAsync(user.Id, "Login", $"تسجيل دخول ناجح من {username}");

                _logger.LogInformation("✅ تسجيل دخول ناجح: {username}", username);
                return Result.Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ خطأ في تسجيل الدخول: {username}", username);
                return Result.Failure<User>("فشل في تسجيل الدخول", ex);
            }
        }

        /// <summary>
        /// تسجيل مستخدم جديد
        /// </summary>
        /// <param name="dto">بيانات المستخدم الجديد</param>
        /// <returns>نتيجة تحتوي على بيانات المستخدم الجديد</returns>
        public async Task<Result<User>> RegisterAsync(RegisterUserDto dto)
        {
            try
            {
                _logger.LogInformation("📝 تسجيل مستخدم جديد: {username}", dto.Username);

                // التحقق من صحة المدخلات
                if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                    return Result.Failure<User>("اسم المستخدم وكلمة المرور مطلوبة");

                if (dto.Password.Length < 6)
                    return Result.Failure<User>("كلمة المرور يجب أن تكون 6 أحرف على الأقل");

                // التحقق من عدم التكرار
                var existing = await _userRepository.GetAsync(u => u.Username == dto.Username && !u.IsDeleted);
                if (existing != null)
                {
                    _logger.LogWarning("⚠️ محاولة تسجيل مستخدم مكرر: {username}", dto.Username);
                    return Result.Failure<User>("اسم المستخدم مسجل بالفعل");
                }

                var user = new User
                {
                    Username = dto.Username,
                    PasswordHash = HashPassword(dto.Password),
                    FullName = dto.FullName,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    RoleId = dto.RoleId ?? 3, // Default: User
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                var result = await _userRepository.AddAsync(user);

                if (result.Success)
                {
                    await LogAuditAsync(user.Id, "Register", $"تسجيل مستخدم جديد: {dto.Username}");
                    _logger.LogInformation("✅ تم تسجيل مستخدم جديد: {username}", dto.Username);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ خطأ في تسجيل مستخدم جديد: {username}", dto.Username);
                return Result.Failure<User>("فشل في التسجيل", ex);
            }
        }

        /// <summary>
        /// تغيير كلمة مرور المستخدم
        /// </summary>
        /// <param name="userId">معرف المستخدم</param>
        /// <param name="currentPassword">كلمة المرور الحالية</param>
        /// <param name="newPassword">كلمة المرور الجديدة</param>
        /// <returns>نتيجة العملية</returns>
        public async Task<Result<bool>> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                _logger.LogInformation("📝 محاولة تغيير كلمة مرور المستخدم: {userId}", userId);

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null || user.IsDeleted)
                {
                    _logger.LogWarning("⚠️ محاولة تغيير كلمة مرور مستخدم غير موجود: {userId}", userId);
                    return Result.Failure<bool>("المستخدم غير موجود");
                }

                if (!VerifyPassword(currentPassword, user.PasswordHash))
                {
                    _logger.LogWarning("⚠️ محاولة تغيير كلمة مرور بكلمة مرور خاطئة: {userId}", userId);
                    return Result.Failure<bool>("كلمة المرور الحالية غير صحيحة");
                }

                if (newPassword.Length < 6)
                    return Result.Failure<bool>("كلمة المرور الجديدة يجب أن تكون 6 أحرف على الأقل");

                user.PasswordHash = HashPassword(newPassword);
                user.UpdatedAt = DateTime.Now;

                var updateResult = await _userRepository.UpdateAsync(user);

                if (updateResult.Success)
                {
                    await LogAuditAsync(userId, "ChangePassword", "تغيير كلمة المرور");
                    _logger.LogInformation("✅ تم تغيير كلمة المرور: {userId}", userId);
                }

                return Result.Ok(updateResult.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ خطأ في تغيير كلمة المرور: {userId}", userId);
                return Result.Failure<bool>("فشل في تغيير كلمة المرور", ex);
            }
        }

        /// <summary>
        /// الحصول على جميع المستخدمين
        /// </summary>
        /// <returns>قائمة بجميع المستخدمين النشطين</returns>
        public async Task<Result<IEnumerable<User>>> GetAllUsersAsync()
        {
            try
            {
                _logger.LogInformation("📖 جلب جميع المستخدمين");
                var users = await _userRepository.GetAllAsync(u => u.Role);
                return Result.Ok(users.Where(u => !u.IsDeleted).OrderBy(u => u.Username).AsEnumerable());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ خطأ في جلب المستخدمين");
                return Result.Failure<IEnumerable<User>>("فشل في جلب المستخدمين", ex);
            }
        }

        /// <summary>
        /// الحصول على مستخدم بواسطة معرفه
        /// </summary>
        /// <param name="id">معرف المستخدم</param>
        /// <returns>بيانات المستخدم</returns>
        public async Task<Result<User>> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id, u => u.Role);
                if (user == null || user.IsDeleted)
                    return Result.Failure<User>("المستخدم غير موجود");

                return Result.Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ خطأ في جلب المستخدم: {userId}", id);
                return Result.Failure<User>("فشل في جلب المستخدم", ex);
            }
        }

        /// <summary>
        /// تحديث بيانات المستخدم
        /// </summary>
        /// <param name="id">معرف المستخدم</param>
        /// <param name="dto">البيانات الجديدة</param>
        /// <returns>بيانات المستخدم المحدثة</returns>
        public async Task<Result<User>> UpdateUserAsync(int id, UpdateUserDto dto)
        {
            try
            {
                _logger.LogInformation("📝 تحديث بيانات المستخدم: {userId}", id);

                var user = await _userRepository.GetByIdAsync(id);
                if (user == null || user.IsDeleted)
                    return Result.Failure<User>("المستخدم غير موجود");

                user.FullName = dto.FullName;
                user.Email = dto.Email;
                user.Phone = dto.Phone;
                user.RoleId = dto.RoleId ?? user.RoleId;
                user.IsActive = dto.IsActive ?? user.IsActive;
                user.UpdatedAt = DateTime.Now;

                var result = await _userRepository.UpdateAsync(user);

                if (result.Success)
                {
                    await LogAuditAsync(id, "UpdateUser", $"تحديث بيانات المستخدم: {user.Username}");
                    _logger.LogInformation("✅ تم تحديث المستخدم: {userId}", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ خطأ في تحديث المستخدم: {userId}", id);
                return Result.Failure<User>("فشل في تحديث المستخدم", ex);
            }
        }

        /// <summary>
        /// حذف مستخدم (soft delete)
        /// </summary>
        /// <param name="id">معرف المستخدم</param>
        /// <returns>نتيجة العملية</returns>
        public async Task<Result<bool>> DeleteUserAsync(int id)
        {
            try
            {
                _logger.LogInformation("📝 حذف المستخدم: {userId}", id);

                var user = await _userRepository.GetByIdAsync(id);
                if (user == null || user.IsDeleted)
                    return Result.Failure<bool>("المستخدم غير موجود");

                user.IsDeleted = true;
                user.DeletedAt = DateTime.Now;

                var updateResult = await _userRepository.UpdateAsync(user);

                if (updateResult.Success)
                {
                    await LogAuditAsync(id, "DeleteUser", $"حذف المستخدم: {user.Username}");
                    _logger.LogInformation("✅ تم حذف المستخدم: {userId}", id);
                }

                return Result.Ok(updateResult.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ خطأ في حذف المستخدم: {userId}", id);
                return Result.Failure<bool>("فشل في حذف المستخدم", ex);
            }
        }

        /// <summary>
        /// إنشاء دور جديد
        /// </summary>
        /// <param name="dto">بيانات الدور الجديد</param>
        /// <returns>بيانات الدور الجديد</returns>
        public async Task<Result<Role>> CreateRoleAsync(CreateRoleDto dto)
        {
            try
            {
                _logger.LogInformation("📝 إنشاء دور جديد: {roleName}", dto.Name);

                var role = new Role
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    CreatedAt = DateTime.Now
                };

                var result = await _roleRepository.AddAsync(role);
                if (result.Success)
                {
                    _logger.LogInformation("✅ تم إنشاء دور جديد: {roleName}", dto.Name);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ خطأ في إنشاء الدور: {roleName}", dto.Name);
                return Result.Failure<Role>("فشل في إضافة الدور", ex);
            }
        }

        /// <summary>
        /// الحصول على جميع الأدوار
        /// </summary>
        /// <returns>قائمة بجميع الأدوار</returns>
        public async Task<Result<IEnumerable<Role>>> GetAllRolesAsync()
        {
            try
            {
                _logger.LogInformation("📖 جلب جميع الأدوار");
                var roles = await _roleRepository.GetAllAsync();
                return Result.Ok(roles.Where(r => !r.IsDeleted).OrderBy(r => r.Name).AsEnumerable());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ خطأ في جلب الأدوار");
                return Result.Failure<IEnumerable<Role>>("فشل في جلب الأدوار", ex);
            }
        }

        /// <summary>
        /// تعيين صلاحيات لدور
        /// </summary>
        /// <param name="roleId">معرف الدور</param>
        /// <param name="permissionIds">قائمة بمعرفات الصلاحيات</param>
        /// <returns>نتيجة العملية</returns>
        public async Task<Result<bool>> AssignPermissionsAsync(int roleId, IEnumerable<int> permissionIds)
        {
            try
            {
                _logger.LogInformation("📝 تعيين صلاحيات للدور: {roleId}", roleId);

                var role = await _roleRepository.GetByIdAsync(roleId, r => r.Permissions);
                if (role == null || role.IsDeleted)
                    return Result.Failure<bool>("الدور غير موجود");

                role.Permissions.Clear();

                foreach (var permissionId in permissionIds)
                {
                    var permission = await _permissionRepository.GetByIdAsync(permissionId);
                    if (permission != null && !permission.IsDeleted)
                    {
                        role.Permissions.Add(permission);
                    }
                }

                var updateResult = await _roleRepository.UpdateAsync(role);
                if (updateResult.Success)
                {
                    _logger.LogInformation("✅ تم تعيين الصلاحيات: {roleId}", roleId);
                }
                return Result.Ok(updateResult.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ خطأ في تعيين الصلاحيات: {roleId}", roleId);
                return Result.Failure<bool>("فشل في تعيين الصلاحيات", ex);
            }
        }

        /// <summary>
        /// التحقق من وجود صلاحية للمستخدم
        /// </summary>
        /// <param name="userId">معرف المستخدم</param>
        /// <param name="permissionName">اسم الصلاحية</param>
        /// <returns>هل يملك المستخدم الصلاحية</returns>
        public async Task<Result<bool>> HasPermissionAsync(int userId, string permissionName)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId, u => u.Role);
                if (user == null || user.IsDeleted || user.Role == null)
                    return Result.Ok(false);

                var hasPermission = user.Role.Permissions != null &&
                    user.Role.Permissions.Any(p => p.Name == permissionName && !p.IsDeleted);

                return Result.Ok(hasPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ خطأ في التحقق من الصلاحية: {userId}, {permission}", userId, permissionName);
                return Result.Failure<bool>("فشل في التحقق من الصلاحية", ex);
            }
        }


        #region Helper Methods

        /// <summary>
        /// تشفير كلمة المرور باستخدام BCrypt
        /// </summary>
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        /// <summary>
        /// التحقق من صحة كلمة المرور
        /// </summary>
        private bool VerifyPassword(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }

        /// <summary>
        /// تسجيل عملية في سجل التدقيق
        /// </summary>
        /// <param name="userId">معرف المستخدم</param>
        /// <param name="action">نوع الإجراء</param>
        /// <param name="description">وصف الإجراء</param>
        private async Task LogAuditAsync(int userId, string action, string description)
        {
            try
            {
                var log = new AuditLog
                {
                    UserId = userId,
                    Action = action,
                    Description = description,
                    Timestamp = DateTime.Now,
                    IpAddress = "127.0.0.1" // يمكن تحسينه للحصول على الـ IP الفعلي
                };

                await _auditLogRepository.AddAsync(log);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️ فشل تسجيل العملية في سجل التدقيق: {action}", action);
                // لا نرمي استثناء لتجنب فشل العملية الرئيسية
            }
        }

        #endregion
    }
}
