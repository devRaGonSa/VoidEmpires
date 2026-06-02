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
        await AssertGroupStateAsync(dbContext, source.Id, source.Quantity, source.Status);
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
        await AssertGroupStateAsync(dbContext, target.Id, target.Quantity, target.Status);
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
        Assert.Empty(await dbContext.Set<OrbitalGroup>().ToListAsync());
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
        await AssertGroupsUnchangedAsync(dbContext, (target.Id, target.Quantity, target.Status), (source.Id, source.Quantity, source.Status));
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
        await AssertGroupsUnchangedAsync(dbContext, (target.Id, target.Quantity, target.Status), (source.Id, source.Quantity, source.Status));
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
        await AssertGroupsUnchangedAsync(dbContext, (target.Id, target.Quantity, target.Status), (source.Id, source.Quantity, source.Status));
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
        await AssertGroupsUnchangedAsync(dbContext, (target.Id, target.Quantity, target.Status), (source.Id, source.Quantity, source.Status));
    }

    [Fact]
    public async Task MergeAsyncRejectsActiveTransferSourceGroup()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var planetId = Guid.NewGuid();
        var target = OrbitalGroup.CreateStationed(civilizationId, Guid.NewGuid(), planetId, SpaceAssetType.ScoutCraft, 1);
        var source = OrbitalGroup.CreateStationed(civilizationId, Guid.NewGuid(), planetId, SpaceAssetType.ScoutCraft, 1);
        source.Reserve();
        dbContext.Set<OrbitalGroup>().AddRange(target, source);
        dbContext.Set<OrbitalTransfer>().Add(CreateTransfer(source, Guid.NewGuid()));
        await dbContext.SaveChangesAsync();
        var service = new OrbitalGroupMergeService(dbContext);

        var result = await service.MergeAsync(new MergeOrbitalGroupsRequest(civilizationId, target.Id, source.Id));

        Assert.False(result.Succeeded);
        Assert.Contains("Source orbital group already has an active transfer.", result.Errors);
        await AssertGroupsUnchangedAsync(dbContext, (target.Id, target.Quantity, target.Status), (source.Id, source.Quantity, source.Status));
    }

    [Fact]
    public async Task MergeAsyncRejectsActiveTransferTargetGroup()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var planetId = Guid.NewGuid();
        var target = OrbitalGroup.CreateStationed(civilizationId, Guid.NewGuid(), planetId, SpaceAssetType.ScoutCraft, 1);
        var source = OrbitalGroup.CreateStationed(civilizationId, Guid.NewGuid(), planetId, SpaceAssetType.ScoutCraft, 1);
        target.Reserve();
        dbContext.Set<OrbitalGroup>().AddRange(target, source);
        dbContext.Set<OrbitalTransfer>().Add(CreateTransfer(target, Guid.NewGuid()));
        await dbContext.SaveChangesAsync();
        var service = new OrbitalGroupMergeService(dbContext);

        var result = await service.MergeAsync(new MergeOrbitalGroupsRequest(civilizationId, target.Id, source.Id));

        Assert.False(result.Succeeded);
        Assert.Contains("Target orbital group already has an active transfer.", result.Errors);
        await AssertGroupsUnchangedAsync(dbContext, (target.Id, target.Quantity, target.Status), (source.Id, source.Quantity, source.Status));
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
        Assert.Equal(OrbitalGroupStatus.Stationed, persistedTarget.Status);
        Assert.Equal(planetId, persistedTarget.CurrentPlanetId);
        Assert.False(await dbContext.Set<OrbitalGroup>().AnyAsync(x => x.Id == source.Id));
    }

    [Fact]
    public async Task MergeAsyncRepeatedInvalidCallsDoNotMutateQuantitiesOrRemoveGroups()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var target = OrbitalGroup.CreateStationed(civilizationId, Guid.NewGuid(), Guid.NewGuid(), SpaceAssetType.ScoutCraft, 2);
        var source = OrbitalGroup.CreateStationed(civilizationId, Guid.NewGuid(), Guid.NewGuid(), SpaceAssetType.ScoutCraft, 3);
        dbContext.Set<OrbitalGroup>().AddRange(target, source);
        await dbContext.SaveChangesAsync();
        var service = new OrbitalGroupMergeService(dbContext);
        var request = new MergeOrbitalGroupsRequest(civilizationId, target.Id, source.Id);

        var firstResult = await service.MergeAsync(request);
        var secondResult = await service.MergeAsync(request);

        Assert.False(firstResult.Succeeded);
        Assert.False(secondResult.Succeeded);
        Assert.Contains("Orbital groups must be at the same current planet.", firstResult.Errors);
        Assert.Contains("Orbital groups must be at the same current planet.", secondResult.Errors);
        await AssertGroupsUnchangedAsync(dbContext, (target.Id, target.Quantity, target.Status), (source.Id, source.Quantity, source.Status));
    }

    private static VoidEmpiresDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new VoidEmpiresDbContext(options);
    }

    private static OrbitalTransfer CreateTransfer(OrbitalGroup group, Guid destinationPlanetId) =>
        OrbitalTransfer.CreatePlanned(
            group.CivilizationId,
            group.Id,
            group.CurrentPlanetId,
            destinationPlanetId,
            1,
            new DateTime(2026, 5, 30, 12, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 5, 30, 13, 0, 0, DateTimeKind.Utc));

    private static Task AssertGroupsUnchangedAsync(
        VoidEmpiresDbContext dbContext,
        params (Guid Id, int Quantity, OrbitalGroupStatus Status)[] expectedGroups) =>
        Task.WhenAll(expectedGroups.Select(expected => AssertGroupStateAsync(dbContext, expected.Id, expected.Quantity, expected.Status)));

    private static async Task AssertGroupStateAsync(
        VoidEmpiresDbContext dbContext,
        Guid groupId,
        int expectedQuantity,
        OrbitalGroupStatus expectedStatus)
    {
        var persisted = await dbContext.Set<OrbitalGroup>().SingleAsync(x => x.Id == groupId);
        Assert.Equal(expectedQuantity, persisted.Quantity);
        Assert.Equal(expectedStatus, persisted.Status);
    }
}
