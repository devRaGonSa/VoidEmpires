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
}
