using System.ComponentModel.DataAnnotations;
using WebApiCoreSeed.SampleRestaurant.Models.Enums;

namespace WebApiCoreSeed.Api.ViewModels
{
    public class PratoRequestViewModel : MainRequestViewModel
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(200, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 2)]
        public required string Titulo { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(800, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 2)]
        public required string Descricao { get; set; }

        [ScaffoldColumn(false)]
        public string? FotoUpload { get; set; }

        public string? Foto { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public required double Preco { get; set; }

        public bool? Ativo { get; set; }

        public ETipoPrato? TipoPrato { get; set; }
    }
}
