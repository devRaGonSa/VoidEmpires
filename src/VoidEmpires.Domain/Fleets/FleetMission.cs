using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Economy;

namespace VoidEmpires.Domain.Fleets;

public enum FleetMissionType
{
    Deploy = 1,
    Transport = 2,
    Expedition = 3,
    Colonization = 4
}

public enum FleetMissionStatus
{
    Preparing = 1,
    Outbound = 2,
    Processing = 3,
    Returning = 4,
    Completed = 5,
    Recalled = 6,
    Cancelled = 7
}

public sealed class FleetMission
{
    private readonly List<FleetMissionShip> _ships = [];
    private readonly List<FleetMissionCargo> _cargo = [];

    private FleetMission()
    {
        Ships = _ships.AsReadOnly();
        Cargo = _cargo.AsReadOnly();
    }

    public IReadOnlyCollection<FleetMissionShip> Ships { get; private set; }
    public IReadOnlyCollection<FleetMissionCargo> Cargo { get; private set; }

    public Guid Id { get; private set; }
    public Guid CivilizationId { get; private set; }
    public Guid OriginPlanetId { get; private set; }
    public Guid DestinationPlanetId { get; private set; }
    public FleetMissionType MissionType { get; private set; }
    public FleetMissionStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime OutboundDepartureUtc { get; private set; }
    public DateTime OutboundArrivalUtc { get; private set; }
    public DateTime? ReturnDepartureUtc { get; private set; }
    public DateTime? ReturnArrivalUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public DateTime? RecalledAtUtc { get; private set; }
    public int StateVersion { get; private set; }

    public static FleetMission CreatePreparing(Guid civilizationId, Guid originPlanetId, Guid destinationPlanetId,
        FleetMissionType missionType, DateTime createdAtUtc, DateTime outboundDepartureUtc, DateTime outboundArrivalUtc)
    {
        if (civilizationId == Guid.Empty || originPlanetId == Guid.Empty || destinationPlanetId == Guid.Empty)
        {
            throw new ArgumentException("Civilization, origin and destination ids are required.");
        }

        if (originPlanetId == destinationPlanetId || !Enum.IsDefined(missionType))
        {
            throw new ArgumentException("Distinct planets and a known mission type are required.");
        }

        RequireUtc(createdAtUtc, outboundDepartureUtc, outboundArrivalUtc);
        if (createdAtUtc > outboundDepartureUtc || outboundArrivalUtc <= outboundDepartureUtc)
        {
            throw new ArgumentException("Creation must precede or equal departure, and arrival must follow departure.");
        }

        return new FleetMission
        {
            Id = Guid.NewGuid(),
            CivilizationId = civilizationId,
            OriginPlanetId = originPlanetId,
            DestinationPlanetId = destinationPlanetId,
            MissionType = missionType,
            Status = FleetMissionStatus.Preparing,
            CreatedAtUtc = createdAtUtc,
            OutboundDepartureUtc = outboundDepartureUtc,
            OutboundArrivalUtc = outboundArrivalUtc
        };
    }

    public void AddShips(SpaceAssetType assetType, int quantity)
    {
        Require(Status == FleetMissionStatus.Preparing, "Only preparing missions can change composition.");
        var addition = FleetMissionShip.Create(Id, assetType, quantity);
        var existing = _ships.SingleOrDefault(ship => ship.AssetType == assetType);
        var total = checked((existing?.Quantity ?? 0) + quantity);
        IncrementVersion();
        if (existing is null) _ships.Add(addition);
        else existing.SetQuantity(total);
    }

    public void AddCargo(ResourceType resourceType, decimal amount)
    {
        FleetMissionCargo.ValidateResourceType(resourceType);
        FleetMissionCargo.ValidateAmount(amount, true, nameof(amount));
        // A valid zero request configures nothing, regardless of the current phase.
        if (amount == 0) return;
        Require(Status == FleetMissionStatus.Preparing, "Only preparing missions can load cargo.");
        var existing = _cargo.SingleOrDefault(cargo => cargo.ResourceType == resourceType);
        var total = checked((existing?.LoadedAmount ?? 0) + amount);
        FleetMissionCargo.ValidateAmount(total, false, nameof(amount));
        var addition = existing is null ? FleetMissionCargo.Create(Id, resourceType, amount) : null;
        IncrementVersion();
        if (existing is null) _cargo.Add(addition!);
        else existing.SetLoadedAmount(total);
    }

    /// <summary>
    /// Records a positive delta in Processing. Supply the previous delivered counter for each new installment.
    /// Returns false when expected + amount is already the current counter; older/conflicting expectations fail.
    /// The caller must persist any stockpile credit atomically with this versioned ledger change.
    /// </summary>
    public bool RecordCargoDelivered(ResourceType resourceType, decimal amount, decimal expectedDeliveredAmount = 0) =>
        RecordCargoSettlement(resourceType, amount, expectedDeliveredAmount, false);

    /// <summary>Same accounting precondition as delivery, using the returned counter, only in Returning.</summary>
    public bool RecordCargoReturned(ResourceType resourceType, decimal amount, decimal expectedReturnedAmount = 0) =>
        RecordCargoSettlement(resourceType, amount, expectedReturnedAmount, true);

