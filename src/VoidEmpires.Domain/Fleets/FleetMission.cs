using VoidEmpires.Domain.Assets;

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

    private FleetMission() => Ships = _ships.AsReadOnly();

    public IReadOnlyCollection<FleetMissionShip> Ships { get; private set; }

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
