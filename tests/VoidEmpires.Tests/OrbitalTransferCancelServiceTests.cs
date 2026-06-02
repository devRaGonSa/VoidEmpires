using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class OrbitalTransferCancelServiceTests
{
    [Fact]
    public async Task CancelAsyncRejectsMissingTransfer()
    {
        await using var dbContext = CreateDbContext();

        var result = await new OrbitalTransferCancelService(dbContext).CancelAsync(new CancelOrbitalTransferRequest(
            Guid.NewGuid(),
            Guid.NewGuid()));

        Assert.False(result.Succeeded);
        Assert.Equal(CancelOrbitalTransferResultStatus.NotFound, result.Status);
        Assert.Contains("Orbital transfer was not found.", result.Errors);
    }

    [Fact]
    public async Task CancelAsyncRejectsCivilizationMismatch()
    {
        await using var dbContext = CreateDbContext();
        var group = CreateReservedGroup(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var transfer = CreateTransfer(group, Guid.NewGuid());
        dbContext.Set<OrbitalGroup>().Add(group);
        dbContext.Set<OrbitalTransfer>().Add(transfer);
        await dbContext.SaveChangesAsync();

        var result = await new OrbitalTransferCancelService(dbContext).CancelAsync(new CancelOrbitalTransferRequest(
            Guid.NewGuid(),
            transfer.Id));

        Assert.False(result.Succeeded);
        Assert.Equal(CancelOrbitalTransferResultStatus.Conflict, result.Status);
        Assert.Contains("Orbital transfer does not belong to the civilization.", result.Errors);
        Assert.Equal(OrbitalTransferStatus.Planned, transfer.Status);
        Assert.Equal(OrbitalGroupStatus.Reserved, group.Status);
    }

    [Fact]
    public async Task CancelAsyncRejectsCompletedTransfer()
    {
        await using var dbContext = CreateDbContext();
        var group = CreateReservedGroup(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var destinationPlanetId = Guid.NewGuid();
        var transfer = CreateTransfer(group, destinationPlanetId);
        group.ArriveAt(destinationPlanetId);
        transfer.Complete(transfer.ArrivalAtUtc);
        dbContext.Set<OrbitalGroup>().Add(group);
        dbContext.Set<OrbitalTransfer>().Add(transfer);
        await dbContext.SaveChangesAsync();

        var result = await new OrbitalTransferCancelService(dbContext).CancelAsync(new CancelOrbitalTransferRequest(
            group.CivilizationId,
            transfer.Id));

        Assert.False(result.Succeeded);
        Assert.Equal(CancelOrbitalTransferResultStatus.Conflict, result.Status);
        Assert.Contains("Completed orbital transfers cannot be cancelled.", result.Errors);
    }

    [Fact]
    public async Task CancelAsyncRejectsAlreadyCancelledTransfer()
    {
        await using var dbContext = CreateDbContext();
        var group = CreateReservedGroup(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var transfer = CreateTransfer(group, Guid.NewGuid());
        transfer.Cancel();
        group.ReleaseReservation();
        dbContext.Set<OrbitalGroup>().Add(group);
        dbContext.Set<OrbitalTransfer>().Add(transfer);
        await dbContext.SaveChangesAsync();

        var result = await new OrbitalTransferCancelService(dbContext).CancelAsync(new CancelOrbitalTransferRequest(
            group.CivilizationId,
            transfer.Id));

        Assert.False(result.Succeeded);
        Assert.Equal(CancelOrbitalTransferResultStatus.Conflict, result.Status);
        Assert.Contains("Orbital transfer is already cancelled.", result.Errors);
    }

    [Fact]
    public async Task CancelAsyncMarksTransferCancelledReleasesGroupAndDoesNotRefund()
    {
        await using var dbContext = CreateDbContext();
        var currentPlanetId = Guid.NewGuid();
        var group = CreateReservedGroup(Guid.NewGuid(), Guid.NewGuid(), currentPlanetId);
        var transfer = CreateTransfer(group, Guid.NewGuid());
        var stockpile = PlanetResourceStockpile.Create(currentPlanetId);
        dbContext.Set<OrbitalGroup>().Add(group);
        dbContext.Set<OrbitalTransfer>().Add(transfer);
        dbContext.PlanetResourceStockpiles.Add(stockpile);
        await dbContext.SaveChangesAsync();

        var result = await new OrbitalTransferCancelService(dbContext).CancelAsync(new CancelOrbitalTransferRequest(
            group.CivilizationId,
            transfer.Id));

        Assert.True(result.Succeeded);
        Assert.Equal(CancelOrbitalTransferResultStatus.Succeeded, result.Status);
        Assert.Equal(transfer.Id, result.OrbitalTransferId);
        Assert.Equal(group.Id, result.OrbitalGroupId);

        var persistedTransfer = await dbContext.Set<OrbitalTransfer>().SingleAsync(x => x.Id == transfer.Id);
        Assert.Equal(OrbitalTransferStatus.Cancelled, persistedTransfer.Status);

        var persistedGroup = await dbContext.Set<OrbitalGroup>().SingleAsync(x => x.Id == group.Id);
        Assert.Equal(OrbitalGroupStatus.Stationed, persistedGroup.Status);
        Assert.Equal(currentPlanetId, persistedGroup.CurrentPlanetId);

        var persistedStockpile = await dbContext.PlanetResourceStockpiles.SingleAsync(x => x.PlanetId == currentPlanetId);
        Assert.Equal(0, persistedStockpile.Credits);
        Assert.Equal(0, persistedStockpile.Gas);
    }

    [Fact]
    public async Task CancelAsyncRejectsNonReservedGroupWithoutMutatingTransfer()
    {
        await using var dbContext = CreateDbContext();
        var group = OrbitalGroup.CreateStationed(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            SpaceAssetType.ScoutCraft,
            1);
        var transfer = CreateTransfer(group, Guid.NewGuid());
        dbContext.Set<OrbitalGroup>().Add(group);
        dbContext.Set<OrbitalTransfer>().Add(transfer);
        await dbContext.SaveChangesAsync();

        var result = await new OrbitalTransferCancelService(dbContext).CancelAsync(new CancelOrbitalTransferRequest(
            group.CivilizationId,
            transfer.Id));

        Assert.False(result.Succeeded);
        Assert.Equal(CancelOrbitalTransferResultStatus.Conflict, result.Status);
        Assert.Contains("Only reserved orbital groups can be released.", result.Errors);

        var persistedTransfer = await dbContext.Set<OrbitalTransfer>().SingleAsync(x => x.Id == transfer.Id);
        Assert.Equal(OrbitalTransferStatus.Planned, persistedTransfer.Status);

        var persistedGroup = await dbContext.Set<OrbitalGroup>().SingleAsync(x => x.Id == group.Id);
        Assert.Equal(OrbitalGroupStatus.Stationed, persistedGroup.Status);
        Assert.Equal(group.CurrentPlanetId, persistedGroup.CurrentPlanetId);
    }

    private static OrbitalGroup CreateReservedGroup(Guid civilizationId, Guid originPlanetId, Guid currentPlanetId)
    {
        var group = OrbitalGroup.CreateStationed(
            civilizationId,
            originPlanetId,
            currentPlanetId,
            SpaceAssetType.ScoutCraft,
            1);
        group.Reserve();
        return group;
    }

    private static OrbitalTransfer CreateTransfer(OrbitalGroup group, Guid destinationPlanetId) =>
        OrbitalTransfer.CreatePlanned(
            group.CivilizationId,
            group.Id,
            group.CurrentPlanetId,
            destinationPlanetId,
            1,
            new DateTime(2026, 5, 29, 12, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 5, 29, 13, 0, 0, DateTimeKind.Utc));

    private static VoidEmpiresDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}
