using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Assets;

namespace VoidEmpires.Tests;

public class DevOrbitalGroupEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task CreateFromStockReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-groups/create-from-stock", ValidRequest());

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateFromStockReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClientWithPersistenceDisabled();

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-groups/create-from-stock", ValidRequest());

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task CreateFromStockReturnsBadRequestForInvalidRequest()
    {
        using var client = CreateConfiguredClient(CreateOrbitalGroupResult.Success(Guid.NewGuid()));

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-groups/create-from-stock", new { });
        var payload = await response.Content.ReadFromJsonAsync<CreateOrbitalGroupResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Civilization id is required.", payload.Errors);
        Assert.Contains("Origin planet id is required.", payload.Errors);
        Assert.Contains("Current planet id is required.", payload.Errors);
        Assert.Contains("Space asset type is required.", payload.Errors);
        Assert.Contains("Quantity must be positive.", payload.Errors);
    }

    [Fact]
    public async Task CreateFromStockReturnsCreatedForSuccessfulRequest()
    {
        var groupId = Guid.Parse("4a332c10-5a76-407e-8fb5-d1a29ad568bc");
        using var client = CreateConfiguredClient(CreateOrbitalGroupResult.Success(groupId));

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-groups/create-from-stock", ValidRequest());
        var payload = await response.Content.ReadFromJsonAsync<CreateOrbitalGroupResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.Equal(groupId, payload.OrbitalGroupId);
        Assert.Empty(payload.Errors);
    }

    [Fact]
    public async Task CreateFromStockReturnsConflictWhenServiceRejectsRequest()
    {
        using var client = CreateConfiguredClient(CreateOrbitalGroupResult.Failure("Insufficient stock."));

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-groups/create-from-stock", ValidRequest());
        var payload = await response.Content.ReadFromJsonAsync<CreateOrbitalGroupResponse>();

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Insufficient stock.", payload.Errors);
    }

    private HttpClient CreateConfiguredClient(CreateOrbitalGroupResult result) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_dev_endpoint_tests"
                }));
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IOrbitalGroupService>(new FakeOrbitalGroupService(result));
            });
        }).CreateClient();

    private static object ValidRequest() => new
    {
        civilizationId = Guid.Parse("3659791b-71b4-4582-b9cd-ee396930d075"),
        originPlanetId = Guid.Parse("4813dab0-0de6-454c-8c6c-d70944b87bfa"),
        currentPlanetId = Guid.Parse("0dd85c9f-7c1d-4a0d-9247-32ab0d61a8c7"),
        assetType = SpaceAssetType.ScoutCraft,
        quantity = 2
    };

    private sealed class FakeOrbitalGroupService(CreateOrbitalGroupResult result) : IOrbitalGroupService
    {
        public Task<CreateOrbitalGroupResult> CreateFromLocalStockAsync(
            CreateOrbitalGroupRequest request,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(result);
    }

    private sealed record CreateOrbitalGroupResponse(
        bool Succeeded,
        Guid? OrbitalGroupId,
        string[] Errors);
}
