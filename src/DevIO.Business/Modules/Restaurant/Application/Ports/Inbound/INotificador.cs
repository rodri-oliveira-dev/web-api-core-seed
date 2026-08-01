using System.Collections.Generic;
using Restaurante.IO.Business.Notificacoes;

namespace Restaurante.IO.Business.Intefaces
{
    public interface INotificador
    {
        bool TemNotificacao();
        List<Notificacao> ObterNotificacoes();
        void Handle(Notificacao notificacao);
    }
}