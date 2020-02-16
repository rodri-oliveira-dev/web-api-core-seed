using Restaurante.IO.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Restaurante.IO.Business.Models.Enums;

namespace Restaurante.IO.Data.Mappings
{
    public class PedidoMapping : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            var converter = new EnumToNumberConverter<ELocalizacaoMesa, int>();

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Numero)
                .IsRequired()
                .HasColumnType("varchar(50)");

            builder.Property(e => e.DataHoraCadastro)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            builder.Property(e => e.DataHoraEncerrado).HasColumnType("datetime");

            builder.HasOne(d => d.Atendente)
                .WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.AtendenteId)
                .HasConstraintName("FK_Pedidos_Atendentes");

            builder.HasOne(d => d.Mesa)
                .WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.MesaId)
                .HasConstraintName("FK_Pedidos_Mesas");

            builder.ToTable("Pedidos");
        }
    }
}