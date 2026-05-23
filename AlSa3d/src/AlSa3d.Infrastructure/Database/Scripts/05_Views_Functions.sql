-- ============================================
-- AlSa3d Database - Views & Functions
-- العروض والدوال
-- ============================================

USE AlSa3d;
GO

-- ============================================
-- 1. Views (العروض)
-- ============================================

-- عرض الفواتير مع التفاصيل
CREATE OR ALTER VIEW vw_InvoicesWithDetails
AS
SELECT 
    i.InvoiceId,
    i.InvoiceNumber,
    i.InvoiceDate,
    i.InvoiceType,
    c.Name AS CustomerName,
    c.Phone AS CustomerPhone,
    i.PaymentMethod,
    i.SubTotal,
    i.Discount,
    i.TaxAmount,
    i.TotalAmount,
    i.Status,
    u.FullName AS CreatedBy,
    i.CreatedAt,
    (SELECT COUNT(*) FROM InvoiceItems WHERE InvoiceId = i.InvoiceId) AS ItemCount
FROM Invoices i
INNER JOIN Customers c ON i.CustomerId = c.CustomerId
LEFT JOIN Users u ON i.CreatedBy = u.UserId
WHERE i.IsDeleted = 0;
GO

-- عرض أرصدة العملاء
CREATE OR ALTER VIEW vw_CustomerBalance
AS
SELECT 
    c.CustomerId,
    c.Name,
    c.Email,
    c.Phone,
    COUNT(i.InvoiceId) AS TotalInvoices,
    SUM(i.TotalAmount) AS TotalPurchases,
    SUM(CASE WHEN i.PaymentMethod = 'Credit' THEN i.TotalAmount ELSE 0 END) AS CreditBalance,
    MAX(i.InvoiceDate) AS LastInvoiceDate
FROM Customers c
LEFT JOIN Invoices i ON c.CustomerId = i.CustomerId AND i.IsDeleted = 0
WHERE c.IsDeleted = 0
GROUP BY c.CustomerId, c.Name, c.Email, c.Phone;
GO

-- عرض حالة المخزون
CREATE OR ALTER VIEW vw_ProductStockStatus
AS
SELECT 
    p.ProductId,
    p.Name,
    p.Code,
    p.Barcode,
    c.Name AS CategoryName,
    w.Name AS WarehouseName,
    pw.Quantity AS CurrentStock,
    p.MinStockLevel,
    p.MaxStockLevel,
    p.UnitPrice,
    p.CostPrice,
    CASE 
        WHEN pw.Quantity <= p.MinStockLevel THEN 'Critical'
        WHEN pw.Quantity <= p.MinStockLevel * 1.5 THEN 'Low'
        WHEN pw.Quantity >= p.MaxStockLevel * 0.8 THEN 'Overstocked'
        ELSE 'OK'
    END AS StockStatus,
    pw.Quantity * p.UnitPrice AS StockValue,
    pw.LastStockCheck
FROM Products p
INNER JOIN Categories c ON p.CategoryId = c.CategoryId
INNER JOIN ProductWarehouse pw ON p.ProductId = pw.ProductId
INNER JOIN Warehouses w ON pw.WarehouseId = w.WarehouseId
WHERE p.IsDeleted = 0 AND w.IsDeleted = 0;
GO

-- عرض ملخص حضور الموظفين
CREATE OR ALTER VIEW vw_EmployeeAttendanceSummary
AS
SELECT 
    e.EmployeeId,
    e.FirstName + ' ' + e.LastName AS FullName,
    d.Name AS DepartmentName,
    e.JobTitle,
    CAST(GETDATE() AS DATE) AS ReportDate,
    COUNT(CASE WHEN a.Status = 'Present' THEN 1 END) AS PresentDays,
    COUNT(CASE WHEN a.Status = 'Absent' THEN 1 END) AS AbsentDays,
    COUNT(CASE WHEN a.Status = 'Late' THEN 1 END) AS LateDays,
    COUNT(*) AS TotalDays,
    CAST(COUNT(CASE WHEN a.Status = 'Present' THEN 1 END) AS DECIMAL(18,2)) / COUNT(*) * 100 AS AttendancePercentage
FROM Employees e
INNER JOIN Departments d ON e.DepartmentId = d.DepartmentId
LEFT JOIN Attendance a ON e.EmployeeId = a.EmployeeId AND MONTH(a.Date) = MONTH(GETDATE()) AND YEAR(a.Date) = YEAR(GETDATE())
WHERE e.IsDeleted = 0
GROUP BY e.EmployeeId, e.FirstName, e.LastName, d.Name, e.JobTitle;
GO

-- عرض تقرير المبيعات اليومي
CREATE OR ALTER VIEW vw_DailySalesReport
AS
SELECT 
    CAST(i.InvoiceDate AS DATE) AS SaleDate,
    COUNT(i.InvoiceId) AS InvoiceCount,
    SUM(i.SubTotal) AS TotalSales,
    SUM(i.TaxAmount) AS TotalTax,
    SUM(i.TotalAmount) AS GrandTotal,
    AVG(i.TotalAmount) AS AverageInvoice,
    COUNT(DISTINCT i.CustomerId) AS UniqueCustomers
