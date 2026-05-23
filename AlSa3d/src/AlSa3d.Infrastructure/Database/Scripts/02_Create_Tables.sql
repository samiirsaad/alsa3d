-- =============================================
-- AlSa3d Accounting System - Tables Creation
-- إنشاء الجداول والعلاقات
-- =============================================
-- التاريخ: 2026
-- الإصدار: 1.0
-- =============================================

USE [AlSa3d];
GO

-- =============================================
-- 1. جدول المستخدمين (Users)
-- =============================================
IF OBJECT_ID(N'[dbo].[Users]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Users](
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [UserName] NVARCHAR(50) NOT NULL UNIQUE,
        [Email] NVARCHAR(100) NOT NULL UNIQUE,
        [PasswordHash] NVARCHAR(256) NOT NULL,
        [FullName] NVARCHAR(100) NOT NULL,
        [Phone] NVARCHAR(20),
        [RoleId] UNIQUEIDENTIFIER NOT NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [LastLoginDate] DATETIME2 NULL,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [ModifiedDate] DATETIME2 NULL,
        [CreatedBy] UNIQUEIDENTIFIER NULL,
        [ModifiedBy] UNIQUEIDENTIFIER NULL
    );
    
    -- فهارس للأداء
    CREATE NONCLUSTERED INDEX [IX_Users_UserName] ON [dbo].[Users]([UserName]);
    CREATE NONCLUSTERED INDEX [IX_Users_Email] ON [dbo].[Users]([Email]);
    CREATE NONCLUSTERED INDEX [IX_Users_RoleId] ON [dbo].[Users]([RoleId]);
    CREATE NONCLUSTERED INDEX [IX_Users_IsActive] ON [dbo].[Users]([IsActive]) WHERE [IsDeleted] = 0;
    
    PRINT '✓ تم إنشاء جدول Users';
END
GO

-- =============================================
-- 2. جدول الأدوار (Roles)
-- =============================================
IF OBJECT_ID(N'[dbo].[Roles]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Roles](
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [RoleName] NVARCHAR(50) NOT NULL UNIQUE,
        [Description] NVARCHAR(200),
        [IsSystem] BIT NOT NULL DEFAULT 0,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [ModifiedDate] DATETIME2 NULL
    );
    
    CREATE NONCLUSTERED INDEX [IX_Roles_RoleName] ON [dbo].[Roles]([RoleName]);
    
    PRINT '✓ تم إنشاء جدول Roles';
END
GO

-- =============================================
-- 3. جدول الصلاحيات (Permissions)
-- =============================================
IF OBJECT_ID(N'[dbo].[Permissions]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Permissions](
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [PermissionName] NVARCHAR(100) NOT NULL,
        [DisplayName] NVARCHAR(100) NOT NULL,
        [Category] NVARCHAR(50) NOT NULL,
        [Description] NVARCHAR(200),
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE()
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Permissions_PermissionName] ON [dbo].[Permissions]([PermissionName]) WHERE [IsDeleted] = 0;
    CREATE NONCLUSTERED INDEX [IX_Permissions_Category] ON [dbo].[Permissions]([Category]);
    
    PRINT '✓ تم إنشاء جدول Permissions';
END
GO

-- =============================================
-- 4. جدول صلاحيات الأدوار (RolePermissions)
-- =============================================
IF OBJECT_ID(N'[dbo].[RolePermissions]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[RolePermissions](
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [RoleId] UNIQUEIDENTIFIER NOT NULL,
        [PermissionId] UNIQUEIDENTIFIER NOT NULL,
        [CanView] BIT NOT NULL DEFAULT 0,
        [CanAdd] BIT NOT NULL DEFAULT 0,
        [CanEdit] BIT NOT NULL DEFAULT 0,
        [CanDelete] BIT NOT NULL DEFAULT 0,
        [CanPrint] BIT NOT NULL DEFAULT 0,
        [CanExport] BIT NOT NULL DEFAULT 0,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        
        CONSTRAINT [FK_RolePermissions_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RolePermissions_Permissions] FOREIGN KEY ([PermissionId]) REFERENCES [dbo].[Permissions]([Id]) ON DELETE CASCADE,
        CONSTRAINT [UK_RolePermission] UNIQUE ([RoleId], [PermissionId])
    );
    
    CREATE NONCLUSTERED INDEX [IX_RolePermissions_RoleId] ON [dbo].[RolePermissions]([RoleId]);
    CREATE NONCLUSTERED INDEX [IX_RolePermissions_PermissionId] ON [dbo].[RolePermissions]([PermissionId]);
    
    PRINT '✓ تم إنشاء جدول RolePermissions';
