using System.Linq.Expressions;
using GymApplication.Repository.Abstractions.Entity;
using GymApplication.Repository.Extension;
using GymApplication.Repository.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace GymApplication.Repository.Repository;

public class RepoBase<TEntity, TKey> : IRepoBase<TEntity, TKey> 
    where TEntity : Entity<TKey>
    where TKey : struct
{
    private readonly ApplicationDbContext _context;

    public RepoBase(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<TEntity> GetQueryable()
    {
        return _context.Set<TEntity>();
    }

    public Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, object>>[] includes)
    {
        var query = _context.Set<TEntity>()
            .AsQueryable()
            .IncludeMultiple(includes);
        
        return query.ToListAsync();
    }

    public Task<List<TEntity>> GetByConditionsAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>>[] includes)
    {
        var query = _context.Set<TEntity>()
            .AsQueryable()
            .IncludeMultiple(includes);
        
        return query.Where(predicate)
            .ToListAsync();
    }

    public Task<TEntity?> GetByIdAsync(TKey id, Expression<Func<TEntity, object>>[] includes)
    {
        var query = _context.Set<TEntity>()
            .AsQueryable()
            .IncludeMultiple(includes);
        
        return query.FirstOrDefaultAsync(x => x.Id.Equals(id));
    }

    public void Add(TEntity entity)
    {
        _context.Set<TEntity>().Add(entity);
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        _context.Set<TEntity>().AddRange(entities);
    }

    public void Update(TEntity entity)
    {
        var entry = _context.Entry(entity);
        entry.State = EntityState.Modified;
    }

    public void Delete(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
    }
}