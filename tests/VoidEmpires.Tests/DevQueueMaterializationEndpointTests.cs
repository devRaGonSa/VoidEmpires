using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Gameplay;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class DevQueueMaterializationEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task MaterializeDueReturnsNotFoundOutsideDevelopmentByDefault()
    {
        using var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production")).CreateClient();

        using var response = await client.PostAsJsonAsync("/api/dev/queues/materialize-due", new { });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task MaterializeDueProcessesScopedDueOrdersAndIsRepeatSafe()
    {
        var civilizationId = Guid.NewGuid();
        var otherCivilizationId = Guid.NewGuid();
        var planetId = Guid.NewGuid();
        var otherPlanetId = Guid.NewGuid();
        var nowUtc = new DateTime(2026, 6, 15, 12, 0, 0, DateTimeKind.Utc);
        using var configuredFactory = factory.WithInMemoryPersistence();
        using var client = configuredFactory.CreateClient();

        using (var scope = configuredFactory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
            dbContext.PlanetOwnerships.Add(PlanetOwnership.Create(planetId, civilizationId));
            dbContext.PlanetOwnerships.Add(PlanetOwnership.Create(otherPlanetId, otherCivilizationId));
            dbContext.PlanetConstructionOrders.Add(CreateConstructionOrder(planetId, 1, nowUtc.AddMinutes(-1)));
            dbContext.PlanetConstructionOrders.Add(CreateConstructionOrder(otherPlanetId, 1, nowUtc.AddMinutes(-1)));
            await dbContext.SaveChangesAsync();
        }

        var request = new
        {
            civilizationId,
            planetId,
            nowUtc,
            includeConstruction = true,
            includeResearch = false,
            includeShipyard = false
        };

        using var firstResponse = await client.PostAsJsonAsync("/api/dev/queues/materialize-due", request);
        var firstPayload = await firstResponse.Content.ReadFromJsonAsync<MaterializeQueuesResponse>();
        using var secondResponse = await client.PostAsJsonAsync("/api/dev/queues/materialize-due", request);
        var secondPayload = await secondResponse.Content.ReadFromJsonAsync<MaterializeQueuesResponse>();

        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        Assert.Equal(new QueueMaterializationSummary(1, 1, 0), firstPayload!.Construction);
        Assert.Equal(HttpStatusCode.OK, secondResponse.StatusCode);
        Assert.Equal(new QueueMaterializationSummary(0, 0, 0), secondPayload!.Construction);

        using var verifyScope = configuredFactory.Services.CreateScope();
        var verifyContext = verifyScope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
        Assert.Contains(verifyContext.PlanetConstructionOrders, x => x.PlanetId == planetId && x.Status == ConstructionQueueItemStatus.Completed);
        Assert.Contains(verifyContext.PlanetConstructionOrders, x => x.PlanetId == otherPlanetId && x.Status == ConstructionQueueItemStatus.Active);
    }

    [Fact]
    public async Task MaterializeDueSkipsNotDueOrdersWithoutMutatingState()
    {
        var civilizationId = Guid.NewGuid();
        var planetId = Guid.NewGuid();
        var nowUtc = new DateTime(2026, 6, 15, 12, 0, 0, DateTimeKind.Utc);
        using var configuredFactory = factory.WithInMemoryPersistence();
        using var client = configuredFactory.CreateClient();

        Guid orderId;
        using (var scope = configuredFactory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
            dbContext.PlanetOwnerships.Add(PlanetOwnership.Create(planetId, civilizationId));
            var order = CreateConstructionOrder(planetId, 1, nowUtc.AddMinutes(30));
            dbContext.PlanetConstructionOrders.Add(order);
            await dbContext.SaveChangesAsync();
            orderId = order.Id;
        }

        using var response = await client.PostAsJsonAsync("/api/dev/queues/materialize-due", new
        {
            civilizationId,
            planetId,
            nowUtc,
            includeConstruction = true,
            includeResearch = false,
            includeShipyard = false
        });
        var payload = await response.Content.ReadFromJsonAsync<MaterializeQueuesResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(new QueueMaterializationSummary(0, 0, 1), payload!.Construction);

        using var verifyScope = configuredFactory.Services.CreateScope();
        var verifyContext = verifyScope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
        var storedOrder = await verifyContext.PlanetConstructionOrders.SingleAsync(x => x.Id == orderId);
        Assert.Equal(ConstructionQueueItemStatus.Active, storedOrder.Status);
        Assert.Empty(await verifyContext.PlanetBuildings.Where(x => x.PlanetId == planetId).ToListAsync());
    }

    private static PlanetConstructionOrder CreateConstructionOrder(Guid planetId, int sequence, DateTime endsAtUtc)
        => PlanetConstructionOrder.Create(planetId, ConstructionQueueItemAction.Construct, BuildingType.MetalMine, 1, sequence, endsAtUtc.AddMinutes(-5), endsAtUtc, ConstructionQueueItemStatus.Active);

    private sealed record MaterializeQueuesResponse(
        bool Succeeded,
        QueueMaterializationSummary? Construction,
        QueueMaterializationSummary? Research,
        QueueMaterializationSummary? Shipyard,
        IReadOnlyList<string> Notes);
}
