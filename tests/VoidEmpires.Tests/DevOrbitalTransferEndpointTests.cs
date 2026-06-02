using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class DevOrbitalTransferEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid SeededCivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid SeededOwnedPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");
    private static readonly Guid SeededDestinationPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000002");
    private static readonly DateTime SeededRequestedAtUtc = new(2026, 6, 2, 9, 0, 0, DateTimeKind.Utc);
    private static readonly Guid CivilizationId = Guid.Parse("3659791b-71b4-4582-b9cd-ee396930d075");
    private static readonly Guid OrbitalGroupId = Guid.Parse("4a332c10-5a76-407e-8fb5-d1a29ad568bc");
    private static readonly Guid TransferId = Guid.Parse("76d909dc-d854-4714-a9e9-b070fa1fa932");
    private static readonly Guid OriginPlanetId = Guid.Parse("4813dab0-0de6-454c-8c6c-d70944b87bfa");
    private static readonly Guid DestinationPlanetId = Guid.Parse("0dd85c9f-7c1d-4a0d-9247-32ab0d61a8c7");
    private static readonly DateTime RequestedAtUtc = new(2026, 5, 29, 12, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime ArrivalAtUtc = RequestedAtUtc.AddHours(1);

    [Fact]
    public async Task CreateOrbitalTransferReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-transfers/create", ValidCreateRequest());

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateOrbitalTransferReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClientWithPersistenceDisabled();

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-transfers/create", ValidCreateRequest());

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task CreateOrbitalTransferReturnsBadRequestForInvalidRequest()
    {
        using var client = CreateConfiguredClient(persistenceService: new FakeOrbitalTransferPersistenceService(SuccessfulPersistResult()));

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-transfers/create", new { });
        var payload = await response.Content.ReadFromJsonAsync<CreateOrbitalTransferResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(PersistOrbitalTransferResultStatus.ValidationFailed, payload.Status);
        Assert.False(payload.Succeeded);
        Assert.Contains("Civilization id is required.", payload.Errors);
        Assert.Contains("Orbital group id is required.", payload.Errors);
        Assert.Contains("Destination planet id is required.", payload.Errors);
        Assert.Contains("Requested date is required.", payload.Errors);
    }

    [Fact]
    public async Task CreateOrbitalTransferReturnsCreatedForSuccessfulRequest()
    {
        using var client = CreateConfiguredClient(persistenceService: new FakeOrbitalTransferPersistenceService(SuccessfulPersistResult()));

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-transfers/create", ValidCreateRequest());
        var payload = await response.Content.ReadFromJsonAsync<CreateOrbitalTransferResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(PersistOrbitalTransferResultStatus.Succeeded, payload.Status);
        Assert.True(payload.Succeeded);
        Assert.Equal(TransferId, payload.OrbitalTransferId);
        Assert.Equal(OrbitalGroupId, payload.OrbitalGroupId);
        Assert.Equal(OriginPlanetId, payload.OriginPlanetId);
        Assert.Equal(DestinationPlanetId, payload.DestinationPlanetId);
        Assert.Equal(1, payload.AbstractDistanceUnits);
        Assert.Equal(RequestedAtUtc, payload.DepartureAtUtc);
        Assert.Equal(ArrivalAtUtc, payload.ArrivalAtUtc);
        Assert.Empty(payload.Errors);
    }

    [Fact]
    public async Task CreateOrbitalTransferReturnsNotFoundWhenServiceCannotFindOwnedGroup()
    {
        using var client = CreateConfiguredClient(
            persistenceService: new FakeOrbitalTransferPersistenceService(PersistOrbitalTransferResult.NotFound("Orbital group was not found for the civilization.")));

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-transfers/create", ValidCreateRequest());
        var payload = await response.Content.ReadFromJsonAsync<CreateOrbitalTransferResponse>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(PersistOrbitalTransferResultStatus.NotFound, payload.Status);
        Assert.False(payload.Succeeded);
        Assert.Contains("Orbital group was not found for the civilization.", payload.Errors);
    }

    [Fact]
    public async Task CreateOrbitalTransferReturnsConflictWhenServiceRejectsRequest()
    {
        using var client = CreateConfiguredClient(
            persistenceService: new FakeOrbitalTransferPersistenceService(PersistOrbitalTransferResult.Conflict("Orbital group already has an active transfer.")));

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-transfers/create", ValidCreateRequest());
        var payload = await response.Content.ReadFromJsonAsync<CreateOrbitalTransferResponse>();

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(PersistOrbitalTransferResultStatus.Conflict, payload.Status);
        Assert.False(payload.Succeeded);
        Assert.Contains("Orbital group already has an active transfer.", payload.Errors);
    }

    [Fact]
    public async Task CreateOrbitalTransferMutatesSeededStateAndRejectsRepeat()
    {
        const string databaseName = "dev-orbital-transfer-create-seeded-scenario";
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: databaseName);
        using var client = configuredFactory.CreateClient();

        using (var seedResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "minimal-validation" }))
        {
            Assert.Equal(HttpStatusCode.OK, seedResponse.StatusCode);
        }

        using var scope = configuredFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
        var seededGroup = await dbContext.Set<OrbitalGroup>()
            .AsNoTracking()
            .Where(x =>
                x.CivilizationId == SeededCivilizationId &&
                x.CurrentPlanetId == SeededOwnedPlanetId &&
                x.Status == OrbitalGroupStatus.Stationed)
            .OrderByDescending(x => x.Quantity)
            .FirstAsync();
        var stockpileBefore = await dbContext.PlanetResourceStockpiles
            .AsNoTracking()
            .SingleAsync(x => x.PlanetId == SeededOwnedPlanetId);
        var transferCountBefore = await dbContext.Set<OrbitalTransfer>().CountAsync();

        using (var initialUiStateResponse = await client.GetAsync($"/api/dev/fleets/ui-state?civilizationId={SeededCivilizationId}"))
        {
            var initialUiState = await initialUiStateResponse.Content.ReadFromJsonAsync<DevFleetUiStateResponse>();
            Assert.Equal(HttpStatusCode.OK, initialUiStateResponse.StatusCode);
            Assert.NotNull(initialUiState?.UiState);
            Assert.Contains(initialUiState.UiState.Groups, x => x.Id == seededGroup.Id && x.Status == OrbitalGroupStatus.Stationed && !x.HasActiveTransfer);
        }

        Guid createdTransferId;
        using (var createResponse = await client.PostAsJsonAsync("/api/dev/fleets/orbital-transfers/create", new
        {
            civilizationId = SeededCivilizationId,
            orbitalGroupId = seededGroup.Id,
            destinationPlanetId = SeededDestinationPlanetId,
            requestedAtUtc = SeededRequestedAtUtc
        }))
        {
            var createPayload = await createResponse.Content.ReadFromJsonAsync<CreateOrbitalTransferResponse>();
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            Assert.NotNull(createPayload);
            Assert.Equal(PersistOrbitalTransferResultStatus.Succeeded, createPayload.Status);
            Assert.True(createPayload.Succeeded);
            Assert.NotNull(createPayload.OrbitalTransferId);
            Assert.Equal(seededGroup.Id, createPayload.OrbitalGroupId);
            Assert.Equal(SeededOwnedPlanetId, createPayload.OriginPlanetId);
            Assert.Equal(SeededDestinationPlanetId, createPayload.DestinationPlanetId);
            Assert.Equal(1, createPayload.AbstractDistanceUnits);
            Assert.Equal(SeededRequestedAtUtc, createPayload.DepartureAtUtc);
            Assert.Equal(SeededRequestedAtUtc.AddHours(1), createPayload.ArrivalAtUtc);
            createdTransferId = createPayload.OrbitalTransferId.Value;
        }

        using (var repeatedCreateResponse = await client.PostAsJsonAsync("/api/dev/fleets/orbital-transfers/create", new
        {
            civilizationId = SeededCivilizationId,
            orbitalGroupId = seededGroup.Id,
            destinationPlanetId = SeededDestinationPlanetId,
            requestedAtUtc = SeededRequestedAtUtc
        }))
        {
            var repeatedPayload = await repeatedCreateResponse.Content.ReadFromJsonAsync<CreateOrbitalTransferResponse>();
            Assert.Equal(HttpStatusCode.Conflict, repeatedCreateResponse.StatusCode);
            Assert.NotNull(repeatedPayload);
            Assert.Equal(PersistOrbitalTransferResultStatus.Conflict, repeatedPayload.Status);
            Assert.False(repeatedPayload.Succeeded);
            Assert.Contains("Orbital group already has an active transfer.", repeatedPayload.Errors);
        }

        using (var followUpUiStateResponse = await client.GetAsync($"/api/dev/fleets/ui-state?civilizationId={SeededCivilizationId}"))
        {
            var followUpUiState = await followUpUiStateResponse.Content.ReadFromJsonAsync<DevFleetUiStateResponse>();
            Assert.Equal(HttpStatusCode.OK, followUpUiStateResponse.StatusCode);
            Assert.NotNull(followUpUiState?.UiState);
            Assert.Contains(followUpUiState.UiState.Groups, x =>
                x.Id == seededGroup.Id &&
                x.Status == OrbitalGroupStatus.Reserved &&
                x.HasActiveTransfer &&
                x.ActiveTransfer is not null &&
                x.ActiveTransfer.Id == createdTransferId &&
                x.ActiveTransfer.DestinationPlanetId == SeededDestinationPlanetId);
            Assert.Contains(followUpUiState.UiState.ResourceContexts, x =>
                x.PlanetId == SeededOwnedPlanetId &&
                x.Balances.Any(balance => balance.ResourceType == ResourceType.Credits && balance.Quantity == stockpileBefore.Credits - 5) &&
                x.Balances.Any(balance => balance.ResourceType == ResourceType.Gas && balance.Quantity == stockpileBefore.Gas - 2));
        }

        var transferCountAfter = await dbContext.Set<OrbitalTransfer>().CountAsync();
        var createdTransfersForGroup = await dbContext.Set<OrbitalTransfer>()
            .AsNoTracking()
            .Where(x => x.OrbitalGroupId == seededGroup.Id && x.Status == OrbitalTransferStatus.Planned)
            .ToListAsync();
        var persistedGroup = await dbContext.Set<OrbitalGroup>()
            .AsNoTracking()
            .SingleAsync(x => x.Id == seededGroup.Id);
        var stockpileAfter = await dbContext.PlanetResourceStockpiles
            .AsNoTracking()
            .SingleAsync(x => x.PlanetId == SeededOwnedPlanetId);

        Assert.Equal(transferCountBefore + 1, transferCountAfter);
        var createdTransfer = Assert.Single(createdTransfersForGroup);
        Assert.Equal(createdTransferId, createdTransfer.Id);
        Assert.Equal(OrbitalGroupStatus.Reserved, persistedGroup.Status);
        Assert.Equal(stockpileBefore.Credits - 5, stockpileAfter.Credits);
        Assert.Equal(stockpileBefore.Gas - 2, stockpileAfter.Gas);
    }

    [Fact]
    public async Task ListOrbitalTransfersReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClientWithPersistenceDisabled();

        using var response = await client.GetAsync($"/api/dev/fleets/orbital-transfers?civilizationId={CivilizationId}");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task ListOrbitalTransfersReturnsBadRequestWhenCivilizationIdIsMissing()
    {
        using var client = CreateConfiguredClient(lookupService: new FakeOrbitalTransferLookupService([]));

        using var response = await client.GetAsync("/api/dev/fleets/orbital-transfers");
        var payload = await response.Content.ReadFromJsonAsync<OrbitalTransferListResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Empty(payload.Transfers);
        Assert.Contains("Civilization id is required.", payload.Errors);
    }

    [Fact]
    public async Task ListOrbitalTransfersReturnsTransfersForCivilization()
    {
        using var client = CreateConfiguredClient(lookupService: new FakeOrbitalTransferLookupService([
            new OrbitalTransferQueryItem(
                TransferId,
                CivilizationId,
                OrbitalGroupId,
                OriginPlanetId,
                DestinationPlanetId,
                1,
                RequestedAtUtc,
                ArrivalAtUtc,
                OrbitalTransferStatus.Planned)
        ]));

        using var response = await client.GetAsync($"/api/dev/fleets/orbital-transfers?civilizationId={CivilizationId}");
        var payload = await response.Content.ReadFromJsonAsync<OrbitalTransferListResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.Empty(payload.Errors);
        var transfer = Assert.Single(payload.Transfers);
        Assert.Equal(TransferId, transfer.Id);
        Assert.Equal(CivilizationId, transfer.CivilizationId);
        Assert.Equal(OrbitalGroupId, transfer.OrbitalGroupId);
        Assert.Equal(OriginPlanetId, transfer.OriginPlanetId);
        Assert.Equal(DestinationPlanetId, transfer.DestinationPlanetId);
        Assert.Equal(1, transfer.AbstractDistanceUnits);
        Assert.Equal(RequestedAtUtc, transfer.DepartureAtUtc);
        Assert.Equal(ArrivalAtUtc, transfer.ArrivalAtUtc);
        Assert.Equal(OrbitalTransferStatus.Planned, transfer.Status);
    }

    [Fact]
    public async Task ListOrbitalTransfersPassesOptionalFiltersToService()
    {
        var fakeService = new FakeOrbitalTransferLookupService([]);
        using var client = CreateConfiguredClient(lookupService: fakeService);

        using var response = await client.GetAsync(
            $"/api/dev/fleets/orbital-transfers?civilizationId={CivilizationId}&orbitalGroupId={OrbitalGroupId}&originPlanetId={OriginPlanetId}&destinationPlanetId={DestinationPlanetId}&status={OrbitalTransferStatus.Planned}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(fakeService.LastRequest);
        Assert.Equal(CivilizationId, fakeService.LastRequest.CivilizationId);
        Assert.Equal(OrbitalGroupId, fakeService.LastRequest.OrbitalGroupId);
        Assert.Equal(OriginPlanetId, fakeService.LastRequest.OriginPlanetId);
        Assert.Equal(DestinationPlanetId, fakeService.LastRequest.DestinationPlanetId);
        Assert.Equal(OrbitalTransferStatus.Planned, fakeService.LastRequest.Status);
    }

    [Fact]
    public async Task CompleteOrbitalTransfersReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClientWithPersistenceDisabled();

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-transfers/complete-due", ValidCompleteRequest());

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task CompleteOrbitalTransfersReturnsBadRequestForInvalidRequest()
    {
        using var client = CreateConfiguredClient(completionService: new FakeOrbitalTransferCompletionService(SuccessfulCompletionResult()));

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-transfers/complete-due", new { });
        var payload = await response.Content.ReadFromJsonAsync<CompleteOrbitalTransfersResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Equal(0, payload.CompletedCount);
        Assert.Empty(payload.CompletedTransferIds);
        Assert.Empty(payload.CompletedOrbitalGroupIds);
        Assert.Contains("Current date is required.", payload.Errors);
    }

    [Fact]
    public async Task CompleteOrbitalTransfersReturnsBadRequestForNonUtcRequest()
    {
        using var client = CreateConfiguredClient(completionService: new FakeOrbitalTransferCompletionService(SuccessfulCompletionResult()));

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-transfers/complete-due", new
        {
            nowUtc = new DateTime(2026, 5, 29, 13, 0, 0, DateTimeKind.Local)
        });
        var payload = await response.Content.ReadFromJsonAsync<CompleteOrbitalTransfersResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Equal(0, payload.CompletedCount);
        Assert.Empty(payload.CompletedTransferIds);
        Assert.Empty(payload.CompletedOrbitalGroupIds);
        Assert.Contains("Current date must be UTC.", payload.Errors);
    }

    [Fact]
    public async Task CompleteOrbitalTransfersReturnsCompletedTransferAndGroupIds()
    {
        using var client = CreateConfiguredClient(completionService: new FakeOrbitalTransferCompletionService(SuccessfulCompletionResult()));

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-transfers/complete-due", ValidCompleteRequest());
        var payload = await response.Content.ReadFromJsonAsync<CompleteOrbitalTransfersResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.Equal(1, payload.CompletedCount);
        Assert.Contains(TransferId, payload.CompletedTransferIds);
        Assert.Contains(OrbitalGroupId, payload.CompletedOrbitalGroupIds);
        Assert.Empty(payload.Errors);
    }

    [Fact]
    public async Task CompleteOrbitalTransfersReturnsOkWithEmptyCompletionSetForIdempotentRepeat()
    {
        using var client = CreateConfiguredClient(completionService: new FakeOrbitalTransferCompletionService(new CompleteOrbitalTransfersResult(0, [], [])));

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-transfers/complete-due", ValidCompleteRequest());
        var payload = await response.Content.ReadFromJsonAsync<CompleteOrbitalTransfersResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.Equal(0, payload.CompletedCount);
        Assert.Empty(payload.CompletedTransferIds);
        Assert.Empty(payload.CompletedOrbitalGroupIds);
        Assert.Empty(payload.Errors);
    }

    [Fact]
    public async Task CancelOrbitalTransferReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-transfers/cancel", ValidCancelRequest());

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CancelOrbitalTransferReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClientWithPersistenceDisabled();

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-transfers/cancel", ValidCancelRequest());

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task CancelOrbitalTransferReturnsBadRequestForInvalidRequest()
    {
        using var client = CreateConfiguredClient(cancelService: new FakeOrbitalTransferCancelService(SuccessfulCancelResult()));

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-transfers/cancel", new { });
        var payload = await response.Content.ReadFromJsonAsync<CancelOrbitalTransferResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(CancelOrbitalTransferResultStatus.ValidationFailed, payload.Status);
        Assert.False(payload.Succeeded);
        Assert.Contains("Civilization id is required.", payload.Errors);
        Assert.Contains("Orbital transfer id is required.", payload.Errors);
    }

    [Fact]
    public async Task CancelOrbitalTransferReturnsOkForSuccessfulRequest()
    {
        using var client = CreateConfiguredClient(cancelService: new FakeOrbitalTransferCancelService(SuccessfulCancelResult()));

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-transfers/cancel", ValidCancelRequest());
        var payload = await response.Content.ReadFromJsonAsync<CancelOrbitalTransferResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(CancelOrbitalTransferResultStatus.Succeeded, payload.Status);
        Assert.True(payload.Succeeded);
        Assert.Equal(TransferId, payload.OrbitalTransferId);
        Assert.Equal(OrbitalGroupId, payload.OrbitalGroupId);
        Assert.Empty(payload.Errors);
    }

    [Fact]
    public async Task CancelOrbitalTransferReturnsNotFoundWhenServiceReportsMissingTransfer()
    {
        using var client = CreateConfiguredClient(
            cancelService: new FakeOrbitalTransferCancelService(CancelOrbitalTransferResult.NotFound("Orbital transfer was not found.")));

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-transfers/cancel", ValidCancelRequest());
        var payload = await response.Content.ReadFromJsonAsync<CancelOrbitalTransferResponse>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(CancelOrbitalTransferResultStatus.NotFound, payload.Status);
        Assert.False(payload.Succeeded);
        Assert.Null(payload.OrbitalTransferId);
        Assert.Null(payload.OrbitalGroupId);
        Assert.Contains("Orbital transfer was not found.", payload.Errors);
    }

    [Fact]
    public async Task CancelOrbitalTransferReturnsConflictWhenServiceRejectsRequest()
    {
        using var client = CreateConfiguredClient(
            cancelService: new FakeOrbitalTransferCancelService(CancelOrbitalTransferResult.Conflict("Completed orbital transfers cannot be cancelled.")));

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-transfers/cancel", ValidCancelRequest());
        var payload = await response.Content.ReadFromJsonAsync<CancelOrbitalTransferResponse>();

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(CancelOrbitalTransferResultStatus.Conflict, payload.Status);
        Assert.False(payload.Succeeded);
        Assert.Contains("Completed orbital transfers cannot be cancelled.", payload.Errors);
    }

    [Fact]
    public async Task CancelOrbitalTransferReturnsConflictForOwnershipRejection()
    {
        using var client = CreateConfiguredClient(
            cancelService: new FakeOrbitalTransferCancelService(CancelOrbitalTransferResult.Conflict("Orbital transfer does not belong to the civilization.")));

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-transfers/cancel", ValidCancelRequest());
        var payload = await response.Content.ReadFromJsonAsync<CancelOrbitalTransferResponse>();

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(CancelOrbitalTransferResultStatus.Conflict, payload.Status);
        Assert.False(payload.Succeeded);
        Assert.Contains("Orbital transfer does not belong to the civilization.", payload.Errors);
    }

    private HttpClient CreateConfiguredClient(
        IOrbitalTransferPersistenceService? persistenceService = null,
        IOrbitalTransferLookupService? lookupService = null,
        IOrbitalTransferCompletionService? completionService = null,
        IOrbitalTransferCancelService? cancelService = null) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_dev_endpoint_tests"
                }));
            builder.ConfigureTestServices(services =>
            {
                if (persistenceService is not null)
                {
                    services.AddSingleton(persistenceService);
                }

                if (lookupService is not null)
                {
                    services.AddSingleton(lookupService);
                }

                if (completionService is not null)
                {
                    services.AddSingleton(completionService);
                }

                if (cancelService is not null)
                {
                    services.AddSingleton(cancelService);
                }
            });
        }).CreateClient();

    private static PersistOrbitalTransferResult SuccessfulPersistResult() =>
        PersistOrbitalTransferResult.Success(
            TransferId,
            OrbitalGroupId,
            OriginPlanetId,
            DestinationPlanetId,
            1,
            RequestedAtUtc,
            ArrivalAtUtc);

    private static CompleteOrbitalTransfersResult SuccessfulCompletionResult() =>
        new(1, [TransferId], [OrbitalGroupId]);

    private static CancelOrbitalTransferResult SuccessfulCancelResult() =>
        CancelOrbitalTransferResult.Success(TransferId, OrbitalGroupId);

    private static object ValidCreateRequest() => new
    {
        civilizationId = CivilizationId,
        orbitalGroupId = OrbitalGroupId,
        destinationPlanetId = DestinationPlanetId,
        requestedAtUtc = RequestedAtUtc
    };

    private static object ValidCompleteRequest() => new
    {
        nowUtc = ArrivalAtUtc
    };

    private static object ValidCancelRequest() => new
    {
        civilizationId = CivilizationId,
        orbitalTransferId = TransferId
    };

    private sealed class FakeOrbitalTransferPersistenceService(PersistOrbitalTransferResult result) : IOrbitalTransferPersistenceService
    {
        public Task<PersistOrbitalTransferResult> PersistAsync(
            PersistOrbitalTransferRequest request,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(result);
    }

    private sealed class FakeOrbitalTransferLookupService(IReadOnlyList<OrbitalTransferQueryItem> transfers) : IOrbitalTransferLookupService
    {
        public OrbitalTransferQueryRequest? LastRequest { get; private set; }

        public Task<IReadOnlyList<OrbitalTransferQueryItem>> ListAsync(
            OrbitalTransferQueryRequest request,
            CancellationToken cancellationToken = default)
        {
            LastRequest = request;
            return Task.FromResult(transfers);
        }
    }

    private sealed class FakeOrbitalTransferCompletionService(CompleteOrbitalTransfersResult result) : IOrbitalTransferCompletionService
    {
        public Task<CompleteOrbitalTransfersResult> CompleteDueAsync(
            DateTime nowUtc,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(result);
    }

    private sealed class FakeOrbitalTransferCancelService(CancelOrbitalTransferResult result) : IOrbitalTransferCancelService
    {
        public Task<CancelOrbitalTransferResult> CancelAsync(
            CancelOrbitalTransferRequest request,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(result);
    }

    private sealed record CreateOrbitalTransferResponse(
        PersistOrbitalTransferResultStatus Status,
        bool Succeeded,
        Guid? OrbitalTransferId,
        Guid? OrbitalGroupId,
        Guid? OriginPlanetId,
        Guid? DestinationPlanetId,
        int AbstractDistanceUnits,
        DateTime? DepartureAtUtc,
        DateTime? ArrivalAtUtc,
        string[] Errors);

    private sealed record OrbitalTransferListItemResponse(
        Guid Id,
        Guid CivilizationId,
        Guid OrbitalGroupId,
        Guid OriginPlanetId,
        Guid DestinationPlanetId,
        int AbstractDistanceUnits,
        DateTime DepartureAtUtc,
        DateTime ArrivalAtUtc,
        OrbitalTransferStatus Status);

    private sealed record OrbitalTransferListResponse(
        bool Succeeded,
        OrbitalTransferListItemResponse[] Transfers,
        string[] Errors);

    private sealed record CompleteOrbitalTransfersResponse(
        bool Succeeded,
        int CompletedCount,
        Guid[] CompletedTransferIds,
        Guid[] CompletedOrbitalGroupIds,
        string[] Errors);

    private sealed record CancelOrbitalTransferResponse(
        CancelOrbitalTransferResultStatus Status,
        bool Succeeded,
        Guid? OrbitalTransferId,
        Guid? OrbitalGroupId,
        string[] Errors);

    private sealed record DevFleetUiStateResponse(
        bool Succeeded,
        GetDevFleetUiStateResult? UiState,
        string[] Errors);
}
