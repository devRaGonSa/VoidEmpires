namespace VoidEmpires.Domain.Buildings;

public sealed class PlanetConstructionOrder
{
    private PlanetConstructionOrder() { }

    private PlanetConstructionOrder(
        Guid planetId,
        ConstructionQueueItemAction action,
        BuildingType buildingType,
        int targetLevel,
        int sequence,
        DateTime startsAtUtc,
        DateTime endsAtUtc,
        ConstructionQueueItemStatus status)
    {
        if (planetId == Guid.Empty)
        {
            throw new ArgumentException("Planet id is required.", nameof(planetId));
        }

        if (targetLevel <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(targetLevel));
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

        Id = Guid.NewGuid();
        PlanetId = planetId;
        Action = action;
        BuildingType = buildingType;
        TargetLevel = targetLevel;
        Sequence = sequence;
        StartsAtUtc = startsAtUtc;
        EndsAtUtc = endsAtUtc;
        Status = status;
    }

    public Guid Id { get; private set; }
    public Guid PlanetId { get; private set; }
    public ConstructionQueueItemAction Action { get; private set; }
    public BuildingType BuildingType { get; private set; }
    public int TargetLevel { get; private set; }
    public int Sequence { get; private set; }
    public DateTime StartsAtUtc { get; private set; }
    public DateTime EndsAtUtc { get; private set; }
    public ConstructionQueueItemStatus Status { get; private set; }

    public bool IsOpen => Status is ConstructionQueueItemStatus.Pending or ConstructionQueueItemStatus.Active;

    public static PlanetConstructionOrder Create(
        Guid planetId,
        ConstructionQueueItemAction action,
        BuildingType buildingType,
        int targetLevel,
        int sequence,
        DateTime startsAtUtc,
        DateTime endsAtUtc,
        ConstructionQueueItemStatus status)
        => new(planetId, action, buildingType, targetLevel, sequence, startsAtUtc, endsAtUtc, status);

    public void MarkCompleted()
    {
        if (!IsOpen)
        {
            throw new InvalidOperationException("Only open construction orders can be completed.");
        }

        Status = ConstructionQueueItemStatus.Completed;
    }
}
