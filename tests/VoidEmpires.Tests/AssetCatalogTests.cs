using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Buildings;

namespace VoidEmpires.Tests;

public class AssetCatalogTests
{
    [Fact]
    public void PlanetaryCatalogContainsAllPlanetaryAssetTypes()
    {
        foreach (var assetType in Enum.GetValues<PlanetaryAssetType>())
        {
            var definition = PlanetaryAssetCatalog.Get(assetType);

            Assert.Equal(assetType, definition.AssetType);
            Assert.True(definition.Requirement.PopulationCapacity >= 0);
            Assert.True(definition.Requirement.OperatorCapacity >= 0);
            Assert.True(definition.Requirement.RequiredBuildingLevel > 0);
        }
    }

    [Fact]
    public void OrbitalCatalogContainsAllSpaceAssetTypes()
    {
        foreach (var assetType in Enum.GetValues<SpaceAssetType>())
        {
            var definition = OrbitalAssetCatalog.Get(assetType);

            Assert.Equal(assetType, definition.AssetType);
            Assert.True(definition.Requirement.PopulationCapacity >= 0);
            Assert.True(definition.Requirement.OperatorCapacity >= 0);
            Assert.True(definition.Requirement.RequiredBuildingLevel > 0);
            Assert.True(definition.StorageCapacity >= 0);
            Assert.True(definition.OperatingRange > 0);
        }
    }

    [Fact]
    public void OrbitalAssetsUseOperatorCapacityInsteadOfPlanetPopulationCapacity()
    {
        var definition = OrbitalAssetCatalog.Get(SpaceAssetType.ScoutCraft);

        Assert.Equal(0, definition.Requirement.PopulationCapacity);
        Assert.True(definition.Requirement.OperatorCapacity > 0);
        Assert.Equal(BuildingType.Shipyard, definition.Requirement.RequiredBuildingType);
    }

    [Fact]
    public void PlanetaryAssetsUsePopulationCapacityInsteadOfOperatorCapacity()
    {
        var definition = PlanetaryAssetCatalog.Get(PlanetaryAssetType.PatrolGroup);

        Assert.True(definition.Requirement.PopulationCapacity > 0);
        Assert.Equal(0, definition.Requirement.OperatorCapacity);
        Assert.Equal(BuildingType.Barracks, definition.Requirement.RequiredBuildingType);
    }
}
