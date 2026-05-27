using VoidEmpires.Domain.Buildings;

namespace VoidEmpires.Tests;

public class BuildingCatalogTests
{
    [Fact]
    public void CatalogContainsAllBuildingTypes()
    {
        foreach (var buildingType in Enum.GetValues<BuildingType>())
        {
            var definition = BuildingCatalog.Get(buildingType);

            Assert.Equal(buildingType, definition.BuildingType);
            Assert.True(definition.InitialLevel > 0);
            Assert.True(definition.Footprint > 0);
        }
    }

    [Fact]
    public void MetalMineHasExpectedInitialDefinition()
    {
        var definition = BuildingCatalog.Get(BuildingType.MetalMine);

        Assert.Equal(1, definition.InitialLevel);
        Assert.Equal(5, definition.Footprint);
        Assert.Equal(60, definition.Cost.Metal);
        Assert.Equal(15, definition.Cost.Crystal);
    }
}
