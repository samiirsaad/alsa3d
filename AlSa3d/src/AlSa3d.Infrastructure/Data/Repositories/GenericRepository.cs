using Microsoft.EntityFrameworkCore;
using AlSa3d.Core;
using AlSa3d.Core.Interfaces;
using AlSa3d.Core.Common;
using System.Linq.Expressions;

namespace AlSa3d.Infrastructure.Data.Repositories;

public class GenericRepository<T> : IGenericRepository<T>, IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        foreach (var include in includes)
            query = query.Include(include);
        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }

    public virtual async Task<T?> GetAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        foreach (var include in includes)
            query = query.Include(include);
        return await query.FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.Where(x => !x.IsDeleted).ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet.Where(x => !x.IsDeleted);
        foreach (var include in includes)
            query = query.Include(include);
        return await query.ToListAsync();
    }

    public virtual async Task<Result<T>> AddAsync(T entity)
    {
        entity.CreatedAt = DateTime.Now;
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return Result<T>.SuccessResult(entity);
    }

    public virtual async Task<Result<T>> UpdateAsync(T entity)
    {
        var existing = await _dbSet.FindAsync(entity.Id);
        if (existing != null)
        {
            entity.UpdatedAt = DateTime.Now;
            _context.Entry(existing).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return Result<T>.SuccessResult(entity);
        }
        return Result<T>.Failure("الكيان غير موجود");
    }

    public virtual async Task DeleteAsync(T entity)
    {
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
    }

    public virtual async Task<Result<bool>> DeleteAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity == null)
            return Result<bool>.Failure("الكيان غير موجود");
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return Result<bool>.SuccessResult(true);
    }

    public virtual async Task<bool> ExistsAsync(int id)
    {
        return await _dbSet.AnyAsync(e => e.Id == id && !e.IsDeleted);
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        if (predicate == null)
            return await _dbSet.CountAsync();
        return await _dbSet.CountAsync(predicate);
    }

    public virtual IQueryable<T> Query()
    {
        return _dbSet.Where(x => !x.IsDeleted).AsQueryable();
    }
}
