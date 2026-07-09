using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Buildings;
using VoidEmpires.Application.Gameplay;
using VoidEmpires.Application.Research;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Research;
using VoidEmpires.Infrastructure;
using VoidEmpires.Infrastructure.Buildings;
using VoidEmpires.Infrastructure.Economy;
using VoidEmpires.Infrastructure.Gameplay;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.Research;

namespace VoidEmpires.Tests;

public class GameplayRefreshServiceTests
{
    [Fact]
    public async Task RefreshAsyncRejectsNonUtcClock()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);

        var result = await service.RefreshAsync(new GameplayRefreshRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.Now));

        Assert.False(result.Succeeded);
        Assert.Equal(["Refresh date must be UTC."], result.Errors);
    }

    [Fact]
    public async Task RefreshAsyncAppliesSafeResourceRefreshAndMaterializesQueues()
    {
        var civilizationId = Guid.NewGuid();
        var planetId = Guid.NewGuid();
        var nowUtc = new DateTime(2026, 7, 9, 12, 0, 0, DateTimeKind.Utc);
        await using var dbContext = CreateDbContext();
        await SeedEconomyAsync(dbContext, civilizationId, planetId, nowUtc.AddMinutes(-30));
        var queues = new FakeQueueMaterializationService(new MaterializeGameplayQueuesResult(
            true,
            new QueueMaterializationSummary(1, 1, 0),
            new QueueMaterializationSummary(2, 2, 0),
            new QueueMaterializationSummary(3, 3, 0),
            ["queues refreshed"]));
        var service = CreateService(dbContext, queues);

        var result = await service.RefreshAsync(new GameplayRefreshRequest(
            civilizationId,
            planetId,
            nowUtc));
        var stockpile = await dbContext.PlanetResourceStockpiles.SingleAsync(x => x.PlanetId == planetId);

        Assert.True(result.Succeeded);
        Assert.True(result.Resources.Attempted);
        Assert.Equal(TimeSpan.FromMinutes(30), result.Resources.AppliedElapsed);
        Assert.Equal(1, result.Resources.ProcessedPlanetCount);
        Assert.Equal(50, stockpile.Credits);
        Assert.Equal(60, stockpile.Metal);
        Assert.Equal(40, stockpile.Crystal);
        Assert.Equal(20, stockpile.Gas);
        Assert.Equal(nowUtc, stockpile.LastAccruedAtUtc);
        Assert.Equal(civilizationId, queues.Requests.Single().CivilizationId);
        Assert.Equal(planetId, queues.Requests.Single().PlanetId);
        Assert.Equal(nowUtc, queues.Requests.Single().NowUtc);
        Assert.Equal(1, result.Construction.ProcessedCount);
        Assert.Equal(2, result.Research.ProcessedCount);
        Assert.Equal(3, result.Production.ProcessedCount);
    }

    [Fact]
    public async Task RefreshAsyncCanBeRepeatedWithoutAddingResourceElapsed()
    {
        var civilizationId = Guid.NewGuid();
        var planetId = Guid.NewGuid();
        var nowUtc = new DateTime(2026, 7, 9, 12, 0, 0, DateTimeKind.Utc);
        await using var dbContext = CreateDbContext();
        await SeedEconomyAsync(dbContext, civilizationId, planetId, nowUtc.AddHours(-1));
        var service = CreateService(dbContext);
        var request = new GameplayRefreshRequest(
            civilizationId,
            planetId,
            nowUtc);

        var first = await service.RefreshAsync(request);
        var second = await service.RefreshAsync(request);
        var stockpile = await dbContext.PlanetResourceStockpiles.SingleAsync(x => x.PlanetId == planetId);

        Assert.True(first.Succeeded);
        Assert.True(second.Succeeded);
        Assert.Equal(TimeSpan.FromHours(1), first.Resources.AppliedElapsed);
        Assert.Equal(TimeSpan.Zero, second.Resources.AppliedElapsed);
        Assert.Equal(100, stockpile.Credits);
        Assert.Equal(120, stockpile.Metal);
        Assert.Equal(80, stockpile.Crystal);
        Assert.Equal(40, stockpile.Gas);
        Assert.Equal(0, second.Construction.ProcessedCount);
        Assert.Equal(0, second.Research.ProcessedCount);
        Assert.Equal(0, second.Production.ProcessedCount);
    }

    [Fact]
    public async Task RefreshAsyncCapsResourcesAtStockpileCapacity()
    {
        var civilizationId = Guid.NewGuid();
        var planetId = Guid.NewGuid();
        var nowUtc = new DateTime(2026, 7, 9, 12, 0, 0, DateTimeKind.Utc);
        await using var dbContext = CreateDbContext();
        await SeedEconomyAsync(dbContext, civilizationId, planetId, nowUtc.AddHours(-1), capacity: 50);
        var service = CreateService(dbContext);

        var result = await service.RefreshAsync(new GameplayRefreshRequest(
            civilizationId,
            planetId,
            nowUtc));
        var stockpile = await dbContext.PlanetResourceStockpiles.SingleAsync(x => x.PlanetId == planetId);

        Assert.True(result.Succeeded);
        Assert.Equal(50, stockpile.Credits);
        Assert.Equal(50, stockpile.Metal);
        Assert.Equal(50, stockpile.Crystal);
        Assert.Equal(40, stockpile.Gas);
    }

    [Fact]
    public async Task RefreshAsyncCompletesDueConstructionAndUnlocksNextUpgradeAction()
    {
        var civilizationId = Guid.NewGuid();
        var planetId = Guid.NewGuid();
        var nowUtc = new DateTime(2026, 7, 9, 12, 0, 0, DateTimeKind.Utc);
        var startsAtUtc = nowUtc.AddMinutes(-10);
        await using var dbContext = CreateDbContext();
        var stockpile = PlanetResourceStockpile.Create(planetId, lastAccruedAtUtc: nowUtc);
        stockpile.Increase(ResourceType.Metal, 500);
        stockpile.Increase(ResourceType.Crystal, 500);
        dbContext.PlanetOwnerships.Add(PlanetOwnership.Create(planetId, civilizationId));
        dbContext.PlanetResourceStockpiles.Add(stockpile);
        dbContext.PlanetBuildingCapacities.Add(PlanetBuildingCapacity.Create(planetId, 100));
        dbContext.PlanetConstructionOrders.Add(PlanetConstructionOrder.Create(
            planetId,
            ConstructionQueueItemAction.Construct,
            BuildingType.MetalMine,
            1,
            1,
            startsAtUtc,
            nowUtc.AddMinutes(-5),
            ConstructionQueueItemStatus.Active));
        await dbContext.SaveChangesAsync();
        var service = CreateService(dbContext, new GameplayQueueMaterializationService(dbContext));

        var request = new GameplayRefreshRequest(
            civilizationId,
            planetId,
            nowUtc,
            IncludeResources: false);

        var result = await service.RefreshAsync(request);
        var repeatedResult = await service.RefreshAsync(request);

        var building = await dbContext.PlanetBuildings.SingleAsync(x => x.PlanetId == planetId && x.BuildingType == BuildingType.MetalMine);
        var completedOrder = await dbContext.PlanetConstructionOrders.SingleAsync();
        var upgradeResult = await new PlanetConstructionQueueService(dbContext).EnqueueAsync(
            new EnqueueConstructionOrderRequest(
                planetId,
                civilizationId,
                ConstructionQueueItemAction.Upgrade,
                BuildingType.MetalMine,
                nowUtc.AddMinutes(1)));

        Assert.True(result.Succeeded);
        Assert.Equal(1, result.Construction.ProcessedCount);
        Assert.True(repeatedResult.Succeeded);
        Assert.Equal(0, repeatedResult.Construction.ProcessedCount);
        Assert.Equal(1, await dbContext.PlanetBuildings.CountAsync(x => x.PlanetId == planetId && x.BuildingType == BuildingType.MetalMine));
        Assert.Equal(1, building.Level);
        Assert.Equal(ConstructionQueueItemStatus.Completed, completedOrder.Status);
        Assert.True(upgradeResult.Succeeded);
        Assert.Equal(ConstructionQueueItemAction.Upgrade, await dbContext.PlanetConstructionOrders
            .Where(x => x.Status == ConstructionQueueItemStatus.Active)
            .Select(x => x.Action)
            .SingleAsync());
    }

    [Fact]
    public async Task RefreshAsyncCompletesDueResearchAndUnlocksNextResearchLevel()
    {
        var civilizationId = Guid.NewGuid();
        var planetId = Guid.NewGuid();
        var nowUtc = new DateTime(2026, 7, 9, 12, 0, 0, DateTimeKind.Utc);
        var startsAtUtc = nowUtc.AddMinutes(-20);
        await using var dbContext = CreateDbContext();
        var stockpile = PlanetResourceStockpile.Create(planetId, lastAccruedAtUtc: nowUtc);
        stockpile.Increase(ResourceType.Metal, 1_000);
        stockpile.Increase(ResourceType.Crystal, 1_000);
        dbContext.PlanetOwnerships.Add(PlanetOwnership.Create(planetId, civilizationId));
        dbContext.PlanetResourceStockpiles.Add(stockpile);
        dbContext.ResearchOrders.Add(ResearchOrder.Create(
            civilizationId,
            planetId,
            ResearchType.PlanetaryEngineering,
            1,
            1,
            startsAtUtc,
            nowUtc.AddMinutes(-5),
            ResearchQueueItemStatus.Active));
        await dbContext.SaveChangesAsync();
        var service = CreateService(dbContext, new GameplayQueueMaterializationService(dbContext));

        var request = new GameplayRefreshRequest(
            civilizationId,
            planetId,
            nowUtc,
            IncludeResources: false);

        var result = await service.RefreshAsync(request);
        var repeatedResult = await service.RefreshAsync(request);

        var project = await dbContext.ResearchProjects.SingleAsync(x => x.CivilizationId == civilizationId && x.ResearchType == ResearchType.PlanetaryEngineering);
        var completedOrder = await dbContext.ResearchOrders.SingleAsync();
        var nextResearch = await new ResearchQueueService(dbContext).EnqueueAsync(
            new EnqueueResearchOrderRequest(
                civilizationId,
                planetId,
                ResearchType.PlanetaryEngineering,
                nowUtc.AddMinutes(1)));

        Assert.True(result.Succeeded);
        Assert.Equal(1, result.Research.ProcessedCount);
        Assert.True(repeatedResult.Succeeded);
        Assert.Equal(0, repeatedResult.Research.ProcessedCount);
        Assert.Equal(1, await dbContext.ResearchProjects.CountAsync(x => x.CivilizationId == civilizationId && x.ResearchType == ResearchType.PlanetaryEngineering));
        Assert.Equal(1, project.Level);
        Assert.Equal(ResearchQueueItemStatus.Completed, completedOrder.Status);
        Assert.True(nextResearch.Succeeded);
        Assert.Equal(2, await dbContext.ResearchOrders
            .Where(x => x.Status == ResearchQueueItemStatus.Active)
            .Select(x => x.TargetLevel)
            .SingleAsync());
    }

    [Fact]
    public async Task RefreshAsyncUpdatesExistingResearchProjectForDueOrder()
    {
        var civilizationId = Guid.NewGuid();
        var planetId = Guid.NewGuid();
        var nowUtc = new DateTime(2026, 7, 9, 12, 0, 0, DateTimeKind.Utc);
        var startsAtUtc = nowUtc.AddMinutes(-20);
        await using var dbContext = CreateDbContext();
        var project = ResearchProject.Create(civilizationId, ResearchType.ResourceExtraction);
        dbContext.ResearchProjects.Add(project);
        dbContext.ResearchOrders.Add(ResearchOrder.Create(
            civilizationId,
            planetId,
            ResearchType.ResourceExtraction,
            2,
            1,
            startsAtUtc,
            nowUtc.AddMinutes(-5),
            ResearchQueueItemStatus.Active));
        await dbContext.SaveChangesAsync();
        var service = CreateService(dbContext, new GameplayQueueMaterializationService(dbContext));

        var result = await service.RefreshAsync(new GameplayRefreshRequest(
            civilizationId,
            planetId,
            nowUtc,
            IncludeResources: false));

        Assert.True(result.Succeeded);
        Assert.Equal(1, result.Research.ProcessedCount);
        Assert.Equal(1, await dbContext.ResearchProjects.CountAsync(x => x.CivilizationId == civilizationId && x.ResearchType == ResearchType.ResourceExtraction));
        Assert.Equal(2, project.Level);
    }

    [Fact]
    public void PersistenceRegistrationRegistersGameplayRefreshService()
    {
        var services = new ServiceCollection();

        services.AddVoidEmpiresPersistence("Host=localhost;Database=voidempires_test");

        using var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<IGameplayRefreshService>());
    }

    private static GameplayRefreshService CreateService(
        VoidEmpiresDbContext dbContext,
        IGameplayQueueMaterializationService? queues = null) =>
        new(dbContext, new PlanetEconomyTickService(dbContext), queues ?? new FakeQueueMaterializationService());

    private static VoidEmpiresDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new VoidEmpiresDbContext(options);
    }

    private static async Task SeedEconomyAsync(
        VoidEmpiresDbContext dbContext,
        Guid civilizationId,
        Guid planetId,
        DateTime lastAccruedAtUtc,
        decimal capacity = PlanetResourceStockpile.DefaultCapacity)
    {
        dbContext.PlanetOwnerships.Add(PlanetOwnership.Create(planetId, civilizationId));
        dbContext.PlanetProductionProfiles.Add(PlanetProductionProfile.Create(planetId, 100, 120, 80, 40));
        dbContext.PlanetResourceStockpiles.Add(PlanetResourceStockpile.Create(planetId, capacity, lastAccruedAtUtc));
        await dbContext.SaveChangesAsync();
    }

    private sealed class FakeQueueMaterializationService(
        MaterializeGameplayQueuesResult? configuredResult = null)
        : IGameplayQueueMaterializationService
    {
        private readonly MaterializeGameplayQueuesResult _configuredResult = configuredResult
            ?? new MaterializeGameplayQueuesResult(
                true,
                new QueueMaterializationSummary(0, 0, 0),
                new QueueMaterializationSummary(0, 0, 0),
                new QueueMaterializationSummary(0, 0, 0),
                ["queues refreshed"]);

        public List<MaterializeGameplayQueuesRequest> Requests { get; } = [];

        public Task<MaterializeGameplayQueuesResult> MaterializeDueAsync(
            MaterializeGameplayQueuesRequest request,
            CancellationToken cancellationToken = default)
        {
            Requests.Add(request);
            return Task.FromResult(Requests.Count == 1
                ? _configuredResult
                : new MaterializeGameplayQueuesResult(
                    true,
                    new QueueMaterializationSummary(0, 0, 0),
                    new QueueMaterializationSummary(0, 0, 0),
                    new QueueMaterializationSummary(0, 0, 0),
                    ["queues refreshed"]));
        }
    }
}
