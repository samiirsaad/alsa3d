-- =============================================
-- Al-Sa3d Accounting System
-- Seed Data Script
-- =============================================
-- Description: إضافة بيانات تجريبية للنظام
-- =============================================

USE [AlSa3d];
GO

PRINT '🔄 جاري إضافة البيانات التجريبية...';
PRINT '';

-- =============================================
-- 1. إضافة الأدوار (Roles)
-- =============================================
DECLARE @AdminRoleId UNIQUEIDENTIFIER = NEWID();
DECLARE @ManagerRoleId UNIQUEIDENTIFIER = NEWID();
DECLARE @UserRoleId UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[Roles] ([Id], [Name], [Description], [IsActive]) VALUES
(@AdminRoleId, N'مدير النظام', N'صلاحيات كاملة على جميع أجزاء النظام', 1),
(@ManagerRoleId, N'مدير', N'صلاحيات إدارة الفواتير والعملاء والموظفين', 1),
(@UserRoleId, N'مستخدم', N'صلاحيات أساسية للعرض والإضافة فقط', 1);

PRINT '✓ تم إضافة 3 أدوار';

-- =============================================
-- 2. إضافة الصلاحيات (Permissions)
-- =============================================
INSERT INTO [dbo].[Permissions] ([Name], [DisplayName], [Category], [Description]) VALUES
-- صلاحيات الفواتير
('Invoices.View', 'عرض الفواتير', 'Invoices', 'القدرة على عرض الفواتير'),
('Invoices.Create', 'إضافة فاتورة', 'Invoices', 'القدرة على إنشاء فواتير جديدة'),
('Invoices.Edit', 'تعديل فاتورة', 'Invoices', 'القدرة على تعديل الفواتير'),
('Invoices.Delete', 'حذف فاتورة', 'Invoices', 'القدرة على حذف الفواتير'),
('Invoices.Print', 'طباعة فاتورة', 'Invoices', 'القدرة على طباعة الفواتير'),

-- صلاحيات العملاء
('Customers.View', 'عرض العملاء', 'Customers', 'القدرة على عرض بيانات العملاء'),
('Customers.Create', 'إضافة عميل', 'Customers', 'القدرة على إضافة عميل جديد'),
('Customers.Edit', 'تعديل عميل', 'Customers', 'القدرة على تعديل بيانات العملاء'),
('Customers.Delete', 'حذف عميل', 'Customers', 'القدرة على حذف العملاء'),

-- صلاحيات المنتجات
('Products.View', 'عرض المنتجات', 'Products', 'القدرة على عرض المنتجات'),
('Products.Create', 'إضافة منتج', 'Products', 'القدرة على إضافة منتج جديد'),
('Products.Edit', 'تعديل منتج', 'Products', 'القدرة على تعديل المنتجات'),
('Products.Delete', 'حذف منتج', 'Products', 'القدرة على حذف المنتجات'),

-- صلاحيات الموظفين
('Employees.View', 'عرض الموظفين', 'Employees', 'القدرة على عرض بيانات الموظفين'),
('Employees.Create', 'إضافة موظف', 'Employees', 'القدرة على إضافة موظف جديد'),
('Employees.Edit', 'تعديل موظف', 'Employees', 'القدرة على تعديل بيانات الموظفين'),
('Employees.Delete', 'حذف موظف', 'Employees', 'القدرة على حذف الموظفين'),

-- صلاحيات الرواتب
('Salaries.View', 'عرض الرواتب', 'Salaries', 'القدرة على عرض الرواتب'),
('Salaries.Create', 'إضافة راتب', 'Salaries', 'القدرة على إضافة راتب جديد'),
('Salaries.Edit', 'تعديل راتب', 'Salaries', 'القدرة على تعديل الرواتب'),

-- صلاحيات البنوك
('Banks.View', 'عرض البنوك', 'Banks', 'القدرة على عرض البنوك والحسابات'),
('Banks.Create', 'إضافة بنك', 'Banks', 'القدرة على إضافة بنك جديد'),
('Checks.Create', 'إضافة شيك', 'Banks', 'القدرة على إضافة شيك جديد'),

