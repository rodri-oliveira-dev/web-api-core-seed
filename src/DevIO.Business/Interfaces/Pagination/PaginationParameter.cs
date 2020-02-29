using System.ComponentModel;

namespace Restaurante.IO.Business.Interfaces.Pagination
{
    public class PaginationParameter
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;

        /// <summary>
        /// Numero da pagina
        /// </summary>
        [DefaultValue(1)]
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Quantidade de itens por pagina
        /// </summary>
        [DefaultValue(10)]
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
            }
        }
    }
}