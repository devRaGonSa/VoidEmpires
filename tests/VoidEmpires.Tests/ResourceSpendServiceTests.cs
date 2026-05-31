using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Economy;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Infrastructure.Economy;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class ResourceSpendServiceTests
{
    [Fact]
    public async Task CheckAffordabilityAndSpendAsyncUsePersistedBalances()
    {
        await using var dbContext = CreateDbContext();
        var planetId = await SeedStockpileAsync(dbContext, 100, 80, 20, 10);
        var service = new ResourceSpendService(dbContext);
        var request = Request(planetId, (ResourceType.Credits, 25), (ResourceType.Metal, 30), (ResourceType.Gas, 5));

        var check = await service.CheckAffordabilityAsync(request);
        await AssertBalancesAsync(dbContext, planetId, 100, 80, 20, 10);
        var spend = await service.SpendAsync(request);

        Assert.True(check.Succeeded);
        Assert.Equal(planetId, check.PlanetId);
        Assert.True(spend.Succeeded);
        await AssertBalancesAsync(dbContext, planetId, 75, 50, 20, 5);
    }

    [Theory]
    [InlineData("insufficient", "Insufficient Metal.")]
    [InlineData("missing", "Planet resource stockpile was not found.")]
    [InlineData("negative", "Resource cost cannot be negative.")]
    public async Task SpendAsyncRejectsInvalidRequests(string scenario, string expectedError)
    {
        await using var dbContext = CreateDbContext();
        var planetId = scenario == "missing"
            ? Guid.NewGuid()
            : await SeedStockpileAsync(dbContext, 100, 10, 0, 0);
        var request = scenario switch
        {
            "negative" => Request(planetId, (ResourceType.Credits, -1)),
            _ => Request(planetId, (ResourceType.Metal, 11))
        };

        var result = await new ResourceSpendService(dbContext).SpendAsync(request);

        Assert.False(result.Succeeded);
        Assert.Equal([expectedError], result.Errors);
    }

    [Fact]
    public async Task SpendAsyncAggregatesMultipleCostsBeforeSpending()
    {
        await using var dbContext = CreateDbContext();
        var planetId = await SeedStockpileAsync(dbContext, 100, 80, 20, 10);

        var result = await new ResourceSpendService(dbContext).SpendAsync(Request(
            planetId,
            (ResourceType.Credits, 25),
            (ResourceType.Credits, 10),
            (ResourceType.Metal, 30),
            (ResourceType.Crystal, 5),
            (ResourceType.Gas, 5)));

        Assert.True(result.Succeeded);
        await AssertBalancesAsync(dbContext, planetId, 65, 50, 15, 5);
    }

    [Fact]
    public async Task SpendAsyncDoesNotPartiallySpendWhenOneResourceIsInsufficient()
    {
        await using var dbContext = CreateDbContext();
        var planetId = await SeedStockpileAsync(dbContext, 100, 10, 20, 10);

        var result = await new ResourceSpendService(dbContext).SpendAsync(Request(
            planetId,
            (ResourceType.Credits, 25),
            (ResourceType.Metal, 11),
            (ResourceType.Crystal, 5)));

        Assert.False(result.Succeeded);
        await AssertBalancesAsync(dbContext, planetId, 100, 10, 20, 10);
    }

    private static VoidEmpiresDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static ResourceSpendRequest Request(Guid planetId, params (ResourceType Type, decimal Quantity)[] costs) =>
        new(planetId, costs.Select(cost => new ResourceCostDto(cost.Type, cost.Quantity)).ToArray());

    private static async Task AssertBalancesAsync(
        VoidEmpiresDbContext dbContext,
        Guid planetId,
        decimal credits,
        decimal metal,
        decimal crystal,
        decimal gas)
    {
        var stockpile = await dbContext.PlanetResourceStockpiles.SingleAsync(item => item.PlanetId == planetId);
        Assert.Equal(credits, stockpile.Credits);
        Assert.Equal(metal, stockpile.Metal);
        Assert.Equal(crystal, stockpile.Crystal);
        Assert.Equal(gas, stockpile.Gas);
    }

    private static async Task<Guid> SeedStockpileAsync(VoidEmpiresDbContext dbContext, decimal credits, decimal metal, decimal crystal, decimal gas)
    {
        var planetId = Guid.NewGuid();
        var stockpile = PlanetResourceStockpile.Create(planetId);
        stockpile.Increase(ResourceType.Credits, credits);
        stockpile.Increase(ResourceType.Metal, metal);
        stockpile.Increase(ResourceType.Crystal, crystal);
        stockpile.Increase(ResourceType.Gas, gas);
        dbContext.PlanetResourceStockpiles.Add(stockpile);
        await dbContext.SaveChangesAsync();
        return planetId;
    }
}
