using System;
using System.ComponentModel.DataAnnotations;
using WebApiCoreSeed.SampleRestaurant.Models.Attributes;
using WebApiCoreSeed.SampleRestaurant.Models.Enums;

namespace WebApiCoreSeed.Api.ViewModels
{
    public class PedidoPratoViewModel : MainViewModel
    {
        [NotEmpty]
        public Guid PedidoId { get; set; }

        [NotEmpty]
        public Guid PratoId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(1000, ErrorMessage = "O campo {0} precisa ter entre no maximo {1} caracteres")]
        public string Observacao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public EStatusProducao StatusProducao { get; set; }

    }
}