END
GO

-- =============================================
-- 5. جدول العملاء (Customers)
-- =============================================
IF OBJECT_ID(N'[dbo].[Customers]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Customers](
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [CustomerCode] NVARCHAR(20) NOT NULL UNIQUE,
        [CustomerName] NVARCHAR(150) NOT NULL,
        [CustomerType] INT NOT NULL DEFAULT 0, -- 0: فرد، 1: شركة
        [NationalId] NVARCHAR(20),
        [TaxNumber] NVARCHAR(20),
        [Phone] NVARCHAR(20),
        [Mobile] NVARCHAR(20),
        [Email] NVARCHAR(100),
        [Address] NVARCHAR(500),
        [City] NVARCHAR(50),
        [Governorate] NVARCHAR(50),
        [PostalCode] NVARCHAR(10),
        [Country] NVARCHAR(50) DEFAULT N'مصر',
        [CreditLimit] DECIMAL(18,2) DEFAULT 0,
        [CurrentBalance] DECIMAL(18,2) DEFAULT 0,
        [Notes] NVARCHAR(1000),
        [IsActive] BIT NOT NULL DEFAULT 1,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [ModifiedDate] DATETIME2 NULL,
        [CreatedBy] UNIQUEIDENTIFIER NULL,
        [ModifiedBy] UNIQUEIDENTIFIER NULL
    );
    
    CREATE NONCLUSTERED INDEX [IX_Customers_CustomerCode] ON [dbo].[Customers]([CustomerCode]);
    CREATE NONCLUSTERED INDEX [IX_Customers_CustomerName] ON [dbo].[Customers]([CustomerName]);
    CREATE NONCLUSTERED INDEX [IX_Customers_Phone] ON [dbo].[Customers]([Phone]);
    CREATE NONCLUSTERED INDEX [IX_Customers_NationalId] ON [dbo].[Customers]([NationalId]) WHERE [NationalId] IS NOT NULL;
    CREATE NONCLUSTERED INDEX [IX_Customers_IsActive] ON [dbo].[Customers]([IsActive]) WHERE [IsDeleted] = 0;
    
    PRINT '✓ تم إنشاء جدول Customers';
END
GO

-- =============================================
-- 6. جدول عناوين العملاء (Addresses)
-- =============================================
IF OBJECT_ID(N'[dbo].[Addresses]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Addresses](
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [CustomerId] UNIQUEIDENTIFIER NOT NULL,
        [AddressType] INT NOT NULL DEFAULT 0, -- 0: رئيسي، 1: فرعي، 2: شحن
        [AddressLine1] NVARCHAR(200) NOT NULL,
        [AddressLine2] NVARCHAR(200),
        [City] NVARCHAR(50) NOT NULL,
        [Governorate] NVARCHAR(50),
        [PostalCode] NVARCHAR(10),
        [Country] NVARCHAR(50) DEFAULT N'مصر',
        [IsDefault] BIT NOT NULL DEFAULT 0,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [ModifiedDate] DATETIME2 NULL,
        
        CONSTRAINT [FK_Addresses_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers]([Id]) ON DELETE CASCADE
    );
    
    CREATE NONCLUSTERED INDEX [IX_Addresses_CustomerId] ON [dbo].[Addresses]([CustomerId]);
    CREATE NONCLUSTERED INDEX [IX_Addresses_City] ON [dbo].[Addresses]([City]);
    
    PRINT '✓ تم إنشاء جدول Addresses';
END
GO

-- =============================================
-- 7. جدول جهات الاتصال (Contacts)
-- =============================================
IF OBJECT_ID(N'[dbo].[Contacts]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Contacts](
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [CustomerId] UNIQUEIDENTIFIER NOT NULL,
        [ContactName] NVARCHAR(100) NOT NULL,
        [Position] NVARCHAR(50),
        [Phone] NVARCHAR(20),
        [Mobile] NVARCHAR(20),
        [Email] NVARCHAR(100),
        [IsPrimary] BIT NOT NULL DEFAULT 0,
        [Notes] NVARCHAR(500),
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [ModifiedDate] DATETIME2 NULL,
        
        CONSTRAINT [FK_Contacts_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers]([Id]) ON DELETE CASCADE
    );
    
    CREATE NONCLUSTERED INDEX [IX_Contacts_CustomerId] ON [dbo].[Contacts]([CustomerId]);
    CREATE NONCLUSTERED INDEX [IX_Contacts_Phone] ON [dbo].[Contacts]([Phone]);
    
    PRINT '✓ تم إنشاء جدول Contacts';
