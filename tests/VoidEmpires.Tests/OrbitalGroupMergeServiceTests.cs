using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class OrbitalGroupMergeServiceTests
{
    [Fact]
    public async Task MergeAsyncRejectsMissingTargetGroup()
    {
        await using var dbContext = CreateDbContext();
        var source = OrbitalGroup.CreateStationed(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), SpaceAssetType.ScoutCraft, 1);
        dbContext.Set<OrbitalGroup>().Add(source);
        await dbContext.SaveChangesAsync();
        var service = new OrbitalGroupMergeService(dbContext);

        var result = await service.MergeAsync(new MergeOrbitalGroupsRequest(source.CivilizationId, Guid.NewGuid(), source.Id));

        Assert.False(result.Succeeded);
        Assert.Contains("Target orbital group was not found.", result.Errors);
    }

    [Fact]
    public async Task MergeAsyncRejectsMissingSourceGroup()
    {
        await using var dbContext = CreateDbContext();
        var target = OrbitalGroup.CreateStationed(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), SpaceAssetType.ScoutCraft, 1);
        dbContext.Set<OrbitalGroup>().Add(target);
        await dbContext.SaveChangesAsync();
        var service = new OrbitalGroupMergeService(dbContext);

        var result = await service.MergeAsync(new MergeOrbitalGroupsRequest(target.CivilizationId, target.Id, Guid.NewGuid()));

        Assert.False(result.Succeeded);
        Assert.Contains("Source orbital group was not found.", result.Errors);
    }

    [Fact]
    public async Task MergeAsyncRejectsSameGroupIds()
    {
        await using var dbContext = CreateDbContext();
        var groupId = Guid.NewGuid();
        var service = new OrbitalGroupMergeService(dbContext);

        var result = await service.MergeAsync(new MergeOrbitalGroupsRequest(Guid.NewGuid(), groupId, groupId));

        Assert.False(result.Succeeded);
        Assert.Contains("Target and source orbital groups must be different.", result.Errors);
    }

    [Fact]
    public async Task MergeAsyncRejectsCivilizationMismatch()
    {
        await using var dbContext = CreateDbContext();
        var planetId = Guid.NewGuid();
        var target = OrbitalGroup.CreateStationed(Guid.NewGuid(), Guid.NewGuid(), planetId, SpaceAssetType.ScoutCraft, 1);
        var source = OrbitalGroup.CreateStationed(Guid.NewGuid(), Guid.NewGuid(), planetId, SpaceAssetType.ScoutCraft, 1);
        dbContext.Set<OrbitalGroup>().AddRange(target, source);
        await dbContext.SaveChangesAsync();
        var service = new OrbitalGroupMergeService(dbContext);

        var result = await service.MergeAsync(new MergeOrbitalGroupsRequest(target.CivilizationId, target.Id, source.Id));

        Assert.False(result.Succeeded);
        Assert.Contains("Both orbital groups must belong to the civilization.", result.Errors);
    }

    [Fact]
    public async Task MergeAsyncRejectsDifferentCurrentPlanets()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var target = OrbitalGroup.CreateStationed(civilizationId, Guid.NewGuid(), Guid.NewGuid(), SpaceAssetType.ScoutCraft, 1);
        var source = OrbitalGroup.CreateStationed(civilizationId, Guid.NewGuid(), Guid.NewGuid(), SpaceAssetType.ScoutCraft, 1);
        dbContext.Set<OrbitalGroup>().AddRange(target, source);
        await dbContext.SaveChangesAsync();
        var service = new OrbitalGroupMergeService(dbContext);

        var result = await service.MergeAsync(new MergeOrbitalGroupsRequest(civilizationId, target.Id, source.Id));

        Assert.False(result.Succeeded);
        Assert.Contains("Orbital groups must be at the same current planet.", result.Errors);
    }

    [Fact]
    public async Task MergeAsyncRejectsDifferentAssetTypes()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var planetId = Guid.NewGuid();
        var target = OrbitalGroup.CreateStationed(civilizationId, Guid.NewGuid(), planetId, SpaceAssetType.ScoutCraft, 1);
        var source = OrbitalGroup.CreateStationed(civilizationId, Guid.NewGuid(), planetId, SpaceAssetType.EscortCraft, 1);
        dbContext.Set<OrbitalGroup>().AddRange(target, source);
        await dbContext.SaveChangesAsync();
        var service = new OrbitalGroupMergeService(dbContext);

        var result = await service.MergeAsync(new MergeOrbitalGroupsRequest(civilizationId, target.Id, source.Id));

        Assert.False(result.Succeeded);
        Assert.Contains("Orbital groups must have the same asset type.", result.Errors);
    }

    [Fact]
    public async Task MergeAsyncRejectsReservedGroup()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var planetId = Guid.NewGuid();
        var target = OrbitalGroup.CreateStationed(civilizationId, Guid.NewGuid(), planetId, SpaceAssetType.ScoutCraft, 1);
        var source = OrbitalGroup.CreateStationed(civilizationId, Guid.NewGuid(), planetId, SpaceAssetType.ScoutCraft, 1);
        source.Reserve();
        dbContext.Set<OrbitalGroup>().AddRange(target, source);
        await dbContext.SaveChangesAsync();
        var service = new OrbitalGroupMergeService(dbContext);

        var result = await service.MergeAsync(new MergeOrbitalGroupsRequest(civilizationId, target.Id, source.Id));

        Assert.False(result.Succeeded);
        Assert.Contains("Only stationed orbital groups can be merged.", result.Errors);
    }

    [Fact]
    public async Task MergeAsyncIncreasesTargetQuantityAndRemovesSource()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var planetId = Guid.NewGuid();
        var target = OrbitalGroup.CreateStationed(civilizationId, Guid.NewGuid(), planetId, SpaceAssetType.EscortCraft, 2);
        var source = OrbitalGroup.CreateStationed(civilizationId, Guid.NewGuid(), planetId, SpaceAssetType.EscortCraft, 3);
        dbContext.Set<OrbitalGroup>().AddRange(target, source);
        await dbContext.SaveChangesAsync();
        var service = new OrbitalGroupMergeService(dbContext);

        var result = await service.MergeAsync(new MergeOrbitalGroupsRequest(civilizationId, target.Id, source.Id));

        Assert.True(result.Succeeded);
        Assert.Equal(target.Id, result.TargetOrbitalGroupId);
        Assert.Equal(source.Id, result.SourceOrbitalGroupId);
        Assert.Equal(5, result.TargetQuantity);

        var persistedTarget = await dbContext.Set<OrbitalGroup>().SingleAsync(x => x.Id == target.Id);
        Assert.Equal(5, persistedTarget.Quantity);
        Assert.False(await dbContext.Set<OrbitalGroup>().AnyAsync(x => x.Id == source.Id));
    }

    private static VoidEmpiresDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new VoidEmpiresDbContext(options);
    }
}
