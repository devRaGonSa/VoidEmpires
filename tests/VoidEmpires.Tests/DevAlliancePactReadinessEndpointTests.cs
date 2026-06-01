using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Diplomacy;

namespace VoidEmpires.Tests;

public class DevAlliancePactReadinessEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid CivilizationId = Guid.Parse("a5f0c3ce-06eb-4492-98de-f1131d54711a");

    [Fact]
    public async Task AlliancePactReadinessReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();
        using var response = await client.GetAsync($"/api/dev/strategic-map/alliances/pacts/readiness?civilizationId={CivilizationId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AlliancePactReadinessReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?> { ["ConnectionStrings:DefaultConnection"] = string.Empty }));
        }).CreateClient();

        using var response = await client.GetAsync($"/api/dev/strategic-map/alliances/pacts/readiness?civilizationId={CivilizationId}");
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("?civilizationId=00000000-0000-0000-0000-000000000000")]
    public async Task AlliancePactReadinessReturnsBadRequestForMissingOrEmptyCivilizationId(string queryString)
    {
        using var client = CreateConfiguredClient(new FakeAlliancePactReadinessQueryService(GetAlliancePactReadinessResult.Success(CivilizationId, [])));
        using var response = await client.GetAsync($"/api/dev/strategic-map/alliances/pacts/readiness{queryString}");
        var payload = await response.Content.ReadFromJsonAsync<AlliancePactReadinessResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Null(payload.AlliancePactReadiness);
        Assert.Contains("Civilization id is required.", payload.Errors);
    }

    [Fact]
    public async Task AlliancePactReadinessReturnsOkForValidReadOnlyRequest()
    {
        var pact = new AlliancePactReadinessDto(
            Guid.NewGuid(),
            new AlliancePactAllianceDto(Guid.NewGuid(), "Void Council", "VC", AllianceStatus.Active),
            new AlliancePactAllianceDto(Guid.NewGuid(), "Star Accord", "STAR", AllianceStatus.Active),
            AlliancePactType.NonAggression,
            AlliancePactStatus.Active,
            new DateTime(2026, 6, 1, 12, 10, 0, DateTimeKind.Utc));
        var fakeService = new FakeAlliancePactReadinessQueryService(GetAlliancePactReadinessResult.Success(CivilizationId, [pact]));
        using var client = CreateConfiguredClient(fakeService);

        using var response = await client.GetAsync($"/api/dev/strategic-map/alliances/pacts/readiness?civilizationId={CivilizationId}");
        var payload = await response.Content.ReadFromJsonAsync<AlliancePactReadinessResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.AlliancePactReadiness);
        Assert.True(payload.Succeeded);
        Assert.Empty(payload.Errors);
        Assert.Equal(CivilizationId, fakeService.LastRequest?.CivilizationId);
        Assert.Equal(1, fakeService.CallCount);
        Assert.Equal("VC", Assert.Single(payload.AlliancePactReadiness.Pacts).SourceAlliance.Tag);
    }

    private HttpClient CreateConfiguredClient(IAlliancePactReadinessQueryService alliancePactReadinessQueryService) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_alliance_pact_readiness_endpoint_tests"
                }));
            builder.ConfigureTestServices(services => services.AddSingleton(alliancePactReadinessQueryService));
        }).CreateClient();

    private sealed class FakeAlliancePactReadinessQueryService(GetAlliancePactReadinessResult result) : IAlliancePactReadinessQueryService
    {
        public int CallCount { get; private set; }
        public GetAlliancePactReadinessRequest? LastRequest { get; private set; }

        public Task<GetAlliancePactReadinessResult> GetAsync(GetAlliancePactReadinessRequest request, CancellationToken cancellationToken = default)
        {
            CallCount++;
            LastRequest = request;
            return Task.FromResult(result);
        }
    }

    private sealed record AlliancePactReadinessResponse(
        bool Succeeded,
        GetAlliancePactReadinessResult? AlliancePactReadiness,
        string[] Errors);
}
