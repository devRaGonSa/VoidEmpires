using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Development;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Development;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.StrategicMap;
using VoidEmpires.Infrastructure.Visuals;

namespace VoidEmpires.Tests;

public class DevEspionageUiStateEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid SeedCivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    [Fact]
    public async Task UiStateReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.GetAsync($"/api/dev/espionage/ui-state?civilizationId={SeedCivilizationId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UiStateReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = string.Empty
                }));
        }).CreateClient();

        using var response = await client.GetAsync($"/api/dev/espionage/ui-state?civilizationId={SeedCivilizationId}");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("?civilizationId=00000000-0000-0000-0000-000000000000")]
    public async Task UiStateReturnsBadRequestForMissingOrEmptyCivilizationId(string queryString)
    {
        using var client = CreateConfiguredClient(new FakeDevEspionageUiStateService(
            new GetDevEspionageUiStateResult(SeedCivilizationId, new(0, 0, 0, 0, 0), [], [], null, [], [], [])));

        using var response = await client.GetAsync($"/api/dev/espionage/ui-state{queryString}");
        var payload = await response.Content.ReadFromJsonAsync<DevEspionageUiStateResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Null(payload.UiState);
        Assert.Contains("Civilization id is required.", payload.Errors);
    }

    [Fact]
    public async Task UiStateReturnsSeededCockpitValidationPayloadWithoutMutatingState()
    {
        await using var dbContext = CreateSeededDbContext("cockpit-validation");
        var countsBefore = await CaptureCounts(dbContext);
        using var client = CreateConfiguredClient(dbContext);

        using var response = await client.GetAsync($"/api/dev/espionage/ui-state?civilizationId={SeedCivilizationId}");
        var payload = await response.Content.ReadFromJsonAsync<DevEspionageUiStateResponse>();
        var countsAfter = await CaptureCounts(dbContext);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.UiState);
        Assert.True(payload.Succeeded);
        Assert.Equal(SeedCivilizationId, payload.UiState.CivilizationId);
        Assert.NotEmpty(payload.UiState.Targets);
        Assert.Contains(payload.UiState.Targets, x => x.PlanetName == "Aurelia");
        Assert.Contains(payload.UiState.Targets, x => x.PlanetName == "Cinder Reach");
        Assert.Contains(payload.UiState.Targets, x => x.PlanetName == "Aether Crown");
        Assert.True(payload.UiState.Overview.OwnedTargetCount >= 1);
        Assert.True(payload.UiState.Overview.VisibleTargetCount >= 1);
        Assert.True(payload.UiState.PassiveSignals.Count >= 1);
        Assert.Contains(payload.UiState.PassiveSignals, x => x.SignalKind == "TransferSignal");
        Assert.NotEmpty(payload.UiState.FutureActions);
        Assert.All(payload.UiState.FutureActions, x => Assert.False(x.IsAvailable));
        Assert.NotEmpty(payload.UiState.Limitations);
        Assert.Equal(countsBefore, countsAfter);
    }

    [Fact]
    public async Task UiStateReturnsStableEmptyStateForUnknownCivilization()
    {
        await using var dbContext = CreateSeededDbContext("cockpit-validation");
        using var client = CreateConfiguredClient(dbContext);

        using var response = await client.GetAsync($"/api/dev/espionage/ui-state?civilizationId={Guid.NewGuid()}");
        var payload = await response.Content.ReadFromJsonAsync<DevEspionageUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.UiState);
        Assert.Empty(payload.UiState.Targets);
        Assert.Empty(payload.UiState.PassiveSignals);
        Assert.Null(payload.UiState.RecommendedFocus);
        Assert.Equal(0, payload.UiState.Overview.OwnedTargetCount);
        Assert.Equal(0, payload.UiState.Overview.VisibleTargetCount);
    }

    private HttpClient CreateConfiguredClient(IDevEspionageUiStateService espionageUiStateService) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_dev_espionage_endpoint_tests"
                }));
            builder.ConfigureTestServices(services => services.AddSingleton(espionageUiStateService));
        }).CreateClient();

    private HttpClient CreateConfiguredClient(VoidEmpiresDbContext dbContext) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_dev_espionage_seeded_tests"
                }));
            builder.ConfigureTestServices(services =>
            {
                var strategicMapService = new StrategicMapService(
                    dbContext,
                    new SystemVisualStateService(dbContext, new PlanetVisualStateService(dbContext)),
                    new MapVisibilityService(dbContext));
                services.AddSingleton<IDevEspionageUiStateService>(new DevEspionageUiStateService(strategicMapService));
            });
        }).CreateClient();

    private static async Task<(int Ownerships, int Knowledge, int Transfers, int Missions)> CaptureCounts(VoidEmpiresDbContext dbContext) =>
        (
            await dbContext.PlanetOwnerships.CountAsync(),
            await dbContext.ExplorationKnowledge.CountAsync(),
            await dbContext.Set<OrbitalTransfer>().CountAsync(),
            await dbContext.ExplorationMissions.CountAsync()
        );

    private static VoidEmpiresDbContext CreateSeededDbContext(string profile)
    {
        var dbContext = new VoidEmpiresDbContext(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
        new DevelopmentSeedService(dbContext).ApplyAsync(new ApplyDevelopmentSeedRequest(profile)).GetAwaiter().GetResult();
        return dbContext;
    }

    private sealed class FakeDevEspionageUiStateService(GetDevEspionageUiStateResult result) : IDevEspionageUiStateService
    {
        public GetDevEspionageUiStateRequest? LastRequest { get; private set; }

        public Task<GetDevEspionageUiStateResult> GetAsync(GetDevEspionageUiStateRequest request, CancellationToken cancellationToken = default)
        {
            LastRequest = request;
            return Task.FromResult(result);
        }
    }

    private sealed record DevEspionageUiStateResponse(
        bool Succeeded,
        GetDevEspionageUiStateResult? UiState,
        string[] Errors);
}
