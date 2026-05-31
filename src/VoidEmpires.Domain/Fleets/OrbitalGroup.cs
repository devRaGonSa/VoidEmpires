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

    public OrbitalGroup SplitOff(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        if (quantity >= Quantity)
        {
            throw new InvalidOperationException("Split quantity must be lower than source quantity.");
        }

        if (Status != OrbitalGroupStatus.Stationed)
        {
            throw new InvalidOperationException("Only stationed orbital groups can be split.");
        }

        Quantity -= quantity;

        return CreateStationed(
            CivilizationId,
            OriginPlanetId,
            CurrentPlanetId,
            AssetType,
            quantity);
    }

    public void MergeFrom(OrbitalGroup source)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (Id == source.Id)
        {
            throw new InvalidOperationException("Target and source orbital groups must be different.");
        }

        if (CivilizationId != source.CivilizationId)
        {
            throw new InvalidOperationException("Orbital groups must belong to the same civilization.");
        }

        if (CurrentPlanetId != source.CurrentPlanetId)
        {
            throw new InvalidOperationException("Orbital groups must be at the same current planet.");
        }

        if (AssetType != source.AssetType)
        {
            throw new InvalidOperationException("Orbital groups must have the same asset type.");
        }

        if (Status != OrbitalGroupStatus.Stationed || source.Status != OrbitalGroupStatus.Stationed)
        {
            throw new InvalidOperationException("Only stationed orbital groups can be merged.");
        }

        Quantity += source.Quantity;
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

    public void ReleaseReservation()
    {
        if (Status != OrbitalGroupStatus.Reserved)
        {
            throw new InvalidOperationException("Only reserved orbital groups can be released.");
        }

        Status = OrbitalGroupStatus.Stationed;
    }
}
