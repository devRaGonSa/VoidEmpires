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
            _ => level
        };

        return new ResearchBonus(researchType, level, definition.BonusKey, value);
    }
}
