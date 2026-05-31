using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class OrbitalGroupSplitServiceTests
{
    [Fact]
    public async Task SplitAsyncRejectsInvalidQuantity()
    {
        await using var dbContext = CreateDbContext();
        var service = new OrbitalGroupSplitService(dbContext);

        var result = await service.SplitAsync(new SplitOrbitalGroupRequest(Guid.NewGuid(), Guid.NewGuid(), 0));

        Assert.False(result.Succeeded);
        Assert.Contains("Quantity must be positive.", result.Errors);
    }

    [Fact]
    public async Task SplitAsyncRejectsMissingSourceGroup()
    {
        await using var dbContext = CreateDbContext();
        var service = new OrbitalGroupSplitService(dbContext);

        var result = await service.SplitAsync(new SplitOrbitalGroupRequest(Guid.NewGuid(), Guid.NewGuid(), 1));

        Assert.False(result.Succeeded);
        Assert.Contains("Source orbital group was not found.", result.Errors);
    }

    [Fact]
    public async Task SplitAsyncRejectsCivilizationMismatch()
    {
        await using var dbContext = CreateDbContext();
        var group = OrbitalGroup.CreateStationed(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), SpaceAssetType.ScoutCraft, 3);
        dbContext.Set<OrbitalGroup>().Add(group);
        await dbContext.SaveChangesAsync();
        var service = new OrbitalGroupSplitService(dbContext);

        var result = await service.SplitAsync(new SplitOrbitalGroupRequest(Guid.NewGuid(), group.Id, 1));

        Assert.False(result.Succeeded);
        Assert.Contains("Source orbital group does not belong to the civilization.", result.Errors);
    }

    [Fact]
    public async Task SplitAsyncRejectsFullQuantity()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var group = OrbitalGroup.CreateStationed(civilizationId, Guid.NewGuid(), Guid.NewGuid(), SpaceAssetType.ScoutCraft, 3);
        dbContext.Set<OrbitalGroup>().Add(group);
        await dbContext.SaveChangesAsync();
        var service = new OrbitalGroupSplitService(dbContext);

        var result = await service.SplitAsync(new SplitOrbitalGroupRequest(civilizationId, group.Id, 3));

        Assert.False(result.Succeeded);
        Assert.Contains("Split quantity must be lower than source quantity.", result.Errors);
    }

    [Fact]
    public async Task SplitAsyncRejectsReservedGroup()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var group = OrbitalGroup.CreateStationed(civilizationId, Guid.NewGuid(), Guid.NewGuid(), SpaceAssetType.ScoutCraft, 3);
        group.Reserve();
        dbContext.Set<OrbitalGroup>().Add(group);
        await dbContext.SaveChangesAsync();
        var service = new OrbitalGroupSplitService(dbContext);

        var result = await service.SplitAsync(new SplitOrbitalGroupRequest(civilizationId, group.Id, 1));

        Assert.False(result.Succeeded);
        Assert.Contains("Only stationed orbital groups can be split.", result.Errors);
    }

    [Fact]
    public async Task SplitAsyncRejectsGroupWithActiveTransfer()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var group = OrbitalGroup.CreateStationed(civilizationId, Guid.NewGuid(), Guid.NewGuid(), SpaceAssetType.ScoutCraft, 3);
        group.Reserve();
        dbContext.Set<OrbitalGroup>().Add(group);
        dbContext.Set<OrbitalTransfer>().Add(CreateTransfer(group, Guid.NewGuid()));
        await dbContext.SaveChangesAsync();
        var service = new OrbitalGroupSplitService(dbContext);

        var result = await service.SplitAsync(new SplitOrbitalGroupRequest(civilizationId, group.Id, 1));

        Assert.False(result.Succeeded);
        Assert.Contains("Source orbital group already has an active transfer.", result.Errors);
    }

    [Fact]
    public async Task SplitAsyncCreatesNewGroupAndDecreasesSourceQuantity()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var originPlanetId = Guid.NewGuid();
        var currentPlanetId = Guid.NewGuid();
        var group = OrbitalGroup.CreateStationed(civilizationId, originPlanetId, currentPlanetId, SpaceAssetType.EscortCraft, 5);
        dbContext.Set<OrbitalGroup>().Add(group);
        await dbContext.SaveChangesAsync();
        var service = new OrbitalGroupSplitService(dbContext);

        var result = await service.SplitAsync(new SplitOrbitalGroupRequest(civilizationId, group.Id, 2));

        Assert.True(result.Succeeded);
        Assert.Equal(group.Id, result.SourceOrbitalGroupId);
        Assert.NotNull(result.NewOrbitalGroupId);
        Assert.Equal(3, result.SourceQuantity);
        Assert.Equal(2, result.NewQuantity);

        var source = await dbContext.Set<OrbitalGroup>().SingleAsync(x => x.Id == group.Id);
        var split = await dbContext.Set<OrbitalGroup>().SingleAsync(x => x.Id == result.NewOrbitalGroupId);
        Assert.Equal(3, source.Quantity);
        Assert.Equal(civilizationId, split.CivilizationId);
        Assert.Equal(originPlanetId, split.OriginPlanetId);
        Assert.Equal(currentPlanetId, split.CurrentPlanetId);
        Assert.Equal(SpaceAssetType.EscortCraft, split.AssetType);
        Assert.Equal(2, split.Quantity);
        Assert.Equal(OrbitalGroupStatus.Stationed, split.Status);
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
}
