using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Buildings;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class PlanetBuildingCapacityConfiguration : IEntityTypeConfiguration<PlanetBuildingCapacity>
{
    public void Configure(EntityTypeBuilder<PlanetBuildingCapacity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PlanetId).IsRequired();
        builder.Property(x => x.BaseCapacity).IsRequired();
        builder.Property(x => x.BonusCapacity).IsRequired();

        builder.HasIndex(x => x.PlanetId).IsUnique();
    }
}
