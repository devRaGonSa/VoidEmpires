using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Buildings;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class PlanetConstructionOrderConfiguration : IEntityTypeConfiguration<PlanetConstructionOrder>
{
    public void Configure(EntityTypeBuilder<PlanetConstructionOrder> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PlanetId).IsRequired();
        builder.Property(x => x.Action).IsRequired();
        builder.Property(x => x.BuildingType).IsRequired();
        builder.Property(x => x.TargetLevel).IsRequired();
        builder.Property(x => x.Sequence).IsRequired();
        builder.Property(x => x.StartsAtUtc).IsRequired();
        builder.Property(x => x.EndsAtUtc).IsRequired();
        builder.Property(x => x.Status).IsRequired();

        builder.HasIndex(x => x.PlanetId);
        builder.HasIndex(x => new { x.PlanetId, x.Sequence }).IsUnique();
        builder.HasIndex(x => new { x.PlanetId, x.Status });
    }
}
