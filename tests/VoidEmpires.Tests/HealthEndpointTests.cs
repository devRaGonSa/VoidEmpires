using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public HealthEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HealthEndpointReturnsExpectedPayload()
    {
        using var client = _factory.CreateClientWithPersistenceDisabled();

        using var response = await client.GetAsync("/health");
        var payload = await response.Content.ReadFromJsonAsync<HealthResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal("ok", payload.Status);
        Assert.Equal("VoidEmpires.Web", payload.Service);
        Assert.False(payload.Persistence.Configured);
        Assert.Equal("Not configured", payload.Persistence.Provider);
        Assert.False(payload.Auth.Configured);
        Assert.Equal("ASP.NET Core Identity", payload.Auth.Provider);
    }

    [Fact]
    public async Task HealthEndpointReportsDefaultPersistenceProviderWithoutExposingConnectionString()
    {
        const string connectionString = "Host=localhost;Database=voidempires_health_default_test";
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = connectionString,
                    ["VoidEmpires:Persistence:Provider"] = string.Empty
                });
            });
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<DbContextOptions<VoidEmpiresDbContext>>();
                services.RemoveAll<VoidEmpiresDbContext>();
                services.AddDbContext<VoidEmpiresDbContext>(options => options.UseNpgsql(connectionString));
            });
        });
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/health");
        var content = await response.Content.ReadAsStringAsync();
        var payload = await response.Content.ReadFromJsonAsync<HealthResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Persistence.Configured);
        Assert.Equal("Npgsql.EntityFrameworkCore.PostgreSQL", payload.Persistence.Provider);
        Assert.True(payload.Auth.Configured);
        Assert.Equal("ASP.NET Core Identity", payload.Auth.Provider);
        Assert.DoesNotContain("localhost", content);
        Assert.DoesNotContain("voidempires_health_default_test", content);
    }

    [Fact]
    public async Task HealthEndpointReportsExplicitSqlServerPersistenceProviderWithoutExposingConnectionString()
    {
        const string connectionString = "Server=localhost;Database=VoidEmpiresHealthSqlServer;User Id=health-test-user;Password=<PASSWORD>;TrustServerCertificate=True;";
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = connectionString,
                    ["VoidEmpires:Persistence:Provider"] = "sqlserver"
                });
            });
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<DbContextOptions<VoidEmpiresDbContext>>();
                services.RemoveAll<VoidEmpiresDbContext>();
                services.AddDbContext<VoidEmpiresDbContext>(options => options.UseSqlServer(connectionString));
            });
        });
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/health");
        var content = await response.Content.ReadAsStringAsync();
        var payload = await response.Content.ReadFromJsonAsync<HealthResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Persistence.Configured);
        Assert.Equal("Microsoft.EntityFrameworkCore.SqlServer", payload.Persistence.Provider);
        Assert.True(payload.Auth.Configured);
        Assert.Equal("ASP.NET Core Identity", payload.Auth.Provider);
        Assert.DoesNotContain("localhost", content);
        Assert.DoesNotContain("VoidEmpiresHealthSqlServer", content);
        Assert.DoesNotContain("health-test-user", content);
        Assert.DoesNotContain("<PASSWORD>", content);
        Assert.DoesNotContain("Password", content);
    }

    [Fact]
    public async Task HealthEndpointReportsInMemoryProviderWhenTestHostOverridesPersistence()
    {
        using var client = _factory.WithInMemoryPersistence().CreateClient();

        using var response = await client.GetAsync("/health");
        var payload = await response.Content.ReadFromJsonAsync<HealthResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Persistence.Configured);
        Assert.Equal("Microsoft.EntityFrameworkCore.InMemory", payload.Persistence.Provider);
        Assert.True(payload.Auth.Configured);
    }

    private sealed record HealthResponse(string Status, string Service, PersistenceHealth Persistence, AuthHealth Auth);

    private sealed record PersistenceHealth(bool Configured, string Provider);

    private sealed record AuthHealth(bool Configured, string Provider);
}
