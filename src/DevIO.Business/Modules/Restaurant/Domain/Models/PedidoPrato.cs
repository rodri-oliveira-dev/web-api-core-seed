using System;
using Restaurante.IO.Business.Models.Enums;

namespace Restaurante.IO.Business.Models
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
