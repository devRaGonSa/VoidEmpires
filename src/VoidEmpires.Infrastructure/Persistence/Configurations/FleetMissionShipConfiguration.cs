using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Fleets;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class FleetMissionShipConfiguration : IEntityTypeConfiguration<FleetMissionShip>
{
    public void Configure(EntityTypeBuilder<FleetMissionShip> builder)
    {
        builder.HasKey(ship => new { ship.MissionId, ship.AssetType });
        builder.Property(ship => ship.MissionId).IsRequired();
        builder.Property(ship => ship.AssetType).IsRequired();
        // Domain enforces positivity; provider-safe relational CHECK hardening belongs to TASK-51BH.
        builder.Property(ship => ship.Quantity).IsRequired();
        var relationship = builder.HasOne<FleetMission>().WithMany(mission => mission.Ships)
            .HasForeignKey(ship => ship.MissionId).OnDelete(DeleteBehavior.Cascade);
        relationship.Metadata.PrincipalToDependent!.SetField("_ships");
        relationship.Metadata.PrincipalToDependent!.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
