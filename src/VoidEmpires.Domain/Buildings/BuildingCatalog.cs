namespace VoidEmpires.Domain.Buildings;

public static class BuildingCatalog
{
    private static readonly IReadOnlyDictionary<BuildingType, BuildingDefinition> Definitions =
        new Dictionary<BuildingType, BuildingDefinition>
        {
            [BuildingType.CommandCenter] = new(BuildingType.CommandCenter, 1, 20, new ConstructionCost(0, 500, 250, 0), BuildingCategory.Civilian),
            [BuildingType.MetalMine] = new(BuildingType.MetalMine, 1, 5, new ConstructionCost(0, 60, 15, 0), BuildingCategory.Industrial),
            [BuildingType.CrystalMine] = new(BuildingType.CrystalMine, 1, 5, new ConstructionCost(0, 50, 40, 0), BuildingCategory.Industrial),
            [BuildingType.GasExtractor] = new(BuildingType.GasExtractor, 1, 6, new ConstructionCost(0, 75, 30, 20), BuildingCategory.Industrial),
            [BuildingType.SolarPlant] = new(BuildingType.SolarPlant, 1, 4, new ConstructionCost(0, 40, 10, 0), BuildingCategory.Industrial),
            [BuildingType.ResearchLab] = new(BuildingType.ResearchLab, 1, 12, new ConstructionCost(0, 200, 120, 25), BuildingCategory.Research),
            [BuildingType.Shipyard] = new(BuildingType.Shipyard, 1, 15, new ConstructionCost(0, 250, 100, 50), BuildingCategory.MilitarySpace),
            [BuildingType.DefenseGrid] = new(BuildingType.DefenseGrid, 1, 10, new ConstructionCost(0, 150, 75, 0), BuildingCategory.Defense),
            [BuildingType.HabitationDistrict] = new(BuildingType.HabitationDistrict, 1, 8, new ConstructionCost(0, 120, 60, 0), BuildingCategory.Civilian),
            [BuildingType.MedicalCenter] = new(BuildingType.MedicalCenter, 1, 6, new ConstructionCost(0, 160, 120, 20), BuildingCategory.Civilian),
            [BuildingType.MilitaryAcademy] = new(BuildingType.MilitaryAcademy, 1, 10, new ConstructionCost(0, 220, 140, 20), BuildingCategory.MilitaryGround),
            [BuildingType.Barracks] = new(BuildingType.Barracks, 1, 9, new ConstructionCost(0, 180, 90, 10), BuildingCategory.MilitaryGround),
            [BuildingType.CrewAcademy] = new(BuildingType.CrewAcademy, 1, 10, new ConstructionCost(0, 240, 160, 40), BuildingCategory.MilitarySpace),
            [BuildingType.FleetCommandCenter] = new(BuildingType.FleetCommandCenter, 1, 14, new ConstructionCost(0, 320, 220, 60), BuildingCategory.MilitarySpace),
            [BuildingType.LogisticsHub] = new(BuildingType.LogisticsHub, 1, 12, new ConstructionCost(0, 200, 100, 30), BuildingCategory.Logistics)
        };

    public static BuildingDefinition Get(BuildingType buildingType) => Definitions[buildingType];
}
