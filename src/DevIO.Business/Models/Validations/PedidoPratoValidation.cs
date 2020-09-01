using FluentValidation;

namespace Restaurante.IO.Business.Models.Validations
{
    public class PedidoPratoValidation : AbstractValidator<PedidoPrato>
    {
        private const string MensagemCampoObrigatorio = "O campo {PropertyName} é obrigatório";

        public PedidoPratoValidation()
        {
            RuleFor(c => c.Prato)
                .NotNull().WithMessage(MensagemCampoObrigatorio);

            RuleFor(c => c.Observacao)
                .Must(c => string.IsNullOrWhiteSpace(c))
                .Length(1000).WithMessage("O campo {PropertyName} precisa ter no maximo {MaxLength} caracteres");
        }
    }
}