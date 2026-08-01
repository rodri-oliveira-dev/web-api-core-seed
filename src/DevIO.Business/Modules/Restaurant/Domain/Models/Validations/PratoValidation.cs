using FluentValidation;

namespace Restaurante.IO.Business.Models.Validations
{
    public class PratoValidation : AbstractValidator<Prato>
    {
        private const string MensagemCampoObrigatorio = "O campo {PropertyName} é obrigatório";

        public PratoValidation()
        {
            RuleFor(c => c.TipoPrato).NotNull().WithMessage(MensagemCampoObrigatorio);

            RuleFor(c => c.Titulo)
                .NotEmpty().WithMessage(MensagemCampoObrigatorio)
                .Length(2, 200).WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");

            RuleFor(c => c.Descricao)
                .NotEmpty().WithMessage(MensagemCampoObrigatorio)
                .Length(2, 800).WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");

            RuleFor(c => c.Foto)
                .NotEmpty().WithMessage(MensagemCampoObrigatorio)
                .Length(2, 200).WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");

            RuleFor(c => c.Preco)
                .NotEmpty().WithMessage(MensagemCampoObrigatorio)
                .GreaterThanOrEqualTo(0).WithMessage("O campo {PropertyName} precisa ter um valor");
        }
    }
}