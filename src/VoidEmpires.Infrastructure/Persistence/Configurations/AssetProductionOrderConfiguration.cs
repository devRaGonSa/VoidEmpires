using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Assets;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class AssetProductionOrderConfiguration : IEntityTypeConfiguration<AssetProductionOrder>
{
    public void Configure(EntityTypeBuilder<AssetProductionOrder> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PlanetId).IsRequired();
        builder.Property(x => x.Target).IsRequired();
        builder.Property(x => x.PlanetaryAssetType);
        builder.Property(x => x.SpaceAssetType);
        builder.Property(x => x.Quantity).IsRequired();
        builder.Property(x => x.Sequence).IsRequired();
        builder.Property(x => x.StartsAtUtc).IsRequired();
        builder.Property(x => x.EndsAtUtc).IsRequired();
        builder.Property(x => x.Status).IsRequired();

        builder.HasIndex(x => x.PlanetId);
        builder.HasIndex(x => new { x.PlanetId, x.Sequence }).IsUnique();
        builder.HasIndex(x => new { x.PlanetId, x.Status });
        builder.HasIndex(x => new { x.Target, x.Status, x.EndsAtUtc, x.Sequence });
    }
}
