using Restaurante.IO.Business.Models.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Restaurante.IO.Api.ViewModels
{
    public class PedidoViewModel: MainViewModel
    {
        [NotEmpty]
        public Guid AtendenteId { get; set; }

        [NotEmpty]
        public Guid MesaId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(50, ErrorMessage = "O campo {0} precisa ter entre no maximo {1} caracteres")]
        public string Numero { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public DateTime DataHoraCadastro { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public DateTime? DataHoraEncerrado { get; set; }

    }
}