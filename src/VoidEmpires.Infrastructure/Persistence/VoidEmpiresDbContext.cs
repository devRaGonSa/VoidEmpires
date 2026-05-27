using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Infrastructure.Identity;

namespace VoidEmpires.Infrastructure.Identity
{
    public sealed class VoidEmpiresUser : IdentityUser
    {
    }
}

namespace VoidEmpires.Infrastructure.Persistence
{
    public sealed class VoidEmpiresDbContext : IdentityDbContext<VoidEmpiresUser>
    {
        public VoidEmpiresDbContext(DbContextOptions<VoidEmpiresDbContext> options)
            : base(options)
        {
        }

        public DbSet<Galaxy> Galaxies => Set<Galaxy>();

        public DbSet<SolarSystem> SolarSystems => Set<SolarSystem>();

        public DbSet<Star> Stars => Set<Star>();

        public DbSet<Planet> Planets => Set<Planet>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(VoidEmpiresDbContext).Assembly);
        }
    }
}
