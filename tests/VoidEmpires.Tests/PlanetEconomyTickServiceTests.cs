using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Economy;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Infrastructure.Economy;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class PlanetEconomyTickServiceTests
{
    [Fact]
    public async Task ApplyProductionAsyncIncreasesPersistedStockpile()
    {
        await using var dbContext = CreateDbContext();
        var planetId = await SeedEconomyAsync(dbContext);
        var service = new PlanetEconomyTickService(dbContext);

        var result = await service.ApplyProductionAsync(new ApplyPlanetProductionRequest(
            planetId,
            TimeSpan.FromMinutes(30)));

        var stockpile = await dbContext.PlanetResourceStockpiles.SingleAsync(item => item.PlanetId == planetId);

        Assert.True(result.Succeeded);
        Assert.Equal(planetId, result.PlanetId);
        Assert.Empty(result.Errors);
        Assert.Equal(50, stockpile.Credits);
        Assert.Equal(60, stockpile.Metal);
        Assert.Equal(40, stockpile.Crystal);
        Assert.Equal(20, stockpile.Gas);
    }

    [Fact]
    public async Task ApplyProductionAsyncAllowsZeroElapsedTime()
    {
        await using var dbContext = CreateDbContext();
        var planetId = await SeedEconomyAsync(dbContext);
        var service = new PlanetEconomyTickService(dbContext);

        var result = await service.ApplyProductionAsync(new ApplyPlanetProductionRequest(
            planetId,
            TimeSpan.Zero));

        var stockpile = await dbContext.PlanetResourceStockpiles.SingleAsync(item => item.PlanetId == planetId);

        Assert.True(result.Succeeded);
        Assert.Equal(0, stockpile.Credits);
        Assert.Equal(0, stockpile.Metal);
        Assert.Equal(0, stockpile.Crystal);
        Assert.Equal(0, stockpile.Gas);
    }

    [Fact]
    public async Task ApplyProductionAsyncRejectsEmptyPlanetId()
    {
        await using var dbContext = CreateDbContext();
        var service = new PlanetEconomyTickService(dbContext);

        var result = await service.ApplyProductionAsync(new ApplyPlanetProductionRequest(
            Guid.Empty,
            TimeSpan.FromMinutes(1)));

        Assert.False(result.Succeeded);
        Assert.Equal(["Planet id is required."], result.Errors);
    }

    [Fact]
    public async Task ApplyProductionAsyncRejectsNegativeElapsedTime()
    {
        await using var dbContext = CreateDbContext();
        var planetId = await SeedEconomyAsync(dbContext);
        var service = new PlanetEconomyTickService(dbContext);

        var result = await service.ApplyProductionAsync(new ApplyPlanetProductionRequest(
            planetId,
            TimeSpan.FromSeconds(-1)));

        Assert.False(result.Succeeded);
        Assert.Equal(["Elapsed time cannot be negative."], result.Errors);
    }

    [Fact]
    public async Task ApplyProductionAsyncRejectsMissingProductionProfile()
    {
        await using var dbContext = CreateDbContext();
        var planetId = Guid.NewGuid();
        dbContext.PlanetResourceStockpiles.Add(PlanetResourceStockpile.Create(planetId));
        await dbContext.SaveChangesAsync();
        var service = new PlanetEconomyTickService(dbContext);

        var result = await service.ApplyProductionAsync(new ApplyPlanetProductionRequest(
            planetId,
            TimeSpan.FromMinutes(1)));

        Assert.False(result.Succeeded);
        Assert.Equal(["Planet production profile was not found."], result.Errors);
    }

    [Fact]
    public async Task ApplyProductionAsyncRejectsMissingStockpile()
    {
        await using var dbContext = CreateDbContext();
        var planetId = Guid.NewGuid();
        dbContext.PlanetProductionProfiles.Add(PlanetProductionProfile.Create(planetId, 1, 1, 1, 1));
        await dbContext.SaveChangesAsync();
        var service = new PlanetEconomyTickService(dbContext);

        var result = await service.ApplyProductionAsync(new ApplyPlanetProductionRequest(
            planetId,
            TimeSpan.FromMinutes(1)));

        Assert.False(result.Succeeded);
        Assert.Equal(["Planet resource stockpile was not found."], result.Errors);
    }

    private static VoidEmpiresDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new VoidEmpiresDbContext(options);
    }

    private static async Task<Guid> SeedEconomyAsync(VoidEmpiresDbContext dbContext)
    {
        var planetId = Guid.NewGuid();

        dbContext.PlanetProductionProfiles.Add(PlanetProductionProfile.Create(planetId, 100, 120, 80, 40));
        dbContext.PlanetResourceStockpiles.Add(PlanetResourceStockpile.Create(planetId));
        await dbContext.SaveChangesAsync();

        return planetId;
    }
}
