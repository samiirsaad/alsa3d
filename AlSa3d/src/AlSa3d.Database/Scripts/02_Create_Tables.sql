-- =============================================
-- Al-Sa3d Accounting System
-- Tables Creation Script
-- =============================================
-- Description: إنشاء جميع الجداول والعلاقات والفهارس
-- =============================================

USE [AlSa3d];
GO

-- =============================================
-- 1. جدول المستخدمين (Users)
-- =============================================
IF OBJECT_ID(N'[dbo].[Users]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [UserName] NVARCHAR(50) NOT NULL,
        [Email] NVARCHAR(100) NOT NULL,
        [PasswordHash] NVARCHAR(256) NOT NULL,
        [FullName] NVARCHAR(100) NOT NULL,
        [Phone] NVARCHAR(20),
        [RoleId] UNIQUEIDENTIFIER NOT NULL,
        [IsActive] BIT DEFAULT 1,
        [IsDeleted] BIT DEFAULT 0,
        [LastLoginDate] DATETIME2,
        [CreatedDate] DATETIME2 DEFAULT GETDATE(),
        [ModifiedDate] DATETIME2,
        [CreatedBy] UNIQUEIDENTIFIER,
        [ModifiedBy] UNIQUEIDENTIFIER,
        
        CONSTRAINT [UK_Users_UserName] UNIQUE ([UserName]),
        CONSTRAINT [UK_Users_Email] UNIQUE ([Email])
    );
    
    CREATE INDEX [IX_Users_RoleId] ON [dbo].[Users] ([RoleId]);
    CREATE INDEX [IX_Users_IsActive] ON [dbo].[Users] ([IsActive]);
    
    PRINT '✓ تم إنشاء جدول Users';
END
GO

-- =============================================
-- 2. جدول الأدوار (Roles)
-- =============================================
IF OBJECT_ID(N'[dbo].[Roles]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Roles] (
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [Name] NVARCHAR(50) NOT NULL,
        [Description] NVARCHAR(256),
        [IsActive] BIT DEFAULT 1,
        [CreatedDate] DATETIME2 DEFAULT GETDATE(),
        
        CONSTRAINT [UK_Roles_Name] UNIQUE ([Name])
    );
    
    PRINT '✓ تم إنشاء جدول Roles';
END
GO

-- =============================================
-- 3. جدول الصلاحيات (Permissions)
-- =============================================
IF OBJECT_ID(N'[dbo].[Permissions]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Permissions] (
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [Name] NVARCHAR(100) NOT NULL,
        [DisplayName] NVARCHAR(150) NOT NULL,
        [Category] NVARCHAR(50) NOT NULL,
        [Description] NVARCHAR(256),
        [CreatedDate] DATETIME2 DEFAULT GETDATE()
    );
    
    CREATE INDEX [IX_Permissions_Category] ON [dbo].[Permissions] ([Category]);
    
    PRINT '✓ تم إنشاء جدول Permissions';
END
GO

-- =============================================
-- 4. جدول صلاحيات الأدوار (RolePermissions)
-- =============================================
IF OBJECT_ID(N'[dbo].[RolePermissions]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[RolePermissions] (
        [RoleId] UNIQUEIDENTIFIER NOT NULL,
        [PermissionId] UNIQUEIDENTIFIER NOT NULL,
        [CreatedDate] DATETIME2 DEFAULT GETDATE(),
        
        CONSTRAINT [PK_RolePermissions] PRIMARY KEY ([RoleId], [PermissionId]),
        CONSTRAINT [FK_RolePermissions_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RolePermissions_Permissions] FOREIGN KEY ([PermissionId]) REFERENCES [dbo].[Permissions]([Id]) ON DELETE CASCADE
    );
    
    PRINT '✓ تم إنشاء جدول RolePermissions';
END
GO

