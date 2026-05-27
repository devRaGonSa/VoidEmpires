namespace VoidEmpires.Domain.Economy;

public sealed class ResourceProductionService
{
    public void ApplyProduction(
        PlanetProductionProfile profile,
        PlanetResourceStockpile stockpile,
        TimeSpan elapsed)
    {
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(stockpile);

        if (profile.PlanetId != stockpile.PlanetId)
        {
            throw new InvalidOperationException("Production profile and stockpile must belong to the same planet.");
        }

        if (elapsed < TimeSpan.Zero)
        {
            throw new ArgumentException("Elapsed time cannot be negative.");
        }

        if (elapsed == TimeSpan.Zero)
        {
            return;
        }

        var hours = (decimal)elapsed.TotalHours;

        stockpile.Increase(ResourceType.Credits, profile.CreditsPerHour * hours);
        stockpile.Increase(ResourceType.Metal, profile.MetalPerHour * hours);
        stockpile.Increase(ResourceType.Crystal, profile.CrystalPerHour * hours);
        stockpile.Increase(ResourceType.Gas, profile.GasPerHour * hours);
    }
}
