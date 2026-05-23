-- ============================================
-- AlSa3d Database - Stored Procedures
-- الإجراءات المخزنة
-- ============================================

USE AlSa3d;
GO

-- ============================================
-- 1. إجراءات لوحة التحكم (Dashboard)
-- ============================================

CREATE OR ALTER PROCEDURE sp_GetDashboardStats
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        (SELECT COUNT(*) FROM Customers WHERE IsDeleted = 0) AS TotalCustomers,
        (SELECT COUNT(*) FROM Products WHERE IsDeleted = 0) AS TotalProducts,
        (SELECT COUNT(*) FROM Invoices WHERE IsDeleted = 0 AND InvoiceType = 'Sale') AS TotalInvoices,
        (SELECT SUM(TotalAmount) FROM Invoices WHERE IsDeleted = 0 AND InvoiceType = 'Sale' AND CAST(InvoiceDate AS DATE) = CAST(GETDATE() AS DATE)) AS TodaySales,
        (SELECT SUM(TotalAmount) FROM Invoices WHERE IsDeleted = 0 AND InvoiceType = 'Sale' AND YEAR(InvoiceDate) = YEAR(GETDATE()) AND MONTH(InvoiceDate) = MONTH(GETDATE())) AS MonthSales,
        (SELECT SUM(Balance) FROM BankAccounts WHERE IsDeleted = 0) AS TotalBankBalance,
        (SELECT COUNT(*) FROM Employees WHERE IsDeleted = 0) AS TotalEmployees,
        (SELECT COUNT(*) FROM Invoices WHERE IsDeleted = 0 AND Status = 'Pending') AS PendingInvoices;
END
GO

-- ============================================
-- 2. إجراءات الفواتير (Invoices)
-- ============================================

CREATE OR ALTER PROCEDURE sp_InsertInvoiceWithItems
    @CustomerId INT,
    @InvoiceType NVARCHAR(50),
    @PaymentMethod NVARCHAR(50),
    @Notes NVARCHAR(MAX),
    @UserId INT,
    @Items NVARCHAR(MAX), -- JSON format
    @InvoiceId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @TotalAmount DECIMAL(18,2);
        DECLARE @Discount DECIMAL(18,2) = 0;
        DECLARE @TaxAmount DECIMAL(18,2);
        DECLARE @FinalTotal DECIMAL(18,2);
        
        -- حساب الإجمالي من الـ JSON
        SELECT @TotalAmount = SUM(CAST(value AS DECIMAL(18,2)))
        FROM OPENJSON(@Items)
        WITH (
            [Total] DECIMAL(18,2) '$.total'
        );
        
        -- إنشاء الفاتورة
        INSERT INTO Invoices (
            CustomerId, InvoiceNumber, InvoiceDate, InvoiceType, 
            PaymentMethod, SubTotal, Discount, TaxAmount, TotalAmount,
            Status, Notes, CreatedBy, CreatedAt
        ) VALUES (
            @CustomerId, 
            dbo.fn_GenerateInvoiceNumber(),
            GETDATE(),
            @InvoiceType,
            @PaymentMethod,
            @TotalAmount,
            @Discount,
            @TotalAmount * 0.14, -- ضريبة 14%
            @TotalAmount * 1.14,
            'Paid',
            @Notes,
            @UserId,
            GETDATE()
        );
        
        SET @InvoiceId = SCOPE_IDENTITY();
        
        -- إضافة أصناف الفاتورة
        INSERT INTO InvoiceItems (InvoiceId, ProductId, Quantity, UnitPrice, Discount, TaxRate, Total, WarehouseId)
        SELECT 
            @InvoiceId,
            CAST(value AS INT),
            CAST(JSON_VALUE(@Items, CONCAT('$[', [key], '].quantity')) AS INT),
            CAST(JSON_VALUE(@Items, CONCAT('$[', [key], '].price')) AS DECIMAL(18,2)),
            CAST(JSON_VALUE(@Items, CONCAT('$[', [key], '].discount')) AS DECIMAL(18,2)),
            CAST(JSON_VALUE(@Items, CONCAT('$[', [key], '].taxRate')) AS DECIMAL(18,2)),
            CAST(JSON_VALUE(@Items, CONCAT('$[', [key], '].total')) AS DECIMAL(18,2)),
            CAST(JSON_VALUE(@Items, CONCAT('$[', [key], '].warehouseId')) AS INT)
        FROM OPENJSON(@Items)
        WITH (
            [ProductId] INT '$.productId'
        );
        
        -- تحديث المخزون
        EXEC sp_UpdateProductStockFromInvoice @InvoiceId;
        
        COMMIT TRANSACTION;
        
        PRINT 'تم إنشاء الفاتورة بنجاح رقم: ' + CAST(@InvoiceId AS NVARCHAR);
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        THROW;
    END CATCH
