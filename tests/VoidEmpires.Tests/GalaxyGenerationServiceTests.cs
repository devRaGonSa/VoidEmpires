using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Galaxy;
using VoidEmpires.Infrastructure;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class GalaxyGenerationServiceTests
{
    [Fact]
    public async Task GenerateAndPersistAsyncSavesGeneratedGalaxyAggregate()
    {
        await using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IGalaxyGenerationService>();

        var result = await service.GenerateAndPersistAsync(new GenerateAndPersistGalaxyRequest(
            "Void Prime",
            "alpha-001",
            12,
            2,
            5));

        var dbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();

        Assert.True(result.Succeeded);
        Assert.NotNull(result.GalaxyId);
        Assert.Equal("Void Prime", result.GalaxyName);
        Assert.Equal(12, result.SolarSystemCount);
        Assert.InRange(result.PlanetCount, 24, 60);
        Assert.Empty(result.Errors);
        Assert.Equal(1, await dbContext.Galaxies.CountAsync());
        Assert.Equal(12, await dbContext.SolarSystems.CountAsync());
        Assert.Equal(12, await dbContext.Stars.CountAsync());
        Assert.InRange(await dbContext.Planets.CountAsync(), 24, 60);
    }

    [Fact]
    public async Task GenerateAndPersistAsyncRejectsDuplicateGalaxyName()
    {
        await using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IGalaxyGenerationService>();
        var request = new GenerateAndPersistGalaxyRequest("Void Prime", "alpha-001", 4, 1, 2);

        var first = await service.GenerateAndPersistAsync(request);
        var duplicate = await service.GenerateAndPersistAsync(request with { Seed = "beta-001" });

        var dbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();

        Assert.True(first.Succeeded);
        Assert.False(duplicate.Succeeded);
        Assert.Equal(["Galaxy 'Void Prime' already exists."], duplicate.Errors);
        Assert.Equal(1, await dbContext.Galaxies.CountAsync());
    }

    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();

        services.AddDbContext<VoidEmpiresDbContext>(options =>
            options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddVoidEmpiresGalaxyGeneration();

        return services.BuildServiceProvider();
    }
}
