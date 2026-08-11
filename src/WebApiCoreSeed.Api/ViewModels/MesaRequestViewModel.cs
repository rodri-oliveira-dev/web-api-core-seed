using System.ComponentModel.DataAnnotations;
using WebApiCoreSeed.SampleRestaurant.Models.Enums;

namespace WebApiCoreSeed.Api.ViewModels
{
    public class MesaRequestViewModel : MainRequestViewModel
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(50, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 1)]
        public required string Numero { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Range(1, int.MaxValue)]
        public required int Lugares { get; set; }

        public bool? Ativo { get; set; }

        public ELocalizacaoMesa? LocalizacaoMesa { get; set; }
    }
}
