using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Exploration;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class ExplorationMissionConfiguration : IEntityTypeConfiguration<ExplorationMission>
{
    public void Configure(EntityTypeBuilder<ExplorationMission> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CivilizationId).IsRequired();
        builder.Property(x => x.TargetSystemId).IsRequired();
        builder.Property(x => x.TargetPlanetId);
        builder.Property(x => x.RequestedAtUtc).IsRequired();
        builder.Property(x => x.DueAtUtc).IsRequired();
        builder.Property(x => x.CompletedAtUtc);
        builder.Property(x => x.Status).IsRequired();

        builder.HasIndex(x => x.CivilizationId);
        builder.HasIndex(x => x.TargetSystemId);
        builder.HasIndex(x => x.TargetPlanetId);
        builder.HasIndex(x => new { x.CivilizationId, x.Status, x.DueAtUtc });
    }
}
