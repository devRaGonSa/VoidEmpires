namespace VoidEmpires.Domain.Players;

public sealed class PlayerProfile
{
    private readonly List<Civilization> _items = [];

    private PlayerProfile() { }

    private PlayerProfile(string userId, string displayName)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User id is required.");
        if (string.IsNullOrWhiteSpace(displayName)) throw new ArgumentException("Display name is required.");

        Id = Guid.NewGuid();
        UserId = userId.Trim();
        DisplayName = displayName.Trim();
        NormalizedDisplayName = NormalizeLookupKey(DisplayName);
    }

    public Guid Id { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string NormalizedDisplayName { get; private set; } = string.Empty;
    public IReadOnlyCollection<Civilization> Civilizations => _items.AsReadOnly();

    public static PlayerProfile Create(string userId, string displayName) => new(userId, displayName);

    public static string NormalizeLookupKey(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Lookup value is required.", nameof(value));
        return value.Trim().ToUpperInvariant();
    }

    public void AddCivilization(Civilization item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (_items.Any(x => x.NormalizedName == item.NormalizedName))
        {
            throw new InvalidOperationException("Duplicate civilization name.");
        }

        _items.Add(item);
    }
}
