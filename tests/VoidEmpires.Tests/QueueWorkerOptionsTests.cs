using VoidEmpires.Infrastructure.Assets;
using VoidEmpires.Infrastructure.Research;

namespace VoidEmpires.Tests;

public class QueueWorkerOptionsTests
{
    [Fact]
    public void ResearchWorkerIntervalUsesConfiguredPositiveSeconds()
    {
        var options = new ResearchQueueWorkerOptions
        {
            Enabled = true,
            IntervalSeconds = 15
        };

        Assert.Equal(TimeSpan.FromSeconds(15), options.GetInterval());
    }

    [Fact]
    public void ResearchWorkerIntervalFallsBackWhenConfiguredSecondsAreInvalid()
    {
        var options = new ResearchQueueWorkerOptions
        {
            Enabled = true,
            IntervalSeconds = 0
        };

        Assert.Equal(TimeSpan.FromSeconds(30), options.GetInterval());
    }

    [Fact]
    public void AssetProductionWorkerIntervalUsesConfiguredPositiveSeconds()
    {
        var options = new AssetProductionWorkerOptions
        {
            Enabled = true,
            IntervalSeconds = 20
        };

        Assert.Equal(TimeSpan.FromSeconds(20), options.GetInterval());
    }

    [Fact]
    public void AssetProductionWorkerIntervalFallsBackWhenConfiguredSecondsAreInvalid()
    {
        var options = new AssetProductionWorkerOptions
        {
            Enabled = true,
            IntervalSeconds = -1
        };

        Assert.Equal(TimeSpan.FromSeconds(30), options.GetInterval());
    }
}