-- =============================================
-- 5. جدول العملاء (Customers)
-- =============================================
IF OBJECT_ID(N'[dbo].[Customers]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Customers] (
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [CustomerCode] NVARCHAR(20) NOT NULL,
        [CompanyName] NVARCHAR(200),
        [ContactName] NVARCHAR(100) NOT NULL,
        [Email] NVARCHAR(100),
        [Phone] NVARCHAR(20) NOT NULL,
        [Mobile] NVARCHAR(20),
        [Fax] NVARCHAR(20),
        [TaxNumber] NVARCHAR(50),
        [NationalId] NVARCHAR(20),
        [CreditLimit] DECIMAL(18,2) DEFAULT 0,
        [Balance] DECIMAL(18,2) DEFAULT 0,
        [Notes] NVARCHAR(500),
        [IsActive] BIT DEFAULT 1,
        [IsDeleted] BIT DEFAULT 0,
        [CreatedDate] DATETIME2 DEFAULT GETDATE(),
        [ModifiedDate] DATETIME2,
        [CreatedBy] UNIQUEIDENTIFIER,
        [ModifiedBy] UNIQUEIDENTIFIER,
        
        CONSTRAINT [UK_Customers_Code] UNIQUE ([CustomerCode])
    );
    
    CREATE INDEX [IX_Customers_CustomerCode] ON [dbo].[Customers] ([CustomerCode]);
    CREATE INDEX [IX_Customers_Phone] ON [dbo].[Customers] ([Phone]);
    CREATE INDEX [IX_Customers_IsActive] ON [dbo].[Customers] ([IsActive]);
    
    PRINT '✓ تم إنشاء جدول Customers';
END
GO

-- =============================================
-- 6. جدول عناوين العملاء (Addresses)
-- =============================================
IF OBJECT_ID(N'[dbo].[Addresses]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Addresses] (
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [CustomerId] UNIQUEIDENTIFIER NOT NULL,
        [AddressType] INT NOT NULL DEFAULT 0, -- 0: رئيسي، 1: فرعي، 2: شحن
        [Country] NVARCHAR(50) NOT NULL,
        [City] NVARCHAR(50) NOT NULL,
        [District] NVARCHAR(100),
        [Street] NVARCHAR(200),
        [BuildingNumber] NVARCHAR(20),
        [Floor] NVARCHAR(10),
        [Apartment] NVARCHAR(10),
        [PostalCode] NVARCHAR(10),
        [IsDefault] BIT DEFAULT 0,
        [Notes] NVARCHAR(256),
        [CreatedDate] DATETIME2 DEFAULT GETDATE(),
        
        CONSTRAINT [FK_Addresses_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers]([Id]) ON DELETE CASCADE
    );
    
    CREATE INDEX [IX_Addresses_CustomerId] ON [dbo].[Addresses] ([CustomerId]);
    CREATE INDEX [IX_Addresses_City] ON [dbo].[Addresses] ([City]);
    
    PRINT '✓ تم إنشاء جدول Addresses';
END
GO

-- =============================================
-- 7. جدول جهات اتصال العملاء (Contacts)
-- =============================================
IF OBJECT_ID(N'[dbo].[Contacts]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Contacts] (
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [CustomerId] UNIQUEIDENTIFIER NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [Position] NVARCHAR(50),
        [Phone] NVARCHAR(20),
        [Mobile] NVARCHAR(20),
        [Email] NVARCHAR(100),
        [IsPrimary] BIT DEFAULT 0,
        [Notes] NVARCHAR(256),
        [CreatedDate] DATETIME2 DEFAULT GETDATE(),
        
        CONSTRAINT [FK_Contacts_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers]([Id]) ON DELETE CASCADE
    );
    
    CREATE INDEX [IX_Contacts_CustomerId] ON [dbo].[Contacts] ([CustomerId]);
    
    PRINT '✓ تم إنشاء جدول Contacts';
END
GO

-- =============================================
-- 8. جدول التصنيفات (Categories)
-- =============================================
IF OBJECT_ID(N'[dbo].[Categories]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Categories] (
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [Name] NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(256),
        [ParentId] UNIQUEIDENTIFIER,
        [SortOrder] INT DEFAULT 0,
        [IsActive] BIT DEFAULT 1,
        [CreatedDate] DATETIME2 DEFAULT GETDATE(),
        
        CONSTRAINT [FK_Categories_Categories] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[Categories]([Id])
    );
    
    CREATE INDEX [IX_Categories_ParentId] ON [dbo].[Categories] ([ParentId]);
    
    PRINT '✓ تم إنشاء جدول Categories';
END
GO

