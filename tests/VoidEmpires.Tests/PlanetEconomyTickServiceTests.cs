using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Economy;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Research;
using VoidEmpires.Infrastructure.Economy;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class PlanetEconomyTickServiceTests
{
    [Fact]
    public async Task ApplyProductionAsyncIncreasesPersistedStockpile()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var planetId = await SeedEconomyAsync(dbContext, civilizationId);
        var service = new PlanetEconomyTickService(dbContext);

        var result = await service.ApplyProductionAsync(new ApplyPlanetProductionRequest(
            planetId,
            civilizationId,
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
    public async Task ApplyProductionAsyncUsesResourceExtractionResearchBonus()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var planetId = await SeedEconomyAsync(dbContext, civilizationId);
        var project = ResearchProject.Create(civilizationId, ResearchType.ResourceExtraction);
        project.Upgrade();
        dbContext.ResearchProjects.Add(project);
        await dbContext.SaveChangesAsync();
        var service = new PlanetEconomyTickService(dbContext);

        var result = await service.ApplyProductionAsync(new ApplyPlanetProductionRequest(
            planetId,
            civilizationId,
            TimeSpan.FromHours(1)));

        var stockpile = await dbContext.PlanetResourceStockpiles.SingleAsync(item => item.PlanetId == planetId);

        Assert.True(result.Succeeded);
        Assert.Equal(110, stockpile.Credits);
        Assert.Equal(132, stockpile.Metal);
        Assert.Equal(88, stockpile.Crystal);
        Assert.Equal(44, stockpile.Gas);
    }

    [Fact]
    public async Task ApplyProductionAsyncAllowsZeroElapsedTime()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var planetId = await SeedEconomyAsync(dbContext, civilizationId);
        var service = new PlanetEconomyTickService(dbContext);

        var result = await service.ApplyProductionAsync(new ApplyPlanetProductionRequest(
            planetId,
            civilizationId,
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
            Guid.NewGuid(),
            TimeSpan.FromMinutes(1)));

        Assert.False(result.Succeeded);
        Assert.Equal(["Planet id is required."], result.Errors);
    }

    [Fact]
    public async Task ApplyProductionAsyncRejectsEmptyCivilizationId()
    {
        await using var dbContext = CreateDbContext();
        var service = new PlanetEconomyTickService(dbContext);

        var result = await service.ApplyProductionAsync(new ApplyPlanetProductionRequest(
            Guid.NewGuid(),
            Guid.Empty,
            TimeSpan.FromMinutes(1)));

        Assert.False(result.Succeeded);
        Assert.Equal(["Civilization id is required."], result.Errors);
    }

    [Fact]
    public async Task ApplyProductionAsyncRejectsNegativeElapsedTime()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var planetId = await SeedEconomyAsync(dbContext, civilizationId);
        var service = new PlanetEconomyTickService(dbContext);

        var result = await service.ApplyProductionAsync(new ApplyPlanetProductionRequest(
            planetId,
            civilizationId,
            TimeSpan.FromSeconds(-1)));

        Assert.False(result.Succeeded);
        Assert.Equal(["Elapsed time cannot be negative."], result.Errors);
    }

    [Fact]
    public async Task ApplyProductionAsyncRejectsMissingProductionProfile()
    {
        await using var dbContext = CreateDbContext();
        var planetId = Guid.NewGuid();
        var civilizationId = Guid.NewGuid();
        dbContext.PlanetResourceStockpiles.Add(PlanetResourceStockpile.Create(planetId));
        dbContext.PlanetOwnerships.Add(PlanetOwnership.Create(planetId, civilizationId));
        await dbContext.SaveChangesAsync();
        var service = new PlanetEconomyTickService(dbContext);

        var result = await service.ApplyProductionAsync(new ApplyPlanetProductionRequest(
            planetId,
            civilizationId,
            TimeSpan.FromMinutes(1)));

        Assert.False(result.Succeeded);
        Assert.Equal(["Planet production profile was not found."], result.Errors);
    }

    [Fact]
    public async Task ApplyProductionAsyncRejectsMissingStockpile()
    {
        await using var dbContext = CreateDbContext();
        var planetId = Guid.NewGuid();
        var civilizationId = Guid.NewGuid();
        dbContext.PlanetProductionProfiles.Add(PlanetProductionProfile.Create(planetId, 1, 1, 1, 1));
        dbContext.PlanetOwnerships.Add(PlanetOwnership.Create(planetId, civilizationId));
        await dbContext.SaveChangesAsync();
        var service = new PlanetEconomyTickService(dbContext);

        var result = await service.ApplyProductionAsync(new ApplyPlanetProductionRequest(
            planetId,
            civilizationId,
            TimeSpan.FromMinutes(1)));

        Assert.False(result.Succeeded);
        Assert.Equal(["Planet resource stockpile was not found."], result.Errors);
    }

    [Fact]
    public async Task ApplyProductionAsyncRemainsCoherentAfterResourceSpend()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var planetId = await SeedEconomyAsync(dbContext, civilizationId);
        var spendService = new ResourceSpendService(dbContext);
        var economyService = new PlanetEconomyTickService(dbContext);

        await economyService.ApplyProductionAsync(new ApplyPlanetProductionRequest(
            planetId,
            civilizationId,
            TimeSpan.FromHours(1)));

        var spendResult = await spendService.SpendAsync(new ResourceSpendRequest(
            planetId,
            [
                new ResourceCostDto(ResourceType.Credits, 30),
                new ResourceCostDto(ResourceType.Metal, 20),
                new ResourceCostDto(ResourceType.Crystal, 10),
                new ResourceCostDto(ResourceType.Gas, 5)
            ]));

        await economyService.ApplyProductionAsync(new ApplyPlanetProductionRequest(
            planetId,
            civilizationId,
            TimeSpan.FromMinutes(30)));

        var stockpile = await dbContext.PlanetResourceStockpiles.SingleAsync(item => item.PlanetId == planetId);

        Assert.True(spendResult.Succeeded);
        Assert.Equal(120, stockpile.Credits);
        Assert.Equal(160, stockpile.Metal);
        Assert.Equal(110, stockpile.Crystal);
        Assert.Equal(55, stockpile.Gas);
    }

    private static VoidEmpiresDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new VoidEmpiresDbContext(options);
    }

    private static async Task<Guid> SeedEconomyAsync(VoidEmpiresDbContext dbContext, Guid civilizationId)
    {
        var planetId = Guid.NewGuid();

        dbContext.PlanetProductionProfiles.Add(PlanetProductionProfile.Create(planetId, 100, 120, 80, 40));
        dbContext.PlanetResourceStockpiles.Add(PlanetResourceStockpile.Create(planetId));
        dbContext.PlanetOwnerships.Add(PlanetOwnership.Create(planetId, civilizationId));
        await dbContext.SaveChangesAsync();

        return planetId;
    }
}
