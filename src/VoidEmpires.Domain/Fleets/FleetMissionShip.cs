using VoidEmpires.Domain.Assets;

namespace VoidEmpires.Domain.Fleets;

public sealed class FleetMissionShip
{
    private FleetMissionShip() { }

    public Guid MissionId { get; private set; }
    public SpaceAssetType AssetType { get; private set; }
    public int Quantity { get; private set; }

    public static FleetMissionShip Create(Guid missionId, SpaceAssetType assetType, int quantity)
    {
        if (missionId == Guid.Empty) throw new ArgumentException("Mission id is required.", nameof(missionId));
        if (!Enum.IsDefined(assetType)) throw new ArgumentOutOfRangeException(nameof(assetType));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        return new FleetMissionShip { MissionId = missionId, AssetType = assetType, Quantity = quantity };
    }

    internal void SetQuantity(int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        Quantity = quantity;
    }
}
