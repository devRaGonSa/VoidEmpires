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

public class DevExplorationKnowledgeEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid CivilizationId = Guid.Parse("ef97e1b3-02d5-4ab1-892e-8c325c3f75d4");

    [Fact]
    public async Task ExplorationKnowledgeReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.GetAsync($"/api/dev/strategic-map/exploration-knowledge?civilizationId={CivilizationId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ExplorationKnowledgeReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
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

        using var response = await client.GetAsync($"/api/dev/strategic-map/exploration-knowledge?civilizationId={CivilizationId}");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("?civilizationId=00000000-0000-0000-0000-000000000000")]
    public async Task ExplorationKnowledgeReturnsBadRequestForMissingOrEmptyCivilizationId(string queryString)
    {
        using var client = CreateConfiguredClient(new FakeExplorationKnowledgeQueryService(
            GetExplorationKnowledgeResult.Success(CivilizationId, [])));

        using var response = await client.GetAsync($"/api/dev/strategic-map/exploration-knowledge{queryString}");
        var payload = await response.Content.ReadFromJsonAsync<ExplorationKnowledgeResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Null(payload.Knowledge);
        Assert.Contains("Civilization id is required.", payload.Errors);
    }

    [Fact]
    public async Task ExplorationKnowledgeReturnsOkForValidRequest()
    {
        var knowledgeId = Guid.Parse("71a28c4c-5e93-4798-8e42-d047829a7206");
        var systemId = Guid.Parse("ed225248-42a3-4eca-8a4c-4ef54c5d5915");
        var discoveredAtUtc = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);
        var result = GetExplorationKnowledgeResult.Success(
            CivilizationId,
            [
                new ExplorationKnowledgeDto(
                    knowledgeId,
                    CivilizationId,
                    systemId,
                    null,
                    ExplorationKnowledgeSource.Seeded,
                    null,
                    discoveredAtUtc)
            ]);
        var fakeService = new FakeExplorationKnowledgeQueryService(result);
        using var client = CreateConfiguredClient(fakeService);

        using var response = await client.GetAsync($"/api/dev/strategic-map/exploration-knowledge?civilizationId={CivilizationId}");
        var payload = await response.Content.ReadFromJsonAsync<ExplorationKnowledgeResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.Empty(payload.Errors);
        Assert.NotNull(payload.Knowledge);
        Assert.True(payload.Knowledge.Succeeded);
        Assert.Equal(CivilizationId, payload.Knowledge.CivilizationId);
        Assert.Equal(CivilizationId, fakeService.LastRequest?.CivilizationId);
        var item = Assert.Single(payload.Knowledge.Knowledge);
        Assert.Equal(knowledgeId, item.ExplorationKnowledgeId);
        Assert.Equal(systemId, item.SystemId);
        Assert.Equal(ExplorationKnowledgeSource.Seeded, item.Source);
    }

    private HttpClient CreateConfiguredClient(IExplorationKnowledgeQueryService queryService) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_exploration_knowledge_endpoint_tests"
                }));
            builder.ConfigureTestServices(services => services.AddSingleton(queryService));
        }).CreateClient();

    private sealed class FakeExplorationKnowledgeQueryService(GetExplorationKnowledgeResult result) : IExplorationKnowledgeQueryService
    {
        public GetExplorationKnowledgeRequest? LastRequest { get; private set; }

        public Task<GetExplorationKnowledgeResult> GetAsync(
            GetExplorationKnowledgeRequest request,
            CancellationToken cancellationToken = default)
        {
            LastRequest = request;
            return Task.FromResult(result);
        }
    }

    private sealed record ExplorationKnowledgeResponse(
        bool Succeeded,
        GetExplorationKnowledgeResult? Knowledge,
        string[] Errors);
}
