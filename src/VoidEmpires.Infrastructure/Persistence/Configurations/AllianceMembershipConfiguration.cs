using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Diplomacy;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class AllianceMembershipConfiguration : IEntityTypeConfiguration<AllianceMembership>
{
    public void Configure(EntityTypeBuilder<AllianceMembership> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.AllianceId).IsRequired();
        builder.Property(x => x.CivilizationId).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.Role).IsRequired();
        builder.Property(x => x.JoinedAtUtc).IsRequired();

        builder.HasIndex(x => x.CivilizationId);
        builder.HasIndex(x => x.AllianceId);
        builder.HasIndex(x => new { x.AllianceId, x.CivilizationId }).IsUnique();
        builder.HasIndex(x => new { x.CivilizationId, x.JoinedAtUtc, x.Id });
    }
}
