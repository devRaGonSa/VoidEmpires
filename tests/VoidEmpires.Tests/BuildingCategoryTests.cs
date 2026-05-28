using VoidEmpires.Domain.Buildings;

namespace VoidEmpires.Tests;

public class BuildingCategoryTests
{
    [Fact]
    public void CatalogContainsCategoryForAllBuildingTypes()
    {
        foreach (var buildingType in Enum.GetValues<BuildingType>())
        {
            var definition = BuildingCatalog.Get(buildingType);

            Assert.Equal(buildingType, definition.BuildingType);
            Assert.NotEqual(default, definition.Category);
        }
    }

    [Fact]
    public void MilitaryBuildingsUseSpecificMilitaryCategories()
    {
        Assert.Equal(BuildingCategory.MilitaryGround, BuildingCatalog.Get(BuildingType.MilitaryAcademy).Category);
        Assert.Equal(BuildingCategory.MilitaryGround, BuildingCatalog.Get(BuildingType.Barracks).Category);
        Assert.Equal(BuildingCategory.MilitarySpace, BuildingCatalog.Get(BuildingType.CrewAcademy).Category);
        Assert.Equal(BuildingCategory.MilitarySpace, BuildingCatalog.Get(BuildingType.FleetCommandCenter).Category);
        Assert.Equal(BuildingCategory.MilitarySpace, BuildingCatalog.Get(BuildingType.Shipyard).Category);
    }

    [Fact]
    public void PopulationBuildingsUseCivilianCategory()
    {
        Assert.Equal(BuildingCategory.Civilian, BuildingCatalog.Get(BuildingType.HabitationDistrict).Category);
        Assert.Equal(BuildingCategory.Civilian, BuildingCatalog.Get(BuildingType.MedicalCenter).Category);
    }
}
