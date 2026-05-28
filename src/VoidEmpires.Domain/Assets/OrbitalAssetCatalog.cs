using VoidEmpires.Domain.Buildings;

namespace VoidEmpires.Domain.Assets;

public static class OrbitalAssetCatalog
{
    private static readonly IReadOnlyDictionary<SpaceAssetType, OrbitalAssetDefinition> Definitions =
        new Dictionary<SpaceAssetType, OrbitalAssetDefinition>
        {
            [SpaceAssetType.ScoutCraft] = new(
                SpaceAssetType.ScoutCraft,
                new AssetRequirement(0, 25, BuildingType.Shipyard, 1),
                new ConstructionCost(0, 120, 80, 40),
                0,
                3),
            [SpaceAssetType.CargoCraft] = new(
                SpaceAssetType.CargoCraft,
                new AssetRequirement(0, 60, BuildingType.Shipyard, 1),
                new ConstructionCost(0, 250, 120, 80),
                1_000,
                2),
            [SpaceAssetType.EscortCraft] = new(
                SpaceAssetType.EscortCraft,
                new AssetRequirement(0, 120, BuildingType.FleetCommandCenter, 1),
                new ConstructionCost(0, 500, 250, 150),
                100,
                4),
            [SpaceAssetType.ColonyCraft] = new(
                SpaceAssetType.ColonyCraft,
                new AssetRequirement(0, 500, BuildingType.LogisticsHub, 2),
                new ConstructionCost(0, 1_500, 800, 500),
                500,
                5)
        };

    public static OrbitalAssetDefinition Get(SpaceAssetType assetType) => Definitions[assetType];
}
