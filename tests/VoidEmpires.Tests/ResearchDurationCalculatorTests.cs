using VoidEmpires.Domain.Research;

namespace VoidEmpires.Tests;

public class ResearchDurationCalculatorTests
{
    [Fact]
    public void CalculateDurationUsesBaseDurationWithoutEnergySystems()
    {
        var duration = ResearchDurationCalculator.CalculateDuration(TimeSpan.FromMinutes(10), 0);

        Assert.Equal(TimeSpan.FromMinutes(10), duration);
    }

    [Fact]
    public void CalculateDurationReducesDurationWithEnergySystems()
    {
        var duration = ResearchDurationCalculator.CalculateDuration(TimeSpan.FromMinutes(10), 10);

        Assert.True(duration < TimeSpan.FromMinutes(10));
    }

    [Fact]
    public void CalculateDurationRejectsZeroBaseDuration()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ResearchDurationCalculator.CalculateDuration(TimeSpan.Zero, 1));
    }
}