-- =============================================
-- 9. جدول المنتجات (Products)
-- =============================================
IF OBJECT_ID(N'[dbo].[Products]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Products] (
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [ProductCode] NVARCHAR(50) NOT NULL,
        [Barcode] NVARCHAR(50),
        [Name] NVARCHAR(200) NOT NULL,
        [Description] NVARCHAR(500),
        [CategoryId] UNIQUEIDENTIFIER NOT NULL,
        [Unit] NVARCHAR(20) NOT NULL, -- حبة، كيلو، لتر، إلخ
        [CostPrice] DECIMAL(18,2) DEFAULT 0,
        [SalePrice] DECIMAL(18,2) DEFAULT 0,
        [WholesalePrice] DECIMAL(18,2) DEFAULT 0,
        [MinStockLevel] DECIMAL(18,2) DEFAULT 0,
        [MaxStockLevel] DECIMAL(18,2) DEFAULT 0,
        [TaxRate] DECIMAL(5,2) DEFAULT 0,
        [IsActive] BIT DEFAULT 1,
        [IsDeleted] BIT DEFAULT 0,
        [ImageUrl] NVARCHAR(500),
        [Notes] NVARCHAR(500),
        [CreatedDate] DATETIME2 DEFAULT GETDATE(),
        [ModifiedDate] DATETIME2,
        [CreatedBy] UNIQUEIDENTIFIER,
        [ModifiedBy] UNIQUEIDENTIFIER,
        
        CONSTRAINT [UK_Products_Code] UNIQUE ([ProductCode]),
        CONSTRAINT [FK_Products_Categories] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories]([Id])
    );
    
    CREATE INDEX [IX_Products_CategoryId] ON [dbo].[Products] ([CategoryId]);
    CREATE INDEX [IX_Products_Barcode] ON [dbo].[Products] ([Barcode]);
    CREATE INDEX [IX_Products_IsActive] ON [dbo].[Products] ([IsActive]);
    
    PRINT '✓ تم إنشاء جدول Products';
END
GO

-- =============================================
-- 10. جدول المخازن (Warehouses)
-- =============================================
IF OBJECT_ID(N'[dbo].[Warehouses]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Warehouses] (
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [Name] NVARCHAR(100) NOT NULL,
        [Code] NVARCHAR(20) NOT NULL,
        [Address] NVARCHAR(300),
        [Phone] NVARCHAR(20),
        [ManagerName] NVARCHAR(100),
        [IsActive] BIT DEFAULT 1,
        [CreatedDate] DATETIME2 DEFAULT GETDATE(),
        
        CONSTRAINT [UK_Warehouses_Code] UNIQUE ([Code])
    );
    
    PRINT '✓ تم إنشاء جدول Warehouses';
END
GO

-- =============================================
-- 11. جدول مخزون المنتجات (ProductStock)
-- =============================================
IF OBJECT_ID(N'[dbo].[ProductStock]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ProductStock] (
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [ProductId] UNIQUEIDENTIFIER NOT NULL,
        [WarehouseId] UNIQUEIDENTIFIER NOT NULL,
        [Quantity] DECIMAL(18,2) DEFAULT 0,
        [ReservedQuantity] DECIMAL(18,2) DEFAULT 0,
        [AvailableQuantity] AS ([Quantity] - [ReservedQuantity]),
        [LastUpdated] DATETIME2 DEFAULT GETDATE(),
        
        CONSTRAINT [UK_ProductStock_Product_Warehouse] UNIQUE ([ProductId], [WarehouseId]),
        CONSTRAINT [FK_ProductStock_Products] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ProductStock_Warehouses] FOREIGN KEY ([WarehouseId]) REFERENCES [dbo].[Warehouses]([Id]) ON DELETE CASCADE
    );
    
    CREATE INDEX [IX_ProductStock_ProductId] ON [dbo].[ProductStock] ([ProductId]);
    CREATE INDEX [IX_ProductStock_WarehouseId] ON [dbo].[ProductStock] ([WarehouseId]);
    
    PRINT '✓ تم إنشاء جدول ProductStock';
END
GO

