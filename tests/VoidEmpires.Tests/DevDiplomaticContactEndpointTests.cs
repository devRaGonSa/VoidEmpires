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

public class DevDiplomaticContactEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid CivilizationId = Guid.Parse("48fd6134-d164-4361-b70c-c8ba308d6008");

    [Fact]
    public async Task DiplomaticContactsReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();
        using var response = await client.GetAsync($"/api/dev/strategic-map/diplomatic-contacts?civilizationId={CivilizationId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DiplomaticContactsReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?> { ["ConnectionStrings:DefaultConnection"] = string.Empty }));
        }).CreateClient();

        using var response = await client.GetAsync($"/api/dev/strategic-map/diplomatic-contacts?civilizationId={CivilizationId}");
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("?civilizationId=00000000-0000-0000-0000-000000000000")]
    public async Task DiplomaticContactsReturnsBadRequestForMissingOrEmptyCivilizationId(string queryString)
    {
        using var client = CreateConfiguredClient(new FakeDiplomaticContactQueryService(GetDiplomaticContactsResult.Success(CivilizationId, [])));
        using var response = await client.GetAsync($"/api/dev/strategic-map/diplomatic-contacts{queryString}");
        var payload = await response.Content.ReadFromJsonAsync<DiplomaticContactsResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Null(payload.DiplomaticContacts);
        Assert.Contains("Civilization id is required.", payload.Errors);
    }

    [Fact]
    public async Task DiplomaticContactsReturnsOkForValidReadOnlyRequest()
    {
        var contact = new DiplomaticContactDto(
            Guid.NewGuid(),
            CivilizationId,
            Guid.NewGuid(),
            DiplomaticContactStatus.Neutral,
            DateTime.UtcNow,
            "manual-dev");
        var fakeService = new FakeDiplomaticContactQueryService(GetDiplomaticContactsResult.Success(CivilizationId, [contact]));
        using var client = CreateConfiguredClient(fakeService);

        using var response = await client.GetAsync($"/api/dev/strategic-map/diplomatic-contacts?civilizationId={CivilizationId}");
        var payload = await response.Content.ReadFromJsonAsync<DiplomaticContactsResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.DiplomaticContacts);
        Assert.True(payload.Succeeded);
        Assert.Empty(payload.Errors);
        Assert.Equal(CivilizationId, fakeService.LastRequest?.CivilizationId);
        Assert.Equal(DiplomaticContactStatus.Neutral, Assert.Single(payload.DiplomaticContacts.Contacts).Status);
    }

    private HttpClient CreateConfiguredClient(IDiplomaticContactQueryService diplomaticContactQueryService) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_diplomatic_contacts_endpoint_tests"
                }));
            builder.ConfigureTestServices(services => services.AddSingleton(diplomaticContactQueryService));
        }).CreateClient();

    private sealed class FakeDiplomaticContactQueryService(GetDiplomaticContactsResult result) : IDiplomaticContactQueryService
    {
        public GetDiplomaticContactsRequest? LastRequest { get; private set; }

        public Task<GetDiplomaticContactsResult> GetAsync(GetDiplomaticContactsRequest request, CancellationToken cancellationToken = default)
        {
            LastRequest = request;
            return Task.FromResult(result);
        }
    }

    private sealed record DiplomaticContactsResponse(
        bool Succeeded,
        GetDiplomaticContactsResult? DiplomaticContacts,
        string[] Errors);
}
