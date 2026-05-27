using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Research;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class ResearchProjectConfiguration : IEntityTypeConfiguration<ResearchProject>
{
    public void Configure(EntityTypeBuilder<ResearchProject> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ResearchType)
            .HasConversion<string>()
            .HasMaxLength(100);

        builder.Property(x => x.Level)
            .IsRequired();

        builder.HasIndex(x => new { x.CivilizationId, x.ResearchType })
            .IsUnique();
    }
}
