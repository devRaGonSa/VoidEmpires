namespace VoidEmpires.Domain.Diplomacy;

public sealed class DiplomaticContact
{
    private DiplomaticContact() { }

    private DiplomaticContact(
        Guid civilizationId,
        Guid contactedCivilizationId,
        DiplomaticContactStatus status,
        DateTime discoveredAtUtc,
        string source)
    {
        if (civilizationId == Guid.Empty) throw new ArgumentException("Civilization id is required.", nameof(civilizationId));
        if (contactedCivilizationId == Guid.Empty) throw new ArgumentException("Contacted civilization id is required.", nameof(contactedCivilizationId));
        if (civilizationId == contactedCivilizationId) throw new ArgumentException("A civilization cannot contact itself.", nameof(contactedCivilizationId));
        if (!Enum.IsDefined(status) || status == DiplomaticContactStatus.Unknown) throw new ArgumentException("Diplomatic contact status is invalid.", nameof(status));
        if (discoveredAtUtc.Kind != DateTimeKind.Utc) throw new ArgumentException("Discovery date must be UTC.", nameof(discoveredAtUtc));
        if (string.IsNullOrWhiteSpace(source)) throw new ArgumentException("Contact source is required.", nameof(source));

        Id = Guid.NewGuid();
        CivilizationId = civilizationId;
        ContactedCivilizationId = contactedCivilizationId;
        Status = status;
        DiscoveredAtUtc = discoveredAtUtc;
        Source = source.Trim();
    }

    public Guid Id { get; private set; }
    public Guid CivilizationId { get; private set; }
    public Guid ContactedCivilizationId { get; private set; }
    public DiplomaticContactStatus Status { get; private set; }
    public DateTime DiscoveredAtUtc { get; private set; }
    public string Source { get; private set; } = string.Empty;

    public static DiplomaticContact Create(
        Guid civilizationId,
        Guid contactedCivilizationId,
        DiplomaticContactStatus status,
        DateTime discoveredAtUtc,
        string source) =>
        new(civilizationId, contactedCivilizationId, status, discoveredAtUtc, source);
}
