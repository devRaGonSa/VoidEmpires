using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Colonization;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class PlanetOwnershipConfiguration : IEntityTypeConfiguration<PlanetOwnership>
{
    public void Configure(EntityTypeBuilder<PlanetOwnership> builder)
    {
        builder.ToTable("planet_ownerships");

        builder.HasKey(ownership => ownership.Id);

        builder.Property(ownership => ownership.Id).HasColumnName("id");
        builder.Property(ownership => ownership.PlanetId).HasColumnName("planet_id");
        builder.Property(ownership => ownership.CivilizationId).HasColumnName("civilization_id");
        builder.Property(ownership => ownership.Status).HasColumnName("status");
        builder.Property(ownership => ownership.ClaimedAtUtc).HasColumnName("claimed_at_utc");

        builder.HasIndex(ownership => ownership.PlanetId)
            .IsUnique()
            .HasDatabaseName("ux_planet_ownerships_planet_id");

        builder.HasIndex(ownership => new { ownership.CivilizationId, ownership.Status })
            .HasDatabaseName("ix_planet_ownerships_civilization_status");
    }
}
