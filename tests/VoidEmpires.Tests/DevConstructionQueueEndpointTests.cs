using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Development;
using VoidEmpires.Application.Buildings;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class DevConstructionQueueEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private const string DefaultCivilizationId = "00000000-0000-0000-0000-000000000001";
    private const string DefaultPlanetId = "40000000-0000-0000-0000-000000000001";
    private const string ForeignPlanetId = "40000000-0000-0000-0000-000000000002";

    [Fact]
    public async Task EnqueueReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.PostAsJsonAsync("/api/dev/buildings/construction-orders/enqueue", ValidEnqueueRequest());

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task EnqueueReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClientWithPersistenceDisabled();

        using var response = await client.PostAsJsonAsync("/api/dev/buildings/construction-orders/enqueue", ValidEnqueueRequest());

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task EnqueueReturnsBadRequestForInvalidRequest()
    {
        using var client = CreateConfiguredClient(
            EnqueueConstructionOrderResult.Success(Guid.NewGuid(), DateTime.UnixEpoch, DateTime.UnixEpoch.AddMinutes(5)),
            new CompleteConstructionOrdersResult(0, []));

        using var response = await client.PostAsJsonAsync("/api/dev/buildings/construction-orders/enqueue", new { });
        var payload = await response.Content.ReadFromJsonAsync<EnqueueConstructionOrderResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Planet id is required.", payload.Errors);
        Assert.Contains("Civilization id is required.", payload.Errors);
        Assert.Contains("Construction action is required.", payload.Errors);
        Assert.Contains("Building type is required.", payload.Errors);
        Assert.Contains("Requested date is required.", payload.Errors);
    }

    [Fact]
    public async Task EnqueueReturnsCreatedForSuccessfulQueueRequest()
    {
        var orderId = Guid.Parse("fac6178d-9c8c-49bf-9ae5-5af7c09d0a35");
        var startsAtUtc = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var endsAtUtc = startsAtUtc.AddMinutes(5);
        using var client = CreateConfiguredClient(
            EnqueueConstructionOrderResult.Success(orderId, startsAtUtc, endsAtUtc),
            new CompleteConstructionOrdersResult(0, []));

        using var response = await client.PostAsJsonAsync("/api/dev/buildings/construction-orders/enqueue", ValidEnqueueRequest(startsAtUtc));
        var payload = await response.Content.ReadFromJsonAsync<EnqueueConstructionOrderResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.Equal(orderId, payload.OrderId);
        Assert.Equal(startsAtUtc, payload.StartsAtUtc);
        Assert.Equal(endsAtUtc, payload.EndsAtUtc);
        Assert.Empty(payload.Errors);
    }

    [Fact]
    public async Task CompleteDueReturnsOkForSuccessfulCompletionRequest()
    {
        var completedOrderId = Guid.Parse("4910b688-c5de-493c-9a68-822eb7729a85");
        using var client = CreateConfiguredClient(
            EnqueueConstructionOrderResult.Success(Guid.NewGuid(), DateTime.UnixEpoch, DateTime.UnixEpoch.AddMinutes(5)),
            new CompleteConstructionOrdersResult(1, [completedOrderId]));

        using var response = await client.PostAsJsonAsync("/api/dev/buildings/construction-orders/complete-due", new
        {
            nowUtc = new DateTime(2026, 1, 1, 12, 10, 0, DateTimeKind.Utc)
        });
        var payload = await response.Content.ReadFromJsonAsync<CompleteConstructionOrdersResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.Equal(1, payload.CompletedCount);
        Assert.Equal([completedOrderId], payload.CompletedOrderIds);
        Assert.Empty(payload.Errors);
    }

    [Fact]
    public async Task PrepareConstructionQaStateReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.PostAsJsonAsync("/api/dev/construction/qa-state/prepare", new { });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PrepareConstructionQaStateReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClientWithPersistenceDisabled();

        using var response = await client.PostAsJsonAsync("/api/dev/construction/qa-state/prepare", new { });

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task PrepareConstructionQaStateCancelsOpenOrdersAndTopsUpResources()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: databaseName);
        using var client = configuredFactory.CreateClient();

        using (var seedResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "planet-full-validation" }))
        {
            Assert.Equal(HttpStatusCode.OK, seedResponse.StatusCode);
        }

        using (var scope = configuredFactory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();

            var stockpile = await dbContext.PlanetResourceStockpiles
                .SingleAsync(x => x.PlanetId == Guid.Parse(DefaultPlanetId));
            dbContext.Entry(stockpile).Property(x => x.Credits).CurrentValue = 40m;
            dbContext.Entry(stockpile).Property(x => x.Metal).CurrentValue = 20m;
            dbContext.Entry(stockpile).Property(x => x.Crystal).CurrentValue = 10m;
            dbContext.Entry(stockpile).Property(x => x.Gas).CurrentValue = 0m;

            var sequence = await dbContext.PlanetConstructionOrders
                .Where(x => x.PlanetId == Guid.Parse(DefaultPlanetId))
                .Select(x => (int?)x.Sequence)
                .MaxAsync() ?? 0;

            dbContext.PlanetConstructionOrders.Add(PlanetConstructionOrder.Create(
                Guid.Parse(DefaultPlanetId),
                ConstructionQueueItemAction.Construct,
                BuildingType.MetalMine,
                1,
                sequence + 1,
                new DateTime(2026, 6, 4, 12, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 6, 4, 12, 5, 0, DateTimeKind.Utc),
                ConstructionQueueItemStatus.Active));

            await dbContext.SaveChangesAsync();
        }

        using var response = await client.PostAsJsonAsync("/api/dev/construction/qa-state/prepare", new
        {
            civilizationId = DefaultCivilizationId,
            planetId = DefaultPlanetId
        });
        var payload = await response.Content.ReadFromJsonAsync<PrepareConstructionQaStateResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.Equal(1, payload.BlockingOrdersBefore);
        Assert.Equal(0, payload.BlockingOrdersAfter);
        Assert.NotNull(payload.ResourcesAfter);
        Assert.NotNull(payload.ResourcesAfter);
        Assert.True(payload.ResourcesAfter.Credits >= 1000m);
        Assert.True(payload.ResourcesAfter.Metal >= 1000m);
        Assert.True(payload.ResourcesAfter.Crystal >= 1000m);
        Assert.True(payload.ResourcesAfter.Gas >= 1000m);

        using var planResponse = await client.GetAsync($"/api/dev/planets/ui-state?civilizationId={DefaultCivilizationId}&planetId={DefaultPlanetId}");
        var planPayload = await planResponse.Content.ReadFromJsonAsync<DevPlanetUiStateResponse>();
        Assert.Equal(HttpStatusCode.OK, planResponse.StatusCode);
        Assert.NotNull(planPayload?.UiState?.Planet);
        Assert.Equal(0, planPayload.UiState.Planet.ConstructionQueue.Count(x => x.Status == ConstructionQueueItemStatus.Active));
    }

    [Fact]
    public async Task PrepareConstructionQaStateIsIdempotent()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: databaseName);
        using var client = configuredFactory.CreateClient();

        using (var seedResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "planet-full-validation" }))
        {
            Assert.Equal(HttpStatusCode.OK, seedResponse.StatusCode);
        }

        using var firstResponse = await client.PostAsJsonAsync("/api/dev/construction/qa-state/prepare", new
        {
            civilizationId = DefaultCivilizationId,
            planetId = DefaultPlanetId
        });
        var firstPayload = await firstResponse.Content.ReadFromJsonAsync<PrepareConstructionQaStateResponse>();

        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        Assert.NotNull(firstPayload);
        Assert.True(firstPayload.Succeeded);
        Assert.Equal(0, firstPayload.BlockingOrdersBefore);
        Assert.Equal(0, firstPayload.BlockingOrdersAfter);

        using var secondResponse = await client.PostAsJsonAsync("/api/dev/construction/qa-state/prepare", new
        {
            civilizationId = DefaultCivilizationId,
            planetId = DefaultPlanetId
        });
        var secondPayload = await secondResponse.Content.ReadFromJsonAsync<PrepareConstructionQaStateResponse>();

        Assert.Equal(HttpStatusCode.OK, secondResponse.StatusCode);
        Assert.NotNull(secondPayload);
        Assert.True(secondPayload.Succeeded);
        Assert.Equal(0, secondPayload.BlockingOrdersBefore);
        Assert.Equal(0, secondPayload.BlockingOrdersAfter);
    }

    [Fact]
    public async Task PrepareConstructionQaStateDoesNotMutateUnrelatedPlanet()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: databaseName);
        using var client = configuredFactory.CreateClient();

        using (var seedResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "planet-full-validation" }))
        {
            Assert.Equal(HttpStatusCode.OK, seedResponse.StatusCode);
        }

        using (var scope = configuredFactory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();

            var foreignStockpile = PlanetResourceStockpile.Create(Guid.Parse(ForeignPlanetId));
            dbContext.PlanetResourceStockpiles.Add(foreignStockpile);
            dbContext.PlanetOwnerships.Add(PlanetOwnership.Create(
                Guid.Parse(ForeignPlanetId),
                Guid.Parse(DefaultCivilizationId)));
            dbContext.PlanetConstructionOrders.Add(PlanetConstructionOrder.Create(
                Guid.Parse(ForeignPlanetId),
                ConstructionQueueItemAction.Construct,
                BuildingType.CrystalMine,
                1,
                1,
                new DateTime(2026, 6, 4, 12, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 6, 4, 12, 5, 0, DateTimeKind.Utc),
                ConstructionQueueItemStatus.Active));

            await dbContext.SaveChangesAsync();
        }

        using (var scope = configuredFactory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
            var foreignPlanet = Guid.Parse(ForeignPlanetId);

            var foreignOrdersBefore = await dbContext.PlanetConstructionOrders.CountAsync(
                x => x.PlanetId == foreignPlanet && x.Status == ConstructionQueueItemStatus.Active);
            var foreignStockpileBefore = await dbContext.PlanetResourceStockpiles
                .SingleAsync(x => x.PlanetId == foreignPlanet);

            var beforeNotes = new
            {
                foreignOrdersBefore,
                foreignStockpileBefore.Credits,
                foreignStockpileBefore.Metal,
                foreignStockpileBefore.Crystal,
                foreignStockpileBefore.Gas
            };

            using var response = await client.PostAsJsonAsync("/api/dev/construction/qa-state/prepare", new
            {
                civilizationId = DefaultCivilizationId,
                planetId = DefaultPlanetId
            });
            var payload = await response.Content.ReadFromJsonAsync<PrepareConstructionQaStateResponse>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(payload);
            Assert.True(payload.Succeeded);

            var foreignOrdersAfter = await dbContext.PlanetConstructionOrders.CountAsync(
                x => x.PlanetId == foreignPlanet && x.Status == ConstructionQueueItemStatus.Active);
            var foreignStockpileAfter = await dbContext.PlanetResourceStockpiles
                .SingleAsync(x => x.PlanetId == foreignPlanet);

            Assert.Equal(beforeNotes.foreignOrdersBefore, foreignOrdersAfter);
            Assert.Equal(beforeNotes.Credits, foreignStockpileAfter.Credits);
            Assert.Equal(beforeNotes.Metal, foreignStockpileAfter.Metal);
            Assert.Equal(beforeNotes.Crystal, foreignStockpileAfter.Crystal);
            Assert.Equal(beforeNotes.Gas, foreignStockpileAfter.Gas);
        }
    }

    private HttpClient CreateConfiguredClient(
        EnqueueConstructionOrderResult enqueueResult,
        CompleteConstructionOrdersResult completionResult) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_dev_endpoint_tests"
                }));
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IPlanetConstructionQueueService>(new FakeConstructionQueueService(enqueueResult));
                services.AddSingleton<IConstructionOrderCompletionService>(new FakeConstructionOrderCompletionService(completionResult));
            });
        }).CreateClient();

    private static object ValidEnqueueRequest(DateTime? requestedAtUtc = null) => new
    {
        planetId = Guid.Parse("4813dab0-0de6-454c-8c6c-d70944b87bfa"),
        civilizationId = Guid.Parse("3659791b-71b4-4582-b9cd-ee396930d075"),
        action = ConstructionQueueItemAction.Construct,
        buildingType = BuildingType.MetalMine,
        requestedAtUtc = requestedAtUtc ?? new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc)
    };

    private sealed class FakeConstructionQueueService(EnqueueConstructionOrderResult result) : IPlanetConstructionQueueService
    {
        public Task<EnqueueConstructionOrderResult> EnqueueAsync(
            EnqueueConstructionOrderRequest request,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(result);
    }

    private sealed class FakeConstructionOrderCompletionService(CompleteConstructionOrdersResult result) : IConstructionOrderCompletionService
    {
        public Task<CompleteConstructionOrdersResult> CompleteDueOrdersAsync(
            DateTime nowUtc,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(result);
    }

    private sealed record EnqueueConstructionOrderResponse(
        bool Succeeded,
        Guid? OrderId,
        DateTime? StartsAtUtc,
        DateTime? EndsAtUtc,
            string[] Errors);

    private sealed record PrepareConstructionQaStateResponse(
        bool Succeeded,
        ConstructionQaStatePreparationResult? Result,
        int BlockingOrdersBefore,
        int BlockingOrdersAfter,
        ConstructionQaStatePreparationResourceState? ResourcesBefore,
        ConstructionQaStatePreparationResourceState? ResourcesAfter,
        string[] Notes,
        string[] Errors);

    private sealed record DevPlanetUiStateResponse(
        bool Succeeded,
        DevPlanetUiStateResult? UiState,
        string[] Errors);

    private sealed record DevPlanetUiStateResult(
        Guid CivilizationId,
        Guid? SelectedPlanetId,
        DevPlanetCockpitDto? Planet,
        string[] Errors);

    private sealed record DevPlanetCockpitDto(
        Guid PlanetId,
        string PlanetName,
        Guid SolarSystemId,
        string SolarSystemName,
        int OrbitalSlot,
        object PlanetType,
        int Size,
        object ColonizationStatus,
        bool IsOwnedByRequestingCivilization,
        Guid? OwnerCivilizationId,
        string? OwnerCivilizationName,
        object? ControlStatus,
        object[] Stockpile,
        object? ProductionSummary,
        object? BuildingCapacity,
        object[] Buildings,
        DevPlanetConstructionQueueItemDto[] ConstructionQueue,
        object ActionSummary,
        object[] ConstructionActions,
        object OrbitalContext,
        object Diagnostics);

    private sealed record DevPlanetConstructionQueueItemDto(
        Guid OrderId,
        ConstructionQueueItemAction Action,
        ConstructionQueueItemStatus Status,
        BuildingType BuildingType,
        object Category,
        int TargetLevel,
        int Sequence,
        DateTime StartsAtUtc,
        DateTime EndsAtUtc,
        bool IsDue,
        object[] Cost,
        object? Display);

    private sealed record CompleteConstructionOrdersResponse(
        bool Succeeded,
        int CompletedCount,
        Guid[] CompletedOrderIds,
        string[] Errors);
}
