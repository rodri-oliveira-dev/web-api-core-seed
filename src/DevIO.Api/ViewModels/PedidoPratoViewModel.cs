using System;
using System.ComponentModel.DataAnnotations;
using Restaurante.IO.Business.Models.Attributes;
using Restaurante.IO.Business.Models.Enums;

namespace Restaurante.IO.Api.ViewModels
{
    public class PedidoPratoViewModel
    {
        [Key]
        [NotEmpty]
        public Guid Id { get; set; }

        [NotEmpty]
        public Guid PedidoId { get; set; }

        [NotEmpty]
        public Guid PratoId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(1000, ErrorMessage = "O campo {0} precisa ter entre no maximo {1} caracteres")]
        public string Observacao { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public EStatusProducao StatusProducao { get; set; }

    }
}