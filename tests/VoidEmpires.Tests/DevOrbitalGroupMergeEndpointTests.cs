using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Fleets;

namespace VoidEmpires.Tests;

public class DevOrbitalGroupMergeEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid CivilizationId = Guid.Parse("3659791b-71b4-4582-b9cd-ee396930d075");
    private static readonly Guid TargetOrbitalGroupId = Guid.Parse("4a332c10-5a76-407e-8fb5-d1a29ad568bc");
    private static readonly Guid SourceOrbitalGroupId = Guid.Parse("76d909dc-d854-4714-a9e9-b070fa1fa932");

    [Fact]
    public async Task MergeOrbitalGroupsReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-groups/merge", ValidRequest());

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task MergeOrbitalGroupsReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClientWithPersistenceDisabled();

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-groups/merge", ValidRequest());

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task MergeOrbitalGroupsReturnsBadRequestForInvalidRequest()
    {
        using var client = CreateConfiguredClient(SuccessfulMergeResult());

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-groups/merge", new { });
        var payload = await response.Content.ReadFromJsonAsync<MergeOrbitalGroupsResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Civilization id is required.", payload.Errors);
        Assert.Contains("Target orbital group id is required.", payload.Errors);
        Assert.Contains("Source orbital group id is required.", payload.Errors);
    }

    [Fact]
    public async Task MergeOrbitalGroupsReturnsOkForSuccessfulRequest()
    {
        using var client = CreateConfiguredClient(SuccessfulMergeResult());

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-groups/merge", ValidRequest());
        var payload = await response.Content.ReadFromJsonAsync<MergeOrbitalGroupsResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.Equal(TargetOrbitalGroupId, payload.TargetOrbitalGroupId);
        Assert.Equal(SourceOrbitalGroupId, payload.SourceOrbitalGroupId);
        Assert.Equal(5, payload.TargetQuantity);
        Assert.Empty(payload.Errors);
    }

    [Fact]
    public async Task MergeOrbitalGroupsReturnsConflictWhenServiceRejectsRequest()
    {
        using var client = CreateConfiguredClient(MergeOrbitalGroupsResult.Failure("Target orbital group already has an active transfer."));

        using var response = await client.PostAsJsonAsync("/api/dev/fleets/orbital-groups/merge", ValidRequest());
        var payload = await response.Content.ReadFromJsonAsync<MergeOrbitalGroupsResponse>();

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Target orbital group already has an active transfer.", payload.Errors);
    }

    private HttpClient CreateConfiguredClient(MergeOrbitalGroupsResult result) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_dev_endpoint_tests"
                }));
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IOrbitalGroupMergeService>(new FakeOrbitalGroupMergeService(result));
            });
        }).CreateClient();

    private static MergeOrbitalGroupsResult SuccessfulMergeResult() =>
        MergeOrbitalGroupsResult.Success(TargetOrbitalGroupId, SourceOrbitalGroupId, 5);

    private static object ValidRequest() => new
    {
        civilizationId = CivilizationId,
        targetOrbitalGroupId = TargetOrbitalGroupId,
        sourceOrbitalGroupId = SourceOrbitalGroupId
    };

    private sealed class FakeOrbitalGroupMergeService(MergeOrbitalGroupsResult result) : IOrbitalGroupMergeService
    {
        public Task<MergeOrbitalGroupsResult> MergeAsync(
            MergeOrbitalGroupsRequest request,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(result);
    }

    private sealed record MergeOrbitalGroupsResponse(
        bool Succeeded,
        Guid? TargetOrbitalGroupId,
        Guid? SourceOrbitalGroupId,
        int TargetQuantity,
        string[] Errors);
}
