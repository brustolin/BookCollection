using Microsoft.EntityFrameworkCore;

namespace BookTracker.Models;

public class PaginatedResponse<T> 
{
    public required List<T> Items { get; init; }
    public int TotalCount { get; init; }
    public int PageSize { get; init; }
    public int PageNumber { get; init; }

    

    public static async Task<PaginatedResponse<T>> PaginateQueryAndMapAsync<TObject>(
        IQueryable<TObject> query,
        int pageNumber,
        int pageSize,
        Func<TObject, T> mapFunc,
        CancellationToken cancellationToken = default)
            where TObject : class
    {
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<T>
        {
            TotalCount = totalCount,
            Items = items.Select(mapFunc).ToList(),
            PageSize = pageSize,
            PageNumber = pageNumber
        };
    }
}
