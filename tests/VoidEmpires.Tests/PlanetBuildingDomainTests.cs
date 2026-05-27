using VoidEmpires.Domain.Buildings;

namespace VoidEmpires.Tests;

public class PlanetBuildingDomainTests
{
    [Fact]
    public void CreateRejectsEmptyPlanetId()
    {
        Assert.Throws<ArgumentException>(() =>
            PlanetBuilding.Create(Guid.Empty, BuildingType.MetalMine, 1, 5));
    }

    [Fact]
    public void CreateRejectsInvalidLevel()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            PlanetBuilding.Create(Guid.NewGuid(), BuildingType.MetalMine, 0, 5));
    }

    [Fact]
    public void CreateRejectsInvalidFootprint()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            PlanetBuilding.Create(Guid.NewGuid(), BuildingType.MetalMine, 1, 0));
    }

    [Fact]
    public void CreateStoresBuildingValues()
    {
        var planetId = Guid.NewGuid();

        var building = PlanetBuilding.Create(planetId, BuildingType.Shipyard, 2, 15);

        Assert.Equal(planetId, building.PlanetId);
        Assert.Equal(BuildingType.Shipyard, building.BuildingType);
        Assert.Equal(2, building.Level);
        Assert.Equal(15, building.Footprint);
    }
}