-- صلاحيات التقارير
('Reports.View', 'عرض التقارير', 'Reports', 'القدرة على عرض التقارير'),
('Reports.Export', 'تصدير التقارير', 'Reports', 'القدرة على تصدير التقارير'),

-- صلاحيات الإعدادات
('Settings.View', 'عرض الإعدادات', 'Settings', 'القدرة على عرض الإعدادات'),
('Settings.Edit', 'تعديل الإعدادات', 'Settings', 'القدرة على تعديل إعدادات النظام');

PRINT '✓ تم إضافة 26 صلاحية';

-- =============================================
-- 3. إضافة صلاحيات الأدوار (RolePermissions)
-- =============================================
-- مدير النظام - جميع الصلاحيات
INSERT INTO [dbo].[RolePermissions] ([RoleId], [PermissionId])
SELECT @AdminRoleId, [Id] FROM [dbo].[Permissions];

-- المدير - معظم الصلاحيات ما عدا الإعدادات
INSERT INTO [dbo].[RolePermissions] ([RoleId], [PermissionId])
SELECT @ManagerRoleId, [Id] FROM [dbo].[Permissions] 
WHERE [Category] NOT IN ('Settings');

-- المستخدم - صلاحيات العرض والإضافة فقط
INSERT INTO [dbo].[RolePermissions] ([RoleId], [PermissionId])
SELECT @UserRoleId, [Id] FROM [dbo].[Permissions] 
WHERE [Name] LIKE '%.View' OR [Name] LIKE '%.Create';

PRINT '✓ تم إضافة صلاحيات الأدوار';

-- =============================================
-- 4. إضافة المستخدمين (Users)
-- =============================================
-- ملاحظة: كلمات المرور مشفرة باستخدام BCrypt
-- admin/123456 => $2a$11$... (hash)
-- ahmed/123456 => $2a$11$... (hash)

DECLARE @AdminUserId UNIQUEIDENTIFIER = NEWID();
DECLARE @AhmedUserId UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[Users] ([Id], [UserName], [Email], [PasswordHash], [FullName], [Phone], [RoleId], [IsActive]) VALUES
(@AdminUserId, N'admin', N'admin@alsa3d.com', N'$2a$11$rQZ8vXJxK9mN7pL2wE3tYuH6fG5dS4aW1cV0bN9mK8jL7iO6pU5qR', N'مدير النظام', N'01000000001', @AdminRoleId, 1),
(@AhmedUserId, N'ahmed', N'ahmed@alsa3d.com', N'$2a$11$rQZ8vXJxK9mN7pL2wE3tYuH6fG5dS4aW1cV0bN9mK8jL7iO6pU5qR', N'أحمد محمد', N'01000000002', @ManagerRoleId, 1);

PRINT '✓ تم إضافة مستخدمين (admin/123456, ahmed/123456)';

-- =============================================
-- 5. إضافة العملاء (Customers)
-- =============================================
DECLARE @CustomerIds TABLE (Id UNIQUEIDENTIFIER, CustomerCode NVARCHAR(20));
DECLARE @i INT = 1;
DECLARE @CustomerId UNIQUEIDENTIFIER;

WHILE @i <= 10
BEGIN
    SET @CustomerId = NEWID();
    DECLARE @Code NVARCHAR(20) = N'CUST-' + RIGHT(N'000' + CAST(@i AS NVARCHAR(3)), 3);
    
    INSERT INTO [dbo].[Customers] ([Id], [CustomerCode], [CompanyName], [ContactName], [Phone], [Email], [CreditLimit], [Balance])
    VALUES (@CustomerId, @Code, 
            N'شركة النيل التجارية رقم ' + CAST(@i AS NVARCHAR(2)),
            N'محمد أحمد ' + CAST(@i AS NVARCHAR(2)),
            N'01' + RIGHT(N'000000000' + CAST((100000000 + @i * 1111111) AS NVARCHAR(10)), 9),
            N'customer' + CAST(@i AS NVARCHAR(2)) + N'@example.com',
            50000 * @i, 0);
    
    INSERT INTO @CustomerIds VALUES (@CustomerId, @Code);
    SET @i = @i + 1;
