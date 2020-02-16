using Restaurante.IO.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Restaurante.IO.Business.Models.Enums;

namespace Restaurante.IO.Data.Mappings
{
    public class PedidoPratoMapping : IEntityTypeConfiguration<PedidoPrato>
    {
        public void Configure(EntityTypeBuilder<PedidoPrato> builder)
        {
            var converter = new EnumToNumberConverter<EStatusProducao, int>();

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Observacao)
                .HasColumnType("varchar(1000)");

            builder.Property(p => p.StatusProducao)
                .IsRequired()
                .HasColumnType("int")
                .HasConversion(converter);

            builder.HasOne(d => d.Pedido)
                .WithMany(p => p.PedidoPrato)
                .HasForeignKey(d => d.PedidoId)
                .HasConstraintName("FK_PedidoPrato_Pedidos");

            builder.HasOne(d => d.Prato)
                .WithMany(p => p.PedidoPrato)
                .HasForeignKey(d => d.PratoId)
                .HasConstraintName("FK_PedidoPrato_Pratos");

            builder.ToTable("PedidoPrato");
        }
    }
}