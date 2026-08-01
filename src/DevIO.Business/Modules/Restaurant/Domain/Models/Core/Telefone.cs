using Restaurante.IO.Business.Models.Enums;

namespace Restaurante.IO.Business.Models.Core
{
    public class Telefone
    {
        public int Ddd { get; set; }

        public int Numero { get; set; }

        public ETipoTelefone TipoTelefone { get; set; }
    }
}
