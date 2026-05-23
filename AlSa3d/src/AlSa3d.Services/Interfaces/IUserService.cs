using AlSa3d.Core;
using AlSa3d.Core.DTOs;
using AlSa3d.Core.Entities;

namespace AlSa3d.Services.Interfaces;

public interface IUserService
{
    Task<Result<User>> LoginAsync(string username, string password);
    Task<Result<User>> RegisterAsync(RegisterUserDto dto);
    Task<Result<bool>> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task<Result<IEnumerable<User>>> GetAllUsersAsync();
    Task<Result<User>> GetUserByIdAsync(int id);
    Task<Result<User>> UpdateUserAsync(int id, UpdateUserDto dto);
    Task<Result<bool>> DeleteUserAsync(int id);
    Task<Result<Role>> CreateRoleAsync(CreateRoleDto dto);
    Task<Result<IEnumerable<Role>>> GetAllRolesAsync();
    Task<Result<bool>> AssignPermissionsAsync(int roleId, IEnumerable<int> permissionIds);
    Task<Result<bool>> HasPermissionAsync(int userId, string permissionName);
}
