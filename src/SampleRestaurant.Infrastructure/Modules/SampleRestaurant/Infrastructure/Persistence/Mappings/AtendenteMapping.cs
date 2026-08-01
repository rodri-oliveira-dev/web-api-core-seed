using WebApiCoreSeed.SampleRestaurant.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApiCoreSeed.SampleRestaurant.Models.Enums;

namespace WebApiCoreSeed.SampleRestaurant.Infrastructure.Mappings
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

            builder.Ignore(p => p.Email);
            builder.Ignore(p => p.Telefone);

            builder.ToTable("Atendentes");
        }
    }
}
