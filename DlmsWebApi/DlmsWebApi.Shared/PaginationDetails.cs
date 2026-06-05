namespace DlmsWebApi.Shared;

public class PaginationDetails
{
    
}

// PaginationParams.cs
public record PaginationParams
{
    private const int MaxPageSize = 50;
    private int _pageSize = 10;

    public int Page { get; init; } = 1;
    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}

// PagedResult.cs
public record PagedResult<T>
{
    public IEnumerable<T> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}