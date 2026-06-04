using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Research;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class DevResearchPersistedFlowTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid SeedCivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedOwnedPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");

    [Fact]
    public async Task ResearchValidationEnqueuePersistsAndAppearsInFollowUpRead()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: databaseName);
        using var client = configuredFactory.CreateClient();

        using var seedResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "research-validation" });
        Assert.Equal(HttpStatusCode.OK, seedResponse.StatusCode);

        using var initialResponse = await client.GetAsync($"/api/dev/research/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var initialPayload = await initialResponse.Content.ReadFromJsonAsync<DevResearchUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, initialResponse.StatusCode);
        Assert.NotNull(initialPayload?.UiState);

        var initialState = initialPayload.UiState;
        var availableHint = initialState.TechnologyHints.Single(x => x.CanEnqueue);
        var blockedBefore = initialState.TechnologyHints.Where(x => !x.CanEnqueue).Select(x => x.ResearchType).ToHashSet();
        var initialQueueLength = initialState.Queue.Length;
        var initialProjectLength = initialState.Projects.Length;

        using var scopeBefore = configuredFactory.Services.CreateScope();
        var dbBefore = scopeBefore.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
        var stockpileBefore = await dbBefore.PlanetResourceStockpiles.AsNoTracking().SingleAsync(x => x.PlanetId == SeedOwnedPlanetId);
        var resourcesBefore = new Dictionary<ResourceType, decimal>
        {
            [ResourceType.Credits] = stockpileBefore.Credits,
            [ResourceType.Metal] = stockpileBefore.Metal,
            [ResourceType.Crystal] = stockpileBefore.Crystal,
            [ResourceType.Gas] = stockpileBefore.Gas
        };

        using var enqueueResponse = await client.PostAsJsonAsync(
            availableHint.EnqueueCommand!.Route,
            new
            {
                civilizationId = availableHint.EnqueueCommand.CivilizationId,
                sourcePlanetId = availableHint.EnqueueCommand.SourcePlanetId,
                researchType = availableHint.EnqueueCommand.ResearchType.ToString(),
                requestedAtUtc = "2026-06-04T12:00:00Z"
            });
        var enqueuePayload = await enqueueResponse.Content.ReadFromJsonAsync<EnqueueResearchOrderResponse>();

        Assert.Equal(HttpStatusCode.Created, enqueueResponse.StatusCode);
        Assert.NotNull(enqueuePayload);
        Assert.True(enqueuePayload!.Succeeded);
        Assert.NotNull(enqueuePayload.OrderId);

        using var followUpResponse = await client.GetAsync($"/api/dev/research/ui-state?civilizationId={SeedCivilizationId}&planetId={SeedOwnedPlanetId}");
        var followUpPayload = await followUpResponse.Content.ReadFromJsonAsync<DevResearchUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, followUpResponse.StatusCode);
        Assert.NotNull(followUpPayload?.UiState);

        var followUpState = followUpPayload.UiState;
        Assert.Equal(initialQueueLength + 1, followUpState.Queue.Length);
        Assert.Equal(initialProjectLength, followUpState.Projects.Length);
        Assert.Contains(
            followUpState.Queue,
            x => x.Id == enqueuePayload.OrderId &&
                x.CivilizationId == SeedCivilizationId &&
                x.SourcePlanetId == SeedOwnedPlanetId &&
                x.ResearchType == availableHint.ResearchType &&
                x.TargetLevel == availableHint.NextLevel &&
                x.Status == ResearchQueueItemStatus.Active);
        Assert.Contains(
            followUpState.TechnologyHints,
            x => x.ResearchType == availableHint.ResearchType &&
                !x.CanEnqueue &&
                x.StatusKey == "InResearch");
        Assert.All(blockedBefore, researchType =>
            Assert.Contains(
                followUpState.TechnologyHints,
                x => x.ResearchType == researchType && !x.CanEnqueue));
        Assert.Contains(
            followUpState.Projects,
            x => x.ResearchType == ResearchType.EnergySystems && x.Level == 1);
        Assert.Contains(
            followUpState.Queue,
            x => x.ResearchType == ResearchType.EnergySystems && x.Status == ResearchQueueItemStatus.Completed);

        using var scopeAfter = configuredFactory.Services.CreateScope();
        var dbAfter = scopeAfter.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
        var stockpileAfter = await dbAfter.PlanetResourceStockpiles.AsNoTracking().SingleAsync(x => x.PlanetId == SeedOwnedPlanetId);
        var persistedOrder = await dbAfter.ResearchOrders.AsNoTracking().SingleAsync(x => x.Id == enqueuePayload.OrderId!.Value);

        Assert.Equal(resourcesBefore[ResourceType.Credits] - availableHint.EstimatedCost.Credits, stockpileAfter.Credits);
        Assert.Equal(resourcesBefore[ResourceType.Metal] - availableHint.EstimatedCost.Metal, stockpileAfter.Metal);
        Assert.Equal(resourcesBefore[ResourceType.Crystal] - availableHint.EstimatedCost.Crystal, stockpileAfter.Crystal);
        Assert.Equal(resourcesBefore[ResourceType.Gas] - availableHint.EstimatedCost.Gas, stockpileAfter.Gas);
        Assert.Equal(SeedCivilizationId, persistedOrder.CivilizationId);
        Assert.Equal(SeedOwnedPlanetId, persistedOrder.SourcePlanetId);
        Assert.Equal(availableHint.ResearchType, persistedOrder.ResearchType);
        Assert.Equal(availableHint.NextLevel, persistedOrder.TargetLevel);
        Assert.Equal(ResearchQueueItemStatus.Active, persistedOrder.Status);
    }

    private sealed record DevResearchUiStateResponse(
        bool Succeeded,
        DevResearchUiStateResult? UiState,
        string[] Errors);

    private sealed record DevResearchUiStateResult(
        Guid CivilizationId,
        Guid? SelectedPlanetId,
        string? SelectedPlanetName,
        object[] Catalog,
        DevResearchOrderDto[] Queue,
        DevResearchProjectDto[] Projects,
        DevResearchTechnologyHintDto[] TechnologyHints,
        string[] Diagnostics,
        string[] Limitations);

    private sealed record DevResearchOrderDto(
        Guid Id,
        Guid CivilizationId,
        Guid SourcePlanetId,
        ResearchType ResearchType,
        int TargetLevel,
        int Sequence,
        DateTime StartsAtUtc,
        DateTime EndsAtUtc,
        ResearchQueueItemStatus Status);

    private sealed record DevResearchProjectDto(
        Guid CivilizationId,
        ResearchType ResearchType,
        int Level);

    private sealed record DevResearchTechnologyHintDto(
        ResearchType ResearchType,
        int CurrentLevel,
        int NextLevel,
        string StatusKey,
        string AvailabilityReasonKey,
        bool CanEnqueue,
        bool CanCompleteDue,
        TimeSpan EstimatedDuration,
        ResearchCost EstimatedCost,
        DevResearchEnqueueCommandDto? EnqueueCommand,
        IReadOnlyList<string> RequirementKeys);

    private sealed record DevResearchEnqueueCommandDto(
        string ActionKey,
        string Method,
        string Route,
        Guid CivilizationId,
        Guid SourcePlanetId,
        ResearchType ResearchType,
        int TargetLevel);

    private sealed record EnqueueResearchOrderResponse(
        bool Succeeded,
        Guid? OrderId,
        DateTime? StartsAtUtc,
        DateTime? EndsAtUtc,
        IReadOnlyList<string> Errors);
}
