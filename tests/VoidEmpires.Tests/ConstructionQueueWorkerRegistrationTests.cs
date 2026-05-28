using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VoidEmpires.Infrastructure;

namespace VoidEmpires.Tests;

public class ConstructionQueueWorkerRegistrationTests
{
    [Fact]
    public void DisabledWorkerConfigurationDoesNotRegisterHostedService()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration(enabled: false);

        services.AddVoidEmpiresConstructionQueueWorker(configuration);

        Assert.DoesNotContain(services, descriptor => descriptor.ServiceType == typeof(IHostedService));
    }

    [Fact]
    public void EnabledWorkerConfigurationRegistersHostedService()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration(enabled: true);

        services.AddVoidEmpiresConstructionQueueWorker(configuration);

        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IHostedService));
    }

    private static IConfiguration CreateConfiguration(bool enabled)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["VoidEmpires:ConstructionQueueWorker:Enabled"] = enabled.ToString(),
                ["VoidEmpires:ConstructionQueueWorker:IntervalSeconds"] = "30"
            })
            .Build();
    }
}
