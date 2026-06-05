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
using VoidEmpires.Domain.Diplomacy;
using VoidEmpires.Infrastructure.Development;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.StrategicMap;

namespace VoidEmpires.Tests;

public class DevAllianceUiStateEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task AllianceUiStateReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.GetAsync($"/api/dev/alliance/ui-state?civilizationId={Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AllianceUiStateReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
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

        using var response = await client.GetAsync($"/api/dev/alliance/ui-state?civilizationId={Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("?civilizationId=00000000-0000-0000-0000-000000000000")]
    public async Task AllianceUiStateReturnsBadRequestForMissingOrEmptyCivilizationId(string queryString)
    {
        using var client = CreateConfiguredClient(new FakeDevAllianceUiStateService(
            new GetDevAllianceUiStateResult(
                Guid.NewGuid(),
                null,
                null,
                [],
                [],
                [],
                null,
                null,
                [],
                [])));

        using var response = await client.GetAsync($"/api/dev/alliance/ui-state{queryString}");
        var payload = await response.Content.ReadFromJsonAsync<DevAllianceUiStateResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Null(payload.UiState);
        Assert.Contains("Civilization id is required.", payload.Errors);
    }

    [Fact]
    public async Task AllianceUiStateReturnsNotFoundForUnknownCivilization()
    {
        using var client = CreateConfiguredClient(CreateDbContext());

        using var response = await client.GetAsync($"/api/dev/alliance/ui-state?civilizationId={Guid.NewGuid()}");
        var payload = await response.Content.ReadFromJsonAsync<DevAllianceUiStateResponse>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Civilization was not found.", payload.Errors);
    }

    [Fact]
    public async Task AllianceUiStateReturnsStableReadModelWithoutMutatingState()
    {
        await using var dbContext = CreateDbContext();
        var seeded = await SeedAllianceScenarioAsync(dbContext);
        var countsBefore = await CaptureCountsAsync(dbContext);
        using var client = CreateConfiguredClient(dbContext);

        using var response = await client.GetAsync($"/api/dev/alliance/ui-state?civilizationId={seeded.CivilizationId}");
        var payload = await response.Content.ReadFromJsonAsync<DevAllianceUiStateResponse>();
        var countsAfter = await CaptureCountsAsync(dbContext);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.UiState);
        Assert.True(payload.Succeeded);
        Assert.NotNull(payload.UiState.Identity);
        Assert.NotNull(payload.UiState.Alliance);
        Assert.NotNull(payload.UiState.ActionSummary);
        Assert.NotNull(payload.UiState.Diagnostics);
        Assert.Equal(seeded.CivilizationId, payload.UiState.Identity.CivilizationId);
        Assert.Equal("Alliance Seed", payload.UiState.Identity.CivilizationName);
        Assert.Equal("ReadOnly", payload.UiState.Alliance.StateKey);
        Assert.True(payload.UiState.Alliance.HasActiveAlliance);
        Assert.Single(payload.UiState.KnownContacts);
        Assert.Equal(3, payload.UiState.FuturePacts.Count);
        Assert.Equal(3, payload.UiState.FutureActions.Count);
        Assert.Contains(payload.UiState.FuturePacts, x => x.PactTypeKey == "TradeIntent" && !x.IsAvailable);
        Assert.Contains(payload.UiState.FutureActions, x => x.ActionKey == "alliance.invitation.future" && !x.IsAvailable);
        Assert.Equal(6, payload.UiState.ActionSummary.DisabledActionCount);
        Assert.Contains(payload.UiState.Limitations, x => x.Contains("read-only", StringComparison.OrdinalIgnoreCase));
        Assert.Equal(countsBefore, countsAfter);
    }

    [Fact]
    public async Task AllianceUiStateReturnsDeterministicReadOnlySeedForCockpitValidation()
    {
        await using var dbContext = CreateDbContext();
        var seedService = new DevelopmentSeedService(dbContext);
        _ = await seedService.ApplyAsync(new ApplyDevelopmentSeedRequest("cockpit-validation"));
        _ = await seedService.ApplyAsync(new ApplyDevelopmentSeedRequest("cockpit-validation"));
        using var client = CreateConfiguredClient(dbContext);

        using var response = await client.GetAsync("/api/dev/alliance/ui-state?civilizationId=00000000-0000-0000-0000-000000000001");
        var payload = await response.Content.ReadFromJsonAsync<DevAllianceUiStateResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.UiState);
        Assert.True(payload.Succeeded);
        Assert.NotNull(payload.UiState.Identity);
        Assert.NotNull(payload.UiState.Alliance);
        Assert.Equal(Guid.Parse("00000000-0000-0000-0000-000000000001"), payload.UiState.Identity.CivilizationId);
        Assert.Equal("Void Seed Civilization", payload.UiState.Identity.CivilizationName);
        Assert.Equal("None", payload.UiState.Alliance.StateKey);
        Assert.False(payload.UiState.Alliance.HasActiveAlliance);
        Assert.Equal(0, payload.UiState.Alliance.ActiveAllianceCount);
        Assert.Single(payload.UiState.KnownContacts);
        Assert.Equal(3, payload.UiState.FuturePacts.Count);
        Assert.Equal(3, payload.UiState.FutureActions.Count);
        Assert.All(payload.UiState.FutureActions, x => Assert.False(x.IsAvailable));
        Assert.Equal(6, payload.UiState.ActionSummary?.DisabledActionCount);
    }

    private HttpClient CreateConfiguredClient(IDevAllianceUiStateService allianceUiStateService) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_dev_alliance_ui_state_tests"
                }));
            builder.ConfigureTestServices(services => services.AddSingleton(allianceUiStateService));
        }).CreateClient();

    private HttpClient CreateConfiguredClient(VoidEmpiresDbContext dbContext) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_dev_alliance_ui_state_seeded_tests"
                }));
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IDevAllianceUiStateService>(new DevAllianceUiStateService(dbContext));
            });
        }).CreateClient();

    private static async Task<(Guid CivilizationId, Guid ContactedCivilizationId)> SeedAllianceScenarioAsync(VoidEmpiresDbContext dbContext)
    {
        var player = Domain.Players.PlayerProfile.Create(Guid.NewGuid().ToString(), "Alliance Tester");
        var civilization = Domain.Players.Civilization.Create(player.Id, "Alliance Seed", Domain.Players.CivilizationArchetype.Balanced);
        var contactTarget = Guid.NewGuid();
        var alliance = Alliance.Create("Void Council", "VC", AllianceStatus.Active, new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc));
        var secondAlliance = Alliance.Create("Star Accord", "STAR", AllianceStatus.Active, new DateTime(2026, 6, 1, 12, 10, 0, DateTimeKind.Utc));

        dbContext.PlayerProfiles.Add(player);
        dbContext.Civilizations.Add(civilization);
        dbContext.Alliances.AddRange(alliance, secondAlliance);
        dbContext.AllianceMemberships.Add(AllianceMembership.Create(
            alliance.Id,
            civilization.Id,
            AllianceMembershipStatus.Active,
            AllianceMembershipRole.Leader,
            new DateTime(2026, 6, 1, 12, 5, 0, DateTimeKind.Utc)));
        dbContext.AlliancePacts.Add(AlliancePact.Create(
            alliance.Id,
            secondAlliance.Id,
            AlliancePactType.NonAggression,
            AlliancePactStatus.Active,
            new DateTime(2026, 6, 1, 12, 20, 0, DateTimeKind.Utc)));
        dbContext.DiplomaticContacts.Add(DiplomaticContact.Create(
            civilization.Id,
            contactTarget,
            DiplomaticContactStatus.Contacted,
            new DateTime(2026, 6, 1, 12, 30, 0, DateTimeKind.Utc),
            "manual-dev"));
        await dbContext.SaveChangesAsync();

        return (civilization.Id, contactTarget);
    }

    private static async Task<(int Alliances, int Memberships, int Pacts, int Contacts)> CaptureCountsAsync(VoidEmpiresDbContext dbContext) =>
        (
            await dbContext.Alliances.CountAsync(),
            await dbContext.AllianceMemberships.CountAsync(),
            await dbContext.AlliancePacts.CountAsync(),
            await dbContext.DiplomaticContacts.CountAsync()
        );

    private static VoidEmpiresDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private sealed class FakeDevAllianceUiStateService(GetDevAllianceUiStateResult result) : IDevAllianceUiStateService
    {
        public Task<GetDevAllianceUiStateResult> GetAsync(GetDevAllianceUiStateRequest request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(result);
        }
    }

    private sealed record DevAllianceUiStateResponse(
        bool Succeeded,
        GetDevAllianceUiStateResult? UiState,
        string[] Errors);
}
