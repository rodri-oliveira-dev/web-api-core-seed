using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApiCoreSeed.Identity.Infrastructure.Context
{
    public class ApplicationDbContext : IdentityDbContext<
        IdentityUser,
        IdentityRole,
        string,
        IdentityUserClaim<string>,
        IdentityUserRole<string>,
        IdentityUserLogin<string>,
        IdentityRoleClaim<string>,
        IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.Property(login => login.LoginProvider).HasMaxLength(128);
                entity.Property(login => login.ProviderKey).HasMaxLength(128);
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.Property(token => token.LoginProvider).HasMaxLength(128);
                entity.Property(token => token.Name).HasMaxLength(128);
            });
        }
    }
}
