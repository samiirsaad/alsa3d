using AlSa3d.Core.Common;

namespace AlSa3d.Core.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int RoleId { get; set; }
    public bool IsActive { get; set; } = true;
    public int LoginCount { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public DateTime? DeletedAt { get; set; }

    public virtual Role? Role { get; set; }
}

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}

public class Permission : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Resource { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

public class RolePermission : BaseEntity
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }

    public virtual Role? Role { get; set; }
    public virtual Permission? Permission { get; set; }
}
