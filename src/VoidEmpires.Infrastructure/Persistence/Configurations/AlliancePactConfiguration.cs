using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Diplomacy;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class AlliancePactConfiguration : IEntityTypeConfiguration<AlliancePact>
{
    public void Configure(EntityTypeBuilder<AlliancePact> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.SourceAllianceId).IsRequired();
        builder.Property(x => x.TargetAllianceId).IsRequired();
        builder.Property(x => x.PactType).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();

        builder.HasIndex(x => new { x.SourceAllianceId, x.TargetAllianceId, x.PactType }).IsUnique();
        builder.HasIndex(x => new { x.SourceAllianceId, x.Status, x.CreatedAtUtc, x.Id });
        builder.HasIndex(x => new { x.TargetAllianceId, x.Status, x.CreatedAtUtc, x.Id });
    }
}
