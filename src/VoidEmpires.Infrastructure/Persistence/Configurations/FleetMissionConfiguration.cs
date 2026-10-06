using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Fleets;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class FleetMissionConfiguration : IEntityTypeConfiguration<FleetMission>
{
    public void Configure(EntityTypeBuilder<FleetMission> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CivilizationId).IsRequired();
        builder.Property(x => x.OriginPlanetId).IsRequired();
        builder.Property(x => x.DestinationPlanetId).IsRequired();
        builder.Property(x => x.MissionType).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.OutboundDepartureUtc).IsRequired();
        builder.Property(x => x.OutboundArrivalUtc).IsRequired();
        builder.Property(x => x.ReturnDepartureUtc);
        builder.Property(x => x.ReturnArrivalUtc);
        builder.Property(x => x.CompletedAtUtc);
        builder.Property(x => x.RecalledAtUtc);
        builder.Property(x => x.StateVersion).IsRequired().IsConcurrencyToken();
        builder.HasIndex(x => x.CivilizationId);
        builder.HasIndex(x => x.OriginPlanetId);
        builder.HasIndex(x => x.DestinationPlanetId);
        builder.HasIndex(x => new { x.Status, x.OutboundArrivalUtc });
        builder.HasIndex(x => new { x.Status, x.ReturnArrivalUtc });
    }
}
