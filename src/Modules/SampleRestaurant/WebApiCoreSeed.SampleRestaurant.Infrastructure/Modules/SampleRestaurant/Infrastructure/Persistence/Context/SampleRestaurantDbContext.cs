using Microsoft.EntityFrameworkCore;
using WebApiCoreSeed.SampleRestaurant.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiCoreSeed.SampleRestaurant.Infrastructure.Context
{
    public class SampleRestaurantDbContext : DbContext
    {
        public SampleRestaurantDbContext(DbContextOptions<SampleRestaurantDbContext> options) : base(options) { }

        public virtual DbSet<Atendente> Atendentes { get; set; } = null!;
        public virtual DbSet<Mesa> Mesas { get; set; } = null!;
        public virtual DbSet<PedidoPrato> PedidoPrato { get; set; } = null!;
        public virtual DbSet<Pedido> Pedidos { get; set; } = null!;
        public virtual DbSet<Prato> Pratos { get; set; } = null!;
        public virtual DbSet<LogginEntity> Loggins { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetProperties()
                    .Where(p => p.ClrType == typeof(string))))
            {
                property.SetColumnType("varchar(100)");
            }

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SampleRestaurantDbContext).Assembly);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys())) relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("DataCadastro") != null))
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("DataCadastro").CurrentValue = DateTime.Now;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Property("DataCadastro").IsModified = false;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
