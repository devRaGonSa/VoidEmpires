using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Exploration;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class ExplorationKnowledgeConfiguration : IEntityTypeConfiguration<ExplorationKnowledge>
{
    public void Configure(EntityTypeBuilder<ExplorationKnowledge> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CivilizationId).IsRequired();
        builder.Property(x => x.SystemId).IsRequired();
        builder.Property(x => x.PlanetId);
        builder.Property(x => x.Source).IsRequired();
        builder.Property(x => x.SourceMissionId);
        builder.Property(x => x.DiscoveredAtUtc).IsRequired();

        builder.HasIndex(x => x.CivilizationId);
        builder.HasIndex(x => x.SystemId);
        builder.HasIndex(x => x.PlanetId);

        builder.HasIndex(x => new { x.CivilizationId, x.SystemId })
            .IsUnique()
            .HasFilter("\"PlanetId\" IS NULL");

        builder.HasIndex(x => new { x.CivilizationId, x.SystemId, x.PlanetId })
            .IsUnique()
            .HasFilter("\"PlanetId\" IS NOT NULL");
    }
}
