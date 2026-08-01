using WebApiCoreSeed.SampleRestaurant.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApiCoreSeed.SampleRestaurant.Models.Enums;

namespace WebApiCoreSeed.SampleRestaurant.Infrastructure.Mappings
{
    public class LogginMapping : IEntityTypeConfiguration<LogginEntity>
    {
        public void Configure(EntityTypeBuilder<LogginEntity> builder)
        {
            var converter = new EnumToNumberConverter<ELogLevel, int>();

            builder.HasKey(p => p.Id);

            builder.Property(p => p.EventId)
                .HasColumnType("int");

            builder.Property(p => p.LogLevel)
                .IsRequired()
                .HasColumnType("int")
                .HasConversion(converter);

            builder.Property(p => p.Message)
                .IsRequired()
                .HasColumnType("varchar(6000)");

            builder.Property(p => p.CreatedTime)
                .HasColumnType("datetime");

            builder.ToTable("Loggin");
        }
    }
}
