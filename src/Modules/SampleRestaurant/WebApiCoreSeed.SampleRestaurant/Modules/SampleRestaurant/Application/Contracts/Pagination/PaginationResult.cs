using System.Collections.Generic;

namespace WebApiCoreSeed.SampleRestaurant.Interfaces.Pagination
{
    public class PaginationResult<T> where T : class, new()
    {
        public List<T> Items { get; set; } = new List<T>();

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages { get; set; }

        public bool HasNextPage { get; set; }

        public bool HasPreviousPage { get; set; }
    }
}
