using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Development;
using VoidEmpires.Application.Players;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Infrastructure.Development;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.Players;

namespace VoidEmpires.Tests;

public class StartingCivilizationServiceTests
{
    private static readonly Guid SeedCivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedOwnedPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");

    [Fact]
    public async Task CreateAsyncCreatesPlayableStartState()
    {
        await using var dbContext = CreateDbContext();
        var service = new StartingCivilizationService(dbContext);

        var result = await service.CreateAsync(new CreateStartingCivilizationRequest(
            "Player One",
            "Solar Dominion",
            "Nova Prime"));

        Assert.True(result.Succeeded);
        Assert.NotNull(result.UserId);
        Assert.NotNull(result.PlayerProfileId);
        Assert.NotNull(result.CivilizationId);
        Assert.NotNull(result.HomePlanetId);
        Assert.NotNull(result.HomeSystemId);
        Assert.Equal("Nova Prime", result.HomePlanetName);
        Assert.Equal(220, result.StartingResources!.Credits);
        Assert.Equal(320, result.StartingResources.Metal);
        Assert.Equal(220, result.StartingResources.Crystal);
        Assert.Equal(120, result.StartingResources.Gas);
        Assert.Contains("Development-only playable start.", result.Limitations);
        Assert.True(await dbContext.PlanetOwnerships.AnyAsync(x => x.PlanetId == result.HomePlanetId && x.CivilizationId == result.CivilizationId));
        Assert.True(await dbContext.PlanetProductionProfiles.AnyAsync(x => x.PlanetId == result.HomePlanetId));
        Assert.True(await dbContext.Set<PlanetBuilding>().AnyAsync(x => x.PlanetId == result.HomePlanetId && x.BuildingType == BuildingType.Shipyard));
    }

    [Fact]
    public async Task CreateAsyncRejectsDuplicateIdentityOrCivilizationName()
    {
        await using var dbContext = CreateDbContext();
        var service = new StartingCivilizationService(dbContext);

        var first = await service.CreateAsync(new CreateStartingCivilizationRequest("Player One", "Solar Dominion"));
        var duplicateDisplay = await service.CreateAsync(new CreateStartingCivilizationRequest("Player One", "Second Dominion"));
        var duplicateCivilization = await service.CreateAsync(new CreateStartingCivilizationRequest("Player Two", "Solar Dominion"));

        Assert.True(first.Succeeded);
        Assert.Equal(["Display name is already in use."], duplicateDisplay.Errors);
        Assert.Equal(["Civilization name is already in use."], duplicateCivilization.Errors);
    }

    [Fact]
    public async Task CreateAsyncRejectsInvalidRequest()
    {
        await using var dbContext = CreateDbContext();
        var service = new StartingCivilizationService(dbContext);

        var result = await service.CreateAsync(new CreateStartingCivilizationRequest(" ", " "));

        Assert.False(result.Succeeded);
        Assert.Contains("Display name is required.", result.Errors);
        Assert.Contains("Civilization name is required.", result.Errors);
    }

    [Fact]
    public async Task CreateAsyncKeepsSeededValidationCivilizationUntouched()
    {
        await using var dbContext = CreateDbContext();
        var seedService = new DevelopmentSeedService(dbContext);
        await seedService.ApplyAsync(new ApplyDevelopmentSeedRequest("cockpit-validation"));

        var service = new StartingCivilizationService(dbContext);
        var result = await service.CreateAsync(new CreateStartingCivilizationRequest("Player One", "Solar Dominion", "Nova Prime"));

        Assert.True(result.Succeeded);
        Assert.True(await dbContext.Civilizations.AnyAsync(x => x.Id == SeedCivilizationId && x.HomePlanetId == SeedOwnedPlanetId));
        Assert.True(await dbContext.PlanetOwnerships.AnyAsync(x => x.PlanetId == SeedOwnedPlanetId && x.CivilizationId == SeedCivilizationId && x.Status == PlanetControlStatus.Active));
        Assert.Equal(2, await dbContext.Civilizations.CountAsync());
        Assert.Equal(2, await dbContext.PlanetOwnerships.CountAsync());
    }

