using Microsoft.EntityFrameworkCore;
using VoidEmpires.Domain.Research;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.Research;

namespace VoidEmpires.Tests;

public class ResearchOrderCompletionServiceTests
{
    [Fact]
    public async Task CompleteDueOrdersAsyncCreatesResearchProjectForDueOrder()
    {
        await using var db = CreateDb();
        var civilizationId = Guid.NewGuid();
        var sourcePlanetId = Guid.NewGuid();
        var startsAtUtc = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var endsAtUtc = startsAtUtc.AddMinutes(10);
        var order = ResearchOrder.Create(
            civilizationId,
            sourcePlanetId,
            ResearchType.PlanetaryEngineering,
            1,
            1,
            startsAtUtc,
            endsAtUtc,
            ResearchQueueItemStatus.Active);

        db.ResearchOrders.Add(order);
        await db.SaveChangesAsync();

        var service = new ResearchOrderCompletionService(db);

        var result = await service.CompleteDueOrdersAsync(endsAtUtc);

        Assert.Equal(1, result.CompletedCount);
        Assert.Equal([order.Id], result.CompletedOrderIds);
        Assert.Equal(ResearchQueueItemStatus.Completed, order.Status);
        var project = Assert.Single(db.ResearchProjects);
        Assert.Equal(civilizationId, project.CivilizationId);
        Assert.Equal(ResearchType.PlanetaryEngineering, project.ResearchType);
        Assert.Equal(1, project.Level);
    }

    [Fact]
    public async Task CompleteDueOrdersAsyncUpgradesExistingResearchProjectForDueOrder()
    {
        await using var db = CreateDb();
        var civilizationId = Guid.NewGuid();
        var sourcePlanetId = Guid.NewGuid();
        var project = ResearchProject.Create(civilizationId, ResearchType.PlanetaryEngineering);
        var startsAtUtc = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var endsAtUtc = startsAtUtc.AddMinutes(20);
        var order = ResearchOrder.Create(
            civilizationId,
            sourcePlanetId,
            ResearchType.PlanetaryEngineering,
            2,
            1,
            startsAtUtc,
            endsAtUtc,
            ResearchQueueItemStatus.Active);

        db.ResearchProjects.Add(project);
        db.ResearchOrders.Add(order);
        await db.SaveChangesAsync();

        var service = new ResearchOrderCompletionService(db);

        var result = await service.CompleteDueOrdersAsync(endsAtUtc);

        Assert.Equal(1, result.CompletedCount);
        Assert.Equal(2, project.Level);
        Assert.Equal(ResearchQueueItemStatus.Completed, order.Status);
    }

    [Fact]
    public async Task CompleteDueOrdersAsyncIgnoresOrdersThatAreNotDue()
    {
        await using var db = CreateDb();
        var civilizationId = Guid.NewGuid();
        var sourcePlanetId = Guid.NewGuid();
        var startsAtUtc = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var endsAtUtc = startsAtUtc.AddMinutes(10);
        var order = ResearchOrder.Create(
            civilizationId,
            sourcePlanetId,
            ResearchType.PlanetaryEngineering,
            1,
            1,
            startsAtUtc,
            endsAtUtc,
            ResearchQueueItemStatus.Active);

        db.ResearchOrders.Add(order);
        await db.SaveChangesAsync();

        var service = new ResearchOrderCompletionService(db);

        var result = await service.CompleteDueOrdersAsync(endsAtUtc.AddTicks(-1));

        Assert.Equal(0, result.CompletedCount);
        Assert.Empty(result.CompletedOrderIds);
        Assert.Equal(ResearchQueueItemStatus.Active, order.Status);
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
