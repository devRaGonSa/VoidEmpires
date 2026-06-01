using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Buildings;
using VoidEmpires.Domain.Buildings;

namespace VoidEmpires.Tests;

public class DevConstructionQueueEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task EnqueueReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.PostAsJsonAsync("/api/dev/buildings/construction-orders/enqueue", ValidEnqueueRequest());

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task EnqueueReturnsServiceUnavailableWhenPersistenceIsNotConfigured()
    {
        using var client = factory.CreateClientWithPersistenceDisabled();

        using var response = await client.PostAsJsonAsync("/api/dev/buildings/construction-orders/enqueue", ValidEnqueueRequest());

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task EnqueueReturnsBadRequestForInvalidRequest()
    {
        using var client = CreateConfiguredClient(
            EnqueueConstructionOrderResult.Success(Guid.NewGuid(), DateTime.UnixEpoch, DateTime.UnixEpoch.AddMinutes(5)),
            new CompleteConstructionOrdersResult(0, []));

        using var response = await client.PostAsJsonAsync("/api/dev/buildings/construction-orders/enqueue", new { });
        var payload = await response.Content.ReadFromJsonAsync<EnqueueConstructionOrderResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Succeeded);
        Assert.Contains("Planet id is required.", payload.Errors);
        Assert.Contains("Civilization id is required.", payload.Errors);
        Assert.Contains("Construction action is required.", payload.Errors);
        Assert.Contains("Building type is required.", payload.Errors);
        Assert.Contains("Requested date is required.", payload.Errors);
    }

    [Fact]
    public async Task EnqueueReturnsCreatedForSuccessfulQueueRequest()
    {
        var orderId = Guid.Parse("fac6178d-9c8c-49bf-9ae5-5af7c09d0a35");
        var startsAtUtc = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var endsAtUtc = startsAtUtc.AddMinutes(5);
        using var client = CreateConfiguredClient(
            EnqueueConstructionOrderResult.Success(orderId, startsAtUtc, endsAtUtc),
            new CompleteConstructionOrdersResult(0, []));

        using var response = await client.PostAsJsonAsync("/api/dev/buildings/construction-orders/enqueue", ValidEnqueueRequest(startsAtUtc));
        var payload = await response.Content.ReadFromJsonAsync<EnqueueConstructionOrderResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.Equal(orderId, payload.OrderId);
        Assert.Equal(startsAtUtc, payload.StartsAtUtc);
        Assert.Equal(endsAtUtc, payload.EndsAtUtc);
        Assert.Empty(payload.Errors);
    }

    [Fact]
    public async Task CompleteDueReturnsOkForSuccessfulCompletionRequest()
    {
        var completedOrderId = Guid.Parse("4910b688-c5de-493c-9a68-822eb7729a85");
        using var client = CreateConfiguredClient(
            EnqueueConstructionOrderResult.Success(Guid.NewGuid(), DateTime.UnixEpoch, DateTime.UnixEpoch.AddMinutes(5)),
            new CompleteConstructionOrdersResult(1, [completedOrderId]));

        using var response = await client.PostAsJsonAsync("/api/dev/buildings/construction-orders/complete-due", new
        {
            nowUtc = new DateTime(2026, 1, 1, 12, 10, 0, DateTimeKind.Utc)
        });
        var payload = await response.Content.ReadFromJsonAsync<CompleteConstructionOrdersResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.Equal(1, payload.CompletedCount);
        Assert.Equal([completedOrderId], payload.CompletedOrderIds);
        Assert.Empty(payload.Errors);
    }

    private HttpClient CreateConfiguredClient(
        EnqueueConstructionOrderResult enqueueResult,
        CompleteConstructionOrdersResult completionResult) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=voidempires_dev_endpoint_tests"
                }));
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IPlanetConstructionQueueService>(new FakeConstructionQueueService(enqueueResult));
                services.AddSingleton<IConstructionOrderCompletionService>(new FakeConstructionOrderCompletionService(completionResult));
            });
        }).CreateClient();

    private static object ValidEnqueueRequest(DateTime? requestedAtUtc = null) => new
    {
        planetId = Guid.Parse("4813dab0-0de6-454c-8c6c-d70944b87bfa"),
        civilizationId = Guid.Parse("3659791b-71b4-4582-b9cd-ee396930d075"),
        action = ConstructionQueueItemAction.Construct,
        buildingType = BuildingType.MetalMine,
        requestedAtUtc = requestedAtUtc ?? new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc)
    };

    private sealed class FakeConstructionQueueService(EnqueueConstructionOrderResult result) : IPlanetConstructionQueueService
    {
        public Task<EnqueueConstructionOrderResult> EnqueueAsync(
            EnqueueConstructionOrderRequest request,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(result);
    }

    private sealed class FakeConstructionOrderCompletionService(CompleteConstructionOrdersResult result) : IConstructionOrderCompletionService
    {
        public Task<CompleteConstructionOrdersResult> CompleteDueOrdersAsync(
            DateTime nowUtc,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(result);
    }

    private sealed record EnqueueConstructionOrderResponse(
        bool Succeeded,
        Guid? OrderId,
        DateTime? StartsAtUtc,
        DateTime? EndsAtUtc,
        string[] Errors);

    private sealed record CompleteConstructionOrdersResponse(
        bool Succeeded,
        int CompletedCount,
        Guid[] CompletedOrderIds,
        string[] Errors);
}
