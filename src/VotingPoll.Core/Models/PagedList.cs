namespace VotingPoll.Core.Models;

public class PagedList<T>
{
        public PagedList(IEnumerable<T> items, int totalCount, int? currentPage, int?
            pageSize)
        {
            Items.AddRange(items);
            TotalCount = totalCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
            if (pageSize != null) TotalPages = (int)Math.Ceiling(totalCount /
                                                                 (double)pageSize);
        }

        public List<T> Items { get; set; } = [];
        public int? CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int? PageSize { get; private set; }
        public int TotalCount { get; private set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }