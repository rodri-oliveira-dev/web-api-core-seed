using Restaurante.IO.Business.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Restaurante.IO.Api.ViewModels
{
    public class MesaViewModel: MainViewModel
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(50, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 1)]
        public string Numero { get; set; }

        [Range(1, int.MaxValue)]
        public int Lugares { get; set; }

        public bool Ativo { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public ELocalizacaoMesa LocalizacaoMesa { get; set; }

    }
}