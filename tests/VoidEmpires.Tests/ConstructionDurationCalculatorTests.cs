using VoidEmpires.Domain.Buildings;

namespace VoidEmpires.Tests;

public class ConstructionDurationCalculatorTests
{
    [Fact]
    public void CalculateDurationReturnsBaseDurationWithoutAutomation()
    {
        var duration = ConstructionDurationCalculator.CalculateDuration(TimeSpan.FromMinutes(10), 0);

        Assert.Equal(TimeSpan.FromMinutes(10), duration);
    }

    [Fact]
    public void CalculateDurationReducesDurationWithAutomation()
    {
        var duration = ConstructionDurationCalculator.CalculateDuration(TimeSpan.FromMinutes(110), 2);

        Assert.Equal(TimeSpan.FromMinutes(100), duration);
    }

    [Fact]
    public void CalculateDurationRejectsNegativeBaseDuration()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ConstructionDurationCalculator.CalculateDuration(TimeSpan.FromSeconds(-1), 0));
    }
}
