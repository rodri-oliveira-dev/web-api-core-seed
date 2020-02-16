using System;
using System.ComponentModel.DataAnnotations;
using Restaurante.IO.Business.Models.Attributes;
using Restaurante.IO.Business.Models.Enums;

namespace Restaurante.IO.Api.ViewModels
{
    public class AtendenteViewModel
    {
        [Key]
        [NotEmpty]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(100, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 2)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public ETipoAtendente TipoAtendente { get; set; }

    }
}