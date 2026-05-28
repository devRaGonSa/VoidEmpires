using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoidEmpires.Domain.Population;

namespace VoidEmpires.Infrastructure.Persistence.Configurations;

public sealed class PlanetPopulationProfileConfiguration : IEntityTypeConfiguration<PlanetPopulationProfile>
{
    public void Configure(EntityTypeBuilder<PlanetPopulationProfile> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PlanetId).IsRequired();
        builder.Property(x => x.TotalPopulation).IsRequired();
        builder.Property(x => x.BaseRecruitablePopulation).IsRequired();
        builder.Property(x => x.BaseCrewCapacity).IsRequired();

        builder.HasIndex(x => x.PlanetId).IsUnique();
    }
}
