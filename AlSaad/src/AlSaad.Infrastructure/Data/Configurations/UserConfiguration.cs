using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AlSaad.Core.Entities;

namespace AlSaad.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Username)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.HasIndex(u => u.Username)
            .IsUnique();
        
        builder.Property(u => u.PasswordHash)
            .IsRequired();
        
        builder.Property(u => u.Email)
            .HasMaxLength(100);
        
        builder.HasIndex(u => u.Email)
            .IsUnique(false);
        
        builder.Property(u => u.FullName)
            .HasMaxLength(200);
        
        builder.Property(u => u.Phone)
            .HasMaxLength(20);
        
        builder.HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Name)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.HasIndex(r => r.Name)
            .IsUnique();
        
        builder.Property(r => r.Description)
            .HasMaxLength(200);
    }
}

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(p => p.Code)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.HasIndex(p => p.Code)
            .IsUnique();
        
        builder.Property(p => p.Group)
            .HasMaxLength(50);
    }
}

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");
        
        builder.HasKey(rp => rp.Id);
        
        builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId })
            .IsUnique();
        
        builder.HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(rp => rp.Permission)
            .WithMany()
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
