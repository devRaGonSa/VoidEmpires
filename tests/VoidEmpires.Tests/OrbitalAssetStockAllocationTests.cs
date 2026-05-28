using VoidEmpires.Domain.Assets;

namespace VoidEmpires.Tests;

public class OrbitalAssetStockAllocationTests
{
    [Fact]
    public void DecreaseReducesAvailableQuantity()
    {
        var stock = OrbitalAssetStock.Create(Guid.NewGuid(), SpaceAssetType.ScoutCraft, 5);

        stock.Decrease(2);

        Assert.Equal(3, stock.Quantity);
    }

    [Fact]
    public void DecreaseRejectsQuantityGreaterThanAvailable()
    {
        var stock = OrbitalAssetStock.Create(Guid.NewGuid(), SpaceAssetType.ScoutCraft, 1);

        Assert.Throws<InvalidOperationException>(() => stock.Decrease(2));
    }
}
