using FluentValidation;

namespace Restaurante.IO.Business.Models.Validations
{
    public class PratoValidation : AbstractValidator<Prato>
    {
        public PratoValidation()
        {
            RuleFor(c => c.Titulo)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
                .Length(2, 200).WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");

            RuleFor(c => c.Descricao)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
                .Length(2, 800).WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");

            RuleFor(c => c.Foto)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
                .Length(2, 200).WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");

            RuleFor(c => c.Preco)
                .NotEmpty().WithMessage("A campo {PropertyName} precisa ser fornecido")
                .GreaterThanOrEqualTo(0).WithMessage("O campo {PropertyName} precisa ter um valor");
        }
    }
}