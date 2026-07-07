using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Identity;
using VoidEmpires.Application.Planets;
using VoidEmpires.Application.Players;
using VoidEmpires.Infrastructure;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class AuthenticatedPlayableLoopSmokeTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task RegisteredUserCanLoginReadCurrentSessionAndFetchHomePlanetState()
    {
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: Guid.NewGuid().ToString("N"))
            .WithWebHostBuilder(builder => builder.ConfigureTestServices(services => services.AddVoidEmpiresIdentity()));
        using var client = configuredFactory.CreateClient();
        var suffix = Guid.NewGuid().ToString("N")[..10];
        var request = new AccountRegistrationRequest(
            $"loop-{suffix}@example.test",
            $"Loop!7Aa{suffix}",
            $"Loop!7Aa{suffix}",
            $"Loop Commander {suffix}",
            $"Loop Dominion {suffix}",
            $"Loop Prime {suffix}");

        using var register = await client.PostAsJsonAsync("/api/accounts/register", request);
        var registration = await register.Content.ReadFromJsonAsync<AccountRegistrationResponse>();
        Assert.Equal(HttpStatusCode.Created, register.StatusCode);
        Assert.NotNull(registration);
        Assert.True(registration.Succeeded);
        Assert.NotNull(registration.CivilizationId);
        Assert.NotNull(registration.HomePlanetId);

        using var login = await client.PostAsJsonAsync("/api/accounts/login", new AccountLoginRequest(request.Email, request.Password));
        var loginSession = await login.Content.ReadFromJsonAsync<AccountSessionResult>();
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        Assert.NotNull(loginSession);
        Assert.True(loginSession.Succeeded);
        Assert.Equal(registration.CivilizationId, loginSession.CivilizationId);
        Assert.Equal(registration.HomePlanetId, loginSession.HomePlanetId);

        using var me = await client.GetAsync("/api/accounts/me");
        var currentSession = await me.Content.ReadFromJsonAsync<CurrentAccountSession>();
        Assert.Equal(HttpStatusCode.OK, me.StatusCode);
        Assert.NotNull(currentSession);
        Assert.True(currentSession.Succeeded);
        Assert.Equal(registration.CivilizationId, currentSession.CivilizationId);
        Assert.Equal(registration.HomePlanetId, currentSession.HomePlanetId);

        using var planetState = await client.GetAsync($"/api/dev/planets/ui-state?civilizationId={registration.CivilizationId}&planetId={registration.HomePlanetId}");
        var planetPayload = await planetState.Content.ReadFromJsonAsync<DevPlanetUiStateResponse>();
        Assert.Equal(HttpStatusCode.OK, planetState.StatusCode);
        Assert.NotNull(planetPayload?.UiState?.Planet);
        Assert.True(planetPayload.Succeeded);
        Assert.Equal(registration.CivilizationId, planetPayload.UiState.CivilizationId);
        Assert.Equal(registration.HomePlanetId, planetPayload.UiState.SelectedPlanetId);
        Assert.Equal(registration.HomePlanetId, planetPayload.UiState.Planet.PlanetId);
        Assert.True(planetPayload.UiState.Planet.IsOwnedByRequestingCivilization);
        Assert.NotEmpty(planetPayload.UiState.Planet.Stockpile);

        using var scope = configuredFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
        Assert.Equal("Microsoft.EntityFrameworkCore.InMemory", dbContext.Database.ProviderName);
    }

    private sealed record AccountRegistrationResponse(
        bool Succeeded,
        string? UserId,
        Guid? PlayerProfileId,
        Guid? CivilizationId,
        Guid? HomePlanetId,
        string? HomePlanetName,
        string? NextRoute,
        CreateStartingCivilizationResourceSnapshot? StartingResources,
        AccountRegistrationErrorResponse[] Errors);

    private sealed record AccountRegistrationErrorResponse(string Code, string Message, string? Field);

    private sealed record DevPlanetUiStateResponse(
        bool Succeeded,
        GetDevPlanetUiStateResult? UiState,
        string[] Errors);
}