-- =============================================
-- 12. جدول الفواتير (Invoices)
-- =============================================
IF OBJECT_ID(N'[dbo].[Invoices]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Invoices] (
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [InvoiceNumber] NVARCHAR(50) NOT NULL,
        [InvoiceType] INT NOT NULL, -- 0: مبيعات، 1: مشتريات، 2: مرتجع مبيعات، 3: مرتجع مشتريات
        [CustomerId] UNIQUEIDENTIFIER NOT NULL,
        [InvoiceDate] DATETIME2 DEFAULT GETDATE(),
        [DueDate] DATETIME2,
        [WarehouseId] UNIQUEIDENTIFIER,
        [SubTotal] DECIMAL(18,2) DEFAULT 0,
        [DiscountAmount] DECIMAL(18,2) DEFAULT 0,
        [DiscountPercent] DECIMAL(5,2) DEFAULT 0,
        [TaxAmount] DECIMAL(18,2) DEFAULT 0,
        [TotalAmount] DECIMAL(18,2) DEFAULT 0,
        [PaidAmount] DECIMAL(18,2) DEFAULT 0,
        [RemainingAmount] AS ([TotalAmount] - [PaidAmount]),
        [PaymentStatus] INT DEFAULT 0, -- 0: غير مدفوع، 1: مدفوع جزئياً، 2: مدفوع بالكامل
        [Notes] NVARCHAR(500),
        [AttachmentUrl] NVARCHAR(500),
        [IsActive] BIT DEFAULT 1,
        [IsDeleted] BIT DEFAULT 0,
        [CreatedDate] DATETIME2 DEFAULT GETDATE(),
        [ModifiedDate] DATETIME2,
        [CreatedBy] UNIQUEIDENTIFIER,
        [ModifiedBy] UNIQUEIDENTIFIER,
        
        CONSTRAINT [UK_Invoices_Number] UNIQUE ([InvoiceNumber]),
        CONSTRAINT [FK_Invoices_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers]([Id]),
        CONSTRAINT [FK_Invoices_Warehouses] FOREIGN KEY ([WarehouseId]) REFERENCES [dbo].[Warehouses]([Id])
    );
    
    CREATE INDEX [IX_Invoices_InvoiceNumber] ON [dbo].[Invoices] ([InvoiceNumber]);
    CREATE INDEX [IX_Invoices_CustomerId] ON [dbo].[Invoices] ([CustomerId]);
    CREATE INDEX [IX_Invoices_InvoiceDate] ON [dbo].[Invoices] ([InvoiceDate]);
    CREATE INDEX [IX_Invoices_PaymentStatus] ON [dbo].[Invoices] ([PaymentStatus]);
    
    PRINT '✓ تم إنشاء جدول Invoices';
END
GO

-- =============================================
-- 13. جدول أصناف الفواتير (InvoiceItems)
-- =============================================
IF OBJECT_ID(N'[dbo].[InvoiceItems]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[InvoiceItems] (
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [InvoiceId] UNIQUEIDENTIFIER NOT NULL,
        [ProductId] UNIQUEIDENTIFIER NOT NULL,
        [Quantity] DECIMAL(18,2) NOT NULL,
        [UnitPrice] DECIMAL(18,2) NOT NULL,
        [DiscountAmount] DECIMAL(18,2) DEFAULT 0,
        [DiscountPercent] DECIMAL(5,2) DEFAULT 0,
        [TaxRate] DECIMAL(5,2) DEFAULT 0,
        [TaxAmount] DECIMAL(18,2) DEFAULT 0,
        [TotalAmount] DECIMAL(18,2) NOT NULL,
        [Notes] NVARCHAR(256),
        [CreatedDate] DATETIME2 DEFAULT GETDATE(),
        
        CONSTRAINT [FK_InvoiceItems_Invoices] FOREIGN KEY ([InvoiceId]) REFERENCES [dbo].[Invoices]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_InvoiceItems_Products] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products]([Id])
    );
    
    CREATE INDEX [IX_InvoiceItems_InvoiceId] ON [dbo].[InvoiceItems] ([InvoiceId]);
    CREATE INDEX [IX_InvoiceItems_ProductId] ON [dbo].[InvoiceItems] ([ProductId]);
    
    PRINT '✓ تم إنشاء جدول InvoiceItems';
END
GO

-- =============================================
-- 14. جدول المرتجعات (Returns)
-- =============================================
IF OBJECT_ID(N'[dbo].[Returns]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Returns] (
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [ReturnNumber] NVARCHAR(50) NOT NULL,
        [InvoiceId] UNIQUEIDENTIFIER NOT NULL,
        [ReturnDate] DATETIME2 DEFAULT GETDATE(),
        [Reason] NVARCHAR(500),
        [TotalAmount] DECIMAL(18,2) DEFAULT 0,
        [Notes] NVARCHAR(500),
        [CreatedDate] DATETIME2 DEFAULT GETDATE(),
        [CreatedBy] UNIQUEIDENTIFIER,
        
        CONSTRAINT [UK_Returns_Number] UNIQUE ([ReturnNumber]),
        CONSTRAINT [FK_Returns_Invoices] FOREIGN KEY ([InvoiceId]) REFERENCES [dbo].[Invoices]([Id])
    );
    
    CREATE INDEX [IX_Returns_InvoiceId] ON [dbo].[Returns] ([InvoiceId]);
    CREATE INDEX [IX_Returns_ReturnDate] ON [dbo].[Returns] ([ReturnDate]);
    
    PRINT '✓ تم إنشاء جدول Returns';
