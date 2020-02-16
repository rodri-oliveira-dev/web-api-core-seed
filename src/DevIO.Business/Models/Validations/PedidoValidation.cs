using FluentValidation;

namespace Restaurante.IO.Business.Models.Validations
{
    public class PedidoValidation : AbstractValidator<Pedido>
    {
        public PedidoValidation()
        {
            RuleFor(c => c.Mesa)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido");

            RuleFor(c => c.Atendente)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido");

            RuleFor(c => c.PedidoPrato)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
                .Must(p => p.Count == 0).WithMessage("O campo {PropertyName} precisa ser fornecido");

            RuleFor(c => c.Numero)
                .NotNull().WithMessage("A campo {PropertyName} precisa ser fornecido")
                .NotEmpty().WithMessage("A campo {PropertyName} precisa ser fornecido")
                .Length(2, 50).WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");
        }
    }
}