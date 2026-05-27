using VoidEmpires.Domain.Research;

namespace VoidEmpires.Domain.Buildings;

public static class ConstructionDurationCalculator
{
    public static TimeSpan CalculateDuration(TimeSpan baseDuration, int constructionAutomationLevel)
    {
        if (baseDuration < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(baseDuration));
        }

        var multiplier = ResearchBonusCalculator.GetConstructionSpeedMultiplier(constructionAutomationLevel);
        return TimeSpan.FromTicks((long)(baseDuration.Ticks / multiplier));
    }
}
