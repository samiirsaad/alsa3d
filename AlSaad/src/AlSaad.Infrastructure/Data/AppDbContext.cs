using Microsoft.EntityFrameworkCore;
using AlSaad.Domain.Entities;

namespace AlSaad.Infrastructure.Data;

/// <summary>
/// قاعدة بيانات نظام السعد المحاسبي
/// تحتوي على جميع الجداول والعلاقات
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    #region Customers & Addresses

    public DbSet<Customer> Customers { get; set; }
    public DbSet<CustomerAddress> CustomerAddresses { get; set; }
    public DbSet<CustomerContact> CustomerContacts { get; set; }

    #endregion

    #region Invoices & Products

    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<StockMovement> StockMovements { get; set; }

    #endregion

    #region Employees & Salaries

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Salary> Salaries { get; set; }
    public DbSet<Attendance> Attendances { get; set; }

    #endregion

    #region Users & Security

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }

    #endregion

    #region Banks & Financial

    public DbSet<Bank> Banks { get; set; }
    public DbSet<BankAccount> BankAccounts { get; set; }
    public DbSet<Check> Checks { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Expense> Expenses { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // تطبيق جميع الإعدادات من Assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        #region Seed Data

        // إضافة الأدوار الأساسية
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin", NameAr = "مدير النظام", Description = "صلاحيات كاملة", CreatedAt = DateTime.UtcNow },
            new Role { Id = 2, Name = "Manager", NameAr = "مدير", Description = "صلاحيات إدارية", CreatedAt = DateTime.UtcNow },
            new Role { Id = 3, Name = "Accountant", NameAr = "محاسب", Description = "صلاحيات محاسبية", CreatedAt = DateTime.UtcNow },
            new Role { Id = 4, Name = "Sales", NameAr = "مبيعات", Description = "صلاحيات مبيعات", CreatedAt = DateTime.UtcNow },
            new Role { Id = 5, Name = "Viewer", NameAr = "مشاهد", Description = "قراءة فقط", CreatedAt = DateTime.UtcNow }
        );

        // إضافة المستخدم الافتراضي (admin / Admin@123)
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = "AQAAAAEAACcQAAAAEJqKxL8mC5aGz9F3vN8xP7yH2kR4tW6sU9nV0bM1cX3dY5eZ7fA8gB9hC0iD1jE2k==",
                FullName = "مدير النظام",
                Email = "admin@alsaad.com",
                Phone = "01000000000",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        );

        // ربط المدير بالدور
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole { UserId = 1, RoleId = 1 }
        );

        #endregion
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.Entity is BaseEntity && 
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = entry.Entity;
            
            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
