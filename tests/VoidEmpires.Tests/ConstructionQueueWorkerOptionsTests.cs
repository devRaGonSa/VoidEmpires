using VoidEmpires.Infrastructure.Buildings;

namespace VoidEmpires.Tests;

public class ConstructionQueueWorkerOptionsTests
{
    [Fact]
    public void GetIntervalUsesConfiguredPositiveSeconds()
    {
        var options = new ConstructionQueueWorkerOptions
        {
            Enabled = true,
            IntervalSeconds = 15
        };

        Assert.Equal(TimeSpan.FromSeconds(15), options.GetInterval());
    }

    [Fact]
    public void GetIntervalFallsBackWhenConfiguredSecondsAreInvalid()
    {
        var options = new ConstructionQueueWorkerOptions
        {
            Enabled = true,
            IntervalSeconds = 0
        };

        Assert.Equal(TimeSpan.FromSeconds(30), options.GetInterval());
    }
}
