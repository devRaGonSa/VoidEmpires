namespace VoidEmpires.Domain.Economy;

public sealed class PlanetResourceStockpile
{
    private PlanetResourceStockpile() { }

    private PlanetResourceStockpile(Guid planetId)
    {
        if (planetId == Guid.Empty)
        {
            throw new ArgumentException("Planet id is required.");
        }

        Id = Guid.NewGuid();
        PlanetId = planetId;
    }

    public Guid Id { get; private set; }
    public Guid PlanetId { get; private set; }
    public decimal Credits { get; private set; }
    public decimal Metal { get; private set; }
    public decimal Crystal { get; private set; }
    public decimal Gas { get; private set; }

    public static PlanetResourceStockpile Create(Guid planetId) => new(planetId);

    public void Increase(ResourceType type, decimal quantity)
    {
        if (quantity < 0)
        {
            throw new ArgumentException("Quantity cannot be negative.");
        }

        if (type == ResourceType.Credits) Credits += quantity;
        if (type == ResourceType.Metal) Metal += quantity;
        if (type == ResourceType.Crystal) Crystal += quantity;
        if (type == ResourceType.Gas) Gas += quantity;
    }

    public bool HasAtLeast(ResourceType type, decimal quantity)
    {
        if (quantity < 0)
        {
            throw new ArgumentException("Quantity cannot be negative.");
        }

        return type switch
        {
            ResourceType.Credits => Credits >= quantity,
            ResourceType.Metal => Metal >= quantity,
            ResourceType.Crystal => Crystal >= quantity,
            ResourceType.Gas => Gas >= quantity,
            _ => false
        };
    }
}
