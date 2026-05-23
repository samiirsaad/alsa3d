using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using AlSa3d.Core.Entities;
using AlSa3d.Core.Interfaces;
using AlSa3d.Core.DTOs;

namespace AlSa3d.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<Permission> _permissionRepository;
        private readonly IRepository<AuditLog> _auditLogRepository;

        public UserService(
            IRepository<User> userRepository,
            IRepository<Role> roleRepository,
            IRepository<Permission> permissionRepository,
            IRepository<AuditLog> auditLogRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<Result<User>> LoginAsync(string username, string password)
        {
            try
            {
                var user = await _userRepository.GetAsync(u => u.Username == username && !u.IsDeleted);
                
                if (user == null)
                    return Result.Failure<User>("اسم المستخدم غير موجود");

                if (!VerifyPassword(password, user.PasswordHash))
                    return Result.Failure<User>("كلمة المرور غير صحيحة");

                if (!user.IsActive)
                    return Result.Failure<User>("الحساب غير مفعل");

                // تسجيل الدخول
                user.LastLoginAt = DateTime.Now;
                user.LoginCount++;
                await _userRepository.UpdateAsync(user);

                // تسجيل العملية
                await LogAuditAsync(user.Id, "Login", $"تسجيل دخول ناجح من {username}");

                return Result.Success(user);
            }
            catch (Exception ex)
            {
                return Result.Failure<User>($"فشل في تسجيل الدخول: {ex.Message}");
            }
        }

        public async Task<Result<User>> RegisterAsync(RegisterUserDto dto)
        {
            try
            {
                // التحقق من عدم التكرار
                var existing = await _userRepository.GetAsync(u => u.Username == dto.Username && !u.IsDeleted);
                if (existing != null)
                    return Result.Failure<User>("اسم المستخدم مسجل بالفعل");

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
                }

                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<User>($"فشل في التسجيل: {ex.Message}");
            }
        }

        public async Task<Result<bool>> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null || user.IsDeleted)
                    return Result.Failure<bool>("المستخدم غير موجود");

                if (!VerifyPassword(currentPassword, user.PasswordHash))
                    return Result.Failure<bool>("كلمة المرور الحالية غير صحيحة");

                user.PasswordHash = HashPassword(newPassword);
                user.UpdatedAt = DateTime.Now;

                var result = await _userRepository.UpdateAsync(user);
                
                if (result.Success)
                {
                    await LogAuditAsync(userId, "ChangePassword", "تغيير كلمة المرور");
                }

                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<bool>($"فشل في تغيير كلمة المرور: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<User>>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userRepository.GetAllAsync(u => u.Role);
                return Result.Success(users.Where(u => !u.IsDeleted).OrderBy(u => u.Username));
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<User>>($"فشل في جلب المستخدمين: {ex.Message}");
            }
        }

        public async Task<Result<User>> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id, u => u.Role);
                if (user == null || user.IsDeleted)
                    return Result.Failure<User>("المستخدم غير موجود");

                return Result.Success(user);
            }
            catch (Exception ex)
            {
                return Result.Failure<User>($"فشل في جلب المستخدم: {ex.Message}");
            }
        }

        public async Task<Result<User>> UpdateUserAsync(int id, UpdateUserDto dto)
        {
            try
            {
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
                }

                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<User>($"فشل في تحديث المستخدم: {ex.Message}");
            }
        }

        public async Task<Result<bool>> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null || user.IsDeleted)
                    return Result.Failure<bool>("المستخدم غير موجود");

                user.IsDeleted = true;
                user.DeletedAt = DateTime.Now;

                var result = await _userRepository.UpdateAsync(user);
                
                if (result.Success)
                {
                    await LogAuditAsync(id, "DeleteUser", $"حذف المستخدم: {user.Username}");
                }

                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<bool>($"فشل في حذف المستخدم: {ex.Message}");
            }
        }

        public async Task<Result<Role>> CreateRoleAsync(CreateRoleDto dto)
        {
            try
            {
                var role = new Role
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    CreatedAt = DateTime.Now
                };

                var result = await _roleRepository.AddAsync(role);
                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<Role>($"فشل في إضافة الدور: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<Role>>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _roleRepository.GetAllAsync();
                return Result.Success(roles.Where(r => !r.IsDeleted).OrderBy(r => r.Name));
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<Role>>($"فشل في جلب الأدوار: {ex.Message}");
            }
        }

        public async Task<Result<bool>> AssignPermissionsAsync(int roleId, IEnumerable<int> permissionIds)
        {
            try
            {
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

                var result = await _roleRepository.UpdateAsync(role);
                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<bool>($"فشل في تعيين الصلاحيات: {ex.Message}");
            }
        }

        public async Task<Result<bool>> HasPermissionAsync(int userId, string permissionName)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId, u => u.Role);
                if (user == null || user.IsDeleted || user.Role == null)
                    return Result.Failure<bool>(false);

                var hasPermission = user.Role.Permissions != null && 
                    user.Role.Permissions.Any(p => p.Name == permissionName && !p.IsDeleted);

                return Result.Success(hasPermission);
            }
            catch (Exception ex)
            {
                return Result.Failure<bool>($"فشل في التحقق من الصلاحية: {ex.Message}");
            }
        }

        #region Helper Methods

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            var hash = HashPassword(password);
            return hash == storedHash;
        }

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
                    IpAddress = "127.0.0.1" // يمكن تحسينه لاحقاً
                };

                await _auditLogRepository.AddAsync(log);
            }
            catch
            {
                // تجاهل أخطاء اللوج لتجنب فشل العملية الرئيسية
            }
        }

        #endregion
    }
}
