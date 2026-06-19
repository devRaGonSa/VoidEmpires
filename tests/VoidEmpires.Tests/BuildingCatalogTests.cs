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
            Assert.False(string.IsNullOrWhiteSpace(definition.DisplayName));
            Assert.False(string.IsNullOrWhiteSpace(definition.RoleKey));
            Assert.False(string.IsNullOrWhiteSpace(definition.RoleLabel));
            Assert.False(string.IsNullOrWhiteSpace(definition.Description));
            Assert.False(string.IsNullOrWhiteSpace(definition.ModuleKey));
            Assert.False(string.IsNullOrWhiteSpace(definition.ModuleLabel));
            Assert.False(string.IsNullOrWhiteSpace(definition.ImageKey));
            Assert.False(string.IsNullOrWhiteSpace(definition.IconKey));
            Assert.True(definition.SortOrder > 0);
            Assert.False(string.IsNullOrWhiteSpace(definition.DurationPolicyKey));
            Assert.False(string.IsNullOrWhiteSpace(definition.DurationPolicyLabel));
            Assert.False(string.IsNullOrWhiteSpace(definition.PrerequisiteSummary));
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
        Assert.Equal("Mina de metal", definition.DisplayName);
        Assert.Equal("building.metal-mine", definition.ImageKey);
        Assert.Equal("icon.metal-mine", definition.IconKey);
    }
}
