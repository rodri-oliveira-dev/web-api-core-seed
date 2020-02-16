using Restaurante.IO.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Restaurante.IO.Business.Models.Enums;

namespace Restaurante.IO.Data.Mappings
{
    public class AtendenteMapping : IEntityTypeConfiguration<Atendente>
    {
        public void Configure(EntityTypeBuilder<Atendente> builder)
        {
            var converter = new EnumToNumberConverter<ETipoAtendente, int>();

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nome)
                .IsRequired()
                .HasColumnType("varchar(100)");

            builder.Property(p => p.TipoAtendente)
                .IsRequired()
                .HasColumnType("int")
                .HasConversion(converter);

            builder.ToTable("Atendentes");
        }
    }
}