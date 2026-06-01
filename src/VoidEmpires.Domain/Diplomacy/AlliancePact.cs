namespace VoidEmpires.Domain.Diplomacy;

public sealed class AlliancePact
{
    private AlliancePact() { }

    private AlliancePact(
        Guid sourceAllianceId,
        Guid targetAllianceId,
        AlliancePactType pactType,
        AlliancePactStatus status,
        DateTime createdAtUtc)
    {
        if (sourceAllianceId == Guid.Empty) throw new ArgumentException("Source alliance id is required.", nameof(sourceAllianceId));
        if (targetAllianceId == Guid.Empty) throw new ArgumentException("Target alliance id is required.", nameof(targetAllianceId));
        if (sourceAllianceId == targetAllianceId) throw new ArgumentException("Source and target alliances must be different.", nameof(targetAllianceId));
        if (!Enum.IsDefined(pactType) || pactType == AlliancePactType.Unknown) throw new ArgumentException("Alliance pact type is invalid.", nameof(pactType));
        if (!Enum.IsDefined(status) || status == AlliancePactStatus.Unknown) throw new ArgumentException("Alliance pact status is invalid.", nameof(status));
        if (createdAtUtc.Kind != DateTimeKind.Utc) throw new ArgumentException("Created date must be UTC.", nameof(createdAtUtc));

        Id = Guid.NewGuid();
        SourceAllianceId = sourceAllianceId;
        TargetAllianceId = targetAllianceId;
        PactType = pactType;
        Status = status;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; private set; }
    public Guid SourceAllianceId { get; private set; }
    public Guid TargetAllianceId { get; private set; }
    public AlliancePactType PactType { get; private set; }
    public AlliancePactStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static AlliancePact Create(
        Guid sourceAllianceId,
        Guid targetAllianceId,
        AlliancePactType pactType,
        AlliancePactStatus status,
        DateTime createdAtUtc) =>
        new(sourceAllianceId, targetAllianceId, pactType, status, createdAtUtc);
}
