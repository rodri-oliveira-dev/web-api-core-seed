using Restaurante.IO.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Restaurante.IO.Business.Models.Enums;

namespace Restaurante.IO.Data.Mappings
{
    public class PratoMapping : IEntityTypeConfiguration<Prato>
    {
        public void Configure(EntityTypeBuilder<Prato> builder)
        {
            var converter = new EnumToNumberConverter<ETipoPrato, int>();

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Titulo)
                .IsRequired()
                .HasColumnType("varchar(200)");

            builder.Property(p => p.Descricao)
                .IsRequired()
                .HasColumnType("varchar(800)");

            builder.Property(p => p.Foto)
                .IsRequired()
                .HasColumnType("varchar(200)");

            builder.Property(p => p.Preco)
                .IsRequired()
                .HasColumnType("float");

            builder.Property(p => p.Ativo)
                .HasDefaultValue(1)
                .HasColumnType("bit");

            builder.Property(p => p.TipoPrato)
                .IsRequired()
                .HasColumnType("int")
                .HasConversion(converter);

            builder.ToTable("Pratos");
        }
    }
}