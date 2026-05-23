-- ============================================
-- AlSa3d Database - Seed Data
-- البيانات التجريبية الأولية
-- ============================================

USE AlSa3d;
GO

-- ============================================
-- 1. الأدوار والصلاحيات (Roles & Permissions)
-- ============================================

INSERT INTO Roles (Name, Description, CreatedAt) VALUES
(N'Admin', N'مدير النظام - صلاحيات كاملة', GETDATE()),
(N'Manager', N'مدير - صلاحيات إدارة الفواتير والعملاء', GETDATE()),
(N'User', N'مستخدم عادي - صلاحيات محدودة', GETDATE()),
(N'Accountant', N'محاسب - صلاحيات مالية وتقارير', GETDATE());

-- ============================================
-- 2. المستخدمين (Users)
-- ============================================

-- كلمة المرور الافتراضية: 123456 (مشفرة)
INSERT INTO Users (Username, PasswordHash, Email, FullName, RoleId, IsActive, CreatedAt) VALUES
(N'admin', N'$2a$11$rQZ8vXJxK9zL5mN3pO7qWeRtYuIoPlKjHgFdSaZxCvBnMqWrTyUiO', N'admin@alsa3d.com', N'مدير النظام', 1, 1, GETDATE()),
(N'ahmed', N'$2a$11$rQZ8vXJxK9zL5mN3pO7qWeRtYuIoPlKjHgFdSaZxCvBnMqWrTyUiO', N'ahmed@company.com', N'أحمد محمد', 2, 1, GETDATE()),
(N'mohamed', N'$2a$11$rQZ8vXJxK9zL5mN3pO7qWeRtYuIoPlKjHgFdSaZxCvBnMqWrTyUiO', N'mohamed@company.com', N'محمد علي', 3, 1, GETDATE()),
(N'sara', N'$2a$11$rQZ8vXJxK9zL5mN3pO7qWeRtYuIoPlKjHgFdSaZxCvBnMqWrTyUiO', N'sara@company.com', N'سارة أحمد', 4, 1, GETDATE());

-- ============================================
-- 3. العملاء (Customers)
-- ============================================

INSERT INTO Customers (Name, Email, Phone, Mobile, Address, City, Country, TaxNumber, CreatedAt) VALUES
(N'شركة الأمل التجارية', N'info@alamal.com', N'02-23456789', N'010-12345678', N'15 شارع التحرير', N'القاهرة', N'مصر', N'123-456-789', GETDATE()),
(N'مؤسسة النور', N'contact@alnoor.com', N'02-34567890', N'011-23456789', N'25 شارع الزمالك', N'الجيزة', N'مصر', N'234-567-890', GETDATE()),
(N'شركة المستقبل', N'info@mostaqbal.com', N'03-45678901', N'012-34567890', N'10 شارع الكورنيش', N'الإسكندرية', N'مصر', N'345-678-901', GETDATE()),
(N'محل السلام', N'salam@store.com', N'04-56789012', N'015-45678901', N'5 شارع الجمهورية', N'المنصورة', N'مصر', N'456-789-012', GETDATE()),
(N'شركة البركة', N'baraka@company.com', N'05-67890123', N'016-56789012', N'20 شارع سعد', N'المحلة', N'مصر', N'567-890-123', GETDATE()),
(N'مؤسسة الإيمان', N'iman@org.com', N'06-78901234', N'017-67890123', N'30 شارع النصر', N'أسوان', N'مصر', N'678-901-234', GETDATE()),
(N'شركة التوحيد', N'tawheed@co.com', N'07-89012345', N'018-78901234', N'12 شارع السلام', N'أسيوط', N'مصر', N'789-012-345', GETDATE()),
(N'مجموعة الرائد', N'raed@group.com', N'08-90123456', N'019-89012345', N'8 شارع الجامعة', N'الزقازيق', N'مصر', N'890-123-456', GETDATE()),
(N'شركة الإنجاز', N'enjaz@corp.com', N'09-01234567', N'010-90123456', N'18 شارع بورسعيد', N'بورسعيد', N'مصر', N'901-234-567', GETDATE()),
(N'مؤسسة الخير', N'khair@foundation.org', N'10-12345678', N'011-01234567', N'22 شارع الإسماعيلية', N'الإسماعيلية', N'مصر', N'012-345-678', GETDATE());