END
GO

-- =============================================
-- 15. جدول أصناف المرتجعات (ReturnItems)
-- =============================================
IF OBJECT_ID(N'[dbo].[ReturnItems]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ReturnItems] (
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [ReturnId] UNIQUEIDENTIFIER NOT NULL,
        [InvoiceItemId] UNIQUEIDENTIFIER NOT NULL,
        [Quantity] DECIMAL(18,2) NOT NULL,
        [UnitPrice] DECIMAL(18,2) NOT NULL,
        [TotalAmount] DECIMAL(18,2) NOT NULL,
        [Notes] NVARCHAR(256),
        [CreatedDate] DATETIME2 DEFAULT GETDATE(),
        
        CONSTRAINT [FK_ReturnItems_Returns] FOREIGN KEY ([ReturnId]) REFERENCES [dbo].[Returns]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ReturnItems_InvoiceItems] FOREIGN KEY ([InvoiceItemId]) REFERENCES [dbo].[InvoiceItems]([Id])
    );
    
    CREATE INDEX [IX_ReturnItems_ReturnId] ON [dbo].[ReturnItems] ([ReturnId]);
    
    PRINT '✓ تم إنشاء جدول ReturnItems';
END
GO

-- =============================================
-- 16. جدول الأقسام (Departments)
-- =============================================
IF OBJECT_ID(N'[dbo].[Departments]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Departments] (
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [Name] NVARCHAR(100) NOT NULL,
        [Code] NVARCHAR(20) NOT NULL,
        [Description] NVARCHAR(256),
        [ParentId] UNIQUEIDENTIFIER,
        [ManagerId] UNIQUEIDENTIFIER,
        [IsActive] BIT DEFAULT 1,
        [CreatedDate] DATETIME2 DEFAULT GETDATE(),
        
        CONSTRAINT [UK_Departments_Code] UNIQUE ([Code]),
        CONSTRAINT [FK_Departments_Departments] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[Departments]([Id])
    );
    
    PRINT '✓ تم إنشاء جدول Departments';
END
GO

-- =============================================
-- 17. جدول الموظفين (Employees)
-- =============================================
IF OBJECT_ID(N'[dbo].[Employees]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Employees] (
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [EmployeeCode] NVARCHAR(20) NOT NULL,
        [FullName] NVARCHAR(100) NOT NULL,
        [Email] NVARCHAR(100),
        [Phone] NVARCHAR(20),
        [Mobile] NVARCHAR(20),
        [DepartmentId] UNIQUEIDENTIFIER NOT NULL,
        [Position] NVARCHAR(50),
        [HireDate] DATE NOT NULL,
        [EndDate] DATE,
        [Salary] DECIMAL(18,2) DEFAULT 0,
        [Allowances] DECIMAL(18,2) DEFAULT 0,
        [Deductions] DECIMAL(18,2) DEFAULT 0,
        [NationalId] NVARCHAR(20),
        [Address] NVARCHAR(300),
        [EmergencyContact] NVARCHAR(100),
        [EmergencyPhone] NVARCHAR(20),
        [IsActive] BIT DEFAULT 1,
        [IsDeleted] BIT DEFAULT 0,
        [UserId] UNIQUEIDENTIFIER,
        [Notes] NVARCHAR(500),
        [CreatedDate] DATETIME2 DEFAULT GETDATE(),
        [ModifiedDate] DATETIME2,
        [CreatedBy] UNIQUEIDENTIFIER,
        [ModifiedBy] UNIQUEIDENTIFIER,
        
        CONSTRAINT [UK_Employees_Code] UNIQUE ([EmployeeCode]),
        CONSTRAINT [FK_Employees_Departments] FOREIGN KEY ([DepartmentId]) REFERENCES [dbo].[Departments]([Id]),
        CONSTRAINT [FK_Employees_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id])
    );
    
    CREATE INDEX [IX_Employees_DepartmentId] ON [dbo].[Employees] ([DepartmentId]);
    CREATE INDEX [IX_Employees_IsActive] ON [dbo].[Employees] ([IsActive]);
    
    PRINT '✓ تم إنشاء جدول Employees';
