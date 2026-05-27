namespace VoidEmpires.Domain.Buildings;

public static class BuildingCatalog
{
    private static readonly IReadOnlyDictionary<BuildingType, BuildingDefinition> Definitions =
        new Dictionary<BuildingType, BuildingDefinition>
        {
            [BuildingType.CommandCenter] = new(BuildingType.CommandCenter, 1, 20, new ConstructionCost(0, 500, 250, 0)),
            [BuildingType.MetalMine] = new(BuildingType.MetalMine, 1, 5, new ConstructionCost(0, 60, 15, 0)),
            [BuildingType.CrystalMine] = new(BuildingType.CrystalMine, 1, 5, new ConstructionCost(0, 50, 40, 0)),
            [BuildingType.GasExtractor] = new(BuildingType.GasExtractor, 1, 6, new ConstructionCost(0, 75, 30, 20)),
            [BuildingType.SolarPlant] = new(BuildingType.SolarPlant, 1, 4, new ConstructionCost(0, 40, 10, 0)),
            [BuildingType.ResearchLab] = new(BuildingType.ResearchLab, 1, 12, new ConstructionCost(0, 200, 120, 25)),
            [BuildingType.Shipyard] = new(BuildingType.Shipyard, 1, 15, new ConstructionCost(0, 250, 100, 50)),
            [BuildingType.DefenseGrid] = new(BuildingType.DefenseGrid, 1, 10, new ConstructionCost(0, 150, 75, 0))
        };

    public static BuildingDefinition Get(BuildingType buildingType) => Definitions[buildingType];
}
