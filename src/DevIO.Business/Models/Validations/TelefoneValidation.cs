using FluentValidation;
using Restaurante.IO.Business.Models.Core;

namespace Restaurante.IO.Business.Models.Validations
{
    public class TelefoneValidation : AbstractValidator<Telefone>
    {
        private const int MenorDdd = 11;
        private const int MaiorDdd = 99;
        private const string MensagemCampoObrigatorio = "O campo {PropertyName} é obrigatório";

        public TelefoneValidation()
        {
            RuleFor(t => t.Ddd).ExclusiveBetween(MenorDdd, MaiorDdd);
            RuleFor(t => t.Numero).GreaterThan(11111111);
            RuleFor(t => t.TipoTelefone)
                .NotNull().WithMessage(MensagemCampoObrigatorio);
        }
    }
}