using Microsoft.EntityFrameworkCore;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Application.Research;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Research;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.Research;

namespace VoidEmpires.Tests;

public class ResearchQueueServiceTests
{
    [Fact]
    public async Task EnqueueAsyncCreatesResearchOrderAndSpendsResources()
    {
        await using var db = CreateDb();
        var civilizationId = Guid.NewGuid();
        var sourcePlanetId = Guid.NewGuid();
        var requestedAtUtc = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var stockpile = PlanetResourceStockpile.Create(sourcePlanetId);
        stockpile.Increase(ResourceType.Metal, 500);
        stockpile.Increase(ResourceType.Crystal, 500);
        db.PlanetResourceStockpiles.Add(stockpile);
        db.PlanetOwnerships.Add(PlanetOwnership.Create(sourcePlanetId, civilizationId));
        await db.SaveChangesAsync();

        var service = new ResearchQueueService(db);

        var result = await service.EnqueueAsync(new EnqueueResearchOrderRequest(
            civilizationId,
            sourcePlanetId,
            ResearchType.PlanetaryEngineering,
            requestedAtUtc));

        Assert.True(result.Succeeded);
        Assert.NotNull(result.OrderId);
        Assert.Equal(requestedAtUtc, result.StartsAtUtc);
        Assert.Equal(requestedAtUtc.AddMinutes(10), result.EndsAtUtc);
        var order = Assert.Single(db.ResearchOrders);
        Assert.Equal(civilizationId, order.CivilizationId);
        Assert.Equal(sourcePlanetId, order.SourcePlanetId);
        Assert.Equal(ResearchType.PlanetaryEngineering, order.ResearchType);
        Assert.Equal(1, order.TargetLevel);
        Assert.Equal(ResearchQueueItemStatus.Active, order.Status);
        Assert.Empty(db.ResearchProjects);
        Assert.Equal(400, stockpile.Metal);
        Assert.Equal(450, stockpile.Crystal);
    }

    [Fact]
    public async Task EnqueueAsyncRejectsSecondOpenResearchOrderForSameCivilization()
    {
        await using var db = CreateDb();
        var civilizationId = Guid.NewGuid();
        var sourcePlanetId = Guid.NewGuid();
        var requestedAtUtc = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        db.ResearchOrders.Add(ResearchOrder.Create(
            civilizationId,
            sourcePlanetId,
            ResearchType.ResourceExtraction,
            1,
            1,
            requestedAtUtc,
            requestedAtUtc.AddMinutes(10),
            ResearchQueueItemStatus.Active));
        db.PlanetResourceStockpiles.Add(PlanetResourceStockpile.Create(sourcePlanetId));
        db.PlanetOwnerships.Add(PlanetOwnership.Create(sourcePlanetId, civilizationId));
        await db.SaveChangesAsync();

        var service = new ResearchQueueService(db);

        var result = await service.EnqueueAsync(new EnqueueResearchOrderRequest(
            civilizationId,
            sourcePlanetId,
            ResearchType.PlanetaryEngineering,
            requestedAtUtc));

        Assert.False(result.Succeeded);
        Assert.Equal(["Civilization already has an open research order."], result.Errors);
    }

    [Fact]
    public async Task EnqueueAsyncUsesNextResearchLevelForCostAndDuration()
    {
        await using var db = CreateDb();
        var civilizationId = Guid.NewGuid();
        var sourcePlanetId = Guid.NewGuid();
        var requestedAtUtc = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var stockpile = PlanetResourceStockpile.Create(sourcePlanetId);
        stockpile.Increase(ResourceType.Metal, 1000);
        stockpile.Increase(ResourceType.Crystal, 1000);
        db.ResearchProjects.Add(ResearchProject.Create(civilizationId, ResearchType.PlanetaryEngineering));
        db.PlanetResourceStockpiles.Add(stockpile);
        db.PlanetOwnerships.Add(PlanetOwnership.Create(sourcePlanetId, civilizationId));
        await db.SaveChangesAsync();

        var service = new ResearchQueueService(db);

        var result = await service.EnqueueAsync(new EnqueueResearchOrderRequest(
            civilizationId,
            sourcePlanetId,
            ResearchType.PlanetaryEngineering,
            requestedAtUtc));

        Assert.True(result.Succeeded);
        var order = Assert.Single(db.ResearchOrders);
        Assert.Equal(2, order.TargetLevel);
        Assert.Equal(requestedAtUtc.AddMinutes(20), result.EndsAtUtc);
        Assert.Equal(800, stockpile.Metal);
        Assert.Equal(900, stockpile.Crystal);
    }

    private static VoidEmpiresDbContext CreateDb()
    {
        return new VoidEmpiresDbContext(
            new DbContextOptionsBuilder<VoidEmpiresDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);
    }
}
