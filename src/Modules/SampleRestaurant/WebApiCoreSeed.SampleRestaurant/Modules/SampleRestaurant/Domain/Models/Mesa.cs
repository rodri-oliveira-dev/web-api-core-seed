using System.Collections.Generic;
using WebApiCoreSeed.SampleRestaurant.Models.Enums;

namespace WebApiCoreSeed.SampleRestaurant.Models
{
    public class Mesa : Entity
    {
        public Mesa()
        {
            Pedidos = new HashSet<Pedido>();
        }

        public string Numero { get; set; } = string.Empty;
        public int Lugares { get; set; }
        public bool Ativo { get; set; }
        public ELocalizacaoMesa LocalizacaoMesa { get; set; }

        public virtual ICollection<Pedido> Pedidos { get; set; }
    }
}
