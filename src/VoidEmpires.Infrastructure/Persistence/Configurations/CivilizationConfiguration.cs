using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Players;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class CivilizationConfiguration : IEntityTypeConfiguration<Civilization>
{
    public void Configure(EntityTypeBuilder<Civilization> builder)
    {
        builder.ToTable("civilizations");

        builder.HasKey(civilization => civilization.Id);

        builder.Property(civilization => civilization.Id).HasColumnName("id");
        builder.Property(civilization => civilization.PlayerProfileId).HasColumnName("player_profile_id");
        builder.Property(civilization => civilization.Name)
            .HasColumnName("name")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(civilization => civilization.Archetype)
            .HasColumnName("archetype")
            .IsRequired();

        builder.Property(civilization => civilization.Status)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(civilization => civilization.HomePlanetId)
            .HasColumnName("home_planet_id");

        builder.HasIndex(civilization => new { civilization.PlayerProfileId, civilization.Name })
            .IsUnique()
            .HasDatabaseName("ux_civilizations_profile_name");
    }
}
