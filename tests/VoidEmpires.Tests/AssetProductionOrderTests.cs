using VoidEmpires.Domain.Assets;

namespace VoidEmpires.Tests;

public class AssetProductionOrderTests
{
    [Fact]
    public void CreateStoresPlanetaryProductionOrderValues()
    {
        var planetId = Guid.NewGuid();
        var startsAtUtc = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var endsAtUtc = startsAtUtc.AddMinutes(3);

        var order = AssetProductionOrder.Create(
            planetId,
            AssetProductionTarget.Planetary,
            PlanetaryAssetType.PatrolGroup,
            null,
            2,
            1,
            startsAtUtc,
            endsAtUtc,
            AssetProductionOrderStatus.Active);

        Assert.Equal(planetId, order.PlanetId);
        Assert.Equal(AssetProductionTarget.Planetary, order.Target);
        Assert.Equal(PlanetaryAssetType.PatrolGroup, order.PlanetaryAssetType);
        Assert.Null(order.SpaceAssetType);
        Assert.Equal(2, order.Quantity);
        Assert.Equal(1, order.Sequence);
        Assert.True(order.IsOpen);
    }

    [Fact]
    public void MarkCompletedClosesOpenOrder()
    {
        var startsAtUtc = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var order = AssetProductionOrder.Create(
            Guid.NewGuid(),
            AssetProductionTarget.Orbital,
            null,
            SpaceAssetType.ScoutCraft,
            1,
            1,
            startsAtUtc,
            startsAtUtc.AddMinutes(3),
            AssetProductionOrderStatus.Active);

        order.MarkCompleted();

        Assert.Equal(AssetProductionOrderStatus.Completed, order.Status);
        Assert.False(order.IsOpen);
    }
}
