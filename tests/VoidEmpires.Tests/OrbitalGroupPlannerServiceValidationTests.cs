using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class OrbitalGroupPlannerServiceValidationTests
{
    [Fact]
    public async Task PlanAsyncRejectsInvalidRequest()
    {
        await using var dbContext = CreateDbContext();
        var service = new OrbitalGroupPlannerService(dbContext);

        var result = await service.PlanAsync(new PlanOrbitalGroupTransferRequest(
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
    public async Task PlanAsyncRejectsReservedGroup()
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
        var service = new OrbitalGroupPlannerService(dbContext);

        var result = await service.PlanAsync(new PlanOrbitalGroupTransferRequest(
            civilizationId,
            group.Id,
            Guid.NewGuid(),
            new DateTime(2026, 5, 28, 12, 0, 0, DateTimeKind.Utc)));

        Assert.False(result.Succeeded);
        Assert.Contains("Only stationed orbital groups can be planned.", result.Errors);
    }

    [Fact]
    public async Task PlanAsyncRejectsCurrentPlanetAsDestination()
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
        var service = new OrbitalGroupPlannerService(dbContext);

        var result = await service.PlanAsync(new PlanOrbitalGroupTransferRequest(
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
