using System.Collections.Generic;
using Restaurante.IO.Business.Models.Enums;

namespace Restaurante.IO.Business.Models
{
    public class Mesa : Entity
    {
        public Mesa()
        {
            Pedidos = new HashSet<Pedido>();
        }

        public string Numero { get; set; }
        public int Lugares { get; set; }
        public bool Ativo { get; set; }
        public ELocalizacaoMesa LocalizacaoMesa { get; set; }

        public virtual ICollection<Pedido> Pedidos { get; set; }
    }
}
