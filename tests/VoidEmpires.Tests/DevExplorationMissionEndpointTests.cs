using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Exploration;

namespace VoidEmpires.Tests;

public class DevExplorationMissionEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid CivilizationId = Guid.Parse("eea427b3-fb4b-4d83-8fb7-4c38b69ec5d3");
    private static readonly Guid SystemId = Guid.Parse("ff06ed7e-6416-4758-a735-9552413014e9");

    [Fact]
    public async Task CreateReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.PostAsJsonAsync("/api/dev/strategic-map/exploration-missions/create", CreateRequest());

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
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

        using var response = await client.PostAsJsonAsync("/api/dev/strategic-map/exploration-missions/create", CreateRequest());

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task CreateReturnsConflictForVisibilityRejection()
    {
        using var client = CreateConfiguredClient(new FakeExplorationMissionCreateService(
            CreateExplorationMissionResult.Conflict("Target system is not eligible for exploration.")));

        using var response = await client.PostAsJsonAsync("/api/dev/strategic-map/exploration-missions/create", CreateRequest());
        var payload = await response.Content.ReadFromJsonAsync<CreateExplorationMissionResponse>();

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Target system is not eligible for exploration.", payload.Errors);
    }

    [Fact]
    public async Task CreateReturnsCreatedForValidRequest()
    {
        var requestedAtUtc = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);
        var mission = new CreatedExplorationMissionDto(
            Guid.NewGuid(),
            CivilizationId,
            SystemId,
            null,
            ExplorationMissionStatus.Planned,
            requestedAtUtc,
            requestedAtUtc.AddMinutes(30));
        var fakeService = new FakeExplorationMissionCreateService(CreateExplorationMissionResult.Success(mission));
        using var client = CreateConfiguredClient(fakeService);

        using var response = await client.PostAsJsonAsync("/api/dev/strategic-map/exploration-missions/create", CreateRequest(requestedAtUtc));
        var payload = await response.Content.ReadFromJsonAsync<CreateExplorationMissionResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.NotNull(payload.Mission);
        Assert.Equal(mission.ExplorationMissionId, payload.Mission.ExplorationMissionId);
        Assert.Equal(CivilizationId, fakeService.LastRequest?.CivilizationId);
        Assert.Equal(SystemId, fakeService.LastRequest?.TargetSystemId);
    }

    private HttpClient CreateConfiguredClient(IExplorationMissionCreateService createService) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_exploration_mission_endpoint_tests"
                }));
            builder.ConfigureTestServices(services => services.AddSingleton(createService));
        }).CreateClient();

    private static object CreateRequest(DateTime? requestedAtUtc = null) => new
    {
        CivilizationId,
        TargetSystemId = SystemId,
        TargetPlanetId = (Guid?)null,
        RequestedAtUtc = requestedAtUtc ?? new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc)
    };

    private sealed class FakeExplorationMissionCreateService(CreateExplorationMissionResult result) : IExplorationMissionCreateService
    {
        public CreateExplorationMissionRequest? LastRequest { get; private set; }

        public Task<CreateExplorationMissionResult> CreateAsync(
            CreateExplorationMissionRequest request,
            CancellationToken cancellationToken = default)
        {
            LastRequest = request;
            return Task.FromResult(result);
        }
    }

    private sealed record CreateExplorationMissionResponse(
        bool Succeeded,
        CreatedExplorationMissionDto? Mission,
        string[] Errors);
}
