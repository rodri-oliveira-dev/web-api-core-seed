using System;
using System.Collections.Generic;

namespace Restaurante.IO.Business.Models
{
    public class Pedido : Entity
    {
        public Pedido()
        {
            PedidoPrato = new HashSet<PedidoPrato>();
        }

        public Guid AtendenteId { get; set; }
        public Guid MesaId { get; set; }
        public string Numero { get; set; }
        public DateTime DataHoraCadastro { get; set; }
        public DateTime? DataHoraEncerrado { get; set; }

        public virtual Atendente Atendente { get; set; }
        public virtual Mesa Mesa { get; set; }
        public virtual ICollection<PedidoPrato> PedidoPrato { get; set; }
    }
}
