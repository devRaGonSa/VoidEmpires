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
            Assert.True(Enum.IsDefined(definition.AssetType));
            Assert.True(definition.BaseMovementSpeed > 0);
            Assert.True(definition.FuelConsumptionPerDistanceUnit > 0); // All current v1 ships consume Gas.
            const SpaceAssetCapability knownCapabilities = SpaceAssetCapability.Exploration | SpaceAssetCapability.Colonization;
            Assert.Equal(SpaceAssetCapability.None, definition.Capabilities & ~knownCapabilities);
            Assert.False(string.IsNullOrWhiteSpace(definition.CategoryLabel));
            Assert.False(string.IsNullOrWhiteSpace(definition.RoleLabel));
            Assert.False(string.IsNullOrWhiteSpace(definition.ModuleKey));
            Assert.False(string.IsNullOrWhiteSpace(definition.ModuleLabel));
            Assert.False(string.IsNullOrWhiteSpace(definition.DurationPolicyKey));
            Assert.False(string.IsNullOrWhiteSpace(definition.DurationPolicyLabel));
            Assert.False(string.IsNullOrWhiteSpace(definition.FleetHandoffPolicyKey));
            Assert.False(string.IsNullOrWhiteSpace(definition.FleetHandoffPolicyLabel));
            Assert.False(string.IsNullOrWhiteSpace(definition.PrerequisiteSummary));
            Assert.False(string.IsNullOrWhiteSpace(definition.DisplayName));
            Assert.False(string.IsNullOrWhiteSpace(definition.CategoryKey));
            Assert.False(string.IsNullOrWhiteSpace(definition.RoleKey));
            Assert.False(string.IsNullOrWhiteSpace(definition.Description));
            Assert.False(string.IsNullOrWhiteSpace(definition.ImageKey));
            Assert.False(string.IsNullOrWhiteSpace(definition.IconKey));
            Assert.True(definition.SortOrder > 0);
            Assert.NotEmpty(definition.RequirementKeys);
            Assert.NotEmpty(definition.Tags);
        }
    }

    public static IEnumerable<object[]> V1Profiles =>
    [
        [SpaceAssetType.ScoutCraft, 0, 3, 120m, 1.0m, SpaceAssetCapability.Exploration],
        [SpaceAssetType.CargoCraft, 1000, 2, 80m, 2.0m, SpaceAssetCapability.None],
        [SpaceAssetType.EscortCraft, 100, 4, 100m, 2.5m, SpaceAssetCapability.None],
        [SpaceAssetType.ColonyCraft, 500, 5, 60m, 4.0m, SpaceAssetCapability.Colonization]
    ];

    [Theory]
    [MemberData(nameof(V1Profiles))]
    public void OrbitalCatalogProvidesTheAuthoritativeV1Profile(SpaceAssetType type, int storage, int range,
        decimal speed, decimal fuel, SpaceAssetCapability capabilities)
    {
        var definition = OrbitalAssetCatalog.Get(type);
        Assert.Equal(storage, definition.StorageCapacity);
        Assert.Equal(range, definition.OperatingRange);
        Assert.Equal(speed, definition.BaseMovementSpeed);
        Assert.Equal(fuel, definition.FuelConsumptionPerDistanceUnit);
        Assert.Equal(capabilities, definition.Capabilities);
        Assert.Equal(type == SpaceAssetType.ScoutCraft, definition.Capabilities.HasFlag(SpaceAssetCapability.Exploration));
        Assert.Equal(type == SpaceAssetType.ColonyCraft, definition.Capabilities.HasFlag(SpaceAssetCapability.Colonization));
    }

    [Fact]
    public void V1SpeedOrderIsScoutThenEscortThenCargoThenColony()
    {
        var scout = OrbitalAssetCatalog.Get(SpaceAssetType.ScoutCraft).BaseMovementSpeed;
        var escort = OrbitalAssetCatalog.Get(SpaceAssetType.EscortCraft).BaseMovementSpeed;
        var cargo = OrbitalAssetCatalog.Get(SpaceAssetType.CargoCraft).BaseMovementSpeed;
        var colony = OrbitalAssetCatalog.Get(SpaceAssetType.ColonyCraft).BaseMovementSpeed;
        Assert.True(scout > escort);
        Assert.True(escort > cargo);
        Assert.True(cargo > colony);
    }

    [Fact]
    public void CapabilityFlagsContainOnlyExplorationAndColonization()
    {
        Assert.True(Attribute.IsDefined(typeof(SpaceAssetCapability), typeof(FlagsAttribute)));
        Assert.Equal(new[] { SpaceAssetCapability.None, SpaceAssetCapability.Exploration, SpaceAssetCapability.Colonization },
            Enum.GetValues<SpaceAssetCapability>());
        Assert.Equal(0, (int)SpaceAssetCapability.None);
        Assert.Equal(1, (int)SpaceAssetCapability.Exploration);
        Assert.Equal(2, (int)SpaceAssetCapability.Colonization);
        var combined = SpaceAssetCapability.Exploration | SpaceAssetCapability.Colonization;
        Assert.True(combined.HasFlag(SpaceAssetCapability.Exploration));
        Assert.True(combined.HasFlag(SpaceAssetCapability.Colonization));
    }

    [Theory]
    [InlineData(SpaceAssetType.ScoutCraft, BuildingType.Shipyard, 1, 25, 120, 80, 40, "Reconocimiento", "Reconocimiento rapido")]
    [InlineData(SpaceAssetType.CargoCraft, BuildingType.Shipyard, 1, 60, 250, 120, 80, "Transporte", "Transporte de suministros")]
    [InlineData(SpaceAssetType.EscortCraft, BuildingType.FleetCommandCenter, 1, 120, 500, 250, 150, "Cobertura", "Cobertura orbital")]
    [InlineData(SpaceAssetType.ColonyCraft, BuildingType.LogisticsHub, 2, 500, 1500, 800, 500, "Expansion", "Expansion y asentamiento")]
    public void CapabilitiesPreserveProductionRequirementsCostsAndRoles(SpaceAssetType type, BuildingType building,
        int level, int operators, int metal, int crystal, int gas, string roleKey, string roleLabel)
    {
        var definition = OrbitalAssetCatalog.Get(type);
        Assert.Equal(new AssetRequirement(0, operators, building, level), definition.Requirement);
        Assert.Equal(new ConstructionCost(0, metal, crystal, gas), definition.Cost);
        Assert.Equal(roleKey, definition.RoleKey);
        Assert.Equal(roleLabel, definition.RoleLabel);
        Assert.Equal(new[] { $"Building:{building}:{level}", $"OperatorCapacity:{operators}" }, definition.RequirementKeys);
        Assert.Equal("Shipyard", definition.ModuleKey);
        Assert.Equal("PerUnitMinutes", definition.DurationPolicyKey);
        Assert.Equal("StockToFleetDevelopmentFlow", definition.FleetHandoffPolicyKey);
    }

    [Fact]
    public void CapabilityMetadataKeepsDecimalCoefficientsAndExistingCapacityRangeAndRoleNames()
    {
        var type = typeof(OrbitalAssetDefinition);
        Assert.Equal(typeof(decimal), type.GetProperty(nameof(OrbitalAssetDefinition.BaseMovementSpeed))!.PropertyType);
        Assert.Equal(typeof(decimal), type.GetProperty(nameof(OrbitalAssetDefinition.FuelConsumptionPerDistanceUnit))!.PropertyType);
        Assert.Equal(typeof(SpaceAssetCapability), type.GetProperty(nameof(OrbitalAssetDefinition.Capabilities))!.PropertyType);
        Assert.Equal(typeof(int), type.GetProperty(nameof(OrbitalAssetDefinition.StorageCapacity))!.PropertyType);
        Assert.Equal(typeof(int), type.GetProperty(nameof(OrbitalAssetDefinition.OperatingRange))!.PropertyType);
        foreach (var duplicate in new[] { "CargoCapacity", "TransportCapacity", "FleetCargoCapacity", "Range", "FleetRole" })
            Assert.Null(type.GetProperty(duplicate));
    }

    [Theory]
    [InlineData((SpaceAssetType)0)]
    [InlineData((SpaceAssetType)(-1))]
    [InlineData((SpaceAssetType)5)]
    public void UnknownAssetLookupRetainsItsExistingFailure(SpaceAssetType type) =>
        Assert.Throws<KeyNotFoundException>(() => OrbitalAssetCatalog.Get(type));

    [Fact]
    public void OrbitalAssetsUseOperatorCapacityInsteadOfPlanetPopulationCapacity()
    {
        var definition = OrbitalAssetCatalog.Get(SpaceAssetType.ScoutCraft);

        Assert.Equal(0, definition.Requirement.PopulationCapacity);
        Assert.True(definition.Requirement.OperatorCapacity > 0);
        Assert.Equal(BuildingType.Shipyard, definition.Requirement.RequiredBuildingType);
        Assert.Equal("Nave exploradora", definition.DisplayName);
        Assert.Equal("ship.scout-craft", definition.ImageKey);
        Assert.Equal("StockToFleetDevelopmentFlow", definition.FleetHandoffPolicyKey);
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
