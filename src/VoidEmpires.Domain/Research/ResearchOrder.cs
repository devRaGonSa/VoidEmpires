namespace VoidEmpires.Domain.Research;

public sealed class ResearchOrder
{
    private ResearchOrder() { }

    private ResearchOrder(
        Guid civilizationId,
        Guid sourcePlanetId,
        ResearchType researchType,
        int targetLevel,
        int sequence,
        DateTime startsAtUtc,
        DateTime endsAtUtc,
        ResearchQueueItemStatus status)
    {
        if (civilizationId == Guid.Empty)
        {
            throw new ArgumentException("Civilization id is required.", nameof(civilizationId));
        }

        if (sourcePlanetId == Guid.Empty)
        {
            throw new ArgumentException("Source planet id is required.", nameof(sourcePlanetId));
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
        CivilizationId = civilizationId;
        SourcePlanetId = sourcePlanetId;
        ResearchType = researchType;
        TargetLevel = targetLevel;
        Sequence = sequence;
        StartsAtUtc = startsAtUtc;
        EndsAtUtc = endsAtUtc;
        Status = status;
    }

    public Guid Id { get; private set; }
    public Guid CivilizationId { get; private set; }
    public Guid SourcePlanetId { get; private set; }
    public ResearchType ResearchType { get; private set; }
    public int TargetLevel { get; private set; }
    public int Sequence { get; private set; }
    public DateTime StartsAtUtc { get; private set; }
    public DateTime EndsAtUtc { get; private set; }
    public ResearchQueueItemStatus Status { get; private set; }

    public bool IsOpen => Status is ResearchQueueItemStatus.Pending or ResearchQueueItemStatus.Active;

    public static ResearchOrder Create(
        Guid civilizationId,
        Guid sourcePlanetId,
        ResearchType researchType,
        int targetLevel,
        int sequence,
        DateTime startsAtUtc,
        DateTime endsAtUtc,
        ResearchQueueItemStatus status)
        => new(civilizationId, sourcePlanetId, researchType, targetLevel, sequence, startsAtUtc, endsAtUtc, status);

    public void MarkCompleted()
    {
        if (!IsOpen)
        {
            throw new InvalidOperationException("Only open research orders can be completed.");
        }

        Status = ResearchQueueItemStatus.Completed;
    }

    public void MarkCancelled()
    {
        if (!IsOpen)
        {
            throw new InvalidOperationException("Only open research orders can be cancelled.");
        }

        Status = ResearchQueueItemStatus.Cancelled;
    }
}
