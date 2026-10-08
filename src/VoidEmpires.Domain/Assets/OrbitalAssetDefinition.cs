using VoidEmpires.Domain.Buildings;

namespace VoidEmpires.Domain.Assets;

/// <summary>Static mission capabilities; full composition and destination eligibility belongs to launch validation.</summary>
[Flags]
public enum SpaceAssetCapability
{
    None = 0,
    Exploration = 1,
    Colonization = 2
}

public sealed record OrbitalAssetDefinition(
    SpaceAssetType AssetType,
    AssetRequirement Requirement,
    ConstructionCost Cost,
    // Authoritative per-ship cargo capacity; the fleet calculator owns summation and resource-unit conversion.
    int StorageCapacity,
    // Authoritative catalog range, distinct from speed and legacy readiness placeholder ranges.
    int OperatingRange,
    // Positive abstract speed coefficient before research modifiers; not a real-world speed or travel duration.
    decimal BaseMovementSpeed,
    // Nonnegative Gas coefficient per ship per distance unit, before route/speed modifiers.
    decimal FuelConsumptionPerDistanceUnit,
    SpaceAssetCapability Capabilities,
    string DisplayName,
    string CategoryKey,
    string CategoryLabel,
    string RoleKey,
    string RoleLabel,
    string Description,
    string ModuleKey,
    string ModuleLabel,
    string ImageKey,
    string IconKey,
    int SortOrder,
    string DurationPolicyKey,
    string DurationPolicyLabel,
    string FleetHandoffPolicyKey,
    string FleetHandoffPolicyLabel,
    string PrerequisiteSummary,
    IReadOnlyList<string> RequirementKeys,
    IReadOnlyList<string> Tags);
