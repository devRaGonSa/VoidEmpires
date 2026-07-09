namespace VoidEmpires.Domain.Economy;

public sealed class PlanetResourceStockpile
{
    public const decimal DefaultCapacity = 1_000m;

    private PlanetResourceStockpile() { }

    private PlanetResourceStockpile(Guid planetId, decimal capacity, DateTime lastAccruedAtUtc)
    {
        if (planetId == Guid.Empty)
        {
            throw new ArgumentException("Planet id is required.");
        }

        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be positive.");
        }

        if (lastAccruedAtUtc.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("Last accrued date must be UTC.", nameof(lastAccruedAtUtc));
        }

        Id = Guid.NewGuid();
        PlanetId = planetId;
        Capacity = capacity;
        LastAccruedAtUtc = lastAccruedAtUtc;
    }

    public Guid Id { get; private set; }
    public Guid PlanetId { get; private set; }
    public decimal Capacity { get; private set; } = DefaultCapacity;
    public DateTime LastAccruedAtUtc { get; private set; }
    public decimal Credits { get; private set; }
    public decimal Metal { get; private set; }
    public decimal Crystal { get; private set; }
    public decimal Gas { get; private set; }

    public static PlanetResourceStockpile Create(
        Guid planetId,
        decimal capacity = DefaultCapacity,
        DateTime? lastAccruedAtUtc = null) =>
        new(planetId, capacity, lastAccruedAtUtc ?? DateTime.UtcNow);

    public void Increase(ResourceType type, decimal quantity)
    {
        if (quantity < 0)
        {
            throw new ArgumentException("Quantity cannot be negative.");
        }

        if (type == ResourceType.Credits) Credits = ClampToCapacity(Credits + quantity);
        if (type == ResourceType.Metal) Metal = ClampToCapacity(Metal + quantity);
        if (type == ResourceType.Crystal) Crystal = ClampToCapacity(Crystal + quantity);
        if (type == ResourceType.Gas) Gas = ClampToCapacity(Gas + quantity);
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

    public void MarkAccrued(DateTime accruedAtUtc)
    {
        if (accruedAtUtc.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("Accrued date must be UTC.", nameof(accruedAtUtc));
        }

        if (LastAccruedAtUtc <= accruedAtUtc)
        {
            LastAccruedAtUtc = accruedAtUtc;
        }
    }

    private decimal ClampToCapacity(decimal quantity) => Math.Min(quantity, Capacity);
}
