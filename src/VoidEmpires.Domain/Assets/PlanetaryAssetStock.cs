namespace VoidEmpires.Domain.Assets;

public sealed class PlanetaryAssetStock
{
    private PlanetaryAssetStock() { }

    private PlanetaryAssetStock(Guid planetId, PlanetaryAssetType assetType, int quantity)
    {
        if (planetId == Guid.Empty)
        {
            throw new ArgumentException("Planet id is required.", nameof(planetId));
        }

        if (quantity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        Id = Guid.NewGuid();
        PlanetId = planetId;
        AssetType = assetType;
        Quantity = quantity;
    }

    public Guid Id { get; private set; }
    public Guid PlanetId { get; private set; }
    public PlanetaryAssetType AssetType { get; private set; }
    public int Quantity { get; private set; }

    public static PlanetaryAssetStock Create(Guid planetId, PlanetaryAssetType assetType, int quantity = 0)
        => new(planetId, assetType, quantity);

    public void Increase(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        Quantity += quantity;
    }
}
