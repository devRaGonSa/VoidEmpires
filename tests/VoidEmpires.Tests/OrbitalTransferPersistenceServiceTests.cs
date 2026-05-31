using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Economy;
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
        var stockpile = PlanetResourceStockpile.Create(currentPlanetId);
        stockpile.Increase(ResourceType.Credits, 5);
        stockpile.Increase(ResourceType.Gas, 2);
        dbContext.PlanetResourceStockpiles.Add(stockpile);
        await dbContext.SaveChangesAsync();
        var service = CreateService(dbContext);

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
        var persistedStockpile = await dbContext.PlanetResourceStockpiles.SingleAsync(x => x.PlanetId == currentPlanetId);
        Assert.Equal(0, persistedStockpile.Credits);
        Assert.Equal(0, persistedStockpile.Gas);
    }

    [Fact]
    public async Task PersistAsyncRejectsInsufficientResourcesWithoutMutatingTransferGroupOrStockpile()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var currentPlanetId = Guid.NewGuid();
        var destinationPlanetId = Guid.NewGuid();
        var group = OrbitalGroup.CreateStationed(
            civilizationId,
            Guid.NewGuid(),
            currentPlanetId,
            SpaceAssetType.CargoCraft,
            2);
        var stockpile = PlanetResourceStockpile.Create(currentPlanetId);
        stockpile.Increase(ResourceType.Credits, 1);
        stockpile.Increase(ResourceType.Gas, 10);
        dbContext.Set<OrbitalGroup>().Add(group);
        dbContext.PlanetResourceStockpiles.Add(stockpile);
        await dbContext.SaveChangesAsync();

        var result = await CreateService(dbContext).PersistAsync(new PersistOrbitalTransferRequest(
            civilizationId,
            group.Id,
            destinationPlanetId,
            new DateTime(2026, 5, 28, 12, 0, 0, DateTimeKind.Utc)));

        Assert.False(result.Succeeded);
        Assert.Contains("Insufficient Credits.", result.Errors);
        Assert.Empty(await dbContext.Set<OrbitalTransfer>().ToListAsync());
        Assert.Equal(OrbitalGroupStatus.Stationed, (await dbContext.Set<OrbitalGroup>().SingleAsync()).Status);
        var persistedStockpile = await dbContext.PlanetResourceStockpiles.SingleAsync(x => x.PlanetId == currentPlanetId);
        Assert.Equal(1, persistedStockpile.Credits);
        Assert.Equal(10, persistedStockpile.Gas);
    }

    [Fact]
    public async Task PersistAsyncRejectsInvalidRequest()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);

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
        var service = CreateService(dbContext);

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
        var service = CreateService(dbContext);

        var result = await service.PersistAsync(new PersistOrbitalTransferRequest(
            civilizationId,
            group.Id,
            Guid.NewGuid(),
            new DateTime(2026, 5, 28, 12, 0, 0, DateTimeKind.Utc)));

        Assert.False(result.Succeeded);
        Assert.Contains("Only stationed orbital groups can be persisted for transfer.", result.Errors);
    }

    [Fact]
    public async Task PersistAsyncRejectsSecondActiveTransferForGroup()
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
        group.Reserve();
        var existingTransfer = CreateTransfer(group, Guid.NewGuid());
        var stockpile = PlanetResourceStockpile.Create(currentPlanetId);
        stockpile.Increase(ResourceType.Credits, 5);
        stockpile.Increase(ResourceType.Gas, 2);
        dbContext.Set<OrbitalGroup>().Add(group);
        dbContext.Set<OrbitalTransfer>().Add(existingTransfer);
        dbContext.PlanetResourceStockpiles.Add(stockpile);
        await dbContext.SaveChangesAsync();
        var service = CreateService(dbContext);

        var result = await service.PersistAsync(new PersistOrbitalTransferRequest(
            civilizationId,
            group.Id,
            Guid.NewGuid(),
            new DateTime(2026, 5, 30, 12, 0, 0, DateTimeKind.Utc)));

        Assert.False(result.Succeeded);
        Assert.Contains("Orbital group already has an active transfer.", result.Errors);
        Assert.Equal(1, await dbContext.Set<OrbitalTransfer>().CountAsync());
        Assert.Equal(5, (await dbContext.PlanetResourceStockpiles.SingleAsync()).Credits);
        Assert.Equal(2, (await dbContext.PlanetResourceStockpiles.SingleAsync()).Gas);
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
        var service = CreateService(dbContext);

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

    private static OrbitalTransferPersistenceService CreateService(VoidEmpiresDbContext dbContext) =>
        new(dbContext, new ResourceSpendService(dbContext));

    private static OrbitalTransfer CreateTransfer(OrbitalGroup group, Guid destinationPlanetId) =>
        OrbitalTransfer.CreatePlanned(
            group.CivilizationId,
            group.Id,
            group.CurrentPlanetId,
            destinationPlanetId,
            1,
            new DateTime(2026, 5, 30, 12, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 5, 30, 13, 0, 0, DateTimeKind.Utc));
}
