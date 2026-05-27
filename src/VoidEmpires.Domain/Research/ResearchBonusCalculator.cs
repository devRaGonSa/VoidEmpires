namespace VoidEmpires.Domain.Research;

public static class ResearchBonusCalculator
{
    public static decimal GetResourceProductionMultiplier(int resourceExtractionLevel)
    {
        if (resourceExtractionLevel < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(resourceExtractionLevel));
        }

        return 1m + (resourceExtractionLevel * 0.05m);
    }

    public static int GetPlanetaryEngineeringCapacityBonus(int planetaryEngineeringLevel)
    {
        if (planetaryEngineeringLevel < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(planetaryEngineeringLevel));
        }

        return planetaryEngineeringLevel * 10;
    }

    public static decimal GetConstructionSpeedMultiplier(int constructionAutomationLevel)
    {
        if (constructionAutomationLevel < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(constructionAutomationLevel));
        }

        return 1m + (constructionAutomationLevel * 0.05m);
    }

    public static ResearchBonus GetBonus(ResearchType researchType, int level)
    {
        if (level < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(level));
        }

        var definition = ResearchCatalog.Get(researchType);

        var value = researchType switch
        {
            ResearchType.ResourceExtraction => GetResourceProductionMultiplier(level),
            ResearchType.PlanetaryEngineering => GetPlanetaryEngineeringCapacityBonus(level),
            ResearchType.ConstructionAutomation => GetConstructionSpeedMultiplier(level),
            _ => level
        };

        return new ResearchBonus(researchType, level, definition.BonusKey, value);
    }
}
