namespace VoidEmpires.Domain.Research;

public sealed record ResearchDefinition(
    ResearchType ResearchType,
    ResearchCost BaseCost,
    string BonusKey);
