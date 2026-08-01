using FluentValidation;

namespace Restaurante.IO.Business.Models.Validations
{
    public class LogginValidation : AbstractValidator<LogginEntity>
    {
        public LogginValidation()
        {
            RuleFor(c => c.Message)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
                .Length(1, 6000).WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");

            RuleFor(c => c.LogLevel)
                .NotNull().WithMessage("O campo {PropertyName} precisa ser fornecido");
        }
    }
}