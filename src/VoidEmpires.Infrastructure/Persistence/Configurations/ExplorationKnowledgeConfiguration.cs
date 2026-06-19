using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Exploration;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class ExplorationKnowledgeConfiguration : IEntityTypeConfiguration<ExplorationKnowledge>
{
    public void Configure(EntityTypeBuilder<ExplorationKnowledge> builder)
    {
        builder.ToTable("exploration_knowledge");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.CivilizationId).HasColumnName("civilization_id").IsRequired();
        builder.Property(x => x.SystemId).HasColumnName("system_id").IsRequired();
        builder.Property(x => x.PlanetId).HasColumnName("planet_id");
        builder.Property(x => x.Source).HasColumnName("source").IsRequired();
        builder.Property(x => x.SourceMissionId).HasColumnName("source_mission_id");
        builder.Property(x => x.DiscoveredAtUtc).HasColumnName("discovered_at_utc").IsRequired();

        builder.HasIndex(x => x.CivilizationId);
        builder.HasIndex(x => x.SystemId);
        builder.HasIndex(x => x.PlanetId);

        builder.HasIndex(x => new { x.CivilizationId, x.SystemId })
            .IsUnique()
            .HasFilter("planet_id IS NULL");

        builder.HasIndex(x => new { x.CivilizationId, x.SystemId, x.PlanetId })
            .IsUnique()
            .HasFilter("planet_id IS NOT NULL");
    }
}
