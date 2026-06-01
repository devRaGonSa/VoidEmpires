using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Diplomacy;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class AllianceConfiguration : IEntityTypeConfiguration<Alliance>
{
    public void Configure(EntityTypeBuilder<Alliance> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(128);
        builder.Property(x => x.Tag)
            .IsRequired()
            .HasMaxLength(16);
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();

        builder.HasIndex(x => x.Tag).IsUnique();
        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => new { x.Status, x.CreatedAtUtc, x.Id });
    }
}