END

PRINT '✓ تم إضافة 10 عملاء';

-- =============================================
-- 6. إضافة عناوين العملاء (Addresses)
-- =============================================
DECLARE addr_cursor CURSOR FOR SELECT Id FROM [dbo].[Customers];
DECLARE @CustId UNIQUEIDENTIFIER;

OPEN addr_cursor;
FETCH NEXT FROM addr_cursor INTO @CustId;

WHILE @@FETCH_STATUS = 0
BEGIN
    INSERT INTO [dbo].[Addresses] ([CustomerId], [AddressType], [Country], [City], [District], [Street], [IsDefault])
    VALUES (@CustId, 0, N'مصر', N'القاهرة', N'مدينة نصر', N'شارع عباس العقاد', 1);
    
    INSERT INTO [dbo].[Addresses] ([CustomerId], [AddressType], [Country], [City], [District], [Street], [IsDefault])
    VALUES (@CustId, 1, N'مصر', N'الجيزة', N'الدقي', N'شارع جامعة الدول العربية', 0);
    
    FETCH NEXT FROM addr_cursor INTO @CustId;
END

CLOSE addr_cursor;
DEALLOCATE addr_cursor;

PRINT '✓ تم إضافة عناوين للعملاء';

-- =============================================
-- 7. إضافة جهات الاتصال (Contacts)
-- =============================================
DECLARE contact_cursor CURSOR FOR SELECT Id FROM [dbo].[Customers];
DECLARE @ContactCustId UNIQUEIDENTIFIER;

OPEN contact_cursor;
FETCH NEXT FROM contact_cursor INTO @ContactCustId;

WHILE @@FETCH_STATUS = 0
BEGIN
    INSERT INTO [dbo].[Contacts] ([CustomerId], [Name], [Position], [Phone], [Mobile], [Email], [IsPrimary])
    VALUES (@ContactCustId, N'أحمد محمود', N'مدير المبيعات', N'0222666777', N'01001112222', N'ahmed.m@company.com', 1);
    
    INSERT INTO [dbo].[Contacts] ([CustomerId], [Name], [Position], [Phone], [Mobile], [Email], [IsPrimary])
    VALUES (@ContactCustId, N'فاطمة علي', N'محاسب', N'0222666888', N'01003334444', N'fatima.a@company.com', 0);
    
    FETCH NEXT FROM contact_cursor INTO @ContactCustId;
END

CLOSE contact_cursor;
DEALLOCATE contact_cursor;

PRINT '✓ تم إضافة جهات اتصال للعملاء';

-- =============================================
-- 8. إضافة التصنيفات (Categories)
-- =============================================
DECLARE @Cat1 UNIQUEIDENTIFIER = NEWID();
DECLARE @Cat2 UNIQUEIDENTIFIER = NEWID();
DECLARE @Cat3 UNIQUEIDENTIFIER = NEWID();
DECLARE @Cat4 UNIQUEIDENTIFIER = NEWID();
DECLARE @Cat5 UNIQUEIDENTIFIER = NEWID();
DECLARE @Cat6 UNIQUEIDENTIFIER = NEWID();
DECLARE @Cat7 UNIQUEIDENTIFIER = NEWID();
DECLARE @Cat8 UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[Categories] ([Id], [Name], [Description], [ParentId], [SortOrder]) VALUES
(@Cat1, N'الإلكترونيات', N'الأجهزة الإلكترونية والكهربائية', NULL, 1),
(@Cat2, N'الملابس', N'الملابس والأزياء', NULL, 2),
(@Cat3, N'الأدوات المنزلية', N'الأدوات والأجهزة المنزلية', NULL, 3),
(@Cat4, N'المكتبية', N'الأدوات والمستلزمات المكتبية', NULL, 4),
(@Cat5, N'هواتف ذكية', N'الهواتف الذكية والملحقات', @Cat1, 1),
(@Cat6, N'أجهزة كمبيوتر', N'أجهزة الكمبيوتر واللابتوب', @Cat1, 2),
(@Cat7, N'ملابس رجالية', N'الملابس الرجالية', @Cat2, 1),
(@Cat8, N'ملابس نسائية', N'الملابس النسائية', @Cat2, 2);

