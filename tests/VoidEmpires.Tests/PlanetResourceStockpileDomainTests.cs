using VoidEmpires.Domain.Economy;

namespace VoidEmpires.Tests;

public class PlanetResourceStockpileDomainTests
{
    [Fact]
    public void CreateRejectsEmptyPlanetId()
    {
        Assert.Throws<ArgumentException>(() => PlanetResourceStockpile.Create(Guid.Empty));
    }

    [Fact]
    public void CreateStartsWithEmptyBalances()
    {
        var stockpile = PlanetResourceStockpile.Create(Guid.NewGuid());

        Assert.Equal(0, stockpile.Credits);
        Assert.Equal(0, stockpile.Metal);
        Assert.Equal(0, stockpile.Crystal);
        Assert.Equal(0, stockpile.Gas);
    }

    [Fact]
    public void IncreaseAddsToRequestedResource()
    {
        var stockpile = PlanetResourceStockpile.Create(Guid.NewGuid());

        stockpile.Increase(ResourceType.Credits, 10);
        stockpile.Increase(ResourceType.Metal, 20);
        stockpile.Increase(ResourceType.Crystal, 30);
        stockpile.Increase(ResourceType.Gas, 40);

        Assert.Equal(10, stockpile.Credits);
        Assert.Equal(20, stockpile.Metal);
        Assert.Equal(30, stockpile.Crystal);
        Assert.Equal(40, stockpile.Gas);
    }

    [Fact]
    public void IncreaseRejectsNegativeQuantity()
    {
        var stockpile = PlanetResourceStockpile.Create(Guid.NewGuid());

        Assert.Throws<ArgumentException>(() => stockpile.Increase(ResourceType.Metal, -1));
    }

    [Fact]
    public void HasAtLeastReturnsExpectedAvailability()
    {
        var stockpile = PlanetResourceStockpile.Create(Guid.NewGuid());
        stockpile.Increase(ResourceType.Metal, 25);

        Assert.True(stockpile.HasAtLeast(ResourceType.Metal, 25));
        Assert.True(stockpile.HasAtLeast(ResourceType.Metal, 10));
        Assert.False(stockpile.HasAtLeast(ResourceType.Metal, 26));
    }
}