-- عناوين إضافية للعملاء
INSERT INTO Addresses (CustomerId, AddressType, Street, City, Governorate, PostalCode, Country, IsDefault) VALUES
(1, N'billing', N'15 شارع التحرير', N'القاهرة', N'القاهرة', N'11511', N'مصر', 1),
(1, N'shipping', N'20 شارع الدقي', N'الجيزة', N'الجيزة', N'12611', N'مصر', 0),
(2, N'billing', N'25 شارع الزمالك', N'الجيزة', N'الجيزة', N'12111', N'مصر', 1),
(3, N'billing', N'10 شارع الكورنيش', N'الإسكندرية', N'الإسكندرية', N'21511', N'مصر', 1);

-- جهات اتصال للعملاء
INSERT INTO Contacts (CustomerId, ContactName, ContactTitle, Phone, Email, IsPrimary) VALUES
(1, N'أحمد محمود', N'مدير المشتريات', N'010-11111111', N'ahmed.m@alamal.com', 1),
(1, N'فاطمة علي', N'محاسب', N'010-22222222', N'fatima@alamal.com', 0),
(2, N'محمد حسن', N'المدير العام', N'011-33333333', N'm.hassan@alnoor.com', 1),
(3, N'سارة إبراهيم', N'مسؤول المبيعات', N'012-44444444', N'sara@mostaqbal.com', 1);

-- ============================================
-- 4. التصنيفات (Categories)
-- ============================================

INSERT INTO Categories (Name, Description, ParentCategoryId, IsActive) VALUES
(N'إلكترونيات', N'الأجهزة الإلكترونية والكهربائية', NULL, 1),
(N'ملابس', N'الملابس والأزياء', NULL, 1),
(N'أدوات مكتبية', N'الأدوات والمستلزمات المكتبية', NULL, 1),
(N'مواد غذائية', N'المواد الغذائية والمشروبات', NULL, 1),
(N'أثاث', N'الأثاث المنزلي والمكتبي', NULL, 1),
(N'هواتف', N'الهواتف المحمولة والإكسسوارات', 1, 1),
(N'كمبيوتر', N'أجهزة الكمبيوتر واللابتوب', 1, 1),
(N'رجالي', N'ملابس رجالية', 2, 1),
(N'نسائي', N'ملابس نسائية', 2, 1),
(N'قرطاسية', N'أدوات الكتابة والقرطاسية', 3, 1);

-- ============================================
-- 5. المخازن (Warehouses)
-- ============================================

INSERT INTO Warehouses (Name, Code, Address, City, Phone, ManagerName, Capacity, CurrentStock, IsActive) VALUES
(N'المخزن الرئيسي', N'WH-001', N'المنطقة الصناعية', N'القاهرة', N'02-45678901', N' Mahmoud Ali', 10000, 7500, 1),
(N'مخزن الجيزة', N'WH-002', N'الدقي', N'الجيزة', N'02-56789012', N'Fatma Hassan', 5000, 3200, 1),
(N'مخزن الإسكندرية', N'WH-003', N'العامرية', N'الإسكندرية', N'03-67890123', N'Ahmed Samir', 3000, 1800, 1);

-- ============================================
-- 6. المنتجات (Products)
-- ============================================