END
GO

-- =============================================
-- 8. جدول التصنيفات (Categories)
-- =============================================
IF OBJECT_ID(N'[dbo].[Categories]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Categories](
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [CategoryName] NVARCHAR(100) NOT NULL,
        [ParentCategoryId] UNIQUEIDENTIFIER NULL,
        [Description] NVARCHAR(500),
        [SortOrder] INT NOT NULL DEFAULT 0,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [ModifiedDate] DATETIME2 NULL,
        
        CONSTRAINT [FK_Categories_Categories] FOREIGN KEY ([ParentCategoryId]) REFERENCES [dbo].[Categories]([Id])
    );
    
    CREATE NONCLUSTERED INDEX [IX_Categories_CategoryName] ON [dbo].[Categories]([CategoryName]);
    CREATE NONCLUSTERED INDEX [IX_Categories_ParentCategoryId] ON [dbo].[Categories]([ParentCategoryId]) WHERE [ParentCategoryId] IS NOT NULL;
    
    PRINT '✓ تم إنشاء جدول Categories';
END
GO

-- =============================================
-- 9. جدول المنتجات (Products)
-- =============================================
IF OBJECT_ID(N'[dbo].[Products]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Products](
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [ProductCode] NVARCHAR(20) NOT NULL UNIQUE,
        [ProductName] NVARCHAR(150) NOT NULL,
        [CategoryId] UNIQUEIDENTIFIER NOT NULL,
        [Barcode] NVARCHAR(50),
        [QRCode] NVARCHAR(100),
        [Description] NVARCHAR(1000),
        [Unit] NVARCHAR(20) NOT NULL DEFAULT N'قطعة',
        [CostPrice] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [SellingPrice] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [WholesalePrice] DECIMAL(18,2) DEFAULT 0,
        [RetailPrice] DECIMAL(18,2) DEFAULT 0,
        [MinStockLevel] INT NOT NULL DEFAULT 0,
        [MaxStockLevel] INT DEFAULT 0,
        [CurrentStock] INT NOT NULL DEFAULT 0,
        [ReorderPoint] INT DEFAULT 0,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [ImageUrl] NVARCHAR(500),
        [Notes] NVARCHAR(1000),
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [ModifiedDate] DATETIME2 NULL,
        [CreatedBy] UNIQUEIDENTIFIER NULL,
        [ModifiedBy] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [FK_Products_Categories] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories]([Id])
    );
    
    CREATE NONCLUSTERED INDEX [IX_Products_ProductCode] ON [dbo].[Products]([ProductCode]);
    CREATE NONCLUSTERED INDEX [IX_Products_ProductName] ON [dbo].[Products]([ProductName]);
    CREATE NONCLUSTERED INDEX [IX_Products_Barcode] ON [dbo].[Products]([Barcode]) WHERE [Barcode] IS NOT NULL;
    CREATE NONCLUSTERED INDEX [IX_Products_CategoryId] ON [dbo].[Products]([CategoryId]);
    CREATE NONCLUSTERED INDEX [IX_Products_IsActive] ON [dbo].[Products]([IsActive]) WHERE [IsDeleted] = 0;
    
    PRINT '✓ تم إنشاء جدول Products';
END
GO

-- =============================================
-- 10. جدول المخازن (Warehouses)
-- =============================================
IF OBJECT_ID(N'[dbo].[Warehouses]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Warehouses](
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [WarehouseCode] NVARCHAR(20) NOT NULL UNIQUE,
        [WarehouseName] NVARCHAR(100) NOT NULL,
        [Address] NVARCHAR(500),
        [Phone] NVARCHAR(20),
        [ManagerName] NVARCHAR(100),
        [IsActive] BIT NOT NULL DEFAULT 1,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [ModifiedDate] DATETIME2 NULL
    );
    
    CREATE NONCLUSTERED INDEX [IX_Warehouses_WarehouseCode] ON [dbo].[Warehouses]([WarehouseCode]);
    CREATE NONCLUSTERED INDEX [IX_Warehouses_IsActive] ON [dbo].[Warehouses]([IsActive]) WHERE [IsDeleted] = 0;
    
    PRINT '✓ تم إنشاء جدول Warehouses';
END
GO

