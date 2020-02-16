using Restaurante.IO.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Restaurante.IO.Business.Models.Enums;

namespace Restaurante.IO.Data.Mappings
{
    public class MesaMapping : IEntityTypeConfiguration<Mesa>
    {
        public void Configure(EntityTypeBuilder<Mesa> builder)
        {
            var converter = new EnumToNumberConverter<ELocalizacaoMesa, int>();

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Numero)
                .IsRequired()
                .HasColumnType("varchar(50)");

            builder.Property(p => p.Ativo)
                .HasDefaultValue(1)
                .HasColumnType("bit");

            builder.Property(p => p.Lugares)
                .HasColumnType("int")
                .HasDefaultValue(4);

            builder.Property(p => p.LocalizacaoMesa)
                .IsRequired()
                .HasColumnType("int")
                .HasConversion(converter);

            builder.ToTable("Mesas");
        }
    }
}