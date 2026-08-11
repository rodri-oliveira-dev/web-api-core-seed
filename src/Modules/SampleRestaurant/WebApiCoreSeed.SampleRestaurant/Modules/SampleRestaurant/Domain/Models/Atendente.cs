using WebApiCoreSeed.SampleRestaurant.Models.Core;
using WebApiCoreSeed.SampleRestaurant.Models.Enums;
using System.Collections.Generic;

namespace WebApiCoreSeed.SampleRestaurant.Models
{
    public class Atendente : Entity
    {
        public Atendente()
        {
            Pedidos = new HashSet<Pedido>();
        }

        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public Telefone Telefone { get; set; } = null!;

        public ETipoAtendente TipoAtendente { get; set; }

        public virtual ICollection<Pedido> Pedidos { get; set; }
    }
}
