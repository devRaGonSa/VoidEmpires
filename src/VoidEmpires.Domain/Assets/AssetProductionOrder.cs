namespace VoidEmpires.Domain.Assets;

public sealed class AssetProductionOrder
{
    private AssetProductionOrder() { }

    private AssetProductionOrder(
        Guid planetId,
        AssetProductionTarget target,
        PlanetaryAssetType? planetaryAssetType,
        SpaceAssetType? spaceAssetType,
        int quantity,
        int sequence,
        DateTime startsAtUtc,
        DateTime endsAtUtc,
        AssetProductionOrderStatus status)
    {
        if (planetId == Guid.Empty)
        {
            throw new ArgumentException("Planet id is required.", nameof(planetId));
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        if (sequence <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sequence));
        }

        if (startsAtUtc.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("Start date must be UTC.", nameof(startsAtUtc));
        }

        if (endsAtUtc.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("End date must be UTC.", nameof(endsAtUtc));
        }

        if (endsAtUtc <= startsAtUtc)
        {
            throw new ArgumentException("End date must be after start date.", nameof(endsAtUtc));
        }

        if (target == AssetProductionTarget.Planetary && planetaryAssetType is null)
        {
            throw new ArgumentException("Planetary asset type is required.", nameof(planetaryAssetType));
        }

        if (target == AssetProductionTarget.Orbital && spaceAssetType is null)
        {
            throw new ArgumentException("Space asset type is required.", nameof(spaceAssetType));
        }

        Id = Guid.NewGuid();
        PlanetId = planetId;
        Target = target;
        PlanetaryAssetType = planetaryAssetType;
        SpaceAssetType = spaceAssetType;
        Quantity = quantity;
        Sequence = sequence;
        StartsAtUtc = startsAtUtc;
        EndsAtUtc = endsAtUtc;
        Status = status;
    }

    public Guid Id { get; private set; }
    public Guid PlanetId { get; private set; }
    public AssetProductionTarget Target { get; private set; }
    public PlanetaryAssetType? PlanetaryAssetType { get; private set; }
    public SpaceAssetType? SpaceAssetType { get; private set; }
    public int Quantity { get; private set; }
    public int Sequence { get; private set; }
    public DateTime StartsAtUtc { get; private set; }
    public DateTime EndsAtUtc { get; private set; }
    public AssetProductionOrderStatus Status { get; private set; }

    public bool IsOpen => Status is AssetProductionOrderStatus.Pending or AssetProductionOrderStatus.Active;

    public static AssetProductionOrder Create(
        Guid planetId,
        AssetProductionTarget target,
        PlanetaryAssetType? planetaryAssetType,
        SpaceAssetType? spaceAssetType,
        int quantity,
        int sequence,
        DateTime startsAtUtc,
        DateTime endsAtUtc,
        AssetProductionOrderStatus status)
        => new(planetId, target, planetaryAssetType, spaceAssetType, quantity, sequence, startsAtUtc, endsAtUtc, status);

    public void MarkCompleted()
    {
        if (!IsOpen)
        {
            throw new InvalidOperationException("Only open asset production orders can be completed.");
        }

        Status = AssetProductionOrderStatus.Completed;
    }

    public void MarkCancelled()
    {
        if (Status is AssetProductionOrderStatus.Cancelled)
        {
            return;
        }

        if (!IsOpen)
        {
            throw new InvalidOperationException("Only open asset production orders can be cancelled.");
        }

        Status = AssetProductionOrderStatus.Cancelled;
    }
}