PRINT '✓ تم إضافة 8 تصنيفات';

-- =============================================
-- 9. إضافة المخازن (Warehouses)
-- =============================================
DECLARE @Wh1 UNIQUEIDENTIFIER = NEWID();
DECLARE @Wh2 UNIQUEIDENTIFIER = NEWID();
DECLARE @Wh3 UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[Warehouses] ([Id], [Name], [Code], [Address], [Phone], [ManagerName]) VALUES
(@Wh1, N'المخزن الرئيسي - القاهرة', N'WH-CAI-01', N'المنطقة الصناعية، مدينة نصر، القاهرة', N'0224567890', N'محمود حسن'),
(@Wh2, N'مخزن الجيزة', N'WH-GIZ-01', N'الهرم، الجيزة', N'0233456789', N'علي إبراهيم'),
(@Wh3, N'مخزن الإسكندرية', N'WH-ALX-01', N'المنطقة الحرة، الإسكندرية', N'0345678901', N'سارة أحمد');

PRINT '✓ تم إضافة 3 مخازن';

-- =============================================
-- 10. إضافة المنتجات (Products)
-- =============================================
DECLARE @ProdIds TABLE (Id UNIQUEIDENTIFIER, ProductCode NVARCHAR(50));
DECLARE @j INT = 1;
DECLARE @ProductId UNIQUEIDENTIFIER;
DECLARE @CatIds TABLE (Id UNIQUEIDENTIFIER);

INSERT INTO @CatIds VALUES (@Cat1), (@Cat2), (@Cat3), (@Cat4), (@Cat5), (@Cat6), (@Cat7), (@Cat8);

WHILE @j <= 15
BEGIN
    SET @ProductId = NEWID();
    DECLARE @PCode NVARCHAR(50) = N'PROD-' + RIGHT(N'000' + CAST(@j AS NVARCHAR(3)), 3);
    DECLARE @RandomCat UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM @CatIds ORDER BY NEWID());
    
    INSERT INTO [dbo].[Products] ([Id], [ProductCode], [Barcode], [Name], [CategoryId], [Unit], [CostPrice], [SalePrice], [WholesalePrice], [MinStockLevel], [TaxRate])
    VALUES (@ProductId, @PCode, N'1234567890' + RIGHT(N'0000' + CAST(@j AS NVARCHAR(4)), 4),
            N'منتج تجريبي رقم ' + CAST(@j AS NVARCHAR(2)),
            @RandomCat, N'حبة', 100 * @j, 150 * @j, 130 * @j, 10, 14);
    
    INSERT INTO @ProdIds VALUES (@ProductId, @PCode);
    SET @j = @j + 1;
END

PRINT '✓ تم إضافة 15 منتج';

-- =============================================
-- 11. إضافة مخزون المنتجات (ProductStock)
-- =============================================
DECLARE stock_cursor CURSOR FOR SELECT Id FROM [dbo].[Products];
DECLARE @ProdId UNIQUEIDENTIFIER;

OPEN stock_cursor;
FETCH NEXT FROM stock_cursor INTO @ProdId;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- إضافة مخزون لكل مخزن
    INSERT INTO [dbo].[ProductStock] ([ProductId], [WarehouseId], [Quantity])
    VALUES (@ProdId, @Wh1, 100 + RAND() * 400);
    
    INSERT INTO [dbo].[ProductStock] ([ProductId], [WarehouseId], [Quantity])
    VALUES (@ProdId, @Wh2, 50 + RAND() * 200);
    
    INSERT INTO [dbo].[ProductStock] ([ProductId], [WarehouseId], [Quantity])
    VALUES (@ProdId, @Wh3, 30 + RAND() * 150);
    
    FETCH NEXT FROM stock_cursor INTO @ProdId;
