using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Infrastructure.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class OrbitalTravelEstimateServiceTests
{
    [Fact]
    public async Task EstimateAsyncReturnsDistanceDurationAndCosts()
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
        await dbContext.SaveChangesAsync();
        var service = new OrbitalTravelEstimateService(dbContext);

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
        Assert.Contains(result.ResourceCosts, x => x.ResourceType == ResourceType.Credits && x.Quantity == 7.5m);
        Assert.Contains(result.ResourceCosts, x => x.ResourceType == ResourceType.Gas && x.Quantity == 3m);
        Assert.Empty(result.Errors);
        Assert.Empty(await dbContext.Set<OrbitalTransfer>().ToListAsync());
        Assert.Equal(OrbitalGroupStatus.Stationed, (await dbContext.Set<OrbitalGroup>().SingleAsync()).Status);
    }

    [Fact]
    public async Task EstimateAsyncRejectsMissingOrbitalGroup()
    {
        await using var dbContext = CreateDbContext();
        dbContext.Set<Planet>().Add(CreatePlanet(Guid.NewGuid()));
        await dbContext.SaveChangesAsync();
        var service = new OrbitalTravelEstimateService(dbContext);

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
        var service = new OrbitalTravelEstimateService(dbContext);

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
        var service = new OrbitalTravelEstimateService(dbContext);

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
        var service = new OrbitalTravelEstimateService(dbContext);

        var result = await service.EstimateAsync(new EstimateOrbitalTravelRequest(
            civilizationId,
            group.Id,
            currentPlanetId));

        Assert.False(result.Succeeded);
        Assert.Contains("Destination planet must be different from the current planet.", result.Errors);
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
}
