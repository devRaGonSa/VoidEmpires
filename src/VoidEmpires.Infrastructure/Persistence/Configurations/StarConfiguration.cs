using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Galaxy;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class StarConfiguration : IEntityTypeConfiguration<Star>
{
    public void Configure(EntityTypeBuilder<Star> builder)
    {
        builder.ToTable("stars");

        builder.HasKey(star => star.Id);

        builder.Property(star => star.Id)
            .HasColumnName("id");

        builder.Property(star => star.SolarSystemId)
            .HasColumnName("solar_system_id")
            .IsRequired();

        builder.Property(star => star.Name)
            .HasColumnName("name")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(star => star.StarType)
            .HasColumnName("star_type")
            .IsRequired();

        builder.HasIndex(star => star.SolarSystemId)
            .IsUnique()
            .HasDatabaseName("ux_stars_solar_system_id");
    }
}
