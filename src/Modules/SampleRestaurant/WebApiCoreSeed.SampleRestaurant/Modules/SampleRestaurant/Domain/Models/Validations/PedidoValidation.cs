using FluentValidation;

namespace WebApiCoreSeed.SampleRestaurant.Models.Validations
{
    public class PedidoValidation : AbstractValidator<Pedido>
    {
        private const string MensagemCampoObrigatorio = "O campo {PropertyName} é obrigatório";

        public PedidoValidation()
        {
            RuleFor(c => c.Mesa)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(MensagemCampoObrigatorio)
                .SetValidator(_ => new MesaValidation());

            RuleFor(c => c.Atendente)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(MensagemCampoObrigatorio)
                .SetValidator(_ => new AtendenteValidation());

            RuleFor(c => c.PedidoPrato)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage(MensagemCampoObrigatorio)
                .NotEmpty().WithMessage(MensagemCampoObrigatorio);
                //.Must(p => p.Count < 1).WithMessage("O pedido precisa ter ao menos um prato.");

            RuleForEach(c => c.PedidoPrato).SetValidator(new PedidoPratoValidation());

            RuleFor(c => c.Numero)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("A campo {PropertyName} precisa ser fornecido")
                .NotEmpty().WithMessage("A campo {PropertyName} precisa ser fornecido")
                .Length(1, 50).WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");
        }
    }
}
