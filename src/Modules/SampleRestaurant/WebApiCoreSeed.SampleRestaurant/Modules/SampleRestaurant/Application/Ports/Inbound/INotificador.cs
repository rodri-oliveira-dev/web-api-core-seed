using System.Collections.Generic;
using WebApiCoreSeed.SampleRestaurant.Notificacoes;

namespace WebApiCoreSeed.SampleRestaurant.Intefaces
{
    public interface INotificador
    {
        bool TemNotificacao();
        List<Notificacao> ObterNotificacoes();
        void Handle(Notificacao notificacao);
    }
}