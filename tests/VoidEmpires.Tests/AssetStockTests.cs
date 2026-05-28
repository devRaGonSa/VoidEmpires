using VoidEmpires.Domain.Assets;

namespace VoidEmpires.Tests;

public class AssetStockTests
{
    [Fact]
    public void PlanetaryStockCanIncreaseQuantity()
    {
        var stock = PlanetaryAssetStock.Create(Guid.NewGuid(), PlanetaryAssetType.PatrolGroup, 2);

        stock.Increase(3);

        Assert.Equal(5, stock.Quantity);
    }

    [Fact]
    public void OrbitalStockCanIncreaseQuantity()
    {
        var stock = OrbitalAssetStock.Create(Guid.NewGuid(), SpaceAssetType.ScoutCraft, 1);

        stock.Increase(4);

        Assert.Equal(5, stock.Quantity);
    }
}
