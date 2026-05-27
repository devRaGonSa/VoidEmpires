using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Economy;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class PlanetProductionProfileConfiguration : IEntityTypeConfiguration<PlanetProductionProfile>
{
    public void Configure(EntityTypeBuilder<PlanetProductionProfile> builder)
    {
        builder.ToTable("planet_production_profiles");

        builder.HasKey(profile => profile.Id);

        builder.Property(profile => profile.Id).HasColumnName("id");
        builder.Property(profile => profile.PlanetId).HasColumnName("planet_id");

        builder.Property(profile => profile.CreditsPerHour)
            .HasColumnName("credits_per_hour")
            .HasPrecision(18, 4);

        builder.Property(profile => profile.MetalPerHour)
            .HasColumnName("metal_per_hour")
            .HasPrecision(18, 4);

        builder.Property(profile => profile.CrystalPerHour)
            .HasColumnName("crystal_per_hour")
            .HasPrecision(18, 4);

        builder.Property(profile => profile.GasPerHour)
            .HasColumnName("gas_per_hour")
            .HasPrecision(18, 4);

        builder.HasIndex(profile => profile.PlanetId)
            .IsUnique()
            .HasDatabaseName("ux_planet_production_profiles_planet_id");
    }
}
