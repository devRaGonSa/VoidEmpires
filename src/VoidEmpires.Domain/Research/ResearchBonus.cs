namespace VoidEmpires.Domain.Research;

public sealed record ResearchBonus(
    ResearchType ResearchType,
    int Level,
    string BonusKey,
    decimal Value);
