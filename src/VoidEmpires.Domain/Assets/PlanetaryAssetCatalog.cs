using VoidEmpires.Domain.Buildings;

namespace VoidEmpires.Domain.Assets;

public static class PlanetaryAssetCatalog
{
    private static readonly IReadOnlyDictionary<PlanetaryAssetType, PlanetaryAssetDefinition> Definitions =
        new Dictionary<PlanetaryAssetType, PlanetaryAssetDefinition>
        {
            [PlanetaryAssetType.PatrolGroup] = new(
                PlanetaryAssetType.PatrolGroup,
                new AssetRequirement(100, 0, BuildingType.Barracks, 1),
                new ConstructionCost(0, 100, 25, 0)),
            [PlanetaryAssetType.ExpeditionGroup] = new(
                PlanetaryAssetType.ExpeditionGroup,
                new AssetRequirement(250, 0, BuildingType.MilitaryAcademy, 1),
                new ConstructionCost(0, 180, 60, 10)),
            [PlanetaryAssetType.VehicleGroup] = new(
                PlanetaryAssetType.VehicleGroup,
                new AssetRequirement(75, 0, BuildingType.Barracks, 2),
                new ConstructionCost(0, 450, 180, 40)),
            [PlanetaryAssetType.SupportGroup] = new(
                PlanetaryAssetType.SupportGroup,
                new AssetRequirement(150, 0, BuildingType.LogisticsHub, 1),
                new ConstructionCost(0, 220, 120, 20))
        };

    public static PlanetaryAssetDefinition Get(PlanetaryAssetType assetType) => Definitions[assetType];
}
