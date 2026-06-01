using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Fleets;

namespace VoidEmpires.Tests;

public class DevOrbitalTransferEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
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
    public async Task CreateOrbitalTransferReturnsConflictWhenServiceRejectsRequest()
    {
        using var client = CreateConfiguredClient(
            persistenceService: new FakeOrbitalTransferPersistenceService(PersistOrbitalTransferResult.Failure("Orbital group already has an active transfer.")));

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-transfers/create", ValidCreateRequest());
        var payload = await response.Content.ReadFromJsonAsync<CreateOrbitalTransferResponse>();

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Orbital group already has an active transfer.", payload.Errors);
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

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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
        Assert.False(payload.Succeeded);
        Assert.Contains("Completed orbital transfers cannot be cancelled.", payload.Errors);
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
        bool Succeeded,
        Guid? OrbitalTransferId,
        Guid? OrbitalGroupId,
        string[] Errors);
}
