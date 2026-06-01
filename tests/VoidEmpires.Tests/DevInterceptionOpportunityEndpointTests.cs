using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Fleets;

namespace VoidEmpires.Tests;

public class DevInterceptionOpportunityEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid CivilizationId = Guid.Parse("c32b6b9b-a8e1-406b-a666-f355d8509284");

    [Fact]
    public async Task InterceptionOpportunitiesReturnNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();
        using var response = await client.GetAsync($"/api/dev/strategic-map/interception-opportunities?civilizationId={CivilizationId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task InterceptionOpportunitiesReturnServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?> { ["ConnectionStrings:DefaultConnection"] = string.Empty }));
        }).CreateClient();

        using var response = await client.GetAsync($"/api/dev/strategic-map/interception-opportunities?civilizationId={CivilizationId}");
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("?civilizationId=00000000-0000-0000-0000-000000000000")]
    public async Task InterceptionOpportunitiesReturnBadRequestForMissingOrEmptyCivilizationId(string queryString)
    {
        using var client = CreateConfiguredClient(new FakeInterceptionOpportunityService(new GetInterceptionOpportunitiesResult(CivilizationId, [])));
        using var response = await client.GetAsync($"/api/dev/strategic-map/interception-opportunities{queryString}");
        var payload = await response.Content.ReadFromJsonAsync<InterceptionOpportunityResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Null(payload.InterceptionOpportunities);
        Assert.Contains("Civilization id is required.", payload.Errors);
    }

    [Fact]
    public async Task InterceptionOpportunitiesReturnOkForValidReadOnlyRequest()
    {
        var opportunity = new InterceptionOpportunityDto(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            2,
            new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc),
            OrbitalTransferStatus.Planned,
            InterceptionOpportunityStatus.ObservedOwnTransfer,
            [InterceptionOpportunityBlockReason.SelfObservedTransfer],
            false,
            "Observed through requesting-civilization fleet state.",
            "Own active transfers are surfaced as non-hostile readiness metadata only.");
        var fakeService = new FakeInterceptionOpportunityService(new GetInterceptionOpportunitiesResult(CivilizationId, [opportunity]));
        using var client = CreateConfiguredClient(fakeService);

        using var response = await client.GetAsync($"/api/dev/strategic-map/interception-opportunities?civilizationId={CivilizationId}");
        var payload = await response.Content.ReadFromJsonAsync<InterceptionOpportunityResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.InterceptionOpportunities);
        Assert.True(payload.Succeeded);
        Assert.Empty(payload.Errors);
        Assert.Equal(CivilizationId, fakeService.LastRequest?.CivilizationId);
        Assert.Equal(InterceptionOpportunityStatus.ObservedOwnTransfer, Assert.Single(payload.InterceptionOpportunities.Opportunities).OpportunityStatus);
    }

    private HttpClient CreateConfiguredClient(IInterceptionOpportunityService service) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_interception_endpoint_tests"
                }));
            builder.ConfigureTestServices(services => services.AddSingleton(service));
        }).CreateClient();

    private sealed class FakeInterceptionOpportunityService(GetInterceptionOpportunitiesResult result) : IInterceptionOpportunityService
    {
        public GetInterceptionOpportunitiesRequest? LastRequest { get; private set; }

        public Task<GetInterceptionOpportunitiesResult> GetAsync(
            GetInterceptionOpportunitiesRequest request,
            CancellationToken cancellationToken = default)
        {
            LastRequest = request;
            return Task.FromResult(result);
        }
    }

    private sealed record InterceptionOpportunityResponse(
        bool Succeeded,
        GetInterceptionOpportunitiesResult? InterceptionOpportunities,
        string[] Errors);
}
