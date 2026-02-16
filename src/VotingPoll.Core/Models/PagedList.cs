namespace VotingPoll.Core.Models;

public class PagedList<T>
{
    public PagedList(IEnumerable<T> items, int count, int? pageNumber, int? pageSize)
    {
        Items.AddRange(items);
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        if (pageSize != null) TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }

    public List<T> Items { get; set; } = [];

    public int? CurrentPage { get; private set; }

    public int TotalPages { get; private set; }

    public int? PageSize { get; private set; }

    public int TotalCount { get; private set; }

    public bool HasPreviousPage => CurrentPage > 1;

    public bool HasNextPage => CurrentPage < TotalPages;
}