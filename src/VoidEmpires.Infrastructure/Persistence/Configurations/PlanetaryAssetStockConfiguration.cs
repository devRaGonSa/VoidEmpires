using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Assets;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class PlanetaryAssetStockConfiguration : IEntityTypeConfiguration<PlanetaryAssetStock>
{
    public void Configure(EntityTypeBuilder<PlanetaryAssetStock> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PlanetId).IsRequired();
        builder.Property(x => x.AssetType).IsRequired();
        builder.Property(x => x.Quantity).IsRequired();

        builder.HasIndex(x => new { x.PlanetId, x.AssetType }).IsUnique();
    }
}
