using System;
using System.ComponentModel.DataAnnotations;
using Restaurante.IO.Business.Models.Enums;

namespace Restaurante.IO.Api.ViewModels
{
    public class PratoViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(200, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 2)]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(800, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 2)]
        public string Descricao { get; set; }

        [ScaffoldColumn(false)]
        public string FotoUpload { get; set; }

        public string Foto { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public double Preco { get; set; }

        public bool Ativo { get; set; }

        public ETipoPrato TipoPrato { get; set; }
    }
}