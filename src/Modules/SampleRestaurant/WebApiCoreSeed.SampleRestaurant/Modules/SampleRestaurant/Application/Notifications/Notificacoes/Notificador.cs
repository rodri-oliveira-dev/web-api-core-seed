using System.Collections.Generic;
using WebApiCoreSeed.SampleRestaurant.Intefaces;

namespace WebApiCoreSeed.SampleRestaurant.Notificacoes
{
    public class Notificador : INotificador
    {
        private readonly List<Notificacao> _notificacoes = new();

        public void Handle(Notificacao notificacao)
        {
            _notificacoes.Add(notificacao);
        }

        public List<Notificacao> ObterNotificacoes()
        {
            return _notificacoes;
        }

        public bool TemNotificacao()
        {
            return _notificacoes.Count != 0;
        }
    }
}