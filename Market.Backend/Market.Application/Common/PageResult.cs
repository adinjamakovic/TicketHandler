namespace Market.Application.Common;

public sealed class PageResult<T>
{
    public int Total { get; init; }
    public int TotalItems { get; init; }
    public int TotalPages { get; init; }
    public int PageSize { get; init; }
    public int CurrentPage { get; init; }
    public bool IncludedTotal { get; init; }
    public IReadOnlyList<T> Items { get; init; }

    /// <summary>
    /// Creates a PageResult from an IQueryable using EF Core asynchronous methods.
    /// </summary>
    public static async Task<PageResult<T>> FromQueryableAsync(
        IQueryable<T> query,
        PageRequest paging,
        CancellationToken ct = default,
        bool includeTotal = true)
    {
        int total = 0;
        if (includeTotal)
            total = await query.CountAsync(ct);

        var items = await query
            .Skip(paging.SkipCount)
            .Take(paging.PageSize)
            .ToListAsync(ct);

        return new PageResult<T>
        {
            Total = total,
            TotalItems = total,
            TotalPages = paging.PageSize > 0 ? (int)Math.Ceiling((double)total / paging.PageSize) : 1,
            PageSize = paging.PageSize,
            CurrentPage = paging.Page,
            IncludedTotal = includeTotal,
            Items = items,
        };
    }
}