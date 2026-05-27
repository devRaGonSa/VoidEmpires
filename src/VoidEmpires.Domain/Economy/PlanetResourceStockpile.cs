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

    public bool CanSpend(decimal credits, decimal metal, decimal crystal, decimal gas)
    {
        if (credits < 0 || metal < 0 || crystal < 0 || gas < 0)
        {
            throw new ArgumentException("Spend values cannot be negative.");
        }

        return Credits >= credits && Metal >= metal && Crystal >= crystal && Gas >= gas;
    }

    public void Spend(decimal credits, decimal metal, decimal crystal, decimal gas)
    {
        if (!CanSpend(credits, metal, crystal, gas))
        {
            throw new InvalidOperationException("Insufficient resources.");
        }

        Credits -= credits;
        Metal -= metal;
        Crystal -= crystal;
        Gas -= gas;
    }
}