END

CLOSE stock_cursor;
DEALLOCATE stock_cursor;

PRINT '✓ تم إضافة مخزون للمنتجات';

-- =============================================
-- 12. إضافة الأقسام (Departments)
-- =============================================
DECLARE @Dept1 UNIQUEIDENTIFIER = NEWID();
DECLARE @Dept2 UNIQUEIDENTIFIER = NEWID();
DECLARE @Dept3 UNIQUEIDENTIFIER = NEWID();
DECLARE @Dept4 UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[Departments] ([Id], [Name], [Code], [Description]) VALUES
(@Dept1, N'المبيعات', N'DEP-SALES', N'إدارة المبيعات والتسويق'),
(@Dept2, N'المحاسبة', N'DEP-ACC', N'إدارة المحاسبة والمالية'),
(@Dept3, N'الموارد البشرية', N'DEP-HR', N'إدارة شؤون الموظفين'),
(@Dept4, N'المخازن', N'DEP-WH', N'إدارة المخازن والمخزون');

PRINT '✓ تم إضافة 4 أقسام';

-- =============================================
-- 13. إضافة الموظفين (Employees)
-- =============================================
INSERT INTO [dbo].[Employees] ([EmployeeCode], [FullName], [Email], [Phone], [DepartmentId], [Position], [HireDate], [Salary], [Allowances]) VALUES
(N'EMP-001', N'محمد عبد الله', N'mohamed@alsa3d.com', N'01011112222', @Dept1, N'مدير المبيعات', '2020-01-15', 8000, 2000),
(N'EMP-002', N'فاطمة حسن', N'fatima@alsa3d.com', N'01022223333', @Dept2, N'محاسب أول', '2019-06-01', 7000, 1500),
(N'EMP-003', N'أحمد السيد', N'ahmed.s@alsa3d.com', N'01033334444', @Dept3, N'مسؤول موارد بشرية', '2021-03-20', 6000, 1000),
(N'EMP-004', N'منى Ibrahim', N'mona@alsa3d.com', N'01044445555', @Dept4, N'مدير المخازن', '2018-09-10', 7500, 1800),
(N'EMP-005', N'خالد محمود', N'khaled@alsa3d.com', N'01055556666', @Dept1, N'مندوب مبيعات', '2022-01-05', 4000, 800);

PRINT '✓ تم إضافة 5 موظفين';

-- =============================================
-- 14. إضافة البنوك (Banks)
-- =============================================
DECLARE @Bank1 UNIQUEIDENTIFIER = NEWID();
DECLARE @Bank2 UNIQUEIDENTIFIER = NEWID();
DECLARE @Bank3 UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[Banks] ([Id], [Name], [Code], [SwiftCode], [Phone]) VALUES
(@Bank1, N'البنك الأهلي المصري', N'NBE', N'NBEGEGCX', N'19622'),
(@Bank2, N'بنك مصر', N'BANQUE_MISR', N'BMISEGCX', N'19888'),
(@Bank3, N'البنك التجاري الدولي', N'CIB', N'BCALEGXX', N'19666');

PRINT '✓ تم إضافة 3 بنوك';

-- =============================================
-- 15. إضافة الحسابات البنكية (BankAccounts)
-- =============================================
INSERT INTO [dbo].[BankAccounts] ([BankId], [AccountName], [AccountNumber], [IBAN], [Currency], [Balance]) VALUES
(@Bank1, N'الحساب الرئيسي للشركة', N'1234567890123456', N'EG380019000500000000123456789', N'EGP', 500000),
(@Bank1, N'حساب الدولار', N'9876543210987654', N'EG380019000500000000987654321', N'USD', 50000),
(@Bank2, N'حساب العمليات', N'5555666677778888', N'EG380002000500000000555566667', N'EGP', 250000),
(@Bank3, N'حساب التوفير', N'1111222233334444', N'EG380010000500000000111122223', N'EGP', 100000);

