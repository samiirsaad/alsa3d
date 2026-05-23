using AlSa3d.Core.Entities;
using AlSa3d.Core.Entities.CustomerModule;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlSa3d.Services.Interfaces
{
    /// <summary>
    /// واجهة خدمة العملاء - المسؤولة عن إدارة بيانات العملاء والعناوين
    /// </summary>
    public interface ICustomerService
    {
        #region Customer Operations

        /// <summary>
        /// إنشاء عميل جديد
        /// </summary>
        Task<Result<Customer>> CreateCustomerAsync(Customer customer);

        /// <summary>
        /// تحديث بيانات عميل
        /// </summary>
        Task<Result<Customer>> UpdateCustomerAsync(Customer customer);

        /// <summary>
        /// حذف عميل (Soft Delete)
        /// </summary>
        Task<Result<bool>> DeleteCustomerAsync(Guid customerId);

        /// <summary>
        /// الحصول على عميل بالمعرف
        /// </summary>
        Task<Result<Customer>> GetCustomerByIdAsync(Guid customerId);

        /// <summary>
        /// الحصول على جميع العملاء
        /// </summary>
        Task<Result<List<Customer>>> GetAllCustomersAsync();

        /// <summary>
        /// البحث عن عميل بالاسم
        /// </summary>
        Task<Result<List<Customer>>> SearchByNameAsync(string name);

        /// <summary>
        /// البحث عن عميل برقم الهاتف
        /// </summary>
        Task<Result<List<Customer>>> SearchByPhoneAsync(string phone);

        /// <summary>
        /// البحث عن عميل بالبريد الإلكتروني
        /// </summary>
        Task<Result<List<Customer>>> SearchByEmailAsync(string email);

        /// <summary>
        /// الحصول على عميل بالكود
        /// </summary>
        Task<Result<Customer>> GetCustomerByCodeAsync(string customerCode);

        /// <summary>
        /// تفعيل/تعطيل عميل
        /// </summary>
        Task<Result<bool>> ToggleCustomerStatusAsync(Guid customerId, bool isActive);

        #endregion

        #region Address Operations

        /// <summary>
        /// إضافة عنوان للعميل
        /// </summary>
        Task<Result<Address>> AddAddressAsync(Guid customerId, Address address);

        /// <summary>
        /// تحديث عنوان
        /// </summary>
        Task<Result<Address>> UpdateAddressAsync(Guid addressId, Address address);

        /// <summary>
        /// حذف عنوان
        /// </summary>
        Task<Result<bool>> DeleteAddressAsync(Guid addressId);

        /// <summary>
        /// الحصول على عناوين العميل
        /// </summary>
        Task<Result<List<Address>>> GetCustomerAddressesAsync(Guid customerId);

        /// <summary>
        /// تعيين العنوان الأساسي
        /// </summary>
        Task<Result<bool>> SetDefaultAddressAsync(Guid customerId, Guid addressId);

        #endregion

        #region Contact Person Operations

        /// <summary>
        /// إضافة جهة اتصال للعميل
        /// </summary>
        Task<Result<ContactPerson>> AddContactPersonAsync(Guid customerId, ContactPerson contact);

        /// <summary>
        /// تحديث جهة اتصال
        /// </summary>
        Task<Result<ContactPerson>> UpdateContactPersonAsync(Guid contactId, ContactPerson contact);

        /// <summary>
        /// حذف جهة اتصال
        /// </summary>
        Task<Result<bool>> DeleteContactPersonAsync(Guid contactId);

        /// <summary>
        /// الحصول على جهات اتصال العميل
        /// </summary>
        Task<Result<List<ContactPerson>>> GetCustomerContactsAsync(Guid customerId);

        #endregion

        #region Financial Operations

        /// <summary>
        /// الحصول على رصيد العميل
        /// </summary>
        Task<Result<decimal>> GetCustomerBalanceAsync(Guid customerId);

        /// <summary>
        /// الحصول على حد الائتمان
        /// </summary>
        Task<Result<decimal>> GetCreditLimitAsync(Guid customerId);

        /// <summary>
        /// تحديث حد الائتمان
        /// </summary>
        Task<Result<bool>> UpdateCreditLimitAsync(Guid customerId, decimal newLimit);

        /// <summary>
        /// التحقق من أهلية الائتمان
        /// </summary>
        Task<Result<bool>> CheckCreditEligibilityAsync(Guid customerId, decimal amount);

        #endregion

        #region Reports

        /// <summary>
        /// تقرير تفصيلي للعميل
        /// </summary>
        Task<Result<CustomerDetailedReport>> GetCustomerDetailedReportAsync(Guid customerId);

        /// <summary>
        /// تقرير العملاء حسب المنطقة
        /// </summary>
        Task<Result<List<CustomerRegionReport>>> GetCustomersByRegionAsync(string region);

        #endregion
    }

    #region Report Models

    public class CustomerDetailedReport
    {
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerType { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal AvailableCredit { get; set; }
        public int TotalInvoices { get; set; }
        public decimal TotalPurchases { get; set; }
        public decimal TotalPaid { get; set; }
        public DateTime LastInvoiceDate { get; set; }
        public List<Address> Addresses { get; set; } = new();
        public List<ContactPerson> Contacts { get; set; } = new();
        public List<TransactionSummary> RecentTransactions { get; set; } = new();
    }

    public class CustomerRegionReport
    {
        public string Region { get; set; }
        public int CustomerCount { get; set; }
        public decimal TotalSales { get; set; }
        public List<CustomerSummary> Customers { get; set; } = new();
    }

    public class CustomerSummary
    {
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public decimal Balance { get; set; }
        public string City { get; set; }
    }

    public class TransactionSummary
    {
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string ReferenceNumber { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
    }

    #endregion
}
