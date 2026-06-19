using VoidEmpires.Domain.Buildings;

namespace VoidEmpires.Domain.Assets;

public sealed record OrbitalAssetDefinition(
    SpaceAssetType AssetType,
    AssetRequirement Requirement,
    ConstructionCost Cost,
    int StorageCapacity,
    int OperatingRange,
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
