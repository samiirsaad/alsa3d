using AlSa3d.Core;
using AlSa3d.Core.DTOs;
using AlSa3d.Core.Entities;

namespace AlSa3d.Services.Interfaces;

/// <summary>
/// واجهة خدمة إدارة العملاء
/// </summary>
public interface ICustomerService
{
    /// <summary>
    /// الحصول على جميع العملاء
    /// </summary>
    /// <returns>قائمة بجميع العملاء النشطين</returns>
    Task<Result<IEnumerable<Customer>>> GetAllCustomersAsync();

    /// <summary>
    /// الحصول على عميل بواسطة معرفه
    /// </summary>
    /// <param name="id">معرف العميل</param>
    /// <returns>بيانات العميل</returns>
    Task<Result<Customer>> GetCustomerByIdAsync(int id);

    /// <summary>
    /// إنشاء عميل جديد
    /// </summary>
    /// <param name="dto">بيانات العميل الجديد</param>
    /// <returns>بيانات العميل المنشأ</returns>
    Task<Result<Customer>> CreateCustomerAsync(CreateCustomerDto dto);

    /// <summary>
    /// تحديث بيانات عميل
    /// </summary>
    /// <param name="id">معرف العميل</param>
    /// <param name="dto">البيانات الجديدة</param>
    /// <returns>بيانات العميل المحدثة</returns>
    Task<Result<Customer>> UpdateCustomerAsync(int id, UpdateCustomerDto dto);

    /// <summary>
    /// حذف عميل
    /// </summary>
    /// <param name="id">معرف العميل</param>
    /// <returns>نتيجة العملية</returns>
    Task<Result<bool>> DeleteCustomerAsync(int id);

    /// <summary>
    /// البحث عن العملاء
    /// </summary>
    /// <param name="searchTerm">مصطلح البحث (اسم، هاتف، بريد إلخ)</param>
    /// <returns>قائمة بنتائج البحث</returns>
    Task<Result<IEnumerable<Customer>>> SearchCustomersAsync(string searchTerm);

    /// <summary>
    /// الحصول على الرصيد المالي للعميل
    /// </summary>
    /// <param name="customerId">معرف العميل</param>
    /// <returns>الرصيد المالي</returns>
    Task<Result<decimal>> GetCustomerBalanceAsync(int customerId);

    /// <summary>
    /// الحصول على إحصائيات العميل
    /// </summary>
    /// <param name="customerId">معرف العميل</param>
    /// <returns>إحصائيات الفواتير والمشتريات</returns>
    Task<Result<CustomerStatisticsDto>> GetCustomerStatisticsAsync(int customerId);

    /// <summary>
    /// إضافة عنوان للعميل
    /// </summary>
    /// <param name="customerId">معرف العميل</param>
    /// <param name="dto">بيانات العنوان الجديد</param>
    /// <returns>بيانات العنوان المضاف</returns>
    Task<Result<Address>> AddAddressAsync(int customerId, CreateAddressDto dto);

    /// <summary>
    /// إضافة جهة اتصال للعميل
    /// </summary>
    /// <param name="customerId">معرف العميل</param>
    /// <param name="dto">بيانات جهة الاتصال الجديدة</param>
    /// <returns>بيانات جهة الاتصال المضافة</returns>
    Task<Result<Contact>> AddContactAsync(int customerId, CreateContactDto dto);
}
