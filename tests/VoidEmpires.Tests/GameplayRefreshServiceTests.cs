using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Economy;
using VoidEmpires.Application.Gameplay;
using VoidEmpires.Infrastructure;
using VoidEmpires.Infrastructure.Gameplay;

namespace VoidEmpires.Tests;

public class GameplayRefreshServiceTests
{
    [Fact]
    public async Task RefreshAsyncRejectsNonUtcClock()
    {
        var service = new GameplayRefreshService(new FakeEconomyTickService(), new FakeQueueMaterializationService());

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
        var nowUtc = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
        var economy = new FakeEconomyTickService();
        var queues = new FakeQueueMaterializationService(new MaterializeGameplayQueuesResult(
            true,
            new QueueMaterializationSummary(1, 1, 0),
            new QueueMaterializationSummary(2, 2, 0),
            new QueueMaterializationSummary(3, 3, 0),
            ["queues refreshed"]));
        var service = new GameplayRefreshService(economy, queues);

        var result = await service.RefreshAsync(new GameplayRefreshRequest(
            civilizationId,
            planetId,
            nowUtc));

        Assert.True(result.Succeeded);
        Assert.True(result.Resources.Attempted);
        Assert.Equal(TimeSpan.Zero, result.Resources.AppliedElapsed);
        Assert.Equal(planetId, economy.Requests.Single().PlanetId);
        Assert.Equal(civilizationId, economy.Requests.Single().CivilizationId);
        Assert.Equal(TimeSpan.Zero, economy.Requests.Single().Elapsed);
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
        var service = new GameplayRefreshService(new FakeEconomyTickService(), new FakeQueueMaterializationService());
        var request = new GameplayRefreshRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc));

        var first = await service.RefreshAsync(request);
        var second = await service.RefreshAsync(request);

        Assert.True(first.Succeeded);
        Assert.True(second.Succeeded);
        Assert.Equal(TimeSpan.Zero, first.Resources.AppliedElapsed);
        Assert.Equal(TimeSpan.Zero, second.Resources.AppliedElapsed);
        Assert.Equal(0, second.Construction.ProcessedCount);
        Assert.Equal(0, second.Research.ProcessedCount);
        Assert.Equal(0, second.Production.ProcessedCount);
    }

    [Fact]
    public void PersistenceRegistrationRegistersGameplayRefreshService()
    {
        var services = new ServiceCollection();

        services.AddVoidEmpiresPersistence("Host=localhost;Database=voidempires_test");

        using var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<IGameplayRefreshService>());
    }

    private sealed class FakeEconomyTickService : IPlanetEconomyTickService
    {
        public List<ApplyPlanetProductionRequest> Requests { get; } = [];

        public Task<ApplyPlanetProductionResult> ApplyProductionAsync(
            ApplyPlanetProductionRequest request,
            CancellationToken cancellationToken = default)
        {
            Requests.Add(request);
            return Task.FromResult(ApplyPlanetProductionResult.Success(request.PlanetId));
        }
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
