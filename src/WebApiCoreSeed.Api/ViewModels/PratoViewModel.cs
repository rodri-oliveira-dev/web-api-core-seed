using System.ComponentModel.DataAnnotations;
using WebApiCoreSeed.SampleRestaurant.Models.Enums;

namespace WebApiCoreSeed.Api.ViewModels
{
    public class PratoViewModel : MainViewModel
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(200, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 2)]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(800, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 2)]
        public string Descricao { get; set; } = string.Empty;

        [ScaffoldColumn(false)]
        public string? FotoUpload { get; set; }

        public string Foto { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public double Preco { get; set; }

        public bool Ativo { get; set; }

        public ETipoPrato TipoPrato { get; set; }
    }
}