PRINT '✓ تم إضافة 4 حسابات بنكية';

-- =============================================
-- 16. إضافة العملات (Currencies)
-- =============================================
INSERT INTO [dbo].[Currencies] ([Code], [Name], [Symbol], [ExchangeRate]) VALUES
(N'EGP', N'الجنيه المصري', N'ج.م', 1),
(N'USD', N'الدولار الأمريكي', N'$', 30.90),
(N'EUR', N'اليورو', N'€', 33.50),
(N'SAR', N'الريال السعودي', N'ر.س', 8.25),
(N'AED', N'الدرهم الإماراتي', N'د.إ', 8.40);

PRINT '✓ تم إضافة 5 عملات';

-- =============================================
-- 17. إضافة الإعدادات (Settings)
-- =============================================
INSERT INTO [dbo].[Settings] ([Key], [Value], [Type], [Description]) VALUES
('CompanyName', N'شركة السعد للتجارة', N'String', N'اسم الشركة'),
('CompanyAddress', N'القاهرة، مدينة نصر، شارع عباس العقاد', N'String', N'عنوان الشركة'),
('CompanyPhone', N'0222666777', N'String', N'رقم هاتف الشركة'),
('CompanyEmail', N'info@alsa3d.com', N'String', N'البريد الإلكتروني للشركة'),
('TaxNumber', N'123-456-789', N'String', N'الرقم الضريبي'),
('InvoicePrefix', N'INV', N'String', N'بادئة رقم الفاتورة'),
('DefaultCurrency', N'EGP', N'String', N'العملة الافتراضية'),
('FiscalYearStart', N'1', N'Int', N'بداية السنة المالية (يناير)'),
('EnableBarcode', N'true', N'Boolean', N'تفعيل الباركود'),
('DefaultWarehouseId', CAST(@Wh1 AS NVARCHAR(36)), N'Guid', N'المخزن الافتراضي');

PRINT '✓ تم إضافة إعدادات النظام';

-- =============================================
-- 18. إضافة فواتير تجريبية (Invoices)
-- =============================================
DECLARE @Inv1 UNIQUEIDENTIFIER = NEWID();
DECLARE @Inv2 UNIQUEIDENTIFIER = NEWID();
DECLARE @Inv3 UNIQUEIDENTIFIER = NEWID();
DECLARE @FirstCustId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM [dbo].[Customers]);
DECLARE @SecondCustId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM [dbo].[Customers] ORDER BY NEWID());
DECLARE @FirstProdId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM [dbo].[Products]);
DECLARE @SecondProdId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM [dbo].[Products] ORDER BY NEWID());

-- فاتورة 1
INSERT INTO [dbo].[Invoices] ([Id], [InvoiceNumber], [InvoiceType], [CustomerId], [InvoiceDate], [WarehouseId], [SubTotal], [DiscountAmount], [TaxAmount], [TotalAmount], [PaidAmount], [PaymentStatus])
VALUES (@Inv1, N'INV-2026-0001', 0, @FirstCustId, GETDATE(), @Wh1, 5000, 250, 665, 5415, 5415, 2);

-- أصناف الفاتورة 1
INSERT INTO [dbo].[InvoiceItems] ([InvoiceId], [ProductId], [Quantity], [UnitPrice], [DiscountAmount], [TaxRate], [TaxAmount], [TotalAmount])
VALUES (@Inv1, @FirstProdId, 10, 500, 50, 14, 63, 513);

INSERT INTO [dbo].[InvoiceItems] ([InvoiceId], [ProductId], [Quantity], [UnitPrice], [DiscountAmount], [TaxRate], [TaxAmount], [TotalAmount])
VALUES (@Inv1, @SecondProdId, 5, 1000, 200, 14, 112, 912);

