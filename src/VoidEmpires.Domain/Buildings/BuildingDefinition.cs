namespace VoidEmpires.Domain.Buildings;

public sealed record BuildingDefinition(
    BuildingType BuildingType,
    int InitialLevel,
    int Footprint,
    ConstructionCost Cost,
    BuildingCategory Category,
    string DisplayName,
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
    string PrerequisiteSummary);
