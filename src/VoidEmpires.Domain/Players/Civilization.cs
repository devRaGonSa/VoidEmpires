namespace VoidEmpires.Domain.Players;

public sealed class Civilization
{
    private Civilization() { }

    private Civilization(Guid playerProfileId,string name,CivilizationArchetype archetype,Guid? homePlanetId)
    {
        if (playerProfileId == Guid.Empty) throw new ArgumentException("Player profile id is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Civilization name is required.");

        Id = Guid.NewGuid();
        PlayerProfileId = playerProfileId;
        Name = name.Trim();
        Archetype = archetype;
        HomePlanetId = homePlanetId;
        Status = CivilizationStatus.Active;
    }

    public Guid Id { get; private set; }
    public Guid PlayerProfileId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public CivilizationArchetype Archetype { get; private set; }
    public CivilizationStatus Status { get; private set; }
    public Guid? HomePlanetId { get; private set; }

    public static Civilization Create(Guid playerProfileId,string name,CivilizationArchetype archetype,Guid? homePlanetId = null)
        => new(playerProfileId,name,archetype,homePlanetId);
}