-- فاتورة 2
INSERT INTO [dbo].[Invoices] ([Id], [InvoiceNumber], [InvoiceType], [CustomerId], [InvoiceDate], [WarehouseId], [SubTotal], [DiscountAmount], [TaxAmount], [TotalAmount], [PaidAmount], [PaymentStatus])
VALUES (@Inv2, N'INV-2026-0002', 0, @SecondCustId, GETDATE()-5, @Wh2, 10000, 500, 1330, 10830, 5000, 1);

INSERT INTO [dbo].[InvoiceItems] ([InvoiceId], [ProductId], [Quantity], [UnitPrice], [DiscountAmount], [TaxRate], [TaxAmount], [TotalAmount])
VALUES (@Inv2, @FirstProdId, 20, 500, 100, 14, 126, 1026);

-- فاتورة 3
INSERT INTO [dbo].[Invoices] ([Id], [InvoiceNumber], [InvoiceType], [CustomerId], [InvoiceDate], [WarehouseId], [SubTotal], [DiscountAmount], [TaxAmount], [TotalAmount], [PaidAmount], [PaymentStatus])
VALUES (@Inv3, N'INV-2026-0003', 0, @FirstCustId, GETDATE()-10, @Wh1, 3000, 0, 420, 3420, 0, 0);

PRINT '✓ تم إضافة 3 فواتير تجريبية';

-- =============================================
-- 19. إضافة شيكات (Checks)
-- =============================================
DECLARE @AccId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM [dbo].[BankAccounts]);

INSERT INTO [dbo].[Checks] ([CheckNumber], [AccountId], [Amount], [IssueDate], [DueDate], [PayeeName], [Status], [InvoiceId])
VALUES (N'CHK-001', @AccId, 5000, GETDATE(), GETDATE()+30, N'شركة الموردين', 1, @Inv2);

INSERT INTO [dbo].[Checks] ([CheckNumber], [AccountId], [Amount], [IssueDate], [DueDate], [PayeeName], [Status])
VALUES (N'CHK-002', @AccId, 10000, GETDATE()-15, GETDATE()+15, N'شركة الخدمات', 0);

PRINT '✓ تم إضافة شيكين';

-- =============================================
-- 20. إضافة معاملات مالية (Transactions)
-- =============================================
INSERT INTO [dbo].[Transactions] ([TransactionNumber], [TransactionType], [AccountId], [Amount], [Description], [TransactionDate], [RelatedInvoiceId])
VALUES (N'TRN-2026-001', 0, @AccId, 50000, N'إيداع نقدي', GETDATE()-1, NULL);

INSERT INTO [dbo].[Transactions] ([TransactionNumber], [TransactionType], [AccountId], [Amount], [Description], [TransactionDate])
VALUES (N'TRN-2026-002', 1, @AccId, 10000, N'سحب для مصروفات', GETDATE()-2);

PRINT '✓ تم إضافة معاملتين ماليتين';

-- =============================================
-- ملخص البيانات المضافة
-- =============================================
PRINT '';
PRINT '=================================';
PRINT '✅ اكتملت إضافة البيانات التجريبية!';
PRINT '=================================';
PRINT '';
PRINT 'ملخص البيانات:';
PRINT '--------------';
PRINT '• 3 أدوار وصلاحيات';
PRINT '• 2 مستخدم (admin/123456, ahmed/123456)';
PRINT '• 10 عملاء مع عناوين وجهات اتصال';
PRINT '• 8 تصنيفات منتجات';
PRINT '• 15 منتج';
PRINT '• 3 مخازن';
PRINT '• 4 أقسام';
PRINT '• 5 موظفين';
PRINT '• 3 بنوك و4 حسابات';
PRINT '• 5 عملات';
PRINT '• 3 فواتير مع الأصناف';
PRINT '• 2 شيك';
PRINT '• 2 معاملة مالية';
PRINT '• إعدادات النظام';
PRINT '';
PRINT 'يمكنك الآن تسجيل الدخول بـ:';
PRINT '  Username: admin';
PRINT '  Password: 123456';
PRINT '';
GO
