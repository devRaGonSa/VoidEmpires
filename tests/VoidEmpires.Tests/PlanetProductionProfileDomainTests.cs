using VoidEmpires.Domain.Economy;

namespace VoidEmpires.Tests;

public class PlanetProductionProfileDomainTests
{
    [Fact]
    public void CreateRejectsEmptyPlanetId()
    {
        Assert.Throws<ArgumentException>(() =>
            PlanetProductionProfile.Create(Guid.Empty, 1, 1, 1, 1));
    }

    [Fact]
    public void CreateRejectsNegativeProduction()
    {
        Assert.Throws<ArgumentException>(() =>
            PlanetProductionProfile.Create(Guid.NewGuid(), -1, 0, 0, 0));
    }

    [Fact]
    public void CreateStoresProductionValues()
    {
        var profile = PlanetProductionProfile.Create(
            Guid.NewGuid(),
            100,
            200,
            300,
            400);

        Assert.Equal(100, profile.CreditsPerHour);
        Assert.Equal(200, profile.MetalPerHour);
        Assert.Equal(300, profile.CrystalPerHour);
        Assert.Equal(400, profile.GasPerHour);
    }
}
