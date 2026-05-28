using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Fleets;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class OrbitalGroupConfiguration : IEntityTypeConfiguration<OrbitalGroup>
{
    public void Configure(EntityTypeBuilder<OrbitalGroup> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CivilizationId).IsRequired();
        builder.Property(x => x.OriginPlanetId).IsRequired();
        builder.Property(x => x.CurrentPlanetId).IsRequired();
        builder.Property(x => x.AssetType).IsRequired();
        builder.Property(x => x.Quantity).IsRequired();
        builder.Property(x => x.Status).IsRequired();

        builder.HasIndex(x => x.CivilizationId);
        builder.HasIndex(x => x.OriginPlanetId);
        builder.HasIndex(x => x.CurrentPlanetId);
        builder.HasIndex(x => new { x.CurrentPlanetId, x.Status });
    }
}
