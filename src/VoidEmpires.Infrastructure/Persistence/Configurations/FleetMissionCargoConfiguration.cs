using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Fleets;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class FleetMissionCargoConfiguration : IEntityTypeConfiguration<FleetMissionCargo>
{
    public void Configure(EntityTypeBuilder<FleetMissionCargo> builder)
    {
        builder.HasKey(cargo => new { cargo.MissionId, cargo.ResourceType });
        builder.Property(cargo => cargo.MissionId).IsRequired();
        builder.Property(cargo => cargo.ResourceType).IsRequired();
        // Domain enforces precision/conservation; provider-safe relational CHECK hardening belongs to TASK-51BH.
        builder.Property(cargo => cargo.LoadedAmount).IsRequired().HasPrecision(18, 4);
        builder.Property(cargo => cargo.DeliveredAmount).IsRequired().HasPrecision(18, 4);
        builder.Property(cargo => cargo.ReturnedAmount).IsRequired().HasPrecision(18, 4);
        builder.Ignore(cargo => cargo.RemainingAmount);
        var relationship = builder.HasOne<FleetMission>().WithMany(mission => mission.Cargo)
            .HasForeignKey(cargo => cargo.MissionId).OnDelete(DeleteBehavior.Cascade);
        relationship.Metadata.PrincipalToDependent!.SetField("_cargo");
        relationship.Metadata.PrincipalToDependent!.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
