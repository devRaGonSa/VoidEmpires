namespace VoidEmpires.Domain.Research;

public sealed record ResearchDefinition(
    ResearchType ResearchType,
    ResearchCost BaseCost,
    string BonusKey,
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
    string LevelRuleKey,
    string LevelRuleLabel,
    string MaxLevelPolicyKey,
    IReadOnlyList<string> RequirementKeys,
    IReadOnlyList<string> Tags);
