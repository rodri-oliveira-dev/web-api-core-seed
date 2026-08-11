using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApiCoreSeed.SampleRestaurant.Interfaces.Pagination
{
    public class PaginationParameter
    {
        public const int DefaultPageNumber = 1;
        public const int DefaultPageSize = 10;
        public const int MinPageSize = 1;
        public const int MaxPageSize = 50;

        /// <summary>
        /// Numero da pagina
        /// </summary>
        [DefaultValue(DefaultPageNumber)]
        [Range(DefaultPageNumber, int.MaxValue, ErrorMessage = "PageNumber deve ser maior ou igual a 1.")]
        public int PageNumber { get; set; } = DefaultPageNumber;

        /// <summary>
        /// Quantidade de itens por pagina
        /// </summary>
        [DefaultValue(DefaultPageSize)]
        [Range(MinPageSize, MaxPageSize, ErrorMessage = "PageSize deve estar entre 1 e 50.")]
        public int PageSize { get; set; } = DefaultPageSize;
    }
}
