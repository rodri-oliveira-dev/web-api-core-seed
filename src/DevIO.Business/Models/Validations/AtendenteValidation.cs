using FluentValidation;

namespace Restaurante.IO.Business.Models.Validations
{
    public class AtendenteValidation : AbstractValidator<Atendente>
    {
        public AtendenteValidation()
        {
            RuleFor(c => c.Nome)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
                .Length(2, 100).WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");

        }
    }
}