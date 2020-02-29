using Restaurante.IO.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using Restaurante.IO.Business.Models.Enums;

namespace Restaurante.IO.Data.Mappings
{
    public class LogginMapping : IEntityTypeConfiguration<LogginEntity>
    {
        public void Configure(EntityTypeBuilder<LogginEntity> builder)
        {
            var converter = new EnumToNumberConverter<LogLevel, int>();

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