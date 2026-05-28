using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Research;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class ResearchOrderConfiguration : IEntityTypeConfiguration<ResearchOrder>
{
    public void Configure(EntityTypeBuilder<ResearchOrder> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CivilizationId).IsRequired();
        builder.Property(x => x.SourcePlanetId).IsRequired();
        builder.Property(x => x.ResearchType).IsRequired();
        builder.Property(x => x.TargetLevel).IsRequired();
        builder.Property(x => x.Sequence).IsRequired();
        builder.Property(x => x.StartsAtUtc).IsRequired();
        builder.Property(x => x.EndsAtUtc).IsRequired();
        builder.Property(x => x.Status).IsRequired();

        builder.HasIndex(x => x.CivilizationId);
        builder.HasIndex(x => new { x.CivilizationId, x.Sequence }).IsUnique();
        builder.HasIndex(x => new { x.CivilizationId, x.Status });
    }
}
