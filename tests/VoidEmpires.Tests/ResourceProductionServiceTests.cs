using VoidEmpires.Domain.Economy;

namespace VoidEmpires.Tests;

public class ResourceProductionServiceTests
{
    [Fact]
    public void ApplyProductionAddsResourcesProportionally()
    {
        var planetId = Guid.NewGuid();
        var profile = PlanetProductionProfile.Create(planetId, 100, 120, 80, 40);
        var stockpile = PlanetResourceStockpile.Create(planetId);
        var service = new ResourceProductionService();

        service.ApplyProduction(profile, stockpile, TimeSpan.FromMinutes(15));

        Assert.Equal(25, stockpile.Credits);
        Assert.Equal(30, stockpile.Metal);
        Assert.Equal(20, stockpile.Crystal);
        Assert.Equal(10, stockpile.Gas);
    }

    [Fact]
    public void ApplyProductionIgnoresZeroElapsedTime()
    {
        var planetId = Guid.NewGuid();
        var profile = PlanetProductionProfile.Create(planetId, 100, 100, 100, 100);
        var stockpile = PlanetResourceStockpile.Create(planetId);
        var service = new ResourceProductionService();

        service.ApplyProduction(profile, stockpile, TimeSpan.Zero);

        Assert.Equal(0, stockpile.Credits);
    }

    [Fact]
    public void ApplyProductionRejectsNegativeElapsedTime()
    {
        var planetId = Guid.NewGuid();
        var profile = PlanetProductionProfile.Create(planetId, 100, 100, 100, 100);
        var stockpile = PlanetResourceStockpile.Create(planetId);
        var service = new ResourceProductionService();

        Assert.Throws<ArgumentException>(() =>
            service.ApplyProduction(profile, stockpile, TimeSpan.FromMinutes(-1)));
    }

    [Fact]
    public void ApplyProductionRejectsDifferentPlanets()
    {
        var profile = PlanetProductionProfile.Create(Guid.NewGuid(), 100, 100, 100, 100);
        var stockpile = PlanetResourceStockpile.Create(Guid.NewGuid());
        var service = new ResourceProductionService();

        Assert.Throws<InvalidOperationException>(() =>
            service.ApplyProduction(profile, stockpile, TimeSpan.FromHours(1)));
    }
}
