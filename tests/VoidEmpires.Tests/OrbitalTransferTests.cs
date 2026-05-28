using VoidEmpires.Domain.Fleets;

namespace VoidEmpires.Tests;

public class OrbitalTransferTests
{
    [Fact]
    public void CreatePlannedCreatesPlannedTransfer()
    {
        var civilizationId = Guid.NewGuid();
        var orbitalGroupId = Guid.NewGuid();
        var originPlanetId = Guid.NewGuid();
        var destinationPlanetId = Guid.NewGuid();
        var departureAtUtc = new DateTime(2026, 5, 28, 12, 0, 0, DateTimeKind.Utc);
        var arrivalAtUtc = departureAtUtc.AddHours(1);

        var transfer = OrbitalTransfer.CreatePlanned(
            civilizationId,
            orbitalGroupId,
            originPlanetId,
            destinationPlanetId,
            1,
            departureAtUtc,
            arrivalAtUtc);

        Assert.NotEqual(Guid.Empty, transfer.Id);
        Assert.Equal(civilizationId, transfer.CivilizationId);
        Assert.Equal(orbitalGroupId, transfer.OrbitalGroupId);
        Assert.Equal(originPlanetId, transfer.OriginPlanetId);
        Assert.Equal(destinationPlanetId, transfer.DestinationPlanetId);
        Assert.Equal(1, transfer.AbstractDistanceUnits);
        Assert.Equal(departureAtUtc, transfer.DepartureAtUtc);
        Assert.Equal(arrivalAtUtc, transfer.ArrivalAtUtc);
        Assert.Equal(OrbitalTransferStatus.Planned, transfer.Status);
    }

    [Fact]
    public void CreatePlannedRejectsSameOriginAndDestination()
    {
        var planetId = Guid.NewGuid();
        var departureAtUtc = new DateTime(2026, 5, 28, 12, 0, 0, DateTimeKind.Utc);

        Assert.Throws<ArgumentException>(() => OrbitalTransfer.CreatePlanned(
            Guid.NewGuid(),
            Guid.NewGuid(),
            planetId,
            planetId,
            1,
            departureAtUtc,
            departureAtUtc.AddHours(1)));
    }

    [Fact]
    public void CreatePlannedRequiresUtcDates()
    {
        var departureAtUtc = new DateTime(2026, 5, 28, 12, 0, 0, DateTimeKind.Utc);
        var localArrival = new DateTime(2026, 5, 28, 13, 0, 0, DateTimeKind.Local);

        Assert.Throws<ArgumentException>(() => OrbitalTransfer.CreatePlanned(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            departureAtUtc,
            localArrival));
    }

    [Fact]
    public void CreatePlannedRequiresArrivalAfterDeparture()
    {
        var departureAtUtc = new DateTime(2026, 5, 28, 12, 0, 0, DateTimeKind.Utc);

        Assert.Throws<ArgumentException>(() => OrbitalTransfer.CreatePlanned(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            departureAtUtc,
            departureAtUtc));
    }
}
