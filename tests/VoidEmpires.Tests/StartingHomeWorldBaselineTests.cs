using VoidEmpires.Application.Players;

namespace VoidEmpires.Tests;

public class StartingHomeWorldBaselineTests
{
    [Fact]
    public void CreateResourceStockpileUsesBaselineResources()
    {
        var planetId = Guid.NewGuid();

        var stockpile = StartingHomeWorldBaseline.CreateResourceStockpile(planetId);

        Assert.Equal(planetId, stockpile.PlanetId);
        Assert.Equal(StartingHomeWorldBaseline.StartingCredits, stockpile.Credits);
        Assert.Equal(StartingHomeWorldBaseline.StartingMetal, stockpile.Metal);
        Assert.Equal(StartingHomeWorldBaseline.StartingCrystal, stockpile.Crystal);
        Assert.Equal(StartingHomeWorldBaseline.StartingGas, stockpile.Gas);
    }

    [Fact]
    public void CreateProductionProfileUsesBaselineProduction()
    {
        var planetId = Guid.NewGuid();

        var profile = StartingHomeWorldBaseline.CreateProductionProfile(planetId);

        Assert.Equal(planetId, profile.PlanetId);
        Assert.Equal(StartingHomeWorldBaseline.BaseCreditsPerHour, profile.CreditsPerHour);
        Assert.Equal(StartingHomeWorldBaseline.BaseMetalPerHour, profile.MetalPerHour);
        Assert.Equal(StartingHomeWorldBaseline.BaseCrystalPerHour, profile.CrystalPerHour);
        Assert.Equal(StartingHomeWorldBaseline.BaseGasPerHour, profile.GasPerHour);
    }

    [Fact]
    public void CreateResourceSnapshotUsesBaselineResources()
    {
        var snapshot = StartingHomeWorldBaseline.CreateResourceSnapshot();

        Assert.Equal(StartingHomeWorldBaseline.StartingCredits, snapshot.Credits);
        Assert.Equal(StartingHomeWorldBaseline.StartingMetal, snapshot.Metal);
        Assert.Equal(StartingHomeWorldBaseline.StartingCrystal, snapshot.Crystal);
        Assert.Equal(StartingHomeWorldBaseline.StartingGas, snapshot.Gas);
    }
}
