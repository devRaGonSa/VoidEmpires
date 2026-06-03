using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Development;

namespace VoidEmpires.Tests;

public class DevDevelopmentSeedEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task ApplySeedReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "minimal-validation" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ApplySeedReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClientWithPersistenceDisabled();

        using var response = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "minimal-validation" });

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task ApplySeedReturnsBadRequestForMissingProfile()
    {
        using var client = CreateConfiguredClient(ApplyDevelopmentSeedResult.Success(
            "minimal-validation",
            [],
            DevelopmentSeedProfiles.MinimalValidation,
            DevelopmentSeedProfiles.All));

        using var response = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "" });
        var payload = await response.Content.ReadFromJsonAsync<ApplyDevelopmentSeedResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Seed profile is required.", payload.Errors);
    }

    [Fact]
    public async Task ApplySeedReturnsOkForSuccessfulProfile()
    {
        using var client = CreateConfiguredClient(ApplyDevelopmentSeedResult.Success(
            "minimal-validation",
            ["Seed profile acknowledged."],
            DevelopmentSeedProfiles.MinimalValidation,
            DevelopmentSeedProfiles.All));

        using var response = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "minimal-validation" });
        var payload = await response.Content.ReadFromJsonAsync<ApplyDevelopmentSeedResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.Equal("minimal-validation", payload.Profile);
        Assert.Contains("Seed profile acknowledged.", payload.AppliedSteps);
        Assert.NotNull(payload.ProfileMetadata);
        Assert.Equal("minimal-validation", payload.ProfileMetadata.Name);
        Assert.Contains(payload.KnownProfiles, x => x.Name == "research-validation" && !x.IsImplemented);
        Assert.Empty(payload.Errors);
    }

    private HttpClient CreateConfiguredClient(ApplyDevelopmentSeedResult result) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_dev_seed_endpoint_tests"
                }));
            builder.ConfigureTestServices(services =>
                services.AddSingleton<IDevelopmentSeedService>(new FakeDevelopmentSeedService(result)));
        }).CreateClient();

    private sealed class FakeDevelopmentSeedService(ApplyDevelopmentSeedResult result) : IDevelopmentSeedService
    {
        public Task<ApplyDevelopmentSeedResult> ApplyAsync(
            ApplyDevelopmentSeedRequest request,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(result);
    }

    private sealed record ApplyDevelopmentSeedResponse(
        bool Succeeded,
        string? Profile,
        string[] AppliedSteps,
        string[] Errors,
        DevelopmentSeedProfileMetadata? ProfileMetadata,
        DevelopmentSeedProfileMetadata[] KnownProfiles);
}
