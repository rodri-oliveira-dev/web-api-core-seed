using System;
using WebApiCoreSeed.SampleRestaurant.Models.Enums;

namespace WebApiCoreSeed.SampleRestaurant.Application.Contracts.Queries
{
    public sealed class PratoListItem
    {
        public Guid Id { get; set; }

        public string Titulo { get; set; }

        public string Descricao { get; set; }

        public string Foto { get; set; }

        public double Preco { get; set; }

        public bool Ativo { get; set; }

        public ETipoPrato TipoPrato { get; set; }
    }
}
