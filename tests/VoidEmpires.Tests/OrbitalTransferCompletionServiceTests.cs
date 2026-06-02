using Microsoft.EntityFrameworkCore;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class OrbitalTransferCompletionServiceTests
{
    [Fact]
    public async Task CompleteDueAsyncCompletesArrivedTransferAndMovesGroup()
    {
        await using var dbContext = CreateDbContext();
        var nowUtc = new DateTime(2026, 5, 28, 12, 0, 0, DateTimeKind.Utc);
        var group = CreateReservedGroup(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var destinationPlanetId = Guid.NewGuid();
        var transfer = CreateTransfer(group, destinationPlanetId, nowUtc.AddHours(-1), nowUtc);
        dbContext.Set<OrbitalGroup>().Add(group);
        dbContext.Set<OrbitalTransfer>().Add(transfer);
        await dbContext.SaveChangesAsync();

        var result = await new OrbitalTransferCompletionService(dbContext).CompleteDueAsync(nowUtc);

        Assert.Equal(1, result.CompletedCount);
        Assert.Contains(transfer.Id, result.CompletedTransferIds);
        Assert.Contains(group.Id, result.CompletedOrbitalGroupIds);

        var persistedTransfer = await dbContext.Set<OrbitalTransfer>().SingleAsync(x => x.Id == transfer.Id);
        Assert.Equal(OrbitalTransferStatus.Completed, persistedTransfer.Status);

        var persistedGroup = await dbContext.Set<OrbitalGroup>().SingleAsync(x => x.Id == group.Id);
        Assert.Equal(destinationPlanetId, persistedGroup.CurrentPlanetId);
        Assert.Equal(OrbitalGroupStatus.Stationed, persistedGroup.Status);
    }

    [Fact]
    public async Task CompleteDueAsyncDoesNotCompleteFutureTransfer()
    {
        await using var dbContext = CreateDbContext();
        var nowUtc = new DateTime(2026, 5, 28, 12, 0, 0, DateTimeKind.Utc);
        var group = CreateReservedGroup(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var destinationPlanetId = Guid.NewGuid();
        var transfer = CreateTransfer(group, destinationPlanetId, nowUtc, nowUtc.AddHours(1));
        dbContext.Set<OrbitalGroup>().Add(group);
        dbContext.Set<OrbitalTransfer>().Add(transfer);
        await dbContext.SaveChangesAsync();

        var result = await new OrbitalTransferCompletionService(dbContext).CompleteDueAsync(nowUtc);

        Assert.Equal(0, result.CompletedCount);
        Assert.Empty(result.CompletedTransferIds);
        Assert.Empty(result.CompletedOrbitalGroupIds);

        var persistedTransfer = await dbContext.Set<OrbitalTransfer>().SingleAsync(x => x.Id == transfer.Id);
        Assert.Equal(OrbitalTransferStatus.Planned, persistedTransfer.Status);

        var persistedGroup = await dbContext.Set<OrbitalGroup>().SingleAsync(x => x.Id == group.Id);
        Assert.NotEqual(destinationPlanetId, persistedGroup.CurrentPlanetId);
        Assert.Equal(OrbitalGroupStatus.Reserved, persistedGroup.Status);
    }

    [Fact]
    public async Task CompleteDueAsyncDoesNotProcessCompletedTransferAgain()
    {
        await using var dbContext = CreateDbContext();
        var nowUtc = new DateTime(2026, 5, 28, 12, 0, 0, DateTimeKind.Utc);
        var group = CreateReservedGroup(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var destinationPlanetId = Guid.NewGuid();
        var transfer = CreateTransfer(group, destinationPlanetId, nowUtc.AddHours(-2), nowUtc.AddHours(-1));
        group.ArriveAt(destinationPlanetId);
        transfer.Complete(nowUtc.AddHours(-1));
        dbContext.Set<OrbitalGroup>().Add(group);
        dbContext.Set<OrbitalTransfer>().Add(transfer);
        await dbContext.SaveChangesAsync();

        var result = await new OrbitalTransferCompletionService(dbContext).CompleteDueAsync(nowUtc);

        Assert.Equal(0, result.CompletedCount);
        Assert.Empty(result.CompletedTransferIds);
        Assert.Empty(result.CompletedOrbitalGroupIds);

        var persistedTransfer = await dbContext.Set<OrbitalTransfer>().SingleAsync(x => x.Id == transfer.Id);
        Assert.Equal(OrbitalTransferStatus.Completed, persistedTransfer.Status);

        var persistedGroup = await dbContext.Set<OrbitalGroup>().SingleAsync(x => x.Id == group.Id);
        Assert.Equal(destinationPlanetId, persistedGroup.CurrentPlanetId);
        Assert.Equal(OrbitalGroupStatus.Stationed, persistedGroup.Status);
    }

    [Fact]
    public async Task CompleteDueAsyncIsIdempotentAcrossRepeatedCalls()
    {
        await using var dbContext = CreateDbContext();
        var nowUtc = new DateTime(2026, 5, 28, 12, 0, 0, DateTimeKind.Utc);
        var group = CreateReservedGroup(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var destinationPlanetId = Guid.NewGuid();
        var transfer = CreateTransfer(group, destinationPlanetId, nowUtc.AddHours(-1), nowUtc);
        dbContext.Set<OrbitalGroup>().Add(group);
        dbContext.Set<OrbitalTransfer>().Add(transfer);
        await dbContext.SaveChangesAsync();

        var service = new OrbitalTransferCompletionService(dbContext);
        var firstResult = await service.CompleteDueAsync(nowUtc);
        var secondResult = await service.CompleteDueAsync(nowUtc.AddMinutes(1));

        Assert.Equal(1, firstResult.CompletedCount);
        Assert.Contains(transfer.Id, firstResult.CompletedTransferIds);
        Assert.Contains(group.Id, firstResult.CompletedOrbitalGroupIds);
        Assert.Equal(0, secondResult.CompletedCount);
        Assert.Empty(secondResult.CompletedTransferIds);
        Assert.Empty(secondResult.CompletedOrbitalGroupIds);
    }

    [Fact]
    public async Task CompleteDueAsyncProcessesMultipleDueTransfers()
    {
        await using var dbContext = CreateDbContext();
        var nowUtc = new DateTime(2026, 5, 28, 12, 0, 0, DateTimeKind.Utc);
        var firstGroup = CreateReservedGroup(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var secondGroup = CreateReservedGroup(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var firstDestinationPlanetId = Guid.NewGuid();
        var secondDestinationPlanetId = Guid.NewGuid();
        var firstTransfer = CreateTransfer(firstGroup, firstDestinationPlanetId, nowUtc.AddHours(-2), nowUtc.AddHours(-1));
        var secondTransfer = CreateTransfer(secondGroup, secondDestinationPlanetId, nowUtc.AddHours(-3), nowUtc);
        dbContext.Set<OrbitalGroup>().AddRange(firstGroup, secondGroup);
        dbContext.Set<OrbitalTransfer>().AddRange(firstTransfer, secondTransfer);
        await dbContext.SaveChangesAsync();

        var result = await new OrbitalTransferCompletionService(dbContext).CompleteDueAsync(nowUtc);

        Assert.Equal(2, result.CompletedCount);
        Assert.Contains(firstTransfer.Id, result.CompletedTransferIds);
        Assert.Contains(secondTransfer.Id, result.CompletedTransferIds);
        Assert.Contains(firstGroup.Id, result.CompletedOrbitalGroupIds);
        Assert.Contains(secondGroup.Id, result.CompletedOrbitalGroupIds);

        var transfers = await dbContext.Set<OrbitalTransfer>().ToListAsync();
        Assert.All(transfers, x => Assert.Equal(OrbitalTransferStatus.Completed, x.Status));

        var persistedFirstGroup = await dbContext.Set<OrbitalGroup>().SingleAsync(x => x.Id == firstGroup.Id);
        var persistedSecondGroup = await dbContext.Set<OrbitalGroup>().SingleAsync(x => x.Id == secondGroup.Id);
        Assert.Equal(firstDestinationPlanetId, persistedFirstGroup.CurrentPlanetId);
        Assert.Equal(secondDestinationPlanetId, persistedSecondGroup.CurrentPlanetId);
        Assert.Equal(OrbitalGroupStatus.Stationed, persistedFirstGroup.Status);
        Assert.Equal(OrbitalGroupStatus.Stationed, persistedSecondGroup.Status);
    }

    [Fact]
    public async Task CompleteDueAsyncProcessesDueTransfersAcrossCivilizationsByDesign()
    {
        await using var dbContext = CreateDbContext();
        var nowUtc = new DateTime(2026, 5, 28, 12, 0, 0, DateTimeKind.Utc);
        var firstGroup = CreateReservedGroup(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var secondGroup = CreateReservedGroup(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var firstDestinationPlanetId = Guid.NewGuid();
        var secondDestinationPlanetId = Guid.NewGuid();
        var firstTransfer = CreateTransfer(firstGroup, firstDestinationPlanetId, nowUtc.AddHours(-1), nowUtc);
        var secondTransfer = CreateTransfer(secondGroup, secondDestinationPlanetId, nowUtc.AddHours(-2), nowUtc);
        dbContext.Set<OrbitalGroup>().AddRange(firstGroup, secondGroup);
        dbContext.Set<OrbitalTransfer>().AddRange(firstTransfer, secondTransfer);
        await dbContext.SaveChangesAsync();

        var result = await new OrbitalTransferCompletionService(dbContext).CompleteDueAsync(nowUtc);

        Assert.Equal(2, result.CompletedCount);
        Assert.Contains(firstTransfer.Id, result.CompletedTransferIds);
        Assert.Contains(secondTransfer.Id, result.CompletedTransferIds);
        Assert.Contains(firstGroup.Id, result.CompletedOrbitalGroupIds);
        Assert.Contains(secondGroup.Id, result.CompletedOrbitalGroupIds);

        var persistedTransfers = await dbContext.Set<OrbitalTransfer>()
            .OrderBy(x => x.Id)
            .ToListAsync();
        Assert.All(persistedTransfers, transfer => Assert.Equal(OrbitalTransferStatus.Completed, transfer.Status));

        var persistedGroups = await dbContext.Set<OrbitalGroup>()
            .OrderBy(x => x.Id)
            .ToListAsync();
        Assert.Contains(persistedGroups, group =>
            group.Id == firstGroup.Id &&
            group.CivilizationId == firstGroup.CivilizationId &&
            group.CurrentPlanetId == firstDestinationPlanetId &&
            group.Status == OrbitalGroupStatus.Stationed);
        Assert.Contains(persistedGroups, group =>
            group.Id == secondGroup.Id &&
            group.CivilizationId == secondGroup.CivilizationId &&
            group.CurrentPlanetId == secondDestinationPlanetId &&
            group.Status == OrbitalGroupStatus.Stationed);
    }

    [Fact]
    public async Task CompleteDueAsyncSkipsDueTransferWhenGroupIsNotReserved()
    {
        await using var dbContext = CreateDbContext();
        var nowUtc = new DateTime(2026, 5, 28, 12, 0, 0, DateTimeKind.Utc);
        var group = OrbitalGroup.CreateStationed(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            SpaceAssetType.ScoutCraft,
            1);
        var destinationPlanetId = Guid.NewGuid();
        var transfer = CreateTransfer(group, destinationPlanetId, nowUtc.AddHours(-1), nowUtc);
        dbContext.Set<OrbitalGroup>().Add(group);
        dbContext.Set<OrbitalTransfer>().Add(transfer);
        await dbContext.SaveChangesAsync();

        var result = await new OrbitalTransferCompletionService(dbContext).CompleteDueAsync(nowUtc);

        Assert.Equal(0, result.CompletedCount);
        Assert.Empty(result.CompletedTransferIds);
        Assert.Empty(result.CompletedOrbitalGroupIds);

        var persistedTransfer = await dbContext.Set<OrbitalTransfer>().SingleAsync(x => x.Id == transfer.Id);
        Assert.Equal(OrbitalTransferStatus.Planned, persistedTransfer.Status);

        var persistedGroup = await dbContext.Set<OrbitalGroup>().SingleAsync(x => x.Id == group.Id);
        Assert.Equal(group.CurrentPlanetId, persistedGroup.CurrentPlanetId);
        Assert.Equal(OrbitalGroupStatus.Stationed, persistedGroup.Status);
    }

    [Fact]
    public async Task CompleteDueAsyncRequiresUtcNow()
    {
        await using var dbContext = CreateDbContext();
        var service = new OrbitalTransferCompletionService(dbContext);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CompleteDueAsync(new DateTime(2026, 5, 28, 12, 0, 0, DateTimeKind.Local)));
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

    private static OrbitalTransfer CreateTransfer(
        OrbitalGroup group,
        Guid destinationPlanetId,
        DateTime departureAtUtc,
        DateTime arrivalAtUtc) =>
        OrbitalTransfer.CreatePlanned(
            group.CivilizationId,
            group.Id,
            group.CurrentPlanetId,
            destinationPlanetId,
            1,
            departureAtUtc,
            arrivalAtUtc);

    private static VoidEmpiresDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new VoidEmpiresDbContext(options);
    }
}
