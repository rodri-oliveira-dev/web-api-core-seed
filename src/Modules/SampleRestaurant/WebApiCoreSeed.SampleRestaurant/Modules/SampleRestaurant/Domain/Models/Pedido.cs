using System;
using System.Collections.Generic;

namespace WebApiCoreSeed.SampleRestaurant.Models
{
    public class Pedido : Entity
    {
        public Pedido()
        {
            PedidoPrato = new HashSet<PedidoPrato>();
        }

        public Guid AtendenteId { get; set; }
        public Guid MesaId { get; set; }
        public string Numero { get; set; } = string.Empty;
        public DateTime DataHoraCadastro { get; set; }
        public DateTime? DataHoraEncerrado { get; set; }

        public virtual Atendente Atendente { get; set; } = null!;
        public virtual Mesa Mesa { get; set; } = null!;
        public virtual ICollection<PedidoPrato> PedidoPrato { get; set; }
    }
}