INSERT INTO Products (Name, Code, Barcode, Description, CategoryId, UnitPrice, CostPrice, UnitOfMeasure, MinStockLevel, MaxStockLevel, CurrentStock, IsActive, TaxRate) VALUES
(N'لابتوب Dell Inspiron 15', N'LAP-001', N'1234567890123', N'لابتوب ديل بمواصفات متوسطة', 7, 12000.00, 9500.00, N'قطعة', 5, 50, 25, 1, 14),
(N'آيفون 14 برو', N'IPH-001', N'1234567890124', N'هاتف آيفون أحدث موديل', 6, 45000.00, 38000.00, N'قطعة', 10, 100, 45, 1, 14),
(N'قميص رجالي كلاسيك', N'SHR-001', N'1234567890125', N'قميص رجالي قطن 100%', 8, 350.00, 200.00, N'قطعة', 20, 200, 150, 1, 0),
(N'فستان نسائي أنيق', N'DRS-001', N'1234567890126', N'فستان نسائي مناسب للمناسبات', 9, 850.00, 500.00, N'قطعة', 15, 150, 80, 1, 0),
(N'قلم حبر جاف', N'PEN-001', N'1234567890127', N'قلم حبر جاف أزرق', 10, 5.00, 2.50, N'قطعة', 100, 1000, 500, 1, 0),
(N'ورقة A4', N'PPR-001', N'1234567890128', N'ورقة طباعة A4 - 500 ورقة', 10, 85.00, 60.00, N'رزمة', 50, 500, 300, 1, 0),
(N'مكتب خشبي', N'DSK-001', N'1234567890129', N'مكتب خشبي للمكتب المنزلي', 5, 2500.00, 1800.00, N'قطعة', 5, 30, 12, 1, 14),
(N'كرسي مكتبي', N'CHR-001', N'1234567890130', N'كرسي مكتبي مريح', 5, 1200.00, 850.00, N'قطعة', 10, 50, 28, 1, 14),
(N'أرز مصري', N'RICE-001', N'1234567890131', N'أرز مصري فاخر 5 كجم', 4, 95.00, 70.00, N'كجم', 100, 1000, 600, 1, 0),
(N'سكر أبيض', N'SUGAR-001', N'1234567890132', N'سكر أبيض نقي 1 كجم', 4, 18.00, 12.00, N'كجم', 200, 2000, 1200, 1, 0),
(N'شاشة Samsung 55 بوصة', N'TV-001', N'1234567890133', N'شاشة ذكية UHD', 1, 15000.00, 12000.00, N'قطعة', 3, 20, 8, 1, 14),
(N'طابعة HP LaserJet', N'PRT-001', N'1234567890134', N'طابعة ليزر للأعمال', 3, 4500.00, 3500.00, N'قطعة', 5, 30, 15, 1, 14);

-- مخزون المنتجات في المخازن
INSERT INTO ProductWarehouse (ProductId, WarehouseId, Quantity, ReorderLevel, LastStockCheck) VALUES
(1, 1, 15, 5, GETDATE()),
(1, 2, 10, 5, GETDATE()),
(2, 1, 30, 10, GETDATE()),
(2, 3, 15, 10, GETDATE()),
(3, 1, 100, 20, GETDATE()),
(3, 2, 50, 20, GETDATE()),
(4, 1, 60, 15, GETDATE()),
(5, 1, 300, 100, GETDATE()),
(5, 2, 200, 100, GETDATE()),
(6, 1, 200, 50, GETDATE()),
(7, 1, 8, 5, GETDATE()),
(8, 1, 20, 10, GETDATE()),
(9, 1, 400, 100, GETDATE()),
(9, 2, 200, 100, GETDATE()),
(10, 1, 800, 200, GETDATE()),
(11, 1, 5, 3, GETDATE()),
(12, 1, 10, 5, GETDATE());

-- ============================================
-- 7. الموظفين والأقسام (Employees & Departments)
-- ============================================

INSERT INTO Departments (Name, Code, Description, ManagerId, Budget, IsActive) VALUES
(N'الإدارة', N'DEP-001', N'الإدارة العليا', NULL, 500000.00, 1),
(N'المبيعات', N'DEP-002', N'إدارة المبيعات والتسويق', NULL, 300000.00, 1),
(N'المحاسبة', N'DEP-003', N'الإدارة المالية والمحاسبة', NULL, 200000.00, 1),
(N'المخازن', N'DEP-004', N'إدارة المخازن واللوجستيات', NULL, 150000.00, 1),
(N'الموارد البشرية', N'DEP-005', N'إدارة شؤون الموظفين', NULL, 100000.00, 1),
(N'تكنولوجيا المعلومات', N'DEP-006', N'إدارة الأنظمة والتقنية', NULL, 250000.00, 1);

