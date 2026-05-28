using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class OrbitalGroupLookupServiceTests
{
    [Fact]
    public async Task ListAsyncReturnsGroupsForCivilization()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var originPlanetId = Guid.NewGuid();
        var currentPlanetId = Guid.NewGuid();
        var group = OrbitalGroup.CreateStationed(
            civilizationId,
            originPlanetId,
            currentPlanetId,
            SpaceAssetType.ScoutCraft,
            2);
        dbContext.Set<OrbitalGroup>().Add(group);
        await dbContext.SaveChangesAsync();

        var service = new OrbitalGroupLookupService(dbContext);

        var result = await service.ListAsync(new OrbitalGroupQueryRequest(civilizationId));

        var item = Assert.Single(result);
        Assert.Equal(group.Id, item.Id);
        Assert.Equal(civilizationId, item.CivilizationId);
        Assert.Equal(originPlanetId, item.OriginPlanetId);
        Assert.Equal(currentPlanetId, item.CurrentPlanetId);
        Assert.Equal(SpaceAssetType.ScoutCraft, item.AssetType);
        Assert.Equal(2, item.Quantity);
        Assert.Equal(OrbitalGroupStatus.Stationed, item.Status);
        Assert.True(item.IsStationedAwayFromOrigin);
    }

    [Fact]
    public async Task ListAsyncDoesNotReturnOtherCivilizationGroups()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        dbContext.Set<OrbitalGroup>().Add(OrbitalGroup.CreateStationed(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            SpaceAssetType.ScoutCraft,
            1));
        await dbContext.SaveChangesAsync();

        var service = new OrbitalGroupLookupService(dbContext);

        var result = await service.ListAsync(new OrbitalGroupQueryRequest(civilizationId));

        Assert.Empty(result);
    }

    [Fact]
    public async Task ListAsyncAppliesOptionalFilters()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var originPlanetId = Guid.NewGuid();
        var currentPlanetId = Guid.NewGuid();
        var matchingGroup = OrbitalGroup.CreateStationed(
            civilizationId,
            originPlanetId,
            currentPlanetId,
            SpaceAssetType.ScoutCraft,
            3);
        var wrongCurrentPlanetGroup = OrbitalGroup.CreateStationed(
            civilizationId,
            originPlanetId,
            Guid.NewGuid(),
            SpaceAssetType.ScoutCraft,
            3);
        var wrongOriginPlanetGroup = OrbitalGroup.CreateStationed(
            civilizationId,
            Guid.NewGuid(),
            currentPlanetId,
            SpaceAssetType.ScoutCraft,
            3);
        var wrongAssetTypeGroup = OrbitalGroup.CreateStationed(
            civilizationId,
            originPlanetId,
            currentPlanetId,
            SpaceAssetType.CargoCraft,
            3);
        var wrongStatusGroup = OrbitalGroup.CreateStationed(
            civilizationId,
            originPlanetId,
            currentPlanetId,
            SpaceAssetType.ScoutCraft,
            3);
        wrongStatusGroup.Reserve();
        dbContext.Set<OrbitalGroup>().AddRange(
            matchingGroup,
            wrongCurrentPlanetGroup,
            wrongOriginPlanetGroup,
            wrongAssetTypeGroup,
            wrongStatusGroup);
        await dbContext.SaveChangesAsync();

        var service = new OrbitalGroupLookupService(dbContext);

        var result = await service.ListAsync(new OrbitalGroupQueryRequest(
            civilizationId,
            currentPlanetId,
            originPlanetId,
            SpaceAssetType.ScoutCraft,
            OrbitalGroupStatus.Stationed));

        var item = Assert.Single(result);
        Assert.Equal(matchingGroup.Id, item.Id);
    }

    [Fact]
    public async Task ListAsyncReturnsEmptyForInvalidCivilizationId()
    {
        await using var dbContext = CreateDbContext();
        dbContext.Set<OrbitalGroup>().Add(OrbitalGroup.CreateStationed(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            SpaceAssetType.ScoutCraft,
            1));
        await dbContext.SaveChangesAsync();

        var service = new OrbitalGroupLookupService(dbContext);

        var result = await service.ListAsync(new OrbitalGroupQueryRequest(Guid.Empty));

        Assert.Empty(result);
    }

    private static VoidEmpiresDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new VoidEmpiresDbContext(options);
    }
}
