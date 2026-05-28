using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class OrbitalTransferPersistenceServiceTests
{
    [Fact]
    public async Task PersistAsyncCreatesTransferAndReservesGroup()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var currentPlanetId = Guid.NewGuid();
        var destinationPlanetId = Guid.NewGuid();
        var requestedAtUtc = new DateTime(2026, 5, 28, 12, 0, 0, DateTimeKind.Utc);
        var group = OrbitalGroup.CreateStationed(
            civilizationId,
            Guid.NewGuid(),
            currentPlanetId,
            SpaceAssetType.ScoutCraft,
            2);
        dbContext.Set<OrbitalGroup>().Add(group);
        await dbContext.SaveChangesAsync();
        var service = new OrbitalTransferPersistenceService(dbContext);

        var result = await service.PersistAsync(new PersistOrbitalTransferRequest(
            civilizationId,
            group.Id,
            destinationPlanetId,
            requestedAtUtc));

        Assert.True(result.Succeeded);
        Assert.NotNull(result.OrbitalTransferId);
        Assert.Equal(group.Id, result.OrbitalGroupId);
        Assert.Equal(currentPlanetId, result.OriginPlanetId);
        Assert.Equal(destinationPlanetId, result.DestinationPlanetId);
        Assert.Equal(1, result.AbstractDistanceUnits);
        Assert.Equal(requestedAtUtc, result.DepartureAtUtc);
        Assert.Equal(requestedAtUtc.AddHours(1), result.ArrivalAtUtc);
        Assert.Empty(result.Errors);

        var persistedTransfer = await dbContext.Set<OrbitalTransfer>().SingleAsync();
        Assert.Equal(result.OrbitalTransferId, persistedTransfer.Id);
        Assert.Equal(OrbitalTransferStatus.Planned, persistedTransfer.Status);

        var persistedGroup = await dbContext.Set<OrbitalGroup>().SingleAsync(x => x.Id == group.Id);
        Assert.Equal(OrbitalGroupStatus.Reserved, persistedGroup.Status);
        Assert.Equal(currentPlanetId, persistedGroup.CurrentPlanetId);
    }

    [Fact]
    public async Task PersistAsyncRejectsInvalidRequest()
    {
        await using var dbContext = CreateDbContext();
        var service = new OrbitalTransferPersistenceService(dbContext);

        var result = await service.PersistAsync(new PersistOrbitalTransferRequest(
            Guid.Empty,
            Guid.Empty,
            Guid.Empty,
            new DateTime(2026, 5, 28, 12, 0, 0, DateTimeKind.Local)));

        Assert.False(result.Succeeded);
        Assert.Contains("Civilization id is required.", result.Errors);
        Assert.Contains("Orbital group id is required.", result.Errors);
        Assert.Contains("Destination planet id is required.", result.Errors);
        Assert.Contains("Requested date must be UTC.", result.Errors);
    }

    [Fact]
    public async Task PersistAsyncRejectsUnknownGroupForCivilization()
    {
        await using var dbContext = CreateDbContext();
        var service = new OrbitalTransferPersistenceService(dbContext);

        var result = await service.PersistAsync(new PersistOrbitalTransferRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateTime(2026, 5, 28, 12, 0, 0, DateTimeKind.Utc)));

        Assert.False(result.Succeeded);
        Assert.Contains("Orbital group was not found for the civilization.", result.Errors);
    }

    [Fact]
    public async Task PersistAsyncRejectsReservedGroup()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var group = OrbitalGroup.CreateStationed(
            civilizationId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            SpaceAssetType.ScoutCraft,
            2);
        group.Reserve();
        dbContext.Set<OrbitalGroup>().Add(group);
        await dbContext.SaveChangesAsync();
        var service = new OrbitalTransferPersistenceService(dbContext);

        var result = await service.PersistAsync(new PersistOrbitalTransferRequest(
            civilizationId,
            group.Id,
            Guid.NewGuid(),
            new DateTime(2026, 5, 28, 12, 0, 0, DateTimeKind.Utc)));

        Assert.False(result.Succeeded);
        Assert.Contains("Only stationed orbital groups can be persisted for transfer.", result.Errors);
    }

    [Fact]
    public async Task PersistAsyncRejectsCurrentPlanetAsDestination()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var currentPlanetId = Guid.NewGuid();
        var group = OrbitalGroup.CreateStationed(
            civilizationId,
            Guid.NewGuid(),
            currentPlanetId,
            SpaceAssetType.ScoutCraft,
            2);
        dbContext.Set<OrbitalGroup>().Add(group);
        await dbContext.SaveChangesAsync();
        var service = new OrbitalTransferPersistenceService(dbContext);

        var result = await service.PersistAsync(new PersistOrbitalTransferRequest(
            civilizationId,
            group.Id,
            currentPlanetId,
            new DateTime(2026, 5, 28, 12, 0, 0, DateTimeKind.Utc)));

        Assert.False(result.Succeeded);
        Assert.Contains("Destination planet must be different from the current planet.", result.Errors);
    }

    private static VoidEmpiresDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new VoidEmpiresDbContext(options);
    }
}