INSERT INTO Employees (FirstName, LastName, Email, Phone, Mobile, HireDate, DepartmentId, JobTitle, Salary, IsActive) VALUES
(N'أحمد', N'محمد علي', N'ahmed.ali@company.com', N'02-23456789', N'010-12345678', '2020-01-15', 1, N'مدير عام', 25000.00, 1),
(N'فاطمة', N'حسن Ibrahim', N'fatima.h@company.com', N'02-34567890', N'011-23456789', '2019-06-01', 3, N'محاسب أول', 12000.00, 1),
(N'محمد', N'عبدالله', N'mohamed.a@company.com', N'02-45678901', N'012-34567890', '2021-03-10', 2, N'مدير مبيعات', 18000.00, 1),
(N'سارة', N'محمود', N'sara.m@company.com', N'02-56789012', N'015-45678901', '2022-01-20', 5, N'مسؤول موارد بشرية', 10000.00, 1),
(N'علي', N'إبراهيم', N'ali.i@company.com', N'02-67890123', N'016-56789012', '2021-08-15', 4, N'أمين مخزن', 8000.00, 1),
(N'نور', N'أحمد', N'nour.a@company.com', N'02-78901234', N'017-67890123', '2023-02-01', 6, N'مسؤول IT', 15000.00, 1),
(N'خالد', N'سعيد', N'khaled.s@company.com', N'02-89012345', N'018-78901234', '2022-05-10', 2, N'مندوب مبيعات', 6000.00, 1),
(N'منى', N'حسن', N'mona.h@company.com', N'02-90123456', N'019-89012345', '2023-01-15', 3, N'محاسب', 9000.00, 1);

-- رواتب الموظفين
INSERT INTO Salaries (EmployeeId, Month, Year, BasicSalary, Allowances, Deductions, NetSalary, PaymentDate, IsPaid) VALUES
(1, 1, 2024, 25000.00, 5000.00, 2000.00, 28000.00, '2024-01-31', 1),
(2, 1, 2024, 12000.00, 2000.00, 500.00, 13500.00, '2024-01-31', 1),
(3, 1, 2024, 18000.00, 3000.00, 1000.00, 20000.00, '2024-01-31', 1),
(4, 1, 2024, 10000.00, 1500.00, 300.00, 11200.00, '2024-01-31', 1),
(5, 1, 2024, 8000.00, 1000.00, 200.00, 8800.00, '2024-01-31', 1),
(6, 1, 2024, 15000.00, 2500.00, 800.00, 16700.00, '2024-01-31', 1),
(7, 1, 2024, 6000.00, 1000.00, 150.00, 6850.00, '2024-01-31', 1),
(8, 1, 2024, 9000.00, 1500.00, 250.00, 10250.00, '2024-01-31', 1);

-- حضور وانصراف
INSERT INTO Attendance (EmployeeId, Date, CheckIn, CheckOut, Status, Notes) VALUES
(1, CAST(GETDATE() AS DATE), '08:30', '17:00', N'Present', N''),
(2, CAST(GETDATE() AS DATE), '08:45', '16:45', N'Present', N''),
(3, CAST(GETDATE() AS DATE), '09:00', '18:00', N'Present', N''),
(4, CAST(GETDATE() AS DATE), '08:30', '16:30', N'Present', N''),
(5, CAST(GETDATE() AS DATE), '07:30', '15:30', N'Present', N''),
(6, CAST(GETDATE() AS DATE), '09:00', '17:00', N'Present', N''),
(7, CAST(GETDATE() AS DATE), '08:00', '16:00', N'Present', N''),
(8, CAST(GETDATE() AS DATE), '08:30', '16:30', N'Present', N'');

-- ============================================
-- 8. البنوك والحسابات (Banks & Accounts)
-- ============================================

INSERT INTO Banks (Name, Code, SwiftCode, Address, Phone, IsActive) VALUES
(N'البنك الأهلي المصري', N'NBE', N'NBEGEGCXXXX', N'1111 شارع قصر النيل', N'19623', 1),
(N'بنك مصر', N'BANQUE_MISR', N'BMISEGCA', N'10 شارع محمد فريد', N'19880', 1),
(N'البنك التجاري الدولي', N'CIB', N'COMMEGCA', N'أبراج النيل', N'19666', 1),
(N'بنك قطر الوطني الأهلي', N'QNBA', N'QNBAEGCA', N'شارع التسعين', N'19202', 1);

INSERT INTO BankAccounts (BankId, AccountName, AccountNumber, IBAN, Currency, Balance, AccountType, IsActive) VALUES
(1, N'الشركة - الحساب الرئيسي', N'12345678901234567890', N'EG380019000500000000123456789012', N'EGP', 500000.00, N'Current', 1),
(1, N'حساب الرواتب', N'09876543210987654321', N'EG380019000500000000987654321098', N'EGP', 200000.00, N'Current', 1),
(2, N'حساب التشغيل', N'11223344556677889900', N'EG380002000100000000112233445566', N'EGP', 350000.00, N'Current', 1),
(3, N'حساب الدولار', N'USD123456789', N'EG380011000500000000123456789012', N'USD', 50000.00, N'Foreign', 1);