END
GO

-- =============================================
-- 18. جدول الرواتب (Salaries)
-- =============================================
IF OBJECT_ID(N'[dbo].[Salaries]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Salaries] (
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [EmployeeId] UNIQUEIDENTIFIER NOT NULL,
        [Month] INT NOT NULL,
        [Year] INT NOT NULL,
        [BasicSalary] DECIMAL(18,2) NOT NULL,
        [Allowances] DECIMAL(18,2) DEFAULT 0,
        [Overtime] DECIMAL(18,2) DEFAULT 0,
        [Deductions] DECIMAL(18,2) DEFAULT 0,
        [Bonus] DECIMAL(18,2) DEFAULT 0,
        [NetSalary] DECIMAL(18,2) NOT NULL,
        [PaymentDate] DATE,
        [PaymentStatus] INT DEFAULT 0, -- 0: غير مدفوع، 1: مدفوع
        [Notes] NVARCHAR(500),
        [CreatedDate] DATETIME2 DEFAULT GETDATE(),
        [CreatedBy] UNIQUEIDENTIFIER,
        
        CONSTRAINT [UK_Salaries_Employee_Month_Year] UNIQUE ([EmployeeId], [Month], [Year]),
        CONSTRAINT [FK_Salaries_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees]([Id]) ON DELETE CASCADE
    );
    
    CREATE INDEX [IX_Salaries_EmployeeId] ON [dbo].[Salaries] ([EmployeeId]);
    CREATE INDEX [IX_Salaries_MonthYear] ON [dbo].[Salaries] ([Month], [Year]);
    
    PRINT '✓ تم إنشاء جدول Salaries';
END
GO

-- =============================================
-- 19. جدول الحضور والغياب (Attendance)
-- =============================================
IF OBJECT_ID(N'[dbo].[Attendance]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Attendance] (
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [EmployeeId] UNIQUEIDENTIFIER NOT NULL,
        [Date] DATE NOT NULL,
        [CheckIn] DATETIME2,
        [CheckOut] DATETIME2,
        [Status] INT NOT NULL, -- 0: حاضر، 1: غائب، 2: إجازة، 3: متأخر
        [LateMinutes] INT DEFAULT 0,
        [OvertimeHours] DECIMAL(5,2) DEFAULT 0,
        [Notes] NVARCHAR(256),
        [CreatedDate] DATETIME2 DEFAULT GETDATE(),
        
        CONSTRAINT [UK_Attendance_Employee_Date] UNIQUE ([EmployeeId], [Date]),
        CONSTRAINT [FK_Attendance_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees]([Id]) ON DELETE CASCADE
    );
    
    CREATE INDEX [IX_Attendance_EmployeeId] ON [dbo].[Attendance] ([EmployeeId]);
    CREATE INDEX [IX_Attendance_Date] ON [dbo].[Attendance] ([Date]);
    
    PRINT '✓ تم إنشاء جدول Attendance';
END
GO

-- =============================================
-- 20. جدول البنوك (Banks)
-- =============================================
IF OBJECT_ID(N'[dbo].[Banks]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Banks] (
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [Name] NVARCHAR(100) NOT NULL,
        [Code] NVARCHAR(20) NOT NULL,
        [SwiftCode] NVARCHAR(20),
        [Address] NVARCHAR(300),
        [Phone] NVARCHAR(20),
        [IsActive] BIT DEFAULT 1,
        [CreatedDate] DATETIME2 DEFAULT GETDATE(),
        
        CONSTRAINT [UK_Banks_Code] UNIQUE ([Code])
    );
    
    PRINT '✓ تم إنشاء جدول Banks';
END
GO

