using System;
using WebApiCoreSeed.SampleRestaurant.Models.Enums;

namespace WebApiCoreSeed.SampleRestaurant.Models
{
    public class PedidoPrato : Entity
    {
        public Guid PedidoId { get; set; }
        public Guid PratoId { get; set; }
        public EStatusProducao StatusProducao { get; set; }
        public string Observacao { get; set; }

        public virtual Pedido Pedido { get; set; }
        public virtual Prato Prato { get; set; }
    }
}