-- ============================================
-- 9. العملات وأسعار الصرف (Currencies & Exchange Rates)
-- ============================================

INSERT INTO Currencies (Code, Name, Symbol, ExchangeRate, IsActive) VALUES
(N'EGP', N'الجنيه المصري', N'ج.م', 1.00, 1),
(N'USD', N'الدولار الأمريكي', N'$', 47.50, 1),
(N'EUR', N'اليورو', N'€', 51.20, 1),
(N'SAR', N'الريال السعودي', N'ر.س', 12.65, 1),
(N'AED', N'الدرهم الإماراتي', N'د.إ', 12.95, 1),
(N'GBP', N'الجنيه الإسترليني', N'£', 60.50, 1);

-- ============================================
-- 10. الإعدادات (Settings)
-- ============================================

INSERT INTO Settings (Key, Value, DataType, Description, UpdatedAt) VALUES
(N'CompanyName', N'شركة السعد للمحاسبة', N'String', N'اسم الشركة', GETDATE()),
(N'TaxNumber', N'123-456-789', N'String', N'الرقم الضريبي', GETDATE()),
(N'DefaultCurrency', N'EGP', N'String', N'العملة الافتراضية', GETDATE()),
(N'InvoicePrefix', N'INV-', N'String', N'بادئة رقم الفاتورة', GETDATE()),
(N'InvoiceNumberFormat', N'YYYY-NNNNNN', N'String', N'تنسيق رقم الفاتورة', GETDATE()),
(N'DefaultTaxRate', N'14', N'Decimal', N'نسبة الضريبة الافتراضية', GETDATE()),
(N'FiscalYearStart', N'2024-01-01', N'Date', N'بداية السنة المالية', GETDATE()),
(N'FiscalYearEnd', N'2024-12-31', N'Date', N'نهاية السنة المالية', GETDATE()),
(N'EnableBarcode', N'true', N'Boolean', N'تفعيل الباركود', GETDATE()),
(N'EnableAutoBackup', N'true', N'Boolean', N'تفعيل النسخ الاحتياطي التلقائي', GETDATE()),
(N'BackupPath', N'C:\Backups\AlSa3d', N'String', N'مسار النسخ الاحتياطي', GETDATE()),
(N'ReportLogo', N'logo.png', N'String', N'شعار التقارير', GETDATE()),
(N'ReportFooter', N'شكراً لتعاملكم معنا', N'String', N'تذييل التقارير', GETDATE()),
(N'DecimalPlaces', N'2', N'Integer', N'عدد الخانات العشرية', GETDATE()),
(N'ThousandsSeparator', N',', N'String', N'فاصل الآلاف', GETDATE()),
(N'DecimalSeparator', N'.', N'String', N'الفاصل العشري', GETDATE());

-- ============================================
-- 11. سجل التدقيق (Audit Logs - Sample)
-- ============================================

INSERT INTO AuditLogs (UserId, Action, TableName, RecordId, OldValue, NewValue, IpAddress, UserAgent, CreatedAt) VALUES
(1, N'CREATE', N'Customers', 1, NULL, N'تم إضافة عميل جديد: شركة الأمل التجارية', N'192.168.1.100', N'Mozilla/5.0', GETDATE()),
(1, N'UPDATE', N'Products', 1, N'السعر: 11000', N'السعر: 12000', N'192.168.1.100', N'Mozilla/5.0', GETDATE()),
(2, N'LOGIN', N'Users', 2, NULL, N'تسجيل دخول المستخدم: ahmed', N'192.168.1.101', N'Mozilla/5.0', GETDATE());

-- ============================================
-- نهاية البيانات التجريبية
-- ============================================

PRINT '✅ تم إضافة البيانات التجريبية بنجاح!';
PRINT 'Total Customers: ' + CAST((SELECT COUNT(*) FROM Customers) AS NVARCHAR);
PRINT 'Total Products: ' + CAST((SELECT COUNT(*) FROM Products) AS NVARCHAR);
PRINT 'Total Employees: ' + CAST((SELECT COUNT(*) FROM Employees) AS NVARCHAR);
PRINT 'Total Users: ' + CAST((SELECT COUNT(*) FROM Users) AS NVARCHAR);
