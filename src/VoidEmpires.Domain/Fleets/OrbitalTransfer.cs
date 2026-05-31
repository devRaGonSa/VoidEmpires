namespace VoidEmpires.Domain.Fleets;

public sealed class OrbitalTransfer
{
    private OrbitalTransfer() { }

    private OrbitalTransfer(
        Guid civilizationId,
        Guid orbitalGroupId,
        Guid originPlanetId,
        Guid destinationPlanetId,
        int abstractDistanceUnits,
        DateTime departureAtUtc,
        DateTime arrivalAtUtc,
        OrbitalTransferStatus status)
    {
        if (civilizationId == Guid.Empty)
        {
            throw new ArgumentException("Civilization id is required.", nameof(civilizationId));
        }

        if (orbitalGroupId == Guid.Empty)
        {
            throw new ArgumentException("Orbital group id is required.", nameof(orbitalGroupId));
        }

        if (originPlanetId == Guid.Empty)
        {
            throw new ArgumentException("Origin planet id is required.", nameof(originPlanetId));
        }

        if (destinationPlanetId == Guid.Empty)
        {
            throw new ArgumentException("Destination planet id is required.", nameof(destinationPlanetId));
        }

        if (originPlanetId == destinationPlanetId)
        {
            throw new ArgumentException("Destination planet must be different from the origin planet.", nameof(destinationPlanetId));
        }

        if (abstractDistanceUnits <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(abstractDistanceUnits));
        }

        if (departureAtUtc.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("Departure date must be UTC.", nameof(departureAtUtc));
        }

        if (arrivalAtUtc.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("Arrival date must be UTC.", nameof(arrivalAtUtc));
        }

        if (arrivalAtUtc <= departureAtUtc)
        {
            throw new ArgumentException("Arrival date must be after departure date.", nameof(arrivalAtUtc));
        }

        Id = Guid.NewGuid();
        CivilizationId = civilizationId;
        OrbitalGroupId = orbitalGroupId;
        OriginPlanetId = originPlanetId;
        DestinationPlanetId = destinationPlanetId;
        AbstractDistanceUnits = abstractDistanceUnits;
        DepartureAtUtc = departureAtUtc;
        ArrivalAtUtc = arrivalAtUtc;
        Status = status;
    }

    public Guid Id { get; private set; }
    public Guid CivilizationId { get; private set; }
    public Guid OrbitalGroupId { get; private set; }
    public Guid OriginPlanetId { get; private set; }
    public Guid DestinationPlanetId { get; private set; }
    public int AbstractDistanceUnits { get; private set; }
    public DateTime DepartureAtUtc { get; private set; }
    public DateTime ArrivalAtUtc { get; private set; }
    public OrbitalTransferStatus Status { get; private set; }

    public static OrbitalTransfer CreatePlanned(
        Guid civilizationId,
        Guid orbitalGroupId,
        Guid originPlanetId,
        Guid destinationPlanetId,
        int abstractDistanceUnits,
        DateTime departureAtUtc,
        DateTime arrivalAtUtc) =>
        new(
            civilizationId,
            orbitalGroupId,
            originPlanetId,
            destinationPlanetId,
            abstractDistanceUnits,
            departureAtUtc,
            arrivalAtUtc,
            OrbitalTransferStatus.Planned);

    public void Complete(DateTime completedAtUtc)
    {
        if (completedAtUtc.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("Completion date must be UTC.", nameof(completedAtUtc));
        }

        if (completedAtUtc < ArrivalAtUtc)
        {
            throw new InvalidOperationException("Only arrived orbital transfers can be completed.");
        }

        if (Status == OrbitalTransferStatus.Completed)
        {
            return;
        }

        if (Status != OrbitalTransferStatus.Planned && Status != OrbitalTransferStatus.InTransit)
        {
            throw new InvalidOperationException("Only planned or in-transit orbital transfers can be completed.");
        }

        Status = OrbitalTransferStatus.Completed;
    }

    public void Cancel()
    {
        if (Status == OrbitalTransferStatus.Completed)
        {
            throw new InvalidOperationException("Completed orbital transfers cannot be cancelled.");
        }

        if (Status == OrbitalTransferStatus.Cancelled)
        {
            throw new InvalidOperationException("Orbital transfer is already cancelled.");
        }

        Status = OrbitalTransferStatus.Cancelled;
    }
}
