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
                new ConstructionCost(0, 220, 120, 20)),
            [PlanetaryAssetType.MissileBattery] = new(
                PlanetaryAssetType.MissileBattery,
                new AssetRequirement(5, 0, BuildingType.DefenseGrid, 1),
                new ConstructionCost(0, 120, 40, 10)),
            [PlanetaryAssetType.LaserTurret] = new(
                PlanetaryAssetType.LaserTurret,
                new AssetRequirement(6, 0, BuildingType.DefenseGrid, 1),
                new ConstructionCost(0, 140, 90, 15)),
            [PlanetaryAssetType.IonCannon] = new(
                PlanetaryAssetType.IonCannon,
                new AssetRequirement(8, 0, BuildingType.DefenseGrid, 2),
                new ConstructionCost(0, 220, 160, 45)),
            [PlanetaryAssetType.PlasmaCannon] = new(
                PlanetaryAssetType.PlasmaCannon,
                new AssetRequirement(10, 0, BuildingType.DefenseGrid, 3),
                new ConstructionCost(0, 320, 240, 80))
        };

    public static PlanetaryAssetDefinition Get(PlanetaryAssetType assetType) => Definitions[assetType];
}
