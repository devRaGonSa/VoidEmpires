namespace VoidEmpires.Domain.Exploration;

public sealed class ExplorationMission
{
    private ExplorationMission() { }

    private ExplorationMission(
        Guid civilizationId,
        Guid targetSystemId,
        Guid? targetPlanetId,
        DateTime requestedAtUtc,
        DateTime dueAtUtc,
        ExplorationMissionStatus status)
    {
        if (civilizationId == Guid.Empty)
        {
            throw new ArgumentException("Civilization id is required.", nameof(civilizationId));
        }

        if (targetSystemId == Guid.Empty)
        {
            throw new ArgumentException("Target system id is required.", nameof(targetSystemId));
        }

        if (targetPlanetId == Guid.Empty)
        {
            throw new ArgumentException("Target planet id cannot be empty.", nameof(targetPlanetId));
        }

        if (requestedAtUtc.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("Requested date must be UTC.", nameof(requestedAtUtc));
        }

        if (dueAtUtc.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("Due date must be UTC.", nameof(dueAtUtc));
        }

        if (dueAtUtc < requestedAtUtc)
        {
            throw new ArgumentException("Due date must be on or after requested date.", nameof(dueAtUtc));
        }

        Id = Guid.NewGuid();
        CivilizationId = civilizationId;
        TargetSystemId = targetSystemId;
        TargetPlanetId = targetPlanetId;
        RequestedAtUtc = requestedAtUtc;
        DueAtUtc = dueAtUtc;
        Status = status;
    }

    public Guid Id { get; private set; }
    public Guid CivilizationId { get; private set; }
    public Guid TargetSystemId { get; private set; }
    public Guid? TargetPlanetId { get; private set; }
    public DateTime RequestedAtUtc { get; private set; }
    public DateTime DueAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public ExplorationMissionStatus Status { get; private set; }

    public static ExplorationMission CreatePlanned(
        Guid civilizationId,
        Guid targetSystemId,
        Guid? targetPlanetId,
        DateTime requestedAtUtc,
        DateTime dueAtUtc) =>
        new(
            civilizationId,
            targetSystemId,
            targetPlanetId,
            requestedAtUtc,
            dueAtUtc,
            ExplorationMissionStatus.Planned);

    public void Complete(DateTime completedAtUtc)
    {
        if (completedAtUtc.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("Completion date must be UTC.", nameof(completedAtUtc));
        }

        if (Status == ExplorationMissionStatus.Completed)
        {
            throw new InvalidOperationException("Exploration mission is already completed.");
        }

        Status = ExplorationMissionStatus.Completed;
        CompletedAtUtc = completedAtUtc;
    }
}
