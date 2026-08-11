using System;
using WebApiCoreSeed.SampleRestaurant.Models.Enums;

namespace WebApiCoreSeed.SampleRestaurant.Application.Contracts.Queries
{
    public sealed class PratoListItem
    {
        public Guid Id { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public string Foto { get; set; } = string.Empty;

        public double Preco { get; set; }

        public bool Ativo { get; set; }

        public ETipoPrato TipoPrato { get; set; }
    }
}
