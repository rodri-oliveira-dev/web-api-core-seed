using FluentValidation;

namespace Restaurante.IO.Business.Models.Validations
{
    public class PedidoPratoValidation : AbstractValidator<PedidoPrato>
    {
        public PedidoPratoValidation()
        {
            RuleFor(c => c.Pedido)
                .NotNull().WithMessage("O campo {PropertyName} precisa ser fornecido");

            RuleFor(c => c.Prato)
                .NotNull().WithMessage("O campo {PropertyName} precisa ser fornecido");

            RuleFor(c => c.Observacao)
                .Length(1000).WithMessage("O campo {PropertyName} precisa ter no maximo {MaxLength} caracteres");
        }
    }
}