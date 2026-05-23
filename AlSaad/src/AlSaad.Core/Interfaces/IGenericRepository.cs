using System;
using System.Linq;
using System.Threading.Tasks;

namespace AlSaad.Core.Interfaces;

/// <summary>
/// واجهة المستودع العام
/// </summary>
public interface IGenericRepository<T> where T : class
{
    /// <summary>
    /// الحصول على كل الكيانات
    /// </summary>
    Task<IQueryable<T>> GetAllAsync();
    
    /// <summary>
    /// الحصول على كيان حسب المعرف
    /// </summary>
    Task<T?> GetByIdAsync(int id);
    
    /// <summary>
    /// الحصول على كيان حسب المعرف (GUID)
    /// </summary>
    Task<T?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// إضافة كيان جديد
    /// </summary>
    Task<T> AddAsync(T entity);
    
    /// <summary>
    /// إضافة مجموعة من الكيانات
    /// </summary>
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
    
    /// <summary>
    /// تحديث كيان موجود
    /// </summary>
    void Update(T entity);
    
    /// <summary>
    /// تحديث مجموعة من الكيانات
    /// </summary>
    void UpdateRange(IEnumerable<T> entities);
    
    /// <summary>
    /// حذف كيان
    /// </summary>
    void Delete(T entity);
    
    /// <summary>
    /// حذف مجموعة من الكيانات
    /// </summary>
    void DeleteRange(IEnumerable<T> entities);
    
    /// <summary>
    /// البحث بشرط مخصص
    /// </summary>
    IQueryable<T> Find(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
    
    /// <summary>
    /// البحث عن أول عنصر يطابق الشرط
    /// </summary>
    Task<T?> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
    
    /// <summary>
    /// التحقق من وجود عنصر يطابق الشرط
    /// </summary>
    Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
    
    /// <summary>
    /// عدد العناصر التي تطابق الشرط
    /// </summary>
    Task<int> CountAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
    
    /// <summary>
    /// حفظ التغييرات
    /// </summary>
    Task<int> SaveChangesAsync();
}
