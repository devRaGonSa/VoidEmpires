namespace VoidEmpires.Domain.Diplomacy;

public sealed class AllianceMembership
{
    private AllianceMembership() { }

    private AllianceMembership(
        Guid allianceId,
        Guid civilizationId,
        AllianceMembershipStatus status,
        AllianceMembershipRole role,
        DateTime joinedAtUtc)
    {
        if (allianceId == Guid.Empty) throw new ArgumentException("Alliance id is required.", nameof(allianceId));
        if (civilizationId == Guid.Empty) throw new ArgumentException("Civilization id is required.", nameof(civilizationId));
        if (!Enum.IsDefined(status) || status == AllianceMembershipStatus.Unknown) throw new ArgumentException("Alliance membership status is invalid.", nameof(status));
        if (!Enum.IsDefined(role) || role == AllianceMembershipRole.Unknown) throw new ArgumentException("Alliance membership role is invalid.", nameof(role));
        if (joinedAtUtc.Kind != DateTimeKind.Utc) throw new ArgumentException("Joined date must be UTC.", nameof(joinedAtUtc));

        Id = Guid.NewGuid();
        AllianceId = allianceId;
        CivilizationId = civilizationId;
        Status = status;
        Role = role;
        JoinedAtUtc = joinedAtUtc;
    }

    public Guid Id { get; private set; }
    public Guid AllianceId { get; private set; }
    public Guid CivilizationId { get; private set; }
    public AllianceMembershipStatus Status { get; private set; }
    public AllianceMembershipRole Role { get; private set; }
    public DateTime JoinedAtUtc { get; private set; }

    public static AllianceMembership Create(
        Guid allianceId,
        Guid civilizationId,
        AllianceMembershipStatus status,
        AllianceMembershipRole role,
        DateTime joinedAtUtc) =>
        new(allianceId, civilizationId, status, role, joinedAtUtc);
}
