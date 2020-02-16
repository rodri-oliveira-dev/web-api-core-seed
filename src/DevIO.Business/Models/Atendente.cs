using System.Collections.Generic;
using Restaurante.IO.Business.Models.Enums;

namespace Restaurante.IO.Business.Models
{
    public class Atendente : Entity
    {
        public Atendente()
        {
            Pedidos = new HashSet<Pedido>();
        }

        public string Nome { get; set; }
        public ETipoAtendente TipoAtendente { get; set; }

        public virtual ICollection<Pedido> Pedidos { get; set; }
    }
}
