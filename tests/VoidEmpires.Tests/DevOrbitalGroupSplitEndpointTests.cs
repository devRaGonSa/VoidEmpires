using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Fleets;

namespace VoidEmpires.Tests;

public class DevOrbitalGroupSplitEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid CivilizationId = Guid.Parse("3659791b-71b4-4582-b9cd-ee396930d075");
    private static readonly Guid SourceOrbitalGroupId = Guid.Parse("4a332c10-5a76-407e-8fb5-d1a29ad568bc");
    private static readonly Guid NewOrbitalGroupId = Guid.Parse("76d909dc-d854-4714-a9e9-b070fa1fa932");

    [Fact]
    public async Task SplitOrbitalGroupReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-groups/split", ValidRequest());

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SplitOrbitalGroupReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClient();

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-groups/split", ValidRequest());

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task SplitOrbitalGroupReturnsBadRequestForInvalidRequest()
    {
        using var client = CreateConfiguredClient(SuccessfulSplitResult());

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-groups/split", new { });
        var payload = await response.Content.ReadFromJsonAsync<SplitOrbitalGroupResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Civilization id is required.", payload.Errors);
        Assert.Contains("Source orbital group id is required.", payload.Errors);
        Assert.Contains("Quantity must be positive.", payload.Errors);
    }

    [Fact]
    public async Task SplitOrbitalGroupReturnsCreatedForSuccessfulRequest()
    {
        using var client = CreateConfiguredClient(SuccessfulSplitResult());

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-groups/split", ValidRequest());
        var payload = await response.Content.ReadFromJsonAsync<SplitOrbitalGroupResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.Equal(SourceOrbitalGroupId, payload.SourceOrbitalGroupId);
        Assert.Equal(NewOrbitalGroupId, payload.NewOrbitalGroupId);
        Assert.Equal(3, payload.SourceQuantity);
        Assert.Equal(2, payload.NewQuantity);
        Assert.Empty(payload.Errors);
    }

    [Fact]
    public async Task SplitOrbitalGroupReturnsConflictWhenServiceRejectsRequest()
    {
        using var client = CreateConfiguredClient(SplitOrbitalGroupResult.Failure("Only stationed orbital groups can be split."));

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-groups/split", ValidRequest());
        var payload = await response.Content.ReadFromJsonAsync<SplitOrbitalGroupResponse>();

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Only stationed orbital groups can be split.", payload.Errors);
    }

    private HttpClient CreateConfiguredClient(SplitOrbitalGroupResult result) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_dev_endpoint_tests"
                }));
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IOrbitalGroupSplitService>(new FakeOrbitalGroupSplitService(result));
            });
        }).CreateClient();

    private static SplitOrbitalGroupResult SuccessfulSplitResult() =>
        SplitOrbitalGroupResult.Success(SourceOrbitalGroupId, NewOrbitalGroupId, 3, 2);

    private static object ValidRequest() => new
    {
        civilizationId = CivilizationId,
        sourceOrbitalGroupId = SourceOrbitalGroupId,
        quantity = 2
    };

    private sealed class FakeOrbitalGroupSplitService(SplitOrbitalGroupResult result) : IOrbitalGroupSplitService
    {
        public Task<SplitOrbitalGroupResult> SplitAsync(
            SplitOrbitalGroupRequest request,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(result);
    }

    private sealed record SplitOrbitalGroupResponse(
        bool Succeeded,
        Guid? SourceOrbitalGroupId,
        Guid? NewOrbitalGroupId,
        int SourceQuantity,
        int NewQuantity,
        string[] Errors);
}
