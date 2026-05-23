using Microsoft.EntityFrameworkCore;
using AlSa3d.Core.Entities;

namespace AlSa3d.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Customers
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Contact> Contacts { get; set; }

    // Invoices
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Return> Returns { get; set; }
    public DbSet<ReturnItem> ReturnItems { get; set; }

    // Products
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<ProductWarehouse> ProductWarehouses { get; set; }
    public DbSet<WarehouseProduct> WarehouseProducts { get; set; }
    public DbSet<PricingRule> PricingRules { get; set; }

    // Employees
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Salary> Salaries { get; set; }
    public DbSet<Attendance> Attendances { get; set; }

    // Users & Security
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }

    // Financial
    public DbSet<Bank> Banks { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Check> Checks { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<ExchangeRate> ExchangeRates { get; set; }

    // System
    public DbSet<Setting> Settings { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
