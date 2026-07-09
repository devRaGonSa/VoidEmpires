using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Gameplay;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Infrastructure;
using VoidEmpires.Infrastructure.Economy;
using VoidEmpires.Infrastructure.Gameplay;
using VoidEmpires.Infrastructure.Persistence;

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
