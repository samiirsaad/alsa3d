using AlSa3d.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlSa3d.Services.Interfaces
{
    /// <summary>
    /// واجهة خدمة المنتجات والمخازن
    /// </summary>
    public interface IProductService
    {
        #region Product Operations

        /// <summary>
        /// إنشاء منتج جديد
        /// </summary>
        Task<Result<Product>> CreateProductAsync(Product product);

        /// <summary>
        /// تحديث منتج
        /// </summary>
        Task<Result<Product>> UpdateProductAsync(Product product);

        /// <summary>
        /// حذف منتج (Soft Delete)
        /// </summary>
        Task<Result<bool>> DeleteProductAsync(Guid productId);

        /// <summary>
        /// الحصول على منتج بالمعرف
        /// </summary>
        Task<Result<Product>> GetProductByIdAsync(Guid productId);

        /// <summary>
        /// الحصول على جميع المنتجات
        /// </summary>
        Task<Result<List<Product>>> GetAllProductsAsync();

        /// <summary>
        /// البحث عن منتج بالاسم
        /// </summary>
        Task<Result<List<Product>>> SearchByNameAsync(string name);

        /// <summary>
        /// البحث عن منتج بالباركود
        /// </summary>
        Task<Result<Product>> GetProductByBarcodeAsync(string barcode);

        /// <summary>
        /// الحصول على منتج بالكود
        /// </summary>
        Task<Result<Product>> GetProductByCodeAsync(string productCode);

        /// <summary>
        /// تفعيل/تعطيل منتج
        /// </summary>
        Task<Result<bool>> ToggleProductStatusAsync(Guid productId, bool isActive);

        #endregion

        #region Category Operations

        /// <summary>
        /// إنشاء فئة منتجات
        /// </summary>
        Task<Result<ProductCategory>> CreateCategoryAsync(ProductCategory category);

        /// <summary>
        /// تحديث فئة
        /// </summary>
        Task<Result<ProductCategory>> UpdateCategoryAsync(ProductCategory category);

        /// <summary>
        /// حذف فئة
        /// </summary>
        Task<Result<bool>> DeleteCategoryAsync(Guid categoryId);

        /// <summary>
        /// الحصول على جميع الفئات
        /// </summary>
        Task<Result<List<ProductCategory>>> GetAllCategoriesAsync();

        /// <summary>
        /// الحصول على منتجات الفئة
        /// </summary>
        Task<Result<List<Product>>> GetCategoryProductsAsync(Guid categoryId);

        #endregion

        #region Unit Operations

        /// <summary>
        /// إنشاء وحدة قياس
        /// </summary>
        Task<Result<Unit>> CreateUnitAsync(Unit unit);

        /// <summary>
        /// تحديث وحدة
        /// </summary>
        Task<Result<Unit>> UpdateUnitAsync(Unit unit);

        /// <summary>
        /// حذف وحدة
        /// </summary>
        Task<Result<bool>> DeleteUnitAsync(Guid unitId);

        /// <summary>
        /// الحصول على جميع الوحدات
        /// </summary>
        Task<Result<List<Unit>>> GetAllUnitsAsync();

        #endregion

        #region Warehouse Operations

        /// <summary>
        /// إنشاء مخزن جديد
        /// </summary>
        Task<Result<Warehouse>> CreateWarehouseAsync(Warehouse warehouse);

        /// <summary>
        /// تحديث مخزن
        /// </summary>
        Task<Result<Warehouse>> UpdateWarehouseAsync(Warehouse warehouse);

        /// <summary>
        /// حذف مخزن
        /// </summary>
        Task<Result<bool>> DeleteWarehouseAsync(Guid warehouseId);

        /// <summary>
        /// الحصول على جميع المخازن
        /// </summary>
        Task<Result<List<Warehouse>>> GetAllWarehousesAsync();

        /// <summary>
        /// الحصول على رصيد المنتج في المخزن
        /// </summary>
        Task<Result<decimal>> GetProductStockAsync(Guid productId, Guid warehouseId);

        /// <summary>
        /// الحصول على جميع أرصدة المنتج
        /// </summary>
        Task<Result<List<WarehouseStock>>> GetProductStocksAsync(Guid productId);

        #endregion

        #region Stock Operations

        /// <summary>
        /// إضافة رصيد للمخزن (شراء/إرجاع)
        /// </summary>
        Task<Result<StockTransaction>> AddStockAsync(StockTransaction transaction);

        /// <summary>
        /// خصم من المخزن (بيع/تالف)
        /// </summary>
        Task<Result<StockTransaction>> RemoveStockAsync(StockTransaction transaction);

        /// <summary>
        /// تحويل بين مخازن
        /// </summary>
        Task<Result<StockTransfer>> TransferStockAsync(StockTransfer transfer);

        /// <summary>
        /// تسجيل جرد المخزن
        /// </summary>
        Task<Result<StockAdjustment>> RecordStockAdjustmentAsync(StockAdjustment adjustment);

        /// <summary>
        /// الحصول على حركات المخزن
        /// </summary>
        Task<Result<List<StockTransaction>>> GetWarehouseTransactionsAsync(Guid warehouseId, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// الحصول على حركات المنتج
        /// </summary>
        Task<Result<List<StockTransaction>>> GetProductTransactionsAsync(Guid productId, DateTime? startDate = null, DateTime? endDate = null);

        #endregion

        #region Price Operations

        /// <summary>
        /// تحديث سعر المنتج
        /// </summary>
        Task<Result<ProductPrice>> UpdateProductPriceAsync(Guid productId, decimal price, string currency = "EGP");

        /// <summary>
        /// الحصول على سعر المنتج
        /// </summary>
        Task<Result<ProductPrice>> GetProductPriceAsync(Guid productId, string currency = "EGP");

        /// <summary>
        /// تطبيق عرض سعر
        /// </summary>
        Task<Result<SpecialOffer>> ApplySpecialOfferAsync(SpecialOffer offer);

        /// <summary>
        /// إنهاء عرض السعر
        /// </summary>
        Task<Result<bool>> EndSpecialOfferAsync(Guid offerId);

        #endregion

        #region Supplier Operations

        /// <summary>
        /// إنشاء مورد جديد
        /// </summary>
        Task<Result<Supplier>> CreateSupplierAsync(Supplier supplier);

        /// <summary>
        /// تحديث مورد
        /// </summary>
        Task<Result<Supplier>> UpdateSupplierAsync(Supplier supplier);

        /// <summary>
        /// حذف مورد
        /// </summary>
        Task<Result<bool>> DeleteSupplierAsync(Guid supplierId);

        /// <summary>
        /// الحصول على جميع الموردين
        /// </summary>
        Task<Result<List<Supplier>>> GetAllSuppliersAsync();

        /// <summary>
        /// الحصول على منتجات المورد
        /// </summary>
        Task<Result<List<Product>>> GetSupplierProductsAsync(Guid supplierId);

        #endregion

        #region Purchase Order Operations

        /// <summary>
        /// إنشاء أمر شراء
        /// </summary>
        Task<Result<PurchaseOrder>> CreatePurchaseOrderAsync(PurchaseOrder order);

        /// <summary>
        /// تحديث أمر شراء
        /// </summary>
        Task<Result<PurchaseOrder>> UpdatePurchaseOrderAsync(PurchaseOrder order);

        /// <summary>
        /// اعتماد أمر شراء
        /// </summary>
        Task<Result<bool>> ApprovePurchaseOrderAsync(Guid orderId);

        /// <summary>
        /// استلام أمر شراء
        /// </summary>
        Task<Result<bool>> ReceivePurchaseOrderAsync(Guid orderId);

        /// <summary>
        /// إلغاء أمر شراء
        /// </summary>
        Task<Result<bool>> CancelPurchaseOrderAsync(Guid orderId);

        /// <summary>
        /// الحصول على أمر شراء
        /// </summary>
        Task<Result<PurchaseOrder>> GetPurchaseOrderByIdAsync(Guid orderId);

        /// <summary>
        /// الحصول على أوامر الشراء
        /// </summary>
        Task<Result<List<PurchaseOrder>>> GetPurchaseOrdersAsync(Guid? supplierId = null, PurchaseOrderStatus? status = null);

        #endregion

        #region Reports

        /// <summary>
        /// تقرير المخزون الحالي
        /// </summary>
        Task<Result<InventoryReport>> GetInventoryReportAsync(Guid? warehouseId = null);

        /// <summary>
        /// تقرير حركة الأصناف
        /// </summary>
        Task<Result<StockMovementReport>> GetStockMovementReportAsync(DateTime startDate, DateTime endDate, Guid? productId = null);

        /// <summary>
        /// تقرير الأصناف منخفضة المخزون
        /// </summary>
        Task<Result<List<Product>>> GetLowStockProductsAsync(decimal threshold = 10);

        /// <summary>
        /// تقرير قيمة المخزون
        /// </summary>
        Task<Result<InventoryValuationReport>> GetInventoryValuationReportAsync(Guid? warehouseId = null);

        #endregion
    }

    #region Report Models

    public class InventoryReport
    {
        public Guid? WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public DateTime ReportDate { get; set; }
        public int TotalProducts { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
        public List<InventoryItem> Items { get; set; } = new();
    }

    public class InventoryItem
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string Barcode { get; set; }
        public string Category { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal TotalValue { get; set; }
        public decimal ReorderLevel { get; set; }
        public bool IsLowStock { get; set; }
    }

    public class StockMovementReport
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid? ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal OpeningStock { get; set; }
        public decimal TotalIn { get; set; }
        public decimal TotalOut { get; set; }
        public decimal ClosingStock { get; set; }
        public List<StockTransaction> Transactions { get; set; } = new();
    }

    public class InventoryValuationReport
    {
        public Guid? WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public DateTime ReportDate { get; set; }
        public string ValuationMethod { get; set; } // FIFO, LIFO, Weighted Average
        public decimal TotalInventoryValue { get; set; }
        public List<CategoryValuation> CategoryValuations { get; set; } = new();
    }

    public class CategoryValuation
    {
        public string CategoryName { get; set; }
        public int ProductCount { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
        public decimal PercentageOfTotal { get; set; }
    }

    public class WarehouseStock
    {
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public decimal Quantity { get; set; }
        public decimal ReservedQuantity { get; set; }
        public decimal AvailableQuantity { get; set; }
    }

    #endregion
}