-- =============================================
-- 11. جدول مخزون المنتجات (ProductStock)
-- =============================================
IF OBJECT_ID(N'[dbo].[ProductStock]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ProductStock](
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [ProductId] UNIQUEIDENTIFIER NOT NULL,
        [WarehouseId] UNIQUEIDENTIFIER NOT NULL,
        [Quantity] INT NOT NULL DEFAULT 0,
        [ReservedQuantity] INT NOT NULL DEFAULT 0,
        [AvailableQuantity] AS ([Quantity] - [ReservedQuantity]),
        [LastUpdated] DATETIME2 NOT NULL DEFAULT GETDATE(),
        
        CONSTRAINT [FK_ProductStock_Products] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products]([Id]),
        CONSTRAINT [FK_ProductStock_Warehouses] FOREIGN KEY ([WarehouseId]) REFERENCES [dbo].[Warehouses]([Id]),
        CONSTRAINT [UK_ProductStock_Product_Warehouse] UNIQUE ([ProductId], [WarehouseId])
    );
    
    CREATE NONCLUSTERED INDEX [IX_ProductStock_ProductId] ON [dbo].[ProductStock]([ProductId]);
    CREATE NONCLUSTERED INDEX [IX_ProductStock_WarehouseId] ON [dbo].[ProductStock]([WarehouseId]);
    
    PRINT '✓ تم إنشاء جدول ProductStock';
END
GO

