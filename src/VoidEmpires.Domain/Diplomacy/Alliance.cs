namespace VoidEmpires.Domain.Diplomacy;

public sealed class Alliance
{
    private Alliance() { }

    private Alliance(
        string name,
        string tag,
        AllianceStatus status,
        DateTime createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Alliance name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(tag)) throw new ArgumentException("Alliance tag is required.", nameof(tag));
        if (!Enum.IsDefined(status) || status == AllianceStatus.Unknown) throw new ArgumentException("Alliance status is invalid.", nameof(status));
        if (createdAtUtc.Kind != DateTimeKind.Utc) throw new ArgumentException("Created date must be UTC.", nameof(createdAtUtc));

        Id = Guid.NewGuid();
        Name = name.Trim();
        Tag = tag.Trim();
        Status = status;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Tag { get; private set; } = string.Empty;
    public AllianceStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static Alliance Create(
        string name,
        string tag,
        AllianceStatus status,
        DateTime createdAtUtc) =>
        new(name, tag, status, createdAtUtc);
}
