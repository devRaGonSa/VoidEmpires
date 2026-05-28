using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VoidEmpires.Infrastructure;

namespace VoidEmpires.Tests;

public class QueueWorkerRegistrationTests
{
    [Fact]
    public void DisabledResearchWorkerConfigurationDoesNotRegisterHostedService()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration("VoidEmpires:ResearchQueueWorker", enabled: false);

        services.AddVoidEmpiresResearchQueueWorker(configuration);

        Assert.DoesNotContain(services, descriptor => descriptor.ServiceType == typeof(IHostedService));
    }

    [Fact]
    public void EnabledResearchWorkerConfigurationRegistersHostedService()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration("VoidEmpires:ResearchQueueWorker", enabled: true);

        services.AddVoidEmpiresResearchQueueWorker(configuration);

        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IHostedService));
    }

    [Fact]
    public void DisabledAssetProductionWorkerConfigurationDoesNotRegisterHostedService()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration("VoidEmpires:AssetProductionWorker", enabled: false);

        services.AddVoidEmpiresAssetProductionWorker(configuration);

        Assert.DoesNotContain(services, descriptor => descriptor.ServiceType == typeof(IHostedService));
    }

    [Fact]
    public void EnabledAssetProductionWorkerConfigurationRegistersHostedService()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration("VoidEmpires:AssetProductionWorker", enabled: true);

        services.AddVoidEmpiresAssetProductionWorker(configuration);

        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IHostedService));
    }

    private static IConfiguration CreateConfiguration(string sectionName, bool enabled)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{sectionName}:Enabled"] = enabled.ToString(),
                [$"{sectionName}:IntervalSeconds"] = "30"
            })
            .Build();
    }
}
