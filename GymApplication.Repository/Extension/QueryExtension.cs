using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace GymApplication.Repository.Extension;

public static class QueryExtension
{
    public static IQueryable<TEntity> IncludeMultiple<TEntity>(this IQueryable<TEntity> query, params Expression<Func<TEntity, object>>[] includes) 
        where TEntity : class
    {
        if (includes.Length > 0)
        {
            query = includes.Aggregate(query, (current, include) => current.Include(include));
        }

        return query;
    }
}