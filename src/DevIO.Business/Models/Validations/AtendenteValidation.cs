using FluentValidation;
using Restaurante.IO.Business.Models.Enums;

namespace Restaurante.IO.Business.Models.Validations
{
    public class AtendenteValidation : AbstractValidator<Atendente>
    {
        private const string MensagemCampoObrigatorio = "O campo {PropertyName} é obrigatório";

        public AtendenteValidation()
        {
            RuleFor(c => c.TipoAtendente)
                .NotNull().WithMessage(MensagemCampoObrigatorio);

            When(a => a.TipoAtendente == ETipoAtendente.Garcom, () =>
            {
                RuleFor(c => c.Nome)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotEmpty().WithMessage(MensagemCampoObrigatorio)
                    .NotNull().WithMessage(MensagemCampoObrigatorio)
                    .Length(2, 100).WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");

                RuleFor(c => c.Email)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotEmpty().WithMessage(MensagemCampoObrigatorio)
                    .NotNull().WithMessage(MensagemCampoObrigatorio)
                    .EmailAddress().WithMessage("E-mail envalido");

                RuleFor(c => c.Telefone)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotNull().WithMessage(MensagemCampoObrigatorio)
                    .SetValidator(new TelefoneValidation());
            });

            When(a => a.TipoAtendente == ETipoAtendente.Totem, () =>
            {
                RuleFor(c => c.Nome)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotEmpty().WithMessage(MensagemCampoObrigatorio)
                    .Length(2, 100).WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");
            });


        }
    }
}