using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Galaxy;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class PlanetConfiguration : IEntityTypeConfiguration<Planet>
{
    public void Configure(EntityTypeBuilder<Planet> builder)
    {
        builder.ToTable("planets");

        builder.HasKey(planet => planet.Id);

        builder.Property(planet => planet.Id)
            .HasColumnName("id");

        builder.Property(planet => planet.SolarSystemId)
            .HasColumnName("solar_system_id")
            .IsRequired();

        builder.Property(planet => planet.Name)
            .HasColumnName("name")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(planet => planet.OrbitalSlot)
            .HasColumnName("orbital_slot")
            .IsRequired();

        builder.Property(planet => planet.PlanetType)
            .HasColumnName("planet_type")
            .IsRequired();

        builder.Property(planet => planet.Size)
            .HasColumnName("size")
            .IsRequired();

        builder.Property(planet => planet.ColonizationStatus)
            .HasColumnName("colonization_status")
            .IsRequired();

        builder.HasIndex(planet => new { planet.SolarSystemId, planet.OrbitalSlot })
            .IsUnique()
            .HasDatabaseName("ux_planets_solar_system_orbital_slot");
    }
}
