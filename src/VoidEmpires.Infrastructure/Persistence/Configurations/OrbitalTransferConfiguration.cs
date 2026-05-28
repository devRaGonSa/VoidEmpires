using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Fleets;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class OrbitalTransferConfiguration : IEntityTypeConfiguration<OrbitalTransfer>
{
    public void Configure(EntityTypeBuilder<OrbitalTransfer> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CivilizationId).IsRequired();
        builder.Property(x => x.OrbitalGroupId).IsRequired();
        builder.Property(x => x.OriginPlanetId).IsRequired();
        builder.Property(x => x.DestinationPlanetId).IsRequired();
        builder.Property(x => x.AbstractDistanceUnits).IsRequired();
        builder.Property(x => x.DepartureAtUtc).IsRequired();
        builder.Property(x => x.ArrivalAtUtc).IsRequired();
        builder.Property(x => x.Status).IsRequired();

        builder.HasIndex(x => x.CivilizationId);
        builder.HasIndex(x => x.OrbitalGroupId);
        builder.HasIndex(x => x.OriginPlanetId);
        builder.HasIndex(x => x.DestinationPlanetId);
        builder.HasIndex(x => new { x.Status, x.ArrivalAtUtc });
    }
}