END
GO

-- ============================================
-- 3. إجراءات المخزون (Inventory)
-- ============================================

CREATE OR ALTER PROCEDURE sp_UpdateProductStockFromInvoice
    @InvoiceId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE pw
    SET pw.Quantity = pw.Quantity - ii.Quantity,
        pw.LastStockCheck = GETDATE()
    FROM ProductWarehouse pw
    INNER JOIN InvoiceItems ii ON pw.ProductId = ii.ProductId
    WHERE ii.InvoiceId = @InvoiceId
      AND pw.WarehouseId = ISNULL(ii.WarehouseId, 1);
END
GO

CREATE OR ALTER PROCEDURE sp_GetProductStockAlert
    @MinDays INT = 7
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        p.ProductId,
        p.Name,
        p.Code,
        p.Barcode,
        c.Name AS CategoryName,
        SUM(pw.Quantity) AS TotalStock,
        p.MinStockLevel,
        CASE 
            WHEN SUM(pw.Quantity) <= p.MinStockLevel THEN 'Critical'
            WHEN SUM(pw.Quantity) <= p.MinStockLevel * 1.5 THEN 'Low'
            ELSE 'OK'
        END AS StockStatus,
        p.UnitPrice,
        p.CurrentStock * p.UnitPrice AS StockValue
    FROM Products p
    LEFT JOIN Categories c ON p.CategoryId = c.CategoryId
    LEFT JOIN ProductWarehouse pw ON p.ProductId = pw.ProductId
    WHERE p.IsDeleted = 0
    GROUP BY p.ProductId, p.Name, p.Code, p.Barcode, c.Name, 
             p.MinStockLevel, p.UnitPrice, p.CurrentStock
    HAVING SUM(pw.Quantity) <= p.MinStockLevel * 2
    ORDER BY TotalStock ASC;
END
GO

-- ============================================
-- 4. إجراءات العملاء (Customers)
-- ============================================

CREATE OR ALTER PROCEDURE sp_GetCustomerInvoiceSummary
    @CustomerId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.CustomerId,
        c.Name,
        c.Email,
        c.Phone,
        COUNT(i.InvoiceId) AS TotalInvoices,
        SUM(i.TotalAmount) AS TotalPurchases,
        SUM(CASE WHEN i.PaymentMethod = 'Credit' THEN i.TotalAmount ELSE 0 END) AS CreditAmount,
        MAX(i.InvoiceDate) AS LastPurchaseDate
    FROM Customers c
    LEFT JOIN Invoices i ON c.CustomerId = i.CustomerId AND i.IsDeleted = 0
    WHERE c.CustomerId = @CustomerId AND c.IsDeleted = 0
    GROUP BY c.CustomerId, c.Name, c.Email, c.Phone;
END
GO

CREATE OR ALTER PROCEDURE sp_SearchCustomers
    @SearchTerm NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP 50
        CustomerId,
        Name,
        Email,
        Phone,
        Mobile,
        City,
        Country
    FROM Customers
    WHERE IsDeleted = 0
      AND (
          Name LIKE '%' + @SearchTerm + '%'
          OR Email LIKE '%' + @SearchTerm + '%'
          OR Phone LIKE '%' + @SearchTerm + '%'
          OR Mobile LIKE '%' + @SearchTerm + '%'
          OR TaxNumber LIKE '%' + @SearchTerm + '%'
      )
    ORDER BY Name;
END
GO

-- ============================================
-- 5. إجراءات الموظفين (Employees)
-- ============================================

CREATE OR ALTER PROCEDURE sp_GetEmployeeSalaryReport
    @Month INT,
    @Year INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.EmployeeId,
        e.FirstName + ' ' + e.LastName AS FullName,
        d.Name AS DepartmentName,
        e.JobTitle,
        s.BasicSalary,
        s.Allowances,
        s.Deductions,
        s.NetSalary,
        s.PaymentDate,
        s.IsPaid
    FROM Employees e
    INNER JOIN Departments d ON e.DepartmentId = d.DepartmentId
    INNER JOIN Salaries s ON e.EmployeeId = s.EmployeeId
    WHERE e.IsDeleted = 0
      AND s.Month = @Month
      AND s.Year = @Year
    ORDER BY d.Name, e.FirstName;
END
GO

