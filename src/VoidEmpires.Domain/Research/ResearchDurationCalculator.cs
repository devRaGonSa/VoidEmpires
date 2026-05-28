namespace VoidEmpires.Domain.Research;

public static class ResearchDurationCalculator
{
    private const decimal MinimumDurationRatio = 0.25m;

    public static TimeSpan CalculateDuration(TimeSpan baseDuration, int energySystemsLevel)
    {
        if (baseDuration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(baseDuration));
        }

        var speedMultiplier = 1m + Math.Max(0, energySystemsLevel) * 0.05m;
        var durationRatio = Math.Max(MinimumDurationRatio, 1m / speedMultiplier);
        var ticks = (long)Math.Ceiling(baseDuration.Ticks * durationRatio);

        return TimeSpan.FromTicks(ticks);
    }
}
