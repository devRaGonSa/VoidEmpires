using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class OrbitalTravelEstimateServiceTests
{
    private static readonly Guid CivilizationId = Guid.Parse("b8d33112-74ac-4f0d-85a4-c47d1a8f9f5c");
    private static readonly Guid OriginPlanetId = Guid.Parse("2fd71c50-2a16-4a2b-ae26-3367723a56cd");
    private static readonly Guid CurrentPlanetId = Guid.Parse("aa6c3794-2fa5-4567-85a8-e71690657f98");
    private static readonly Guid DestinationPlanetId = Guid.Parse("ed7db188-8f18-4dfc-992d-1b45bf0ac571");

    [Fact]
    public async Task EstimateAsyncReturnsValidationErrors()
    {
        await using var dbContext = CreateDbContext();
        var service = new OrbitalTravelEstimateService(dbContext);

        var result = await service.EstimateAsync(new EstimateOrbitalTravelRequest(Guid.Empty, Guid.Empty, Guid.Empty));

        Assert.False(result.Succeeded);
        Assert.Contains("Civilization id is required.", result.Errors);
        Assert.Contains("Orbital group id is required.", result.Errors);
        Assert.Contains("Destination planet id is required.", result.Errors);
    }

    [Fact]
    public async Task EstimateAsyncReturnsFailureWhenGroupDoesNotBelongToCivilization()
    {
        await using var dbContext = CreateDbContext();
        var service = new OrbitalTravelEstimateService(dbContext);

        var result = await service.EstimateAsync(new EstimateOrbitalTravelRequest(CivilizationId, Guid.NewGuid(), DestinationPlanetId));

        Assert.False(result.Succeeded);
        Assert.Contains("Orbital group was not found for the civilization.", result.Errors);
    }

    [Fact]
    public async Task EstimateAsyncRequiresStationedGroup()
    {
        await using var dbContext = CreateDbContext();
        var group = OrbitalGroup.CreateStationed(CivilizationId, OriginPlanetId, CurrentPlanetId, SpaceAssetType.ScoutCraft, 1);
        group.Reserve();
        dbContext.Set<OrbitalGroup>().Add(group);
        await dbContext.SaveChangesAsync();
        var service = new OrbitalTravelEstimateService(dbContext);

        var result = await service.EstimateAsync(new EstimateOrbitalTravelRequest(CivilizationId, group.Id, DestinationPlanetId));

        Assert.False(result.Succeeded);
        Assert.Contains("Only stationed orbital groups can be estimated.", result.Errors);
    }

    [Fact]
    public async Task EstimateAsyncRequiresDifferentDestination()
    {
        await using var dbContext = CreateDbContext();
        var group = OrbitalGroup.CreateStationed(CivilizationId, OriginPlanetId, CurrentPlanetId, SpaceAssetType.ScoutCraft, 1);
        dbContext.Set<OrbitalGroup>().Add(group);
        await dbContext.SaveChangesAsync();
        var service = new OrbitalTravelEstimateService(dbContext);

        var result = await service.EstimateAsync(new EstimateOrbitalTravelRequest(CivilizationId, group.Id, CurrentPlanetId));

        Assert.False(result.Succeeded);
        Assert.Contains("Destination planet must be different from the current planet.", result.Errors);
    }

    [Fact]
    public async Task EstimateAsyncReturnsDistanceDurationAndCost()
    {
        await using var dbContext = CreateDbContext();
        var group = OrbitalGroup.CreateStationed(CivilizationId, OriginPlanetId, CurrentPlanetId, SpaceAssetType.CargoCraft, 3);
        dbContext.Set<OrbitalGroup>().Add(group);
        await dbContext.SaveChangesAsync();
        var service = new OrbitalTravelEstimateService(dbContext);

        var result = await service.EstimateAsync(new EstimateOrbitalTravelRequest(CivilizationId, group.Id, DestinationPlanetId));

        Assert.True(result.Succeeded);
        Assert.Equal(group.Id, result.OrbitalGroupId);
        Assert.Equal(CurrentPlanetId, result.CurrentPlanetId);
        Assert.Equal(DestinationPlanetId, result.DestinationPlanetId);
        Assert.Equal(1, result.AbstractDistanceUnits);
        Assert.Equal(TimeSpan.FromHours(1), result.EstimatedDuration);
        var cost = Assert.Single(result.EstimatedCosts);
        Assert.Equal(ResourceType.Gas, cost.ResourceType);
        Assert.Equal(30, cost.Amount);
        Assert.Empty(result.Errors);
    }

    private static VoidEmpiresDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new VoidEmpiresDbContext(options);
    }
}