-- =============================================
-- 12. جدول الفواتير (Invoices)
-- =============================================
IF OBJECT_ID(N'[dbo].[Invoices]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Invoices](
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [InvoiceNumber] NVARCHAR(20) NOT NULL UNIQUE,
        [InvoiceDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [CustomerId] UNIQUEIDENTIFIER NOT NULL,
        [InvoiceType] INT NOT NULL DEFAULT 0, -- 0: مبيعات، 1: مشتريات، 2: مرتجع
        [Status] INT NOT NULL DEFAULT 0, -- 0: مسودة، 1: معتمد، 2: ملغى
        [SubTotal] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [DiscountAmount] DECIMAL(18,2) DEFAULT 0,
        [DiscountPercent] DECIMAL(5,2) DEFAULT 0,
        [TaxAmount] DECIMAL(18,2) DEFAULT 0,
        [TaxPercent] DECIMAL(5,2) DEFAULT 14,
        [TotalAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [PaidAmount] DECIMAL(18,2) DEFAULT 0,
        [RemainingAmount] AS ([TotalAmount] - [PaidAmount]),
        [PaymentMethod] INT DEFAULT 0, -- 0: كاش، 1: شيك، 2: تحويل، 3: آجل
        [Notes] NVARCHAR(1000),
        [ShippingAddress] NVARCHAR(500),
        [WarehouseId] UNIQUEIDENTIFIER,
        [SalesPersonId] UNIQUEIDENTIFIER,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [ModifiedDate] DATETIME2 NULL,
        [CreatedBy] UNIQUEIDENTIFIER NULL,
        [ModifiedBy] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [FK_Invoices_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers]([Id]),
        CONSTRAINT [FK_Invoices_Warehouses] FOREIGN KEY ([WarehouseId]) REFERENCES [dbo].[Warehouses]([Id]),
        CONSTRAINT [FK_Invoices_Users] FOREIGN KEY ([SalesPersonId]) REFERENCES [dbo].[Users]([Id])
    );
    
    CREATE NONCLUSTERED INDEX [IX_Invoices_InvoiceNumber] ON [dbo].[Invoices]([InvoiceNumber]);
    CREATE NONCLUSTERED INDEX [IX_Invoices_InvoiceDate] ON [dbo].[Invoices]([InvoiceDate]);
    CREATE NONCLUSTERED INDEX [IX_Invoices_CustomerId] ON [dbo].[Invoices]([CustomerId]);
    CREATE NONCLUSTERED INDEX [IX_Invoices_Status] ON [dbo].[Invoices]([Status]);
    CREATE NONCLUSTERED INDEX [IX_Invoices_InvoiceType] ON [dbo].[Invoices]([InvoiceType]);
    
    PRINT '✓ تم إنشاء جدول Invoices';
END
GO

-- =============================================
-- 13. جدول أصناف الفواتير (InvoiceItems)
-- =============================================
IF OBJECT_ID(N'[dbo].[InvoiceItems]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[InvoiceItems](
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [InvoiceId] UNIQUEIDENTIFIER NOT NULL,
        [ProductId] UNIQUEIDENTIFIER NOT NULL,
        [ItemCode] NVARCHAR(20) NOT NULL,
        [ItemName] NVARCHAR(150) NOT NULL,
        [Quantity] DECIMAL(18,3) NOT NULL DEFAULT 0,
        [UnitPrice] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [DiscountPercent] DECIMAL(5,2) DEFAULT 0,
        [DiscountAmount] DECIMAL(18,2) DEFAULT 0,
        [TaxPercent] DECIMAL(5,2) DEFAULT 14,
        [TaxAmount] DECIMAL(18,2) DEFAULT 0,
        [TotalAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [Notes] NVARCHAR(500),
        
        CONSTRAINT [FK_InvoiceItems_Invoices] FOREIGN KEY ([InvoiceId]) REFERENCES [dbo].[Invoices]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_InvoiceItems_Products] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products]([Id])
    );
    
    CREATE NONCLUSTERED INDEX [IX_InvoiceItems_InvoiceId] ON [dbo].[InvoiceItems]([InvoiceId]);
    CREATE NONCLUSTERED INDEX [IX_InvoiceItems_ProductId] ON [dbo].[InvoiceItems]([ProductId]);
    
    PRINT '✓ تم إنشاء جدول InvoiceItems';
END
GO

-- =============================================
-- 14. جدول المرتجعات (Returns)
-- =============================================
IF OBJECT_ID(N'[dbo].[Returns]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Returns](
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [ReturnNumber] NVARCHAR(20) NOT NULL UNIQUE,
        [ReturnDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [InvoiceId] UNIQUEIDENTIFIER NOT NULL,
        [CustomerId] UNIQUEIDENTIFIER NOT NULL,
        [Reason] NVARCHAR(500),
        [TotalAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [Status] INT NOT NULL DEFAULT 0, -- 0: قيد المعالجة، 1: معتمد، 2: مرفوض
        [Notes] NVARCHAR(1000),
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [ModifiedDate] DATETIME2 NULL,
        [CreatedBy] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [FK_Returns_Invoices] FOREIGN KEY ([InvoiceId]) REFERENCES [dbo].[Invoices]([Id]),
        CONSTRAINT [FK_Returns_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers]([Id])
    );
    
    CREATE NONCLUSTERED INDEX [IX_Returns_ReturnNumber] ON [dbo].[Returns]([ReturnNumber]);
    CREATE NONCLUSTERED INDEX [IX_Returns_InvoiceId] ON [dbo].[Returns]([InvoiceId]);
    CREATE NONCLUSTERED INDEX [IX_Returns_ReturnDate] ON [dbo].[Returns]([ReturnDate]);
    
    PRINT '✓ تم إنشاء جدول Returns';
END
GO

-- =============================================
-- 15. جدول الأقسام (Departments)
-- =============================================
IF OBJECT_ID(N'[dbo].[Departments]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Departments](
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [DepartmentName] NVARCHAR(100) NOT NULL,
        [ParentDepartmentId] UNIQUEIDENTIFIER NULL,
        [Description] NVARCHAR(500),
        [IsActive] BIT NOT NULL DEFAULT 1,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [ModifiedDate] DATETIME2 NULL,
        
        CONSTRAINT [FK_Departments_Departments] FOREIGN KEY ([ParentDepartmentId]) REFERENCES [dbo].[Departments]([Id])
    );
    
    CREATE NONCLUSTERED INDEX [IX_Departments_DepartmentName] ON [dbo].[Departments]([DepartmentName]);
    CREATE NONCLUSTERED INDEX [IX_Departments_ParentDepartmentId] ON [dbo].[Departments]([ParentDepartmentId]) WHERE [ParentDepartmentId] IS NOT NULL;
    
    PRINT '✓ تم إنشاء جدول Departments';
END
GO

-- =============================================
-- 16. جدول الموظفين (Employees)
-- =============================================
IF OBJECT_ID(N'[dbo].[Employees]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Employees](
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [EmployeeCode] NVARCHAR(20) NOT NULL UNIQUE,
        [EmployeeName] NVARCHAR(100) NOT NULL,
        [NationalId] NVARCHAR(20) UNIQUE,
        [DepartmentId] UNIQUEIDENTIFIER NOT NULL,
        [Position] NVARCHAR(50),
        [HireDate] DATE NOT NULL,
        [EndDate] DATE NULL,
        [Phone] NVARCHAR(20),
        [Mobile] NVARCHAR(20),
        [Email] NVARCHAR(100),
        [Address] NVARCHAR(500),
        [BasicSalary] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [Allowances] DECIMAL(18,2) DEFAULT 0,
        [Deductions] DECIMAL(18,2) DEFAULT 0,
        [BankAccountId] UNIQUEIDENTIFIER NULL,
        [UserId] UNIQUEIDENTIFIER NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [ModifiedDate] DATETIME2 NULL,
        
        CONSTRAINT [FK_Employees_Departments] FOREIGN KEY ([DepartmentId]) REFERENCES [dbo].[Departments]([Id]),
        CONSTRAINT [FK_Employees_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id])
    );
    
    CREATE NONCLUSTERED INDEX [IX_Employees_EmployeeCode] ON [dbo].[Employees]([EmployeeCode]);
    CREATE NONCLUSTERED INDEX [IX_Employees_EmployeeName] ON [dbo].[Employees]([EmployeeName]);
    CREATE NONCLUSTERED INDEX [IX_Employees_DepartmentId] ON [dbo].[Employees]([DepartmentId]);
    CREATE NONCLUSTERED INDEX [IX_Employees_NationalId] ON [dbo].[Employees]([NationalId]) WHERE [NationalId] IS NOT NULL;
    CREATE NONCLUSTERED INDEX [IX_Employees_IsActive] ON [dbo].[Employees]([IsActive]) WHERE [IsDeleted] = 0;
    
    PRINT '✓ تم إنشاء جدول Employees';
END
GO

-- =============================================
-- 17. جدول الرواتب (Salaries)
-- =============================================
IF OBJECT_ID(N'[dbo].[Salaries]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Salaries](
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [EmployeeId] UNIQUEIDENTIFIER NOT NULL,
        [SalaryMonth] DATE NOT NULL,
        [BasicSalary] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [Allowances] DECIMAL(18,2) DEFAULT 0,
        [Overtime] DECIMAL(18,2) DEFAULT 0,
        [Bonuses] DECIMAL(18,2) DEFAULT 0,
        [Deductions] DECIMAL(18,2) DEFAULT 0,
        [AbsenceDeductions] DECIMAL(18,2) DEFAULT 0,
        [NetSalary] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [PaymentDate] DATE NULL,
        [PaymentMethod] INT DEFAULT 0, -- 0: كاش، 1: تحويل بنكي
        [Notes] NVARCHAR(500),
        [IsPaid] BIT NOT NULL DEFAULT 0,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [ModifiedDate] DATETIME2 NULL,
        [CreatedBy] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [FK_Salaries_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees]([Id]),
        CONSTRAINT [UK_Employee_SalaryMonth] UNIQUE ([EmployeeId], [SalaryMonth])
    );
    
    CREATE NONCLUSTERED INDEX [IX_Salaries_EmployeeId] ON [dbo].[Salaries]([EmployeeId]);
    CREATE NONCLUSTERED INDEX [IX_Salaries_SalaryMonth] ON [dbo].[Salaries]([SalaryMonth]);
    CREATE NONCLUSTERED INDEX [IX_Salaries_IsPaid] ON [dbo].[Salaries]([IsPaid]);
    
    PRINT '✓ تم إنشاء جدول Salaries';
END
GO

-- =============================================
-- 18. جدول الحضور والغياب (Attendance)
-- =============================================
IF OBJECT_ID(N'[dbo].[Attendance]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Attendance](
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [EmployeeId] UNIQUEIDENTIFIER NOT NULL,
        [AttendanceDate] DATE NOT NULL,
        [CheckIn] DATETIME2 NULL,
        [CheckOut] DATETIME2 NULL,
        [Status] INT NOT NULL DEFAULT 0, -- 0: حاضر، 1: غائب، 2: إجازة، 3: متأخر
        [LateMinutes] INT DEFAULT 0,
        [OvertimeHours] DECIMAL(5,2) DEFAULT 0,
        [Notes] NVARCHAR(500),
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [ModifiedDate] DATETIME2 NULL,
        
        CONSTRAINT [FK_Attendance_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees]([Id]),
        CONSTRAINT [UK_Employee_AttendanceDate] UNIQUE ([EmployeeId], [AttendanceDate])
    );
    
    CREATE NONCLUSTERED INDEX [IX_Attendance_EmployeeId] ON [dbo].[Attendance]([EmployeeId]);
    CREATE NONCLUSTERED INDEX [IX_Attendance_AttendanceDate] ON [dbo].[Attendance]([AttendanceDate]);
    CREATE NONCLUSTERED INDEX [IX_Attendance_Status] ON [dbo].[Attendance]([Status]);
    
    PRINT '✓ تم إنشاء جدول Attendance';
END
GO

-- =============================================
-- 19. جدول البنوك (Banks)
-- =============================================
IF OBJECT_ID(N'[dbo].[Banks]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Banks](
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [BankName] NVARCHAR(100) NOT NULL,
        [BankCode] NVARCHAR(10) NOT NULL UNIQUE,
        [SwiftCode] NVARCHAR(20),
        [Address] NVARCHAR(500),
        [Phone] NVARCHAR(20),
        [IsActive] BIT NOT NULL DEFAULT 1,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [ModifiedDate] DATETIME2 NULL
    );
    
    CREATE NONCLUSTERED INDEX [IX_Banks_BankName] ON [dbo].[Banks]([BankName]);
    CREATE NONCLUSTERED INDEX [IX_Banks_IsActive] ON [dbo].[Banks]([IsActive]) WHERE [IsDeleted] = 0;
    
    PRINT '✓ تم إنشاء جدول Banks';
END
GO

-- =============================================
-- 20. جدول الحسابات البنكية (Accounts)
-- =============================================
IF OBJECT_ID(N'[dbo].[Accounts]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Accounts](
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [AccountName] NVARCHAR(100) NOT NULL,
        [AccountNumber] NVARCHAR(30) NOT NULL UNIQUE,
        [BankId] UNIQUEIDENTIFIER NOT NULL,
        [CurrencyId] UNIQUEIDENTIFIER NOT NULL,
        [Balance] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [AccountType] INT NOT NULL DEFAULT 0, -- 0: جارٍ، 1: توفير
        [IsActive] BIT NOT NULL DEFAULT 1,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [ModifiedDate] DATETIME2 NULL,
        
        CONSTRAINT [FK_Accounts_Banks] FOREIGN KEY ([BankId]) REFERENCES [dbo].[Banks]([Id]),
        CONSTRAINT [FK_Accounts_Currencies] FOREIGN KEY ([CurrencyId]) REFERENCES [dbo].[Currencies]([Id])
    );
    
    CREATE NONCLUSTERED INDEX [IX_Accounts_AccountNumber] ON [dbo].[Accounts]([AccountNumber]);
    CREATE NONCLUSTERED INDEX [IX_Accounts_BankId] ON [dbo].[Accounts]([BankId]);
    CREATE NONCLUSTERED INDEX [IX_Accounts_IsActive] ON [dbo].[Accounts]([IsActive]) WHERE [IsDeleted] = 0;
    
    PRINT '✓ تم إنشاء جدول Accounts';
END
GO

-- =============================================
-- 21. جدول الشيكات (Checks)
-- =============================================
IF OBJECT_ID(N'[dbo].[Checks]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Checks](
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [CheckNumber] NVARCHAR(20) NOT NULL,
        [CheckDate] DATE NOT NULL,
        [DueDate] DATE NOT NULL,
        [Amount] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [CurrencyId] UNIQUEIDENTIFIER NOT NULL,
        [AccountId] UNIQUEIDENTIFIER NOT NULL,
        [CustomerId] UNIQUEIDENTIFIER NULL,
        [VendorId] UNIQUEIDENTIFIER NULL,
        [Status] INT NOT NULL DEFAULT 0, -- 0: تحت التحصيل، 1: تم الصرف، 2: مرتجع، 3: ملغى
        [Notes] NVARCHAR(500),
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [ModifiedDate] DATETIME2 NULL,
        [CreatedBy] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [FK_Checks_Currencies] FOREIGN KEY ([CurrencyId]) REFERENCES [dbo].[Currencies]([Id]),
        CONSTRAINT [FK_Checks_Accounts] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Accounts]([Id]),
        CONSTRAINT [FK_Checks_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers]([Id])
    );
    
    CREATE NONCLUSTERED INDEX [IX_Checks_CheckNumber] ON [dbo].[Checks]([CheckNumber]);
    CREATE NONCLUSTERED INDEX [IX_Checks_DueDate] ON [dbo].[Checks]([DueDate]);
    CREATE NONCLUSTERED INDEX [IX_Checks_Status] ON [dbo].[Checks]([Status]);
    CREATE NONCLUSTERED INDEX [IX_Checks_AccountId] ON [dbo].[Checks]([AccountId]);
    
    PRINT '✓ تم إنشاء جدول Checks';
END
GO

-- =============================================
-- 22. جدول العملات (Currencies)
-- =============================================
IF OBJECT_ID(N'[dbo].[Currencies]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Currencies](
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [CurrencyCode] NVARCHAR(3) NOT NULL UNIQUE,
        [CurrencyName] NVARCHAR(50) NOT NULL,
        [Symbol] NVARCHAR(5),
        [ExchangeRate] DECIMAL(18,6) NOT NULL DEFAULT 1,
        [IsBaseCurrency] BIT NOT NULL DEFAULT 0,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [ModifiedDate] DATETIME2 NULL
    );
    
    CREATE NONCLUSTERED INDEX [IX_Currencies_CurrencyCode] ON [dbo].[Currencies]([CurrencyCode]);
    CREATE NONCLUSTERED INDEX [IX_Currencies_IsBaseCurrency] ON [dbo].[Currencies]([IsBaseCurrency]) WHERE [IsDeleted] = 0;
    
    PRINT '✓ تم إنشاء جدول Currencies';
END
GO

-- =============================================
-- 23. جدول أسعار الصرف (ExchangeRates)
-- =============================================
IF OBJECT_ID(N'[dbo].[ExchangeRates]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ExchangeRates](
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [FromCurrencyId] UNIQUEIDENTIFIER NOT NULL,
        [ToCurrencyId] UNIQUEIDENTIFIER NOT NULL,
        [Rate] DECIMAL(18,6) NOT NULL,
        [EffectiveDate] DATE NOT NULL DEFAULT GETDATE(),
        [ExpiryDate] DATE NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [CreatedBy] UNIQUEIDENTIFIER NULL,
        
        CONSTRAINT [FK_ExchangeRates_FromCurrencies] FOREIGN KEY ([FromCurrencyId]) REFERENCES [dbo].[Currencies]([Id]),
        CONSTRAINT [FK_ExchangeRates_ToCurrencies] FOREIGN KEY ([ToCurrencyId]) REFERENCES [dbo].[Currencies]([Id])
    );
    
    CREATE NONCLUSTERED INDEX [IX_ExchangeRates_FromCurrencyId] ON [dbo].[ExchangeRates]([FromCurrencyId]);
    CREATE NONCLUSTERED INDEX [IX_ExchangeRates_ToCurrencyId] ON [dbo].[ExchangeRates]([ToCurrencyId]);
    CREATE NONCLUSTERED INDEX [IX_ExchangeRates_EffectiveDate] ON [dbo].[ExchangeRates]([EffectiveDate]);
    
    PRINT '✓ تم إنشاء جدول ExchangeRates';
END
GO

-- =============================================
-- 24. جدول الإعدادات (Settings)
-- =============================================
IF OBJECT_ID(N'[dbo].[Settings]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Settings](
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [SettingKey] NVARCHAR(100) NOT NULL UNIQUE,
        [SettingValue] NVARCHAR(MAX),
        [SettingType] NVARCHAR(20) NOT NULL DEFAULT 'String', -- String, Int, Decimal, Bool, JSON
        [Description] NVARCHAR(500),
        [IsSystem] BIT NOT NULL DEFAULT 0,
        [ModifiedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [ModifiedBy] UNIQUEIDENTIFIER NULL
    );
    
    CREATE NONCLUSTERED INDEX [IX_Settings_SettingKey] ON [dbo].[Settings]([SettingKey]);
    
    PRINT '✓ تم إنشاء جدول Settings';
END
GO

-- =============================================
-- 25. جدول سجل التدقيق (AuditLogs)
-- =============================================
IF OBJECT_ID(N'[dbo].[AuditLogs]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AuditLogs](
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [UserId] UNIQUEIDENTIFIER NULL,
        [Action] NVARCHAR(50) NOT NULL, -- Create, Update, Delete, Login, Logout
        [TableName] NVARCHAR(100) NOT NULL,
        [RecordId] UNIQUEIDENTIFIER NULL,
        [OldData] NVARCHAR(MAX) NULL, -- JSON
        [NewData] NVARCHAR(MAX) NULL, -- JSON
        [IpAddress] NVARCHAR(50),
        [UserAgent] NVARCHAR(500),
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE()
    );
    
    CREATE NONCLUSTERED INDEX [IX_AuditLogs_UserId] ON [dbo].[AuditLogs]([UserId]);
    CREATE NONCLUSTERED INDEX [IX_AuditLogs_Action] ON [dbo].[AuditLogs]([Action]);
    CREATE NONCLUSTERED INDEX [IX_AuditLogs_TableName] ON [dbo].[AuditLogs]([TableName]);
    CREATE NONCLUSTERED INDEX [IX_AuditLogs_CreatedDate] ON [dbo].[AuditLogs]([CreatedDate]);
    
    PRINT '✓ تم إنشاء جدول AuditLogs';
END
GO

-- =============================================
-- عرض ملخص الجداول المنشرة
-- =============================================
PRINT '';
PRINT '=================================';
PRINT '✓ تم إنشاء جميع الجداول بنجاح!';
PRINT '=================================';
PRINT '';

SELECT 
    t.name AS TableName,
    COUNT(c.column_id) AS ColumnCount
FROM sys.tables t
INNER JOIN sys.columns c ON t.object_id = c.object_id
WHERE t.type = 'U' AND t.is_ms_shipped = 0
GROUP BY t.name
ORDER BY t.name;

GO
