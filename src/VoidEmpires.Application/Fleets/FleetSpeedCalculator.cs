using VoidEmpires.Domain.Assets;

namespace VoidEmpires.Application.Fleets;

public static class FleetSpeedCalculator
{
    /// <summary>
    /// Calculates the slowest participating catalog speed, independent of quantity and duplicate rows.
    /// The modifier is backend-supplied; v1 defaults to identity because no Propulsion research formula exists.
    /// </summary>
    public static FleetSpeedResult Calculate(IEnumerable<FleetSpeedShipInput> composition, decimal propulsionModifier = 1.0m) =>
        CalculateCore(composition, propulsionModifier, static type => OrbitalAssetCatalog.Get(type).BaseMovementSpeed);

    // Keep catalog resolution fixed in the public API; isolated tests can exercise future ties/invalid metadata here.
    private static FleetSpeedResult CalculateCore(IEnumerable<FleetSpeedShipInput> composition, decimal propulsionModifier,
        Func<SpaceAssetType, decimal> resolveBaseSpeed)
    {
        ArgumentNullException.ThrowIfNull(composition);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(propulsionModifier);
        decimal baseSpeed = 0;
        SpaceAssetType? limitingAssetType = null;
        foreach (var ship in composition)
        {
            if (ship is null) throw new ArgumentException("Composition cannot contain null ships.", nameof(composition));
            if (!Enum.IsDefined(ship.AssetType))
                throw new ArgumentOutOfRangeException(nameof(composition), "Every ship type must be defined.");
            if (ship.Quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(composition), "Every ship quantity must be positive.");
            var speed = resolveBaseSpeed(ship.AssetType);
            if (speed <= 0) throw new InvalidOperationException("Catalog base movement speed must be positive.");
            if (!limitingAssetType.HasValue || speed < baseSpeed ||
                (speed == baseSpeed && (int)ship.AssetType < (int)limitingAssetType.Value))
            {
                baseSpeed = speed;
                limitingAssetType = ship.AssetType;
            }
        }

        if (!limitingAssetType.HasValue) throw new ArgumentException("Fleet composition cannot be empty.", nameof(composition));
        var effectiveSpeed = checked(baseSpeed * propulsionModifier);
        if (effectiveSpeed <= 0) throw new InvalidOperationException("Effective fleet speed must remain positive.");
        return new FleetSpeedResult(baseSpeed, propulsionModifier, effectiveSpeed, limitingAssetType.Value);
    }
}
