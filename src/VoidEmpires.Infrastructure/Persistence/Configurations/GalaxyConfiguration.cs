using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Galaxy;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class GalaxyConfiguration : IEntityTypeConfiguration<Galaxy>
{
    public void Configure(EntityTypeBuilder<Galaxy> builder)
    {
        builder.ToTable("galaxies");

        builder.HasKey(galaxy => galaxy.Id);

        builder.Property(galaxy => galaxy.Id)
            .HasColumnName("id");

        builder.Property(galaxy => galaxy.Name)
            .HasColumnName("name")
            .HasMaxLength(128)
            .IsRequired();

        builder.HasMany(galaxy => galaxy.SolarSystems)
            .WithOne()
            .HasForeignKey(solarSystem => solarSystem.GalaxyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(galaxy => galaxy.SolarSystems)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
