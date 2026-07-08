using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Seeding;
using VoidEmpires.Infrastructure;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.SeedData.CatalogSources;

namespace VoidEmpires.Tests;

public class FinalCatalogSeedServiceTests
{
    [Fact]
    public async Task RunAsyncDryRunLoadsAndSummarizesCatalogSources()
    {
        var service = CreateServiceProvider().GetRequiredService<IFinalCatalogSeedService>();

        var result = await service.RunAsync(new FinalCatalogSeedRequest(SourceDirectory: GetCatalogSourceDirectory()));

        Assert.True(result.Succeeded);
        Assert.True(result.DryRun);
        Assert.True(result.ApplyDeferred);
        Assert.Equal(5, result.Catalogs.Count);
        Assert.Contains(result.Catalogs, x => x.CatalogFile == "buildings.catalog.json" && x.RowCount == 19);
        Assert.Contains(result.Notes, x => x.Contains("validation succeeded", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task RunAsyncNonDryRunFailsSafelyWithoutApply()
    {
        var service = CreateServiceProvider().GetRequiredService<IFinalCatalogSeedService>();

        var result = await service.RunAsync(new FinalCatalogSeedRequest(DryRun: false, SourceDirectory: GetCatalogSourceDirectory()));

        Assert.False(result.Succeeded);
        Assert.True(result.ApplyDeferred);
        Assert.Contains(result.Errors, x => x.Contains("DryRun=false is not supported yet", StringComparison.Ordinal));
    }

    private static ServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddVoidEmpiresPersistence("Host=localhost;Database=voidempires_seed_service_tests");
        services.AddDbContext<VoidEmpiresDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddScoped<CatalogSeedSourceLoader>();
        return services.BuildServiceProvider();
    }

    private static string GetCatalogSourceDirectory()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "VoidEmpires.sln")))
            {
                return Path.Combine(current.FullName, "src", "VoidEmpires.Infrastructure", "SeedData", "CatalogSources");
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate the repository root from the test output directory.");
    }
}
