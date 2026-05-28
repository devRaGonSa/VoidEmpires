using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class OrbitalGroupPlannerServiceSuccessOnlyTests
{
    [Fact]
    public async Task PlanAsyncReturnsSuccessfulPlanForStationedGroup()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var currentPlanetId = Guid.NewGuid();
        var destinationPlanetId = Guid.NewGuid();
        var requestedAtUtc = new DateTime(2026, 5, 28, 12, 0, 0, DateTimeKind.Utc);
        var group = OrbitalGroup.CreateStationed(civilizationId, Guid.NewGuid(), currentPlanetId, SpaceAssetType.ScoutCraft, 2);
        dbContext.Set<OrbitalGroup>().Add(group);
        await dbContext.SaveChangesAsync();
        var service = new OrbitalGroupPlannerService(dbContext);

        var result = await service.PlanAsync(new PlanOrbitalGroupTransferRequest(civilizationId, group.Id, destinationPlanetId, requestedAtUtc));

        Assert.True(result.Succeeded);
        Assert.Equal(group.Id, result.OrbitalGroupId);
        Assert.Equal(currentPlanetId, result.CurrentPlanetId);
        Assert.Equal(destinationPlanetId, result.DestinationPlanetId);
        Assert.Equal(1, result.AbstractDistanceUnits);
        Assert.Equal(requestedAtUtc, result.DepartureAtUtc);
        Assert.Equal(requestedAtUtc.AddHours(1), result.ArrivalAtUtc);
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
