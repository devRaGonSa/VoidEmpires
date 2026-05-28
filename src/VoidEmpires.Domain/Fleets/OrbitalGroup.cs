using VoidEmpires.Domain.Assets;

namespace VoidEmpires.Domain.Fleets;

public sealed class OrbitalGroup
{
    private OrbitalGroup() { }

    private OrbitalGroup(
        Guid civilizationId,
        Guid originPlanetId,
        Guid currentPlanetId,
        SpaceAssetType assetType,
        int quantity,
        OrbitalGroupStatus status)
    {
        if (civilizationId == Guid.Empty)
        {
            throw new ArgumentException("Civilization id is required.", nameof(civilizationId));
        }

        if (originPlanetId == Guid.Empty)
        {
            throw new ArgumentException("Origin planet id is required.", nameof(originPlanetId));
        }

        if (currentPlanetId == Guid.Empty)
        {
            throw new ArgumentException("Current planet id is required.", nameof(currentPlanetId));
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        Id = Guid.NewGuid();
        CivilizationId = civilizationId;
        OriginPlanetId = originPlanetId;
        CurrentPlanetId = currentPlanetId;
        AssetType = assetType;
        Quantity = quantity;
        Status = status;
    }

    public Guid Id { get; private set; }
    public Guid CivilizationId { get; private set; }
    public Guid OriginPlanetId { get; private set; }
    public Guid CurrentPlanetId { get; private set; }
    public SpaceAssetType AssetType { get; private set; }
    public int Quantity { get; private set; }
    public OrbitalGroupStatus Status { get; private set; }

    public bool IsStationedAwayFromOrigin => CurrentPlanetId != OriginPlanetId;

    public static OrbitalGroup CreateStationed(
        Guid civilizationId,
        Guid originPlanetId,
        Guid currentPlanetId,
        SpaceAssetType assetType,
        int quantity)
        => new(
            civilizationId,
            originPlanetId,
            currentPlanetId,
            assetType,
            quantity,
            OrbitalGroupStatus.Stationed);

    public void Reserve()
    {
        if (Status != OrbitalGroupStatus.Stationed)
        {
            throw new InvalidOperationException("Only stationed orbital groups can be reserved.");
        }

        Status = OrbitalGroupStatus.Reserved;
    }

    public void ArriveAt(Guid destinationPlanetId)
    {
        if (destinationPlanetId == Guid.Empty)
        {
            throw new ArgumentException("Destination planet id is required.", nameof(destinationPlanetId));
        }

        if (Status != OrbitalGroupStatus.Reserved)
        {
            throw new InvalidOperationException("Only reserved orbital groups can arrive.");
        }

        CurrentPlanetId = destinationPlanetId;
        Status = OrbitalGroupStatus.Stationed;
    }
}