    private bool RecordCargoSettlement(ResourceType resourceType, decimal amount, decimal expectedAmount, bool returning)
    {
        Require(Status == (returning ? FleetMissionStatus.Returning : FleetMissionStatus.Processing),
            "Cargo accounting is not allowed in this mission phase.");
        FleetMissionCargo.ValidateResourceType(resourceType);
        var cargo = _cargo.SingleOrDefault(row => row.ResourceType == resourceType)
            ?? throw new InvalidOperationException("The mission has no cargo for this resource.");
        var target = cargo.PrepareSettlement(amount, expectedAmount, returning);
        if (target == (returning ? cargo.ReturnedAmount : cargo.DeliveredAmount)) return false;
        IncrementVersion();
        cargo.ApplySettlement(target, returning);
        return true;
    }

    // Pure snapshot validation; N/O must supply fresh balances and I supplies fuel. Fuel is never a cargo row.
    public void ValidateCargoAvailability(ResourceType resourceType, decimal availableAmount, decimal reservedFuelGas = 0)
    {
        FleetMissionCargo.ValidateResourceType(resourceType);
        FleetMissionCargo.ValidateAmount(availableAmount, true, nameof(availableAmount));
        FleetMissionCargo.ValidateAmount(reservedFuelGas, true, nameof(reservedFuelGas));
        if (resourceType != ResourceType.Gas && reservedFuelGas != 0)
            throw new ArgumentException("Reserved fuel applies only to Gas.", nameof(reservedFuelGas));
        var loaded = _cargo.SingleOrDefault(cargo => cargo.ResourceType == resourceType)?.LoadedAmount ?? 0;
        Require(checked(loaded + reservedFuelGas) <= availableAmount, "Insufficient resources for cargo and fuel.");
    }

    // J owns capacity calculation and unit conversion; these caller-supplied units are not resource amounts.
    public static void ValidateCargoCapacity(decimal requiredCargoUnits, decimal availableCargoCapacityUnits)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(requiredCargoUnits);
        ArgumentOutOfRangeException.ThrowIfNegative(availableCargoCapacityUnits);
        Require(requiredCargoUnits <= availableCargoCapacityUnits, "Insufficient cargo capacity.");
    }

    public void StartOutbound()
    {
        Require(Status == FleetMissionStatus.Preparing, "Only preparing missions can launch.");
        Advance(FleetMissionStatus.Outbound);
    }

    public void BeginProcessing(DateTime nowUtc)
    {
        RequireUtc(nowUtc);
        Require(Status == FleetMissionStatus.Outbound && nowUtc >= OutboundArrivalUtc, "Only arrived outbound missions can process.");
        Advance(FleetMissionStatus.Processing);
    }

    public void BeginReturn(DateTime departureUtc, DateTime arrivalUtc) => ScheduleReturn(departureUtc, arrivalUtc, false);

    // Later recall policy calculates elapsed-travel duration; the aggregate validates and retains its single schedule.
    public void Recall(DateTime recalledAtUtc, DateTime returnArrivalUtc) => ScheduleReturn(recalledAtUtc, returnArrivalUtc, true);

    private void ScheduleReturn(DateTime departureUtc, DateTime arrivalUtc, bool recalled)
    {
        RequireUtc(departureUtc, arrivalUtc);
        Require(arrivalUtc > departureUtc, "Return arrival must follow departure.");
        Require(recalled ? departureUtc >= OutboundDepartureUtc && departureUtc < OutboundArrivalUtc
            : departureUtc >= OutboundArrivalUtc, "Return departure is outside the eligible journey phase.");
        if (Status == FleetMissionStatus.Returning && ReturnDepartureUtc == departureUtc && ReturnArrivalUtc == arrivalUtc
            && RecalledAtUtc.HasValue == recalled)
        {
            return;
        }

        Require(Status == (recalled ? FleetMissionStatus.Outbound : FleetMissionStatus.Processing), "A return leg cannot be replaced.");
        Advance(FleetMissionStatus.Returning);
        ReturnDepartureUtc = departureUtc;
        ReturnArrivalUtc = arrivalUtc;
        RecalledAtUtc = recalled ? departureUtc : null;
    }

    // Call only after authoritative settlement effects; observing a deadline alone does not settle assets.
    public void Complete(DateTime completedAtUtc)
    {
        RequireUtc(completedAtUtc);
        if ((Status is FleetMissionStatus.Completed or FleetMissionStatus.Recalled) && CompletedAtUtc == completedAtUtc)
        {
            return;
        }

        Require(Status is FleetMissionStatus.Processing or FleetMissionStatus.Returning, "Only processing or returning missions can settle.");
        Require(completedAtUtc >= (ReturnArrivalUtc ?? OutboundArrivalUtc), "Settlement cannot precede arrival.");
        Advance(RecalledAtUtc.HasValue ? FleetMissionStatus.Recalled : FleetMissionStatus.Completed);
        CompletedAtUtc = completedAtUtc;
    }

    public void Cancel(DateTime cancelledAtUtc)
    {
        RequireUtc(cancelledAtUtc);
        if (Status == FleetMissionStatus.Cancelled && CompletedAtUtc == cancelledAtUtc)
        {
            return;
        }

        Require(Status == FleetMissionStatus.Preparing && cancelledAtUtc >= CreatedAtUtc, "Only unlaunched missions can cancel after creation.");
        Advance(FleetMissionStatus.Cancelled);
        CompletedAtUtc = cancelledAtUtc;
    }

    private void Advance(FleetMissionStatus status)
    {
        IncrementVersion();
        Status = status;
    }

    private void IncrementVersion() => StateVersion = checked(StateVersion + 1);

    private static void RequireUtc(params DateTime[] timestamps)
    {
        if (timestamps.Any(x => x.Kind != DateTimeKind.Utc))
        {
            throw new ArgumentException("Mission timestamps must be UTC.");
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