    private static VoidEmpiresDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new VoidEmpiresDbContext(options);
    }
}

public class DevStartingCivilizationEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task StartingCivilizationReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.PostAsJsonAsync("/api/dev/players/starting-civilization", new { displayName = "Player One", civilizationName = "Solar Dominion" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task StartingCivilizationReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClientWithPersistenceDisabled();

        using var response = await client.PostAsJsonAsync("/api/dev/players/starting-civilization", new { displayName = "Player One", civilizationName = "Solar Dominion" });

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task StartingCivilizationReturnsBadRequestForInvalidRequest()
    {
        using var client = CreateConfiguredClient(factory, CreateStartingCivilizationResult.Failure("unused"));

        using var response = await client.PostAsJsonAsync("/api/dev/players/starting-civilization", new { displayName = "", civilizationName = "" });
        var payload = await response.Content.ReadFromJsonAsync<StartingCivilizationResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Display name is required.", payload.Errors);
        Assert.Contains("Civilization name is required.", payload.Errors);
    }

    [Fact]
    public async Task StartingCivilizationReturnsCreatedForPlayableStart()
    {
        var result = CreateStartingCivilizationResult.Success(
            "dev-start-123",
            Guid.Parse("10000000-0000-0000-0000-000000000111"),
            Guid.Parse("10000000-0000-0000-0000-000000000222"),
            Guid.Parse("10000000-0000-0000-0000-000000000333"),
            "Nova Prime",
            Guid.Parse("10000000-0000-0000-0000-000000000444"),
            "Nova Prime System",
            new CreateStartingCivilizationResourceSnapshot(220, 320, 220, 120),
            ["Development-only playable start."]);
        using var client = CreateConfiguredClient(factory, result);

        using var response = await client.PostAsJsonAsync("/api/dev/players/starting-civilization", new { displayName = "Player One", civilizationName = "Solar Dominion", homePlanetName = "Nova Prime" });
        var payload = await response.Content.ReadFromJsonAsync<StartingCivilizationResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.Equal("dev-start-123", payload.UserId);
        Assert.Equal("Nova Prime", payload.HomePlanetName);
        Assert.Equal("Nova Prime System", payload.HomeSystemName);
        Assert.Equal(220, payload.StartingResources!.Credits);
        Assert.Contains("Development-only playable start.", payload.Limitations);
    }

    [Fact]
    public async Task StartingCivilizationReturnsConflictForSafeDuplicateResult()
    {
        using var client = CreateConfiguredClient(factory, CreateStartingCivilizationResult.Failure("Display name is already in use."));

        using var response = await client.PostAsJsonAsync("/api/dev/players/starting-civilization", new { displayName = "Player One", civilizationName = "Solar Dominion" });
        var payload = await response.Content.ReadFromJsonAsync<StartingCivilizationResponse>();

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Display name is already in use.", payload.Errors);
    }

    private static HttpClient CreateConfiguredClient(WebApplicationFactory<Program> factory, CreateStartingCivilizationResult result) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_dev_start_endpoint_tests"
                }));
            builder.ConfigureTestServices(services =>
                services.AddSingleton<IStartingCivilizationService>(new FakeStartingCivilizationService(result)));
        }).CreateClient();

    private sealed class FakeStartingCivilizationService(CreateStartingCivilizationResult result) : IStartingCivilizationService
    {
        public Task<CreateStartingCivilizationResult> CreateAsync(
            CreateStartingCivilizationRequest request,
            CancellationToken cancellationToken = default) => Task.FromResult(result);
    }

    private sealed record StartingCivilizationResponse(
        bool Succeeded,
        string? UserId,
        Guid? PlayerProfileId,
        Guid? CivilizationId,
        Guid? HomePlanetId,
        string? HomePlanetName,
        Guid? HomeSystemId,
        string? HomeSystemName,
        CreateStartingCivilizationResourceSnapshot? StartingResources,
        string[] Limitations,
        string[] Errors);
}
