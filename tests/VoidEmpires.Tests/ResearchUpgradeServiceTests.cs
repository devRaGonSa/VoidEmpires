using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Research;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Research;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.Research;

namespace VoidEmpires.Tests;

public class ResearchUpgradeServiceTests
{
    [Fact]
    public async Task UpgradeAsyncCreatesResearchProjectAndSpendsResources()
    {
        await using var db = CreateDb();
        var civilizationId = Guid.NewGuid();
        var planetId = Guid.NewGuid();
        var stockpile = PlanetResourceStockpile.Create(planetId);
        stockpile.Increase(ResourceType.Metal, 500);
        stockpile.Increase(ResourceType.Crystal, 500);
        db.PlanetResourceStockpiles.Add(stockpile);
        await db.SaveChangesAsync();

        var service = new ResearchUpgradeService(db);

        var result = await service.UpgradeAsync(new UpgradeResearchRequest(
            civilizationId,
            planetId,
            ResearchType.ResourceExtraction));

        Assert.True(result.Succeeded);
        Assert.Equal(1, result.NewLevel);
        Assert.Single(db.ResearchProjects);
        Assert.Equal(380, stockpile.Metal);
        Assert.Equal(420, stockpile.Crystal);
    }

    [Fact]
    public async Task UpgradeAsyncUpgradesExistingResearchProject()
    {
        await using var db = CreateDb();
        var civilizationId = Guid.NewGuid();
        var planetId = Guid.NewGuid();
        var project = ResearchProject.Create(civilizationId, ResearchType.ResourceExtraction);
        var stockpile = PlanetResourceStockpile.Create(planetId);
        stockpile.Increase(ResourceType.Metal, 500);
        stockpile.Increase(ResourceType.Crystal, 500);
        db.ResearchProjects.Add(project);
        db.PlanetResourceStockpiles.Add(stockpile);
        await db.SaveChangesAsync();

        var service = new ResearchUpgradeService(db);

        var result = await service.UpgradeAsync(new UpgradeResearchRequest(
            civilizationId,
            planetId,
            ResearchType.ResourceExtraction));

        Assert.True(result.Succeeded);
        Assert.Equal(2, result.NewLevel);
        Assert.Equal(2, project.Level);
        Assert.Equal(260, stockpile.Metal);
        Assert.Equal(340, stockpile.Crystal);
    }

    [Fact]
    public async Task UpgradeAsyncRejectsMissingStockpile()
    {
        await using var db = CreateDb();
        var service = new ResearchUpgradeService(db);

        var result = await service.UpgradeAsync(new UpgradeResearchRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            ResearchType.ResourceExtraction));

        Assert.False(result.Succeeded);
        Assert.Equal(["Planet resource stockpile was not found."], result.Errors);
    }

    [Fact]
    public async Task UpgradeAsyncRejectsInsufficientResources()
    {
        await using var db = CreateDb();
        var planetId = Guid.NewGuid();
        db.PlanetResourceStockpiles.Add(PlanetResourceStockpile.Create(planetId));
        await db.SaveChangesAsync();

        var service = new ResearchUpgradeService(db);

        var result = await service.UpgradeAsync(new UpgradeResearchRequest(
            Guid.NewGuid(),
            planetId,
            ResearchType.ResourceExtraction));

        Assert.False(result.Succeeded);
        Assert.Equal(["Insufficient resources."], result.Errors);
        Assert.Empty(db.ResearchProjects);
    }

    private static VoidEmpiresDbContext CreateDb()
    {
        return new VoidEmpiresDbContext(
            new DbContextOptionsBuilder<VoidEmpiresDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);
    }
}
