using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Research;

namespace VoidEmpires.Infrastructure.Research;

public static class ResearchEnqueueReadinessEvaluator
{
    public static ResearchEnqueueReadiness Evaluate(
        bool hasOwnedSourcePlanet,
        bool hasOpenOrder,
        PlanetResourceStockpile? stockpile,
        ResearchType researchType,
        int currentLevel)
    {
        var targetLevel = currentLevel + 1;
        var definition = ResearchCatalog.Get(researchType);
        var cost = new ResearchCost(
            definition.BaseCost.Credits * targetLevel,
            definition.BaseCost.Metal * targetLevel,
            definition.BaseCost.Crystal * targetLevel,
            definition.BaseCost.Gas * targetLevel);

        if (!hasOwnedSourcePlanet)
        {
            return new ResearchEnqueueReadiness(
                false,
                "Blocked",
                "SourcePlanetMissing",
                targetLevel,
                cost,
                "Planet was not found.");
        }

        if (hasOpenOrder)
        {
            return new ResearchEnqueueReadiness(
                false,
                "InResearch",
                "OpenQueueSlot",
                targetLevel,
                cost,
                "Civilization already has an open research order.");
        }

        if (stockpile is null)
        {
            return new ResearchEnqueueReadiness(
                false,
                "RequirementPending",
                "SourcePlanetMissing",
                targetLevel,
                cost,
                "Planet resource stockpile was not found.");
        }

        if (!stockpile.CanSpend(cost.Credits, cost.Metal, cost.Crystal, cost.Gas))
        {
            return new ResearchEnqueueReadiness(
                false,
                "InsufficientResources",
                "InsufficientResources",
                targetLevel,
                cost,
                "Insufficient resources.");
        }

        return new ResearchEnqueueReadiness(
            true,
            "Available",
            "Ready",
            targetLevel,
            cost,
            null);
    }
}

public sealed record ResearchEnqueueReadiness(
    bool CanEnqueue,
    string StatusKey,
    string AvailabilityReasonKey,
    int TargetLevel,
    ResearchCost Cost,
    string? Error);
