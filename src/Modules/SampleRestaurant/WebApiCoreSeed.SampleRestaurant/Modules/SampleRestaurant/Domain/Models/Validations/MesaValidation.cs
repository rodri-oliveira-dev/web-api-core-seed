using FluentValidation;

namespace WebApiCoreSeed.SampleRestaurant.Models.Validations
{
    public class MesaValidation : AbstractValidator<Mesa>
    {
        private const string MensagemCampoObrigatorio = "O campo {PropertyName} é obrigatório";

        public MesaValidation()
        {
            RuleFor(c => c.LocalizacaoMesa).NotNull().WithMessage(MensagemCampoObrigatorio);

            RuleFor(c => c.Numero)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage(MensagemCampoObrigatorio)
                .NotEmpty().WithMessage(MensagemCampoObrigatorio)
                .Length(1, 50).WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");

            RuleFor(c => c.Lugares)
                .InclusiveBetween(1, 40).WithMessage("A mesa deve possuir ao menos um lugar.");
        }
    }
}
