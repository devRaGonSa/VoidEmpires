using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Buildings;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class PlanetBuildingConfiguration : IEntityTypeConfiguration<PlanetBuilding>
{
    public void Configure(EntityTypeBuilder<PlanetBuilding> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PlanetId).IsRequired();
        builder.Property(x => x.BuildingType).IsRequired();
        builder.Property(x => x.Level).IsRequired();
        builder.Property(x => x.Footprint).IsRequired();

        builder.HasIndex(x => x.PlanetId);
    }
}
