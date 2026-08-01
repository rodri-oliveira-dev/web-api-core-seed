using System.Collections.Generic;
using WebApiCoreSeed.SampleRestaurant.Models.Enums;

namespace WebApiCoreSeed.SampleRestaurant.Models
{
    public class Prato : Entity
    {
        public Prato()
        {
            PedidoPrato = new HashSet<PedidoPrato>();
        }

        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Foto { get; set; }
        public double Preco { get; set; }
        public bool Ativo { get; set; }
        public ETipoPrato TipoPrato { get; set; }

        public virtual ICollection<PedidoPrato> PedidoPrato { get; set; }
    }
}
