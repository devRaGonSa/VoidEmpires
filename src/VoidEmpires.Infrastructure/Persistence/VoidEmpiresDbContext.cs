using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Domain.Players;
using VoidEmpires.Domain.Research;
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

        public DbSet<PlayerProfile> PlayerProfiles => Set<PlayerProfile>();

        public DbSet<Civilization> Civilizations => Set<Civilization>();

        public DbSet<PlanetOwnership> PlanetOwnerships => Set<PlanetOwnership>();

        public DbSet<PlanetResourceStockpile> PlanetResourceStockpiles => Set<PlanetResourceStockpile>();

        public DbSet<PlanetProductionProfile> PlanetProductionProfiles => Set<PlanetProductionProfile>();

        public DbSet<PlanetBuilding> PlanetBuildings => Set<PlanetBuilding>();

        public DbSet<PlanetBuildingCapacity> PlanetBuildingCapacities => Set<PlanetBuildingCapacity>();

        public DbSet<PlanetConstructionOrder> PlanetConstructionOrders => Set<PlanetConstructionOrder>();

        public DbSet<ResearchProject> ResearchProjects => Set<ResearchProject>();

        public DbSet<ResearchOrder> ResearchOrders => Set<ResearchOrder>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(VoidEmpiresDbContext).Assembly);
        }
    }
}
