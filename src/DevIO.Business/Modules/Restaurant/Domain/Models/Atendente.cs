using Restaurante.IO.Business.Models.Core;
using Restaurante.IO.Business.Models.Enums;
using System.Collections.Generic;

namespace Restaurante.IO.Business.Models
{
    public class Atendente : Entity
    {
        public Atendente()
        {
            Pedidos = new HashSet<Pedido>();
        }

        public string Nome { get; set; }

        public string Email { get; set; }

        public Telefone Telefone { get; set; }

        public ETipoAtendente TipoAtendente { get; set; }

        public virtual ICollection<Pedido> Pedidos { get; set; }
    }
}