FROM Invoices i
WHERE i.IsDeleted = 0 AND i.InvoiceType = 'Sale'
GROUP BY CAST(i.InvoiceDate AS DATE);
GO

-- عرض تسوية البنوك
CREATE OR ALTER VIEW vw_BankReconciliation
AS
SELECT 
    ba.BankAccountId,
    b.Name AS BankName,
    ba.AccountName,
    ba.AccountNumber,
    ba.Currency,
    ba.Balance AS BookBalance,
    ISNULL(SUM(CASE WHEN t.TransactionType IN ('Deposit', 'Transfer_In') THEN t.Amount ELSE -t.Amount END), 0) AS MonthTransactions,
    ba.Balance + ISNULL(SUM(CASE WHEN t.TransactionType IN ('Deposit', 'Transfer_In') THEN t.Amount ELSE -t.Amount END), 0) AS CalculatedBalance,
    COUNT(t.TransactionId) AS TransactionCount
FROM BankAccounts ba
INNER JOIN Banks b ON ba.BankId = b.BankId
LEFT JOIN Transactions t ON ba.BankAccountId = t.BankAccountId 
    AND MONTH(t.TransactionDate) = MONTH(GETDATE()) 
    AND YEAR(t.TransactionDate) = YEAR(GETDATE())
    AND t.IsDeleted = 0
WHERE ba.IsDeleted = 0
GROUP BY ba.BankAccountId, b.Name, ba.AccountName, ba.AccountNumber, ba.Currency, ba.Balance;
GO

-- ============================================
-- 2. Functions (الدوال)
-- ============================================

-- دالة حساب إجمالي الفاتورة
CREATE OR ALTER FUNCTION fn_CalculateInvoiceTotal
(
    @InvoiceId INT
)
RETURNS TABLE
AS
RETURN
(
    SELECT 
        InvoiceId,
        SUM(SubTotal) AS SubTotal,
        SUM(Discount) AS TotalDiscount,
        SUM(TaxAmount) AS TotalTax,
        SUM(Total) AS GrandTotal
    FROM InvoiceItems
    WHERE InvoiceId = @InvoiceId
    GROUP BY InvoiceId
);
GO

-- دالة توليد رقم فاتورة تلقائي
CREATE OR ALTER FUNCTION fn_GenerateInvoiceNumber
()
RETURNS NVARCHAR(50)
AS
BEGIN
    DECLARE @Year INT = YEAR(GETDATE());
    DECLARE @Month INT = MONTH(GETDATE());
    DECLARE @Prefix NVARCHAR(10) = 'INV';
    DECLARE @NextNumber INT;
    
    SELECT @NextNumber = ISNULL(MAX(CAST(RIGHT(InvoiceNumber, 6) AS INT)), 0) + 1
    FROM Invoices
    WHERE YEAR(InvoiceDate) = @Year AND MONTH(InvoiceDate) = @Month;
    
    RETURN @Prefix + '-' + CAST(@Year AS NVARCHAR(4)) + '-' + 
           RIGHT('0' + CAST(@Month AS NVARCHAR(2)), 2) + '-' + 
           RIGHT('000000' + CAST(@NextNumber AS NVARCHAR(6)), 6);
END
GO

-- دالة تحويل الأرقام إلى كلمات (للفواتير)
CREATE OR ALTER FUNCTION fn_ConvertToWords
(
    @Number DECIMAL(18,2)
)
RETURNS NVARCHAR(500)
AS
BEGIN
    -- تبسيط: يرجع الرقم كنص فقط
    -- يمكن تطويرها لتحويل حقيقي للأرقام إلى كلمات عربية
    RETURN CAST(@Number AS NVARCHAR(50));
END
GO

-- دالة حساب أيام العمل بين تاريخين
CREATE OR ALTER FUNCTION fn_GetWorkingDays
(
    @StartDate DATE,
    @EndDate DATE
)
RETURNS INT
AS
BEGIN
    DECLARE @WorkingDays INT = 0;
    DECLARE @CurrentDate DATE = @StartDate;
    
    WHILE @CurrentDate <= @EndDate
    BEGIN
        IF DATENAME(WEEKDAY, @CurrentDate) NOT IN ('Friday', 'Saturday')
            SET @WorkingDays = @WorkingDays + 1;
        
        SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate);
    END
    
    RETURN @WorkingDays;
END
GO

-- دالة تنسيق العملة
CREATE OR ALTER FUNCTION fn_FormatCurrency
(
    @Amount DECIMAL(18,2),
    @Currency NVARCHAR(10) = 'EGP'
)
RETURNS NVARCHAR(50)
AS
BEGIN
    DECLARE @Symbol NVARCHAR(10);
    
    SELECT @Symbol = Symbol FROM Currencies WHERE Code = @Currency;
    IF @Symbol IS NULL SET @Symbol = N'ج.م';
    
    RETURN FORMAT(@Amount, 'N2') + N' ' + @Symbol;
END
GO

-- ============================================
-- نهاية Views و Functions
-- ============================================

PRINT '✅ تم إنشاء Views و Functions بنجاح!';
