namespace BlogApi.DTOs
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }

        public PagedResult(List<T> items, int totalCount, int pageNumber = 1, int pageSize = 10)
        {
            Items = items;
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;

            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            HasNextPage = pageNumber < TotalPages;
            HasPreviousPage = pageNumber > 1;
        }
    }
}