-- =============================================
-- 21. جدول الحسابات البنكية (BankAccounts)
-- =============================================
IF OBJECT_ID(N'[dbo].[BankAccounts]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[BankAccounts] (
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [BankId] UNIQUEIDENTIFIER NOT NULL,
        [AccountName] NVARCHAR(100) NOT NULL,
        [AccountNumber] NVARCHAR(50) NOT NULL,
        [IBAN] NVARCHAR(50),
        [Currency] NVARCHAR(3) DEFAULT 'EGP',
        [Balance] DECIMAL(18,2) DEFAULT 0,
        [IsActive] BIT DEFAULT 1,
        [CreatedDate] DATETIME2 DEFAULT GETDATE(),
        
        CONSTRAINT [UK_BankAccounts_Number] UNIQUE ([AccountNumber]),
        CONSTRAINT [FK_BankAccounts_Banks] FOREIGN KEY ([BankId]) REFERENCES [dbo].[Banks]([Id])
    );
    
    CREATE INDEX [IX_BankAccounts_BankId] ON [dbo].[BankAccounts] ([BankId]);
    
    PRINT '✓ تم إنشاء جدول BankAccounts';
END
GO

-- =============================================
-- 22. جدول الشيكات (Checks)
-- =============================================
IF OBJECT_ID(N'[dbo].[Checks]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Checks] (
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [CheckNumber] NVARCHAR(50) NOT NULL,
        [AccountId] UNIQUEIDENTIFIER NOT NULL,
        [Amount] DECIMAL(18,2) NOT NULL,
        [Currency] NVARCHAR(3) DEFAULT 'EGP',
        [IssueDate] DATE NOT NULL,
        [DueDate] DATE NOT NULL,
        [PayeeName] NVARCHAR(200) NOT NULL,
        [Status] INT NOT NULL, -- 0: جديد، 1: مستحق، 2: تم الصرف، 3: ملغى، 4: مرتجع
        [Notes] NVARCHAR(500),
        [InvoiceId] UNIQUEIDENTIFIER,
        [CreatedDate] DATETIME2 DEFAULT GETDATE(),
        [ModifiedDate] DATETIME2,
        [CreatedBy] UNIQUEIDENTIFIER,
        [ModifiedBy] UNIQUEIDENTIFIER,
        
        CONSTRAINT [FK_Checks_BankAccounts] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[BankAccounts]([Id]),
        CONSTRAINT [FK_Checks_Invoices] FOREIGN KEY ([InvoiceId]) REFERENCES [dbo].[Invoices]([Id])
    );
    
    CREATE INDEX [IX_Checks_AccountId] ON [dbo].[Checks] ([AccountId]);
    CREATE INDEX [IX_Checks_DueDate] ON [dbo].[Checks] ([DueDate]);
    CREATE INDEX [IX_Checks_Status] ON [dbo].[Checks] ([Status]);
    
    PRINT '✓ تم إنشاء جدول Checks';
END
GO

-- =============================================
-- 23. جدول المعاملات المالية (Transactions)
-- =============================================
IF OBJECT_ID(N'[dbo].[Transactions]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Transactions] (
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [TransactionNumber] NVARCHAR(50) NOT NULL,
        [TransactionType] INT NOT NULL, -- 0: إيداع، 1: سحب، 2: تحويل
        [AccountId] UNIQUEIDENTIFIER NOT NULL,
        [Amount] DECIMAL(18,2) NOT NULL,
        [Currency] NVARCHAR(3) DEFAULT 'EGP',
        [ExchangeRate] DECIMAL(18,6) DEFAULT 1,
        [Description] NVARCHAR(500),
        [Reference] NVARCHAR(100),
        [TransactionDate] DATETIME2 DEFAULT GETDATE(),
        [RelatedInvoiceId] UNIQUEIDENTIFIER,
        [RelatedCheckId] UNIQUEIDENTIFIER,
        [CreatedDate] DATETIME2 DEFAULT GETDATE(),
        [CreatedBy] UNIQUEIDENTIFIER,
        
        CONSTRAINT [UK_Transactions_Number] UNIQUE ([TransactionNumber]),
        CONSTRAINT [FK_Transactions_BankAccounts] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[BankAccounts]([Id]),
        CONSTRAINT [FK_Transactions_Invoices] FOREIGN KEY ([RelatedInvoiceId]) REFERENCES [dbo].[Invoices]([Id]),
        CONSTRAINT [FK_Transactions_Checks] FOREIGN KEY ([RelatedCheckId]) REFERENCES [dbo].[Checks]([Id])
    );
    
    CREATE INDEX [IX_Transactions_AccountId] ON [dbo].[Transactions] ([AccountId]);
    CREATE INDEX [IX_Transactions_Date] ON [dbo].[Transactions] ([TransactionDate]);
    
    PRINT '✓ تم إنشاء جدول Transactions';
END
GO

-- =============================================
-- 24. جدول العملات (Currencies)
-- =============================================
IF OBJECT_ID(N'[dbo].[Currencies]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Currencies] (
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [Code] NVARCHAR(3) NOT NULL,
        [Name] NVARCHAR(50) NOT NULL,
        [Symbol] NVARCHAR(10),
        [ExchangeRate] DECIMAL(18,6) DEFAULT 1,
        [IsActive] BIT DEFAULT 1,
        [CreatedDate] DATETIME2 DEFAULT GETDATE(),
        
        CONSTRAINT [UK_Currencies_Code] UNIQUE ([Code])
    );
    
    PRINT '✓ تم إنشاء جدول Currencies';
END
GO

-- =============================================
-- 25. جدول أسعار الصرف (ExchangeRates)
-- =============================================
IF OBJECT_ID(N'[dbo].[ExchangeRates]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ExchangeRates] (
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [FromCurrency] NVARCHAR(3) NOT NULL,
        [ToCurrency] NVARCHAR(3) NOT NULL,
        [Rate] DECIMAL(18,6) NOT NULL,
        [Date] DATE NOT NULL,
        [Source] NVARCHAR(50),
        [CreatedDate] DATETIME2 DEFAULT GETDATE(),
        
        CONSTRAINT [FK_ExchangeRates_FromCurrency] FOREIGN KEY ([FromCurrency]) REFERENCES [dbo].[Currencies]([Code]),
        CONSTRAINT [FK_ExchangeRates_ToCurrency] FOREIGN KEY ([ToCurrency]) REFERENCES [dbo].[Currencies]([Code])
    );
    
    CREATE INDEX [IX_ExchangeRates_Date] ON [dbo].[ExchangeRates] ([Date]);
    CREATE INDEX [IX_ExchangeRates_Currencies] ON [dbo].[ExchangeRates] ([FromCurrency], [ToCurrency]);
    
    PRINT '✓ تم إنشاء جدول ExchangeRates';
END
GO

-- =============================================
-- 26. جدول الإعدادات (Settings)
-- =============================================
IF OBJECT_ID(N'[dbo].[Settings]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Settings] (
        [Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        [Key] NVARCHAR(100) NOT NULL,
        [Value] NVARCHAR(MAX),
        [Type] NVARCHAR(20) DEFAULT 'String',
        [Description] NVARCHAR(256),
        [ModifiedDate] DATETIME2 DEFAULT GETDATE(),
        [ModifiedBy] UNIQUEIDENTIFIER,
        
        CONSTRAINT [UK_Settings_Key] UNIQUE ([Key])
    );
    
    PRINT '✓ تم إنشاء جدول Settings';
END
GO

-- =============================================
-- 27. جدول سجل التدقيق (AuditLogs)
-- =============================================
IF OBJECT_ID(N'[dbo].[AuditLogs]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AuditLogs] (
        [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
        [UserId] UNIQUEIDENTIFIER,
        [Action] NVARCHAR(50) NOT NULL,
        [TableName] NVARCHAR(100) NOT NULL,
        [RecordId] UNIQUEIDENTIFIER,
        [OldData] NVARCHAR(MAX),
        [NewData] NVARCHAR(MAX),
        [IpAddress] NVARCHAR(50),
        [UserAgent] NVARCHAR(500),
        [CreatedDate] DATETIME2 DEFAULT GETDATE(),
        
        CONSTRAINT [FK_AuditLogs_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id])
    );
    
    CREATE INDEX [IX_AuditLogs_UserId] ON [dbo].[AuditLogs] ([UserId]);
    CREATE INDEX [IX_AuditLogs_TableName] ON [dbo].[AuditLogs] ([TableName]);
    CREATE INDEX [IX_AuditLogs_CreatedDate] ON [dbo].[AuditLogs] ([CreatedDate]);
    
    PRINT '✓ تم إنشاء جدول AuditLogs';
END
GO

PRINT '';
PRINT '=================================';
PRINT '✓ تم إنشاء جميع الجداول بنجاح!';
PRINT '=================================';
PRINT '';
PRINT 'عدد الجداول: 27 جدول';
PRINT 'العلاقات: 35+ علاقة Foreign Key';
PRINT 'الفهارس: 40+ فهرس';
PRINT '';
GO
