using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.SampleRestaurant.Infrastructure.Mappings
{
    public class PedidoMapping : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
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