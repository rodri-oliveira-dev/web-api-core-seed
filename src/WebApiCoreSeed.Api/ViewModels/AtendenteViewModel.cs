using System;
using System.ComponentModel.DataAnnotations;
using WebApiCoreSeed.SampleRestaurant.Models.Attributes;
using WebApiCoreSeed.SampleRestaurant.Models.Enums;

namespace WebApiCoreSeed.Api.ViewModels
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