CREATE OR ALTER PROCEDURE sp_GetEmployeeAttendanceSummary
    @FromDate DATE,
    @ToDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.EmployeeId,
        e.FirstName + ' ' + e.LastName AS FullName,
        d.Name AS DepartmentName,
        COUNT(CASE WHEN a.Status = 'Present' THEN 1 END) AS PresentDays,
        COUNT(CASE WHEN a.Status = 'Absent' THEN 1 END) AS AbsentDays,
        COUNT(CASE WHEN a.Status = 'Late' THEN 1 END) AS LateDays,
        COUNT(*) AS TotalDays,
        CAST(COUNT(CASE WHEN a.Status = 'Present' THEN 1 END) AS DECIMAL(18,2)) / COUNT(*) * 100 AS AttendancePercentage
    FROM Employees e
    INNER JOIN Departments d ON e.DepartmentId = d.DepartmentId
    LEFT JOIN Attendance a ON e.EmployeeId = a.EmployeeId
        AND a.Date BETWEEN @FromDate AND @ToDate
    WHERE e.IsDeleted = 0
    GROUP BY e.EmployeeId, e.FirstName, e.LastName, d.Name
    ORDER BY d.Name, e.FirstName;
END
GO

-- ============================================
-- 6. إجراءات البنوك (Banks)
-- ============================================

CREATE OR ALTER PROCEDURE sp_GetBankReconciliation
    @BankAccountId INT,
    @FromDate DATE,
    @ToDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ba.AccountName,
        ba.AccountNumber,
        ba.Balance AS CurrentBalance,
        SUM(CASE WHEN t.TransactionType IN ('Deposit', 'Transfer_In') THEN t.Amount ELSE -t.Amount END) AS NetTransactions,
        ba.Balance + SUM(CASE WHEN t.TransactionType IN ('Deposit', 'Transfer_In') THEN t.Amount ELSE -t.Amount END) AS CalculatedBalance
    FROM BankAccounts ba
    LEFT JOIN Transactions t ON ba.BankAccountId = t.BankAccountId
        AND t.TransactionDate BETWEEN @FromDate AND @ToDate
        AND t.IsDeleted = 0
    WHERE ba.BankAccountId = @BankAccountId AND ba.IsDeleted = 0
    GROUP BY ba.AccountName, ba.AccountNumber, ba.Balance;
END
GO

-- ============================================
-- 7. إجراءات التقارير العامة (Reports)
-- ============================================

CREATE OR ALTER PROCEDURE sp_GetDailyTransactions
    @TransactionDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        t.TransactionId,
        t.TransactionType,
        t.Amount,
        t.Currency,
        t.Description,
        ba.AccountName,
        b.Name AS BankName,
        u.FullName AS CreatedBy,
        t.CreatedAt
    FROM Transactions t
    INNER JOIN BankAccounts ba ON t.BankAccountId = ba.BankAccountId
    INNER JOIN Banks b ON ba.BankId = b.BankId
    LEFT JOIN Users u ON t.CreatedBy = u.UserId
    WHERE CAST(t.TransactionDate AS DATE) = @TransactionDate
      AND t.IsDeleted = 0
    ORDER BY t.CreatedAt DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_GetMonthlySalesReport
    @Month INT,
    @Year INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        CAST(i.InvoiceDate AS DATE) AS SaleDate,
        COUNT(i.InvoiceId) AS InvoiceCount,
        SUM(i.SubTotal) AS TotalSales,
        SUM(i.TaxAmount) AS TotalTax,
        SUM(i.TotalAmount) AS GrandTotal,
        AVG(i.TotalAmount) AS AverageInvoice
    FROM Invoices i
    WHERE i.IsDeleted = 0
      AND i.InvoiceType = 'Sale'
      AND YEAR(i.InvoiceDate) = @Year
      AND MONTH(i.InvoiceDate) = @Month
    GROUP BY CAST(i.InvoiceDate AS DATE)
    ORDER BY SaleDate;
END
GO

-- ============================================
-- 8. إجراءات الأرشفة (Archiving)
-- ============================================

CREATE OR ALTER PROCEDURE sp_ArchiveOldData
    @ArchiveBefore DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- أرشفة الفواتير القديمة
        UPDATE Invoices
        SET IsArchived = 1
        WHERE InvoiceDate < @ArchiveBefore
          AND IsArchived = 0;
        
        -- أرشفة الحركات القديمة
        UPDATE Transactions
        SET IsArchived = 1
        WHERE TransactionDate < @ArchiveBefore
          AND IsArchived = 0;
        
        COMMIT TRANSACTION;
        
        PRINT 'تمت الأرشفة بنجاح';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- ============================================
-- نهاية الإجراءات المخزنة
-- ============================================

PRINT '✅ تم إنشاء الإجراءات المخزنة بنجاح!';
