using FluentValidation;

namespace Restaurante.IO.Business.Models.Validations
{
    public class PedidoValidation : AbstractValidator<Pedido>
    {
        private const string MensagemCampoObrigatorio = "O campo {PropertyName} é obrigatório";

        public PedidoValidation()
        {
            RuleFor(c => c.Mesa)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage(MensagemCampoObrigatorio)
                .SetValidator(new MesaValidation());

            RuleFor(c => c.Atendente)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage(MensagemCampoObrigatorio)
                .SetValidator(new AtendenteValidation());

            RuleFor(c => c.PedidoPrato)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage(MensagemCampoObrigatorio)
                .NotEmpty().WithMessage(MensagemCampoObrigatorio);
                //.Must(p => p.Count < 1).WithMessage("O pedido precisa ter ao menos um prato.");

            RuleForEach(c => c.PedidoPrato).SetValidator(new PedidoPratoValidation());

            RuleFor(c => c.Numero)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("A campo {PropertyName} precisa ser fornecido")
                .NotEmpty().WithMessage("A campo {PropertyName} precisa ser fornecido")
                .Length(1, 50).WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");
        }
    }
}