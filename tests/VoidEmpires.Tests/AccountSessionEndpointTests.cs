using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Identity;
using VoidEmpires.Infrastructure;

namespace VoidEmpires.Tests;

public class AccountSessionEndpointTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task MeReturnsCurrentAccountSessionAfterLogin()
    {
        using var configuredFactory = CreateConfiguredFactory();
        using var client = configuredFactory.CreateClient();
        await RegisterAndLoginAsync(client);
        using var response = await client.GetAsync("/api/accounts/me");
        var payload = await response.Content.ReadFromJsonAsync<CurrentAccountSession>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.NotNull(payload.UserId);
        Assert.NotNull(payload.PlayerProfileId);
        Assert.NotNull(payload.CivilizationId);
        Assert.NotNull(payload.HomePlanetId);
        Assert.Equal("Nova Prime", payload.HomePlanetName);
        Assert.Equal($"/planet?civilizationId={payload.CivilizationId}&planetId={payload.HomePlanetId}", payload.NextRoute);
    }

    [Fact]
    public async Task MeReturnsUnauthorizedForAnonymousRequest()
    {
        using var configuredFactory = CreateConfiguredFactory();
        using var client = configuredFactory.CreateClient();
        using var response = await client.GetAsync("/api/accounts/me");
        var payload = await response.Content.ReadFromJsonAsync<CurrentAccountSession>();
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains(payload.Errors, error => error.Code == "Unauthenticated");
    }

    [Fact]
    public async Task LogoutClearsCurrentAccountSession()
    {
        using var configuredFactory = CreateConfiguredFactory();
        using var client = configuredFactory.CreateClient();
        await RegisterAndLoginAsync(client);
        using var logout = await client.PostAsync("/api/accounts/logout", null);
        using var me = await client.GetAsync("/api/accounts/me");
        var payload = await me.Content.ReadFromJsonAsync<CurrentAccountSession>();
        Assert.Equal(HttpStatusCode.OK, logout.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, me.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains(payload.Errors, error => error.Code == "Unauthenticated");
    }

    private static async Task RegisterAndLoginAsync(HttpClient client)
    {
        using var register = await client.PostAsJsonAsync("/api/accounts/register", new AccountRegistrationRequest("player@example.test", "P@ssw0rd!23", "P@ssw0rd!23", "Commander Vega", "Solar Dominion", "Nova Prime"));
        using var login = await client.PostAsJsonAsync("/api/accounts/login", new AccountLoginRequest("player@example.test", "P@ssw0rd!23"));
        Assert.Equal(HttpStatusCode.Created, register.StatusCode);
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
    }

    private WebApplicationFactory<Program> CreateConfiguredFactory() =>
        factory.WithInMemoryPersistence(databaseName: Guid.NewGuid().ToString("N"))
            .WithWebHostBuilder(builder => builder.ConfigureTestServices(services => services.AddVoidEmpiresIdentity()));
}
