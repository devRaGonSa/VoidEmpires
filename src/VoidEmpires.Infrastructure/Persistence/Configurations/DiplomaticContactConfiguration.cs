using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Diplomacy;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class DiplomaticContactConfiguration : IEntityTypeConfiguration<DiplomaticContact>
{
    public void Configure(EntityTypeBuilder<DiplomaticContact> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CivilizationId).IsRequired();
        builder.Property(x => x.ContactedCivilizationId).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.DiscoveredAtUtc).IsRequired();
        builder.Property(x => x.Source)
            .IsRequired()
            .HasMaxLength(64);

        builder.HasIndex(x => x.CivilizationId);
        builder.HasIndex(x => x.ContactedCivilizationId);
        builder.HasIndex(x => new { x.CivilizationId, x.ContactedCivilizationId })
            .IsUnique();
    }
}
