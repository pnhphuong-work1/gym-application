using System.Linq.Expressions;

namespace GymApplication.Repository.Repository.Abstraction;

public interface IRepoBase<TEntity, in TKey>
{
    IQueryable<TEntity> GetQueryable();
    
    Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, object>>[] includes);
    Task<List<TEntity>> GetByConditionsAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>>[] includes);
    Task<TEntity?> GetByIdAsync(TKey id, Expression<Func<TEntity, object>>[] includes);
    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}