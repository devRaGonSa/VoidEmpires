using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class DevConstructionPersistedFlowTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid SeedCivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedOwnedPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedForeignPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000002");

    [Fact]
    public async Task PlanetFullValidationConstructionEnqueuePersistsAndAppearsInFollowUpRead()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: databaseName);
        using var client = configuredFactory.CreateClient();

        using var seedResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "planet-full-validation" });
        Assert.Equal(HttpStatusCode.OK, seedResponse.StatusCode);

        using var initialResponse = await client.GetAsync($"/api/dev/planets/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var initialPayload = await initialResponse.Content.ReadFromJsonAsync<DevPlanetUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, initialResponse.StatusCode);
        Assert.NotNull(initialPayload?.UiState?.Planet);

        var initialPlanet = initialPayload.UiState.Planet;
        var selectedAction = initialPlanet.ConstructionActions.First(x => x.AvailabilityStatus == "Available");
        var initialStockpile = initialPlanet.Stockpile.ToDictionary(x => x.ResourceType, x => x.Quantity);
        var initialQueueCount = initialPlanet.ConstructionQueue.Length;

        var requestedAtUtc = new DateTime(2026, 6, 4, 12, 0, 0, DateTimeKind.Utc);
        using var enqueueResponse = await client.PostAsJsonAsync(
            "/api/dev/buildings/construction-orders/enqueue",
            new
            {
                planetId = SeedOwnedPlanetId,
                civilizationId = SeedCivilizationId,
                action = selectedAction.Action,
                buildingType = selectedAction.BuildingType,
                requestedAtUtc
            });
        var enqueuePayload = await enqueueResponse.Content.ReadFromJsonAsync<EnqueueConstructionOrderResponse>();

        Assert.Equal(HttpStatusCode.Created, enqueueResponse.StatusCode);
        Assert.NotNull(enqueuePayload);
        Assert.True(enqueuePayload!.Succeeded);
        Assert.NotNull(enqueuePayload.OrderId);

        using var followUpResponse = await client.GetAsync($"/api/dev/planets/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var followUpPayload = await followUpResponse.Content.ReadFromJsonAsync<DevPlanetUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, followUpResponse.StatusCode);
        Assert.NotNull(followUpPayload?.UiState?.Planet);

        var followUpPlanet = followUpPayload.UiState.Planet;
        Assert.Equal(initialQueueCount + 1, followUpPlanet.ConstructionQueue.Length);
        Assert.Equal("Blocked", followUpPlanet.ActionSummary.QueueActionStatus);
        Assert.Contains(
            followUpPlanet.ConstructionQueue,
            x => x.OrderId == enqueuePayload.OrderId &&
                x.Action == selectedAction.Action &&
                x.BuildingType == selectedAction.BuildingType &&
                x.TargetLevel == selectedAction.TargetLevel &&
                x.Status == ConstructionQueueItemStatus.Active);

        foreach (var cost in selectedAction.Cost)
        {
            var before = initialStockpile[cost.ResourceType];
            var after = followUpPlanet.Stockpile.Single(x => x.ResourceType == cost.ResourceType).Quantity;
            Assert.Equal(before - cost.Quantity, after);
        }

        using var scope = configuredFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
        var persistedOrder = await dbContext.Set<PlanetConstructionOrder>()
            .SingleAsync(x => x.Id == enqueuePayload.OrderId!.Value);
        var activeOwnership = await dbContext.Set<PlanetOwnership>()
            .SingleAsync(x => x.PlanetId == SeedOwnedPlanetId && x.CivilizationId == SeedCivilizationId);

        Assert.Equal(SeedOwnedPlanetId, persistedOrder.PlanetId);
        Assert.Equal(selectedAction.BuildingType, persistedOrder.BuildingType);
        Assert.Equal(selectedAction.TargetLevel, persistedOrder.TargetLevel);
        Assert.Equal(ConstructionQueueItemStatus.Active, persistedOrder.Status);
        Assert.Equal(PlanetControlStatus.Active, activeOwnership.Status);
    }

    [Fact]
    public async Task ConstructionEnqueueRejectsForeignPlanetAndSecondOpenOrderWithoutMutatingExtraState()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: databaseName);
        using var client = configuredFactory.CreateClient();

        using var seedResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "planet-full-validation" });
        Assert.Equal(HttpStatusCode.OK, seedResponse.StatusCode);

        using var initialResponse = await client.GetAsync($"/api/dev/planets/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var initialPayload = await initialResponse.Content.ReadFromJsonAsync<DevPlanetUiStateResponse>();
        Assert.Equal(HttpStatusCode.OK, initialResponse.StatusCode);
        Assert.NotNull(initialPayload?.UiState?.Planet);

        var initialPlanet = initialPayload.UiState.Planet;
        var availableAction = initialPlanet.ConstructionActions.First(x => x.AvailabilityStatus == "Available");
        var initialQueueLength = initialPlanet.ConstructionQueue.Length;
        var initialStockpile = initialPlanet.Stockpile.ToDictionary(x => x.ResourceType, x => x.Quantity);

        using var foreignPlanetResponse = await client.PostAsJsonAsync("/api/dev/buildings/construction-orders/enqueue", new
        {
            planetId = SeedForeignPlanetId,
            civilizationId = SeedCivilizationId,
            action = ConstructionQueueItemAction.Construct,
            buildingType = BuildingType.MetalMine,
            requestedAtUtc = "2026-06-04T12:00:00Z"
        });
        var foreignPlanetPayload = await foreignPlanetResponse.Content.ReadFromJsonAsync<EnqueueConstructionOrderResponse>();

        Assert.Equal(HttpStatusCode.Conflict, foreignPlanetResponse.StatusCode);
        Assert.NotNull(foreignPlanetPayload);
        Assert.False(foreignPlanetPayload!.Succeeded);
        Assert.Equal(["Planet is not owned by the requesting civilization."], foreignPlanetPayload.Errors);

        using var firstEnqueueResponse = await client.PostAsJsonAsync("/api/dev/buildings/construction-orders/enqueue", new
        {
            planetId = SeedOwnedPlanetId,
            civilizationId = SeedCivilizationId,
            action = availableAction.Action,
            buildingType = availableAction.BuildingType,
            requestedAtUtc = "2026-06-04T12:05:00Z"
        });
        var firstEnqueuePayload = await firstEnqueueResponse.Content.ReadFromJsonAsync<EnqueueConstructionOrderResponse>();

        Assert.Equal(HttpStatusCode.Created, firstEnqueueResponse.StatusCode);
        Assert.NotNull(firstEnqueuePayload);
        Assert.True(firstEnqueuePayload!.Succeeded);

        using var secondEnqueueResponse = await client.PostAsJsonAsync("/api/dev/buildings/construction-orders/enqueue", new
        {
            planetId = SeedOwnedPlanetId,
            civilizationId = SeedCivilizationId,
            action = availableAction.Action,
            buildingType = availableAction.BuildingType,
            requestedAtUtc = "2026-06-04T12:06:00Z"
        });
        var secondEnqueuePayload = await secondEnqueueResponse.Content.ReadFromJsonAsync<EnqueueConstructionOrderResponse>();

        Assert.Equal(HttpStatusCode.Conflict, secondEnqueueResponse.StatusCode);
        Assert.NotNull(secondEnqueuePayload);
        Assert.False(secondEnqueuePayload!.Succeeded);
        Assert.Equal(["Planet already has an open construction order."], secondEnqueuePayload.Errors);

        using var followUpResponse = await client.GetAsync($"/api/dev/planets/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var followUpPayload = await followUpResponse.Content.ReadFromJsonAsync<DevPlanetUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, followUpResponse.StatusCode);
        Assert.NotNull(followUpPayload?.UiState?.Planet);

        var followUpPlanet = followUpPayload.UiState.Planet;
        Assert.Equal(initialQueueLength + 1, followUpPlanet.ConstructionQueue.Length);
        foreach (var cost in availableAction.Cost)
        {
            var expectedAfterFirstSuccess = initialStockpile[cost.ResourceType] - cost.Quantity;
            var actual = followUpPlanet.Stockpile.Single(x => x.ResourceType == cost.ResourceType).Quantity;
            Assert.Equal(expectedAfterFirstSuccess, actual);
        }
    }

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
        DevPlanetResourceBalanceDto[] Stockpile,
        object? ProductionSummary,
        object? BuildingCapacity,
        object[] Buildings,
        DevPlanetConstructionQueueItemDto[] ConstructionQueue,
        DevPlanetConstructionActionSummaryDto ActionSummary,
        DevPlanetConstructionActionDto[] ConstructionActions,
        object OrbitalContext,
        object Diagnostics);

    private sealed record DevPlanetResourceBalanceDto(
        ResourceType ResourceType,
        decimal Quantity);

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
        DevPlanetResourceBalanceDto[] Cost,
        object? Display);

    private sealed record DevPlanetConstructionActionSummaryDto(
        string QueueActionStatus,
        string QueueActionReason,
        bool CompleteDueSupported,
        string CompleteDueActionStatus,
        string CompleteDueActionReason,
        int DueConstructionCount,
        object? Display);

    private sealed record DevPlanetConstructionActionDto(
        ConstructionQueueItemAction Action,
        BuildingType BuildingType,
        object Category,
        int CurrentLevel,
        int TargetLevel,
        string AvailabilityStatus,
        string AvailabilityReason,
        TimeSpan EstimatedDuration,
        DevPlanetResourceBalanceDto[] Cost,
        object? Display);

    private sealed record EnqueueConstructionOrderResponse(
        bool Succeeded,
        Guid? OrderId,
        DateTime? StartsAtUtc,
        DateTime? EndsAtUtc,
        IReadOnlyList<string> Errors);
}
