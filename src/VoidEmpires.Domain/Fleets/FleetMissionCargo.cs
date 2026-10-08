using VoidEmpires.Domain.Economy;

namespace VoidEmpires.Domain.Fleets;

public sealed class FleetMissionCargo
{
    public const decimal MaximumAmount = 99_999_999_999_999.9999m;

    private FleetMissionCargo() { }

    public Guid MissionId { get; private set; }
    public ResourceType ResourceType { get; private set; }
    public decimal LoadedAmount { get; private set; }
    public decimal DeliveredAmount { get; private set; }
    public decimal ReturnedAmount { get; private set; }
    public decimal RemainingAmount => checked(LoadedAmount - DeliveredAmount - ReturnedAmount);

    public static FleetMissionCargo Create(Guid missionId, ResourceType resourceType, decimal loadedAmount)
    {
        if (missionId == Guid.Empty) throw new ArgumentException("Mission id is required.", nameof(missionId));
        ValidateResourceType(resourceType);
        ValidateAmount(loadedAmount, false, nameof(loadedAmount));
        return new FleetMissionCargo { MissionId = missionId, ResourceType = resourceType, LoadedAmount = loadedAmount };
    }

    internal static void ValidateResourceType(ResourceType resourceType)
    {
        if (!Enum.IsDefined(resourceType)) throw new ArgumentOutOfRangeException(nameof(resourceType));
    }

    internal static void ValidateAmount(decimal amount, bool allowZero, string parameterName)
    {
        if (amount < 0 || (!allowZero && amount == 0)) throw new ArgumentOutOfRangeException(parameterName);
        if (amount > MaximumAmount) throw new OverflowException("Cargo amount exceeds decimal(18,4) storage range.");
        if (decimal.Round(amount, 4) != amount)
            throw new ArgumentException("Cargo amounts must be exactly representable as decimal(18,4).", parameterName);
    }

    // Preparation is mutation-free so the aggregate can check StateVersion before applying the result.
    internal decimal PrepareSettlement(decimal amount, decimal expectedAmount, bool returning)
    {
        ValidateAmount(amount, false, nameof(amount));
        ValidateAmount(expectedAmount, true, nameof(expectedAmount));
        var target = checked(expectedAmount + amount);
        ValidateAmount(target, false, nameof(amount));
        var current = returning ? ReturnedAmount : DeliveredAmount;
        if (current == target) return target;
        if (current != expectedAmount) throw new InvalidOperationException("Cargo accounting expectation is stale.");
        if (amount > RemainingAmount) throw new InvalidOperationException("Cargo settlement exceeds remaining cargo.");
        return target;
    }

    // Only FleetMission applies prevalidated values, after its checked version increment.
    internal void SetLoadedAmount(decimal amount) => LoadedAmount = amount;

    internal void ApplySettlement(decimal target, bool returning)
    {
        if (returning) ReturnedAmount = target;
        else DeliveredAmount = target;
    }
}
