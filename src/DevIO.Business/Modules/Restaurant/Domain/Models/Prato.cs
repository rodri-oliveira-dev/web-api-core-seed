using System.Collections.Generic;
using Restaurante.IO.Business.Models.Enums;

namespace Restaurante.IO.Business.Models
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
