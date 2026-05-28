using VoidEmpires.Domain.Population;

namespace VoidEmpires.Tests;

public class PlanetPopulationProfileTests
{
    [Fact]
    public void CreateStoresPopulationCapacityValues()
    {
        var planetId = Guid.NewGuid();

        var profile = PlanetPopulationProfile.Create(planetId, 1_000_000, 50_000, 10_000);

        Assert.Equal(planetId, profile.PlanetId);
        Assert.Equal(1_000_000, profile.TotalPopulation);
        Assert.Equal(50_000, profile.BaseRecruitablePopulation);
        Assert.Equal(10_000, profile.BaseCrewCapacity);
    }

    [Fact]
    public void CanRecruitUsesBaseAndBuildingBonusCapacity()
    {
        var profile = PlanetPopulationProfile.Create(Guid.NewGuid(), 1_000_000, 10_000, 5_000);

        Assert.True(profile.CanRecruit(12_000, 2_000));
        Assert.False(profile.CanRecruit(12_001, 2_000));
    }

    [Fact]
    public void CanCrewLocallyBuiltShipsUsesBaseAndBuildingBonusCapacity()
    {
        var profile = PlanetPopulationProfile.Create(Guid.NewGuid(), 1_000_000, 10_000, 5_000);

        Assert.True(profile.CanCrewLocallyBuiltShips(6_000, 1_000));
        Assert.False(profile.CanCrewLocallyBuiltShips(6_001, 1_000));
    }
}
