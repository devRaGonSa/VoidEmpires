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

public class DevAllianceReadinessEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid CivilizationId = Guid.Parse("f86e1f7c-9402-45f7-a1fe-7610d7610cc3");

    [Fact]
    public async Task AllianceReadinessReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();
        using var response = await client.GetAsync($"/api/dev/strategic-map/alliances/readiness?civilizationId={CivilizationId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AllianceReadinessReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?> { ["ConnectionStrings:DefaultConnection"] = string.Empty }));
        }).CreateClient();

        using var response = await client.GetAsync($"/api/dev/strategic-map/alliances/readiness?civilizationId={CivilizationId}");
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("?civilizationId=00000000-0000-0000-0000-000000000000")]
    public async Task AllianceReadinessReturnsBadRequestForMissingOrEmptyCivilizationId(string queryString)
    {
        using var client = CreateConfiguredClient(new FakeAllianceReadinessQueryService(GetAllianceReadinessResult.Success(CivilizationId, [])));
        using var response = await client.GetAsync($"/api/dev/strategic-map/alliances/readiness{queryString}");
        var payload = await response.Content.ReadFromJsonAsync<AllianceReadinessResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Null(payload.AllianceReadiness);
        Assert.Contains("Civilization id is required.", payload.Errors);
    }

    [Fact]
    public async Task AllianceReadinessReturnsOkForValidReadOnlyRequest()
    {
        var membership = new AllianceMembershipDto(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CivilizationId,
            AllianceMembershipStatus.Active,
            AllianceMembershipRole.Member,
            new DateTime(2026, 6, 1, 12, 5, 0, DateTimeKind.Utc));
        var alliance = new AllianceReadinessDto(
            membership.AllianceId,
            "Void Council",
            "VC",
            AllianceStatus.Active,
            new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc),
            membership);
        var fakeService = new FakeAllianceReadinessQueryService(GetAllianceReadinessResult.Success(CivilizationId, [alliance]));
        using var client = CreateConfiguredClient(fakeService);

        using var response = await client.GetAsync($"/api/dev/strategic-map/alliances/readiness?civilizationId={CivilizationId}");
        var payload = await response.Content.ReadFromJsonAsync<AllianceReadinessResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.AllianceReadiness);
        Assert.True(payload.Succeeded);
        Assert.Empty(payload.Errors);
        Assert.Equal(CivilizationId, fakeService.LastRequest?.CivilizationId);
        Assert.Equal("VC", Assert.Single(payload.AllianceReadiness.Alliances).Tag);
    }

    private HttpClient CreateConfiguredClient(IAllianceReadinessQueryService allianceReadinessQueryService) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_alliance_readiness_endpoint_tests"
                }));
            builder.ConfigureTestServices(services => services.AddSingleton(allianceReadinessQueryService));
        }).CreateClient();

    private sealed class FakeAllianceReadinessQueryService(GetAllianceReadinessResult result) : IAllianceReadinessQueryService
    {
        public GetAllianceReadinessRequest? LastRequest { get; private set; }

        public Task<GetAllianceReadinessResult> GetAsync(GetAllianceReadinessRequest request, CancellationToken cancellationToken = default)
        {
            LastRequest = request;
            return Task.FromResult(result);
        }
    }

    private sealed record AllianceReadinessResponse(
        bool Succeeded,
        GetAllianceReadinessResult? AllianceReadiness,
        string[] Errors);
}
