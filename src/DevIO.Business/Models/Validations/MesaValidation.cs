using FluentValidation;

namespace Restaurante.IO.Business.Models.Validations
{
    public class MesaValidation : AbstractValidator<Mesa>
    {
        public MesaValidation()
        {
            RuleFor(c => c.Numero)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
                .Length(1, 50).WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");

            RuleFor(c => c.Lugares)
                .LessThanOrEqualTo(0).WithMessage("O campo {PropertyName} precisa ser maior que zero");
        }
    }
}