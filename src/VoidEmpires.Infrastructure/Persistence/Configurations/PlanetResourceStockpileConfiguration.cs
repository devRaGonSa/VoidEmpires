using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Economy;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class PlanetResourceStockpileConfiguration : IEntityTypeConfiguration<PlanetResourceStockpile>
{
    public void Configure(EntityTypeBuilder<PlanetResourceStockpile> builder)
    {
        builder.ToTable("planet_resource_stockpiles");

        builder.HasKey(stockpile => stockpile.Id);

        builder.Property(stockpile => stockpile.Id).HasColumnName("id");
        builder.Property(stockpile => stockpile.PlanetId).HasColumnName("planet_id");
        builder.Property(stockpile => stockpile.Capacity)
            .HasColumnName("capacity")
            .HasPrecision(18, 4)
            .HasDefaultValue(PlanetResourceStockpile.DefaultCapacity);
        builder.Property(stockpile => stockpile.LastAccruedAtUtc)
            .HasColumnName("last_accrued_at_utc")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(stockpile => stockpile.Credits)
            .HasColumnName("credits")
            .HasPrecision(18, 4);

        builder.Property(stockpile => stockpile.Metal)
            .HasColumnName("metal")
            .HasPrecision(18, 4);

        builder.Property(stockpile => stockpile.Crystal)
            .HasColumnName("crystal")
            .HasPrecision(18, 4);

        builder.Property(stockpile => stockpile.Gas)
            .HasColumnName("gas")
            .HasPrecision(18, 4);

        builder.HasIndex(stockpile => stockpile.PlanetId)
            .IsUnique()
            .HasDatabaseName("ux_planet_resource_stockpiles_planet_id");
    }
}
