using AlSa3d.Core.Entities;
using AlSa3d.Core.Entities.InvoiceModule;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlSa3d.Services.Interfaces
{
    /// <summary>
    /// واجهة خدمة الفواتير - المسؤولة عن جميع عمليات الفواتير
    /// </summary>
    public interface IInvoiceService
    {
        #region Invoice Operations

        /// <summary>
        /// إنشاء فاتورة جديدة
        /// </summary>
        Task<Result<Invoice>> CreateInvoiceAsync(Invoice invoice);

        /// <summary>
        /// تحديث فاتورة موجودة
        /// </summary>
        Task<Result<Invoice>> UpdateInvoiceAsync(Invoice invoice);

        /// <summary>
        /// حذف فاتورة (Soft Delete)
        /// </summary>
        Task<Result<bool>> DeleteInvoiceAsync(Guid invoiceId);

        /// <summary>
        /// الحصول على فاتورة بالمعرف
        /// </summary>
        Task<Result<Invoice>> GetInvoiceByIdAsync(Guid invoiceId);

        /// <summary>
        /// الحصول على جميع الفواتير
        /// </summary>
        Task<Result<List<Invoice>>> GetAllInvoicesAsync();

        /// <summary>
        /// البحث عن فواتير برقم الفاتورة
        /// </summary>
        Task<Result<List<Invoice>>> SearchByInvoiceNumberAsync(string invoiceNumber);

        /// <summary>
        /// الحصول على فواتير عميل معين
        /// </summary>
        Task<Result<List<Invoice>>> GetInvoicesByCustomerIdAsync(Guid customerId);

        /// <summary>
        /// الحصول على فواتير تاريخ معين
        /// </summary>
        Task<Result<List<Invoice>>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// تأكيد الفاتورة (لا يمكن التعديل بعد التأكيد)
        /// </summary>
        Task<Result<bool>> ConfirmInvoiceAsync(Guid invoiceId);

        /// <summary>
        /// إلغاء الفاتورة
        /// </summary>
        Task<Result<bool>> CancelInvoiceAsync(Guid invoiceId);

        #endregion

        #region Invoice Items Operations

        /// <summary>
        /// إضافة صنف للفاتورة
        /// </summary>
        Task<Result<InvoiceItem>> AddInvoiceItemAsync(Guid invoiceId, InvoiceItem item);

        /// <summary>
        /// تحديث صنف في الفاتورة
        /// </summary>
        Task<Result<InvoiceItem>> UpdateInvoiceItemAsync(Guid itemId, InvoiceItem item);

        /// <summary>
        /// حذف صنف من الفاتورة
        /// </summary>
        Task<Result<bool>> DeleteInvoiceItemAsync(Guid itemId);

        /// <summary>
        /// الحصول على أصناف الفاتورة
        /// </summary>
        Task<Result<List<InvoiceItem>>> GetInvoiceItemsAsync(Guid invoiceId);

        #endregion

        #region Calculations

        /// <summary>
        /// حساب إجمالي الفاتورة
        /// </summary>
        Task<Result<decimal>> CalculateInvoiceTotalAsync(Guid invoiceId);

        /// <summary>
        /// حساب الضريبة
        /// </summary>
        Task<Result<decimal>> CalculateTaxAsync(Guid invoiceId, decimal taxRate = 14.0m);

        /// <summary>
        /// حساب الخصم
        /// </summary>
        Task<Result<decimal>> ApplyDiscountAsync(Guid invoiceId, decimal discountAmount, DiscountType discountType);

        #endregion

        #region Reports

        /// <summary>
        /// تقرير الفواتير اليومية
        /// </summary>
        Task<Result<DailyInvoiceReport>> GetDailyReportAsync(DateTime date);

        /// <summary>
        /// تقرير فواتير العميل
        /// </summary>
        Task<Result<CustomerInvoiceReport>> GetCustomerReportAsync(Guid customerId, DateTime? startDate = null, DateTime? endDate = null);

        #endregion
    }

    #region Report Models

    public class DailyInvoiceReport
    {
        public DateTime ReportDate { get; set; }
        public int TotalInvoices { get; set; }
        public int ConfirmedInvoices { get; set; }
        public int CancelledInvoices { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal NetTotal { get; set; }
        public List<InvoiceSummary> InvoiceSummaries { get; set; } = new();
    }

    public class CustomerInvoiceReport
    {
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int TotalInvoices { get; set; }
        public decimal TotalPurchases { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal RemainingBalance { get; set; }
        public List<InvoiceSummary> InvoiceSummaries { get; set; } = new();
    }

    public class InvoiceSummary
    {
        public Guid InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
        public decimal Paid { get; set; }
        public decimal Remaining { get; set; }
        public InvoiceStatus Status { get; set; }
    }

    #endregion
}
