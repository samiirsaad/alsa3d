using AlSa3d.Core.Entities;
using AlSa3d.Core.Entities.FinancialModule;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlSa3d.Services.Interfaces
{
    /// <summary>
    /// واجهة الخدمة المالية - البنوك، الشيكات، الخزينة
    /// </summary>
    public interface IFinancialService
    {
        #region Bank Account Operations

        /// <summary>
        /// إنشاء حساب بنكي جديد
        /// </summary>
        Task<Result<BankAccount>> CreateBankAccountAsync(BankAccount account);

        /// <summary>
        /// تحديث حساب بنكي
        /// </summary>
        Task<Result<BankAccount>> UpdateBankAccountAsync(BankAccount account);

        /// <summary>
        /// حذف حساب بنكي (Soft Delete)
        /// </summary>
        Task<Result<bool>> DeleteBankAccountAsync(Guid accountId);

        /// <summary>
        /// الحصول على حساب بنكي بالمعرف
        /// </summary>
        Task<Result<BankAccount>> GetBankAccountByIdAsync(Guid accountId);

        /// <summary>
        /// الحصول على جميع الحسابات البنكية
        /// </summary>
        Task<Result<List<BankAccount>>> GetAllBankAccountsAsync();

        /// <summary>
        /// الحصول على رصيد الحساب
        /// </summary>
        Task<Result<decimal>> GetAccountBalanceAsync(Guid accountId);

        /// <summary>
        /// تفعيل/تعطيل حساب
        /// </summary>
        Task<Result<bool>> ToggleAccountStatusAsync(Guid accountId, bool isActive);

        #endregion

        #region Transaction Operations

        /// <summary>
        /// تسجيل معاملة بنكية (إيداع/سحب)
        /// </summary>
        Task<Result<BankTransaction>> RecordTransactionAsync(BankTransaction transaction);

        /// <summary>
        /// إيداع مبلغ
        /// </summary>
        Task<Result<BankTransaction>> DepositAsync(Guid accountId, decimal amount, string description, string referenceNumber);

        /// <summary>
        /// سحب مبلغ
        /// </summary>
        Task<Result<BankTransaction>> WithdrawAsync(Guid accountId, decimal amount, string description, string referenceNumber);

        /// <summary>
        /// تحويل بين حسابات
        /// </summary>
        Task<Result<TransferTransaction>> TransferBetweenAccountsAsync(TransferTransaction transfer);

        /// <summary>
        /// الحصول على معاملات الحساب
        /// </summary>
        Task<Result<List<BankTransaction>>> GetAccountTransactionsAsync(Guid accountId, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// البحث عن معاملة بالرقم المرجعي
        /// </summary>
        Task<Result<BankTransaction>> GetTransactionByReferenceAsync(string referenceNumber);

        #endregion

        #region Check Operations

        /// <summary>
        /// إصدار شيك جديد
        /// </summary>
        Task<Result<Check>> IssueCheckAsync(Check check);

        /// <summary>
        /// استلام شيك
        /// </summary>
        Task<Result<Check>> ReceiveCheckAsync(Check check);

        /// <summary>
        /// صرف شيك
        /// </summary>
        Task<Result<bool>> CashCheckAsync(Guid checkId, Guid accountId);

        /// <summary>
        /// إرجاع شيك (بدون رصيد)
        /// </summary>
        Task<Result<bool>> BounceCheckAsync(Guid checkId, string reason);

        /// <summary>
        /// إلغاء شيك
        /// </summary>
        Task<Result<bool>> CancelCheckAsync(Guid checkId, string reason);

        /// <summary>
        /// الحصول على شيك بالمعرف
        /// </summary>
        Task<Result<Check>> GetCheckByIdAsync(Guid checkId);

        /// <summary>
        /// الحصول على الشيكات الصادرة
        /// </summary>
        Task<Result<List<Check>>> GetIssuedChecksAsync(Guid? accountId = null, CheckStatus? status = null);

        /// <summary>
        /// الحصول على الشيكات الواردة
        /// </summary>
        Task<Result<List<Check>>> GetReceivedChecksAsync(Guid? accountId = null, CheckStatus? status = null);

        /// <summary>
        /// الشيكات المستحقة الدفع
        /// </summary>
        Task<Result<List<Check>>> GetChecksDueForPaymentAsync(DateTime dueDate);

        /// <summary>
        /// البحث عن شيك بالرقم
        /// </summary>
        Task<Result<Check>> GetCheckByNumberAsync(string checkNumber);

        #endregion

        #region Treasury Operations

        /// <summary>
        /// إنشاء خزانة جديدة
        /// </summary>
        Task<Result<Treasury>> CreateTreasuryAsync(Treasury treasury);

        /// <summary>
        /// تحديث خزانة
        /// </summary>
        Task<Result<Treasury>> UpdateTreasuryAsync(Treasury treasury);

        /// <summary>
        /// حذف خزانة
        /// </summary>
        Task<Result<bool>> DeleteTreasuryAsync(Guid treasuryId);

        /// <summary>
        /// الحصول على جميع الخزائن
        /// </summary>
        Task<Result<List<Treasury>>> GetAllTreasuriesAsync();

        /// <summary>
        /// الحصول على رصيد الخزانة
        /// </summary>
        Task<Result<decimal>> GetTreasuryBalanceAsync(Guid treasuryId);

        /// <summary>
        /// إيداع في الخزانة
        /// </summary>
        Task<Result<TreasuryTransaction>> TreasuryDepositAsync(Guid treasuryId, decimal amount, string description, string referenceNumber);

        /// <summary>
        /// سحب من الخزانة
        /// </summary>
        Task<Result<TreasuryTransaction>> TreasuryWithdrawAsync(Guid treasuryId, decimal amount, string description, string referenceNumber);

        /// <summary>
        /// الحصول على معاملات الخزانة
        /// </summary>
        Task<Result<List<TreasuryTransaction>>> GetTreasuryTransactionsAsync(Guid treasuryId, DateTime? startDate = null, DateTime? endDate = null);

        #endregion

        #region Payment Operations

        /// <summary>
        /// تسجيل دفعة من عميل
        /// </summary>
        Task<Result<Payment>> RecordCustomerPaymentAsync(Payment payment);

        /// <summary>
        /// تسجيل دفعة لمورد
        /// </summary>
        Task<Result<Payment>> RecordSupplierPaymentAsync(Payment payment);

        /// <summary>
        /// الحصول على مدفوعات العميل
        /// </summary>
        Task<Result<List<Payment>>> GetCustomerPaymentsAsync(Guid customerId, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// الحصول على مدفوعات المورد
        /// </summary>
        Task<Result<List<Payment>>> GetSupplierPaymentsAsync(Guid supplierId, DateTime? startDate = null, DateTime? endDate = null);

        #endregion

        #region Reports

        /// <summary>
        /// تقرير كشف الحساب البنكي
        /// </summary>
        Task<Result<BankStatement>> GetBankStatementAsync(Guid accountId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// تقرير الشيكات
        /// </summary>
        Task<Result<CheckReport>> GetCheckReportAsync(DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// تقرير حركة الخزينة
        /// </summary>
        Task<Result<TreasuryReport>> GetTreasuryReportAsync(Guid treasuryId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// التقرير المالي اليومي
        /// </summary>
        Task<Result<DailyFinancialReport>> GetDailyFinancialReportAsync(DateTime date);

        #endregion
    }

    #region Report Models

    public class BankStatement
    {
        public Guid AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal TotalDeposits { get; set; }
        public decimal TotalWithdrawals { get; set; }
        public decimal ClosingBalance { get; set; }
        public List<BankTransaction> Transactions { get; set; } = new();
    }

    public class CheckReport
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalIssued { get; set; }
        public int TotalReceived { get; set; }
        public int CashedChecks { get; set; }
        public int BouncedChecks { get; set; }
        public int CancelledChecks { get; set; }
        public int PendingChecks { get; set; }
        public decimal TotalIssuedAmount { get; set; }
        public decimal TotalReceivedAmount { get; set; }
        public List<CheckSummary> CheckSummaries { get; set; } = new();
    }

    public class CheckSummary
    {
        public Guid CheckId { get; set; }
        public string CheckNumber { get; set; }
        public CheckType CheckType { get; set; }
        public decimal Amount { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public CheckStatus Status { get; set; }
        public string BankName { get; set; }
        public string PartyName { get; set; }
    }

    public class TreasuryReport
    {
        public Guid TreasuryId { get; set; }
        public string TreasuryName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal TotalDeposits { get; set; }
        public decimal TotalWithdrawals { get; set; }
        public decimal ClosingBalance { get; set; }
        public List<TreasuryTransactionSummary> Transactions { get; set; } = new();
    }

    public class TreasuryTransactionSummary
    {
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public string Description { get; set; }
        public string ReferenceNumber { get; set; }
    }

    public class DailyFinancialReport
    {
        public DateTime ReportDate { get; set; }
        public decimal TotalBankDeposits { get; set; }
        public decimal TotalBankWithdrawals { get; set; }
        public decimal TotalTreasuryDeposits { get; set; }
        public decimal TotalTreasuryWithdrawals { get; set; }
        public decimal TotalCustomerPayments { get; set; }
        public decimal TotalSupplierPayments { get; set; }
        public int ChecksIssued { get; set; }
        public int ChecksReceived { get; set; }
        public decimal ChecksIssuedAmount { get; set; }
        public decimal ChecksReceivedAmount { get; set; }
        public List<TransactionSummary> TransactionSummaries { get; set; } = new();
    }

    public class TransactionSummary
    {
        public DateTime Time { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public string ReferenceNumber { get; set; }
        public string Description { get; set; }
    }

    #endregion
}
