using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Infrastructure.Economy;
using VoidEmpires.Infrastructure.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class OrbitalTravelEstimateServiceTests
{
    [Fact]
    public async Task EstimateAsyncReturnsAffordableDistanceDurationAndCosts()
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
        dbContext.Set<OrbitalGroup>().Add(group);
        dbContext.Set<Planet>().Add(CreatePlanet(destinationPlanetId));
        var stockpile = PlanetResourceStockpile.Create(currentPlanetId);
        stockpile.Increase(ResourceType.Credits, 10);
        stockpile.Increase(ResourceType.Gas, 5);
        dbContext.PlanetResourceStockpiles.Add(stockpile);
        await dbContext.SaveChangesAsync();
        var service = CreateService(dbContext);

        var result = await service.EstimateAsync(new EstimateOrbitalTravelRequest(
            civilizationId,
            group.Id,
            destinationPlanetId));

        Assert.True(result.Succeeded);
        Assert.Equal(group.Id, result.OrbitalGroupId);
        Assert.Equal(currentPlanetId, result.CurrentPlanetId);
        Assert.Equal(destinationPlanetId, result.DestinationPlanetId);
        Assert.Equal(1, result.AbstractDistanceUnits);
        Assert.Equal(TimeSpan.FromHours(1), result.EstimatedDuration);
        Assert.NotNull(result.RouteProfile);
        Assert.Equal(OrbitalRouteClass.LocalOrbit, result.RouteProfile.RouteClass);
        Assert.Equal(1, result.RouteProfile.DistanceBand);
        Assert.Equal(OrbitalRouteRiskBand.Low, result.RouteProfile.RiskBand);
        Assert.Equal(1m, result.RouteProfile.FuelMultiplier);
        Assert.True(result.RouteProfile.IsSupported);
        Assert.Contains(result.ResourceCosts, x => x.ResourceType == ResourceType.Credits && x.Quantity == 7.5m);
        Assert.Contains(result.ResourceCosts, x => x.ResourceType == ResourceType.Gas && x.Quantity == 3m);
        Assert.True(result.CanAfford);
        Assert.Empty(result.InsufficientResources);
        Assert.Empty(result.Errors);
        Assert.Empty(await dbContext.Set<OrbitalTransfer>().ToListAsync());
        Assert.Equal(OrbitalGroupStatus.Stationed, (await dbContext.Set<OrbitalGroup>().SingleAsync()).Status);
        Assert.Equal(10, (await dbContext.PlanetResourceStockpiles.SingleAsync()).Credits);
        Assert.Equal(5, (await dbContext.PlanetResourceStockpiles.SingleAsync()).Gas);
    }

    [Fact]
    public async Task EstimateAsyncReturnsInsufficientResourcesWithoutMutatingStockpile()
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
        stockpile.Increase(ResourceType.Gas, 2);
        dbContext.Set<OrbitalGroup>().Add(group);
        dbContext.Set<Planet>().Add(CreatePlanet(destinationPlanetId));
        dbContext.PlanetResourceStockpiles.Add(stockpile);
        await dbContext.SaveChangesAsync();

        var result = await CreateService(dbContext).EstimateAsync(new EstimateOrbitalTravelRequest(
            civilizationId,
            group.Id,
            destinationPlanetId));

        Assert.True(result.Succeeded);
        Assert.False(result.CanAfford);
        Assert.Contains(result.InsufficientResources, x =>
            x.ResourceType == ResourceType.Credits &&
            x.RequiredQuantity == 7.5m &&
            x.AvailableQuantity == 1m);
        Assert.Contains(result.InsufficientResources, x =>
            x.ResourceType == ResourceType.Gas &&
            x.RequiredQuantity == 3m &&
            x.AvailableQuantity == 2m);
        Assert.Equal(1, (await dbContext.PlanetResourceStockpiles.SingleAsync()).Credits);
        Assert.Equal(2, (await dbContext.PlanetResourceStockpiles.SingleAsync()).Gas);
    }

    [Fact]
    public async Task EstimateAsyncRejectsMissingOrbitalGroup()
    {
        await using var dbContext = CreateDbContext();
        dbContext.Set<Planet>().Add(CreatePlanet(Guid.NewGuid()));
        await dbContext.SaveChangesAsync();
        var service = CreateService(dbContext);

        var result = await service.EstimateAsync(new EstimateOrbitalTravelRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid()));

        Assert.False(result.Succeeded);
        Assert.Contains("Orbital group was not found for the civilization.", result.Errors);
    }

    [Fact]
    public async Task EstimateAsyncRejectsCivilizationMismatch()
    {
        await using var dbContext = CreateDbContext();
        var destinationPlanetId = Guid.NewGuid();
        var group = OrbitalGroup.CreateStationed(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            SpaceAssetType.ScoutCraft,
            1);
        dbContext.Set<OrbitalGroup>().Add(group);
        dbContext.Set<Planet>().Add(CreatePlanet(destinationPlanetId));
        await dbContext.SaveChangesAsync();
        var service = CreateService(dbContext);

        var result = await service.EstimateAsync(new EstimateOrbitalTravelRequest(
            Guid.NewGuid(),
            group.Id,
            destinationPlanetId));

        Assert.False(result.Succeeded);
        Assert.Contains("Orbital group was not found for the civilization.", result.Errors);
    }

    [Fact]
    public async Task EstimateAsyncRejectsMissingDestinationPlanet()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var group = OrbitalGroup.CreateStationed(
            civilizationId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            SpaceAssetType.ScoutCraft,
            1);
        dbContext.Set<OrbitalGroup>().Add(group);
        await dbContext.SaveChangesAsync();
        var service = CreateService(dbContext);

        var result = await service.EstimateAsync(new EstimateOrbitalTravelRequest(
            civilizationId,
            group.Id,
            Guid.NewGuid()));

        Assert.False(result.Succeeded);
        Assert.Contains("Destination planet was not found.", result.Errors);
    }

    [Fact]
    public async Task EstimateAsyncRejectsSameDestinationAsCurrentPlanet()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var currentPlanetId = Guid.NewGuid();
        var group = OrbitalGroup.CreateStationed(
            civilizationId,
            Guid.NewGuid(),
            currentPlanetId,
            SpaceAssetType.ScoutCraft,
            1);
        dbContext.Set<OrbitalGroup>().Add(group);
        dbContext.Set<Planet>().Add(CreatePlanet(currentPlanetId));
        await dbContext.SaveChangesAsync();
        var service = CreateService(dbContext);

        var result = await service.EstimateAsync(new EstimateOrbitalTravelRequest(
            civilizationId,
            group.Id,
            currentPlanetId));

        Assert.False(result.Succeeded);
        Assert.Contains("Destination planet must be different from the current planet.", result.Errors);
    }

    [Fact]
    public async Task EstimateAsyncRejectsGroupWithActiveTransfer()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var currentPlanetId = Guid.NewGuid();
        var destinationPlanetId = Guid.NewGuid();
        var group = OrbitalGroup.CreateStationed(
            civilizationId,
            Guid.NewGuid(),
            currentPlanetId,
            SpaceAssetType.ScoutCraft,
            1);
        group.Reserve();
        dbContext.Set<OrbitalGroup>().Add(group);
        dbContext.Set<OrbitalTransfer>().Add(CreateTransfer(group, destinationPlanetId));
        dbContext.Set<Planet>().Add(CreatePlanet(destinationPlanetId));
        await dbContext.SaveChangesAsync();
        var service = CreateService(dbContext);

        var result = await service.EstimateAsync(new EstimateOrbitalTravelRequest(
            civilizationId,
            group.Id,
            destinationPlanetId));

        Assert.False(result.Succeeded);
        Assert.Contains("Orbital group already has an active transfer.", result.Errors);
    }

    private static Planet CreatePlanet(Guid planetId) =>
        new(planetId, Guid.NewGuid(), "Asterion", 1, PlanetType.Terran, 100);

    private static VoidEmpiresDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new VoidEmpiresDbContext(options);
    }

    private static OrbitalTravelEstimateService CreateService(VoidEmpiresDbContext dbContext) =>
        new(dbContext, new ResourceSpendService(dbContext), new OrbitalRouteProfileService());

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
