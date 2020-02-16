using System.Collections.Generic;

namespace Restaurante.IO.Business.Intefaces.Pagination
{
    public class PaginationResult<T> where T : class, new()
    {
        public int PageNumber { get; set; }

        public int TotalPages { get; set; }

        public int TotalItens { get; set; }

        public List<T> Data { get; set; }
    }
}