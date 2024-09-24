using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

namespace GymApplication.Shared.Common;

public class PagedResult<TEntity>
    where TEntity : notnull
{
    private const int UpperPageSize = 100;
    private const int DefaultPageSize = 10;
    private const int DefaultPageIndex = 1;
    
    private PagedResult(ReadOnlyCollection<TEntity> items, int pageIndex, int pageSize, int totalCount)
    {
        Items = items;
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
    
    public ReadOnlyCollection<TEntity> Items { get; }
    public int PageIndex { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public bool HasNextPage => PageIndex * PageSize < TotalCount;
    public bool HasPreviousPage => PageIndex > 1;
    
    public static async Task<PagedResult<TEntity>> CreateAsync(IQueryable<TEntity> query, int pageIndex, int pageSize)
    {
        pageIndex = pageIndex <= 0 ? DefaultPageIndex : pageIndex;
        pageSize = pageSize <= 0
            ? DefaultPageSize
            : pageSize > UpperPageSize
                ? UpperPageSize : pageSize;
        
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return new PagedResult<TEntity>(items.AsReadOnly(), pageIndex, pageSize, totalCount);
    }
    
    public static PagedResult<TEntity> Create(List<TEntity> items, int pageIndex, int pageSize, int totalCount)
        => new(items.AsReadOnly(), pageIndex, pageSize, totalCount);
}