using WebApiCoreSeed.SampleRestaurant.Models.Enums;

namespace WebApiCoreSeed.SampleRestaurant.Models.Core
{
    public class Telefone
    {
        public int Ddd { get; set; }

        public int Numero { get; set; }

        public ETipoTelefone TipoTelefone { get; set; }
    }
}
