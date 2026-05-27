using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Galaxy;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class SolarSystemConfiguration : IEntityTypeConfiguration<SolarSystem>
{
    public void Configure(EntityTypeBuilder<SolarSystem> builder)
    {
        builder.ToTable("solar_systems");

        builder.HasKey(solarSystem => solarSystem.Id);

        builder.Property(solarSystem => solarSystem.Id)
            .HasColumnName("id");

        builder.Property(solarSystem => solarSystem.GalaxyId)
            .HasColumnName("galaxy_id")
            .IsRequired();

        builder.Property(solarSystem => solarSystem.Name)
            .HasColumnName("name")
            .HasMaxLength(128)
            .IsRequired();

        builder.ComplexProperty(solarSystem => solarSystem.Coordinates, coordinates =>
        {
            coordinates.Property(value => value.X)
                .HasColumnName("coordinates_x")
                .IsRequired();

            coordinates.Property(value => value.Y)
                .HasColumnName("coordinates_y")
                .IsRequired();

            coordinates.Property(value => value.Z)
                .HasColumnName("coordinates_z")
                .IsRequired();
        });

        builder.HasIndex(solarSystem => new
            {
                solarSystem.GalaxyId,
                solarSystem.Coordinates.X,
                solarSystem.Coordinates.Y,
                solarSystem.Coordinates.Z
            })
            .IsUnique()
            .HasDatabaseName("ux_solar_systems_galaxy_coordinates");

        builder.HasOne(solarSystem => solarSystem.Star)
            .WithOne()
            .HasForeignKey<Star>(star => star.SolarSystemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(solarSystem => solarSystem.Planets)
            .WithOne()
            .HasForeignKey(planet => planet.SolarSystemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(solarSystem => solarSystem.Planets)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
