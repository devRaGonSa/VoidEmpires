using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class DevOrbitalGroupSplitMergeSeededScenarioTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid SeededCivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid SeededOwnedPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");

    [Fact]
    public async Task SplitThenMergeSeededScoutGroupsPreservesTotalQuantityAndExpectedState()
    {
        const string databaseName = "dev-orbital-group-split-merge-seeded-scenario";
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: databaseName);
        using var client = configuredFactory.CreateClient();

        await ApplyMinimalValidationSeedAsync(client);

        using var scope = configuredFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
        var sourceScout = await dbContext.Set<OrbitalGroup>()
            .AsNoTracking()
            .Where(x =>
                x.CivilizationId == SeededCivilizationId &&
                x.CurrentPlanetId == SeededOwnedPlanetId &&
                x.AssetType == SpaceAssetType.ScoutCraft &&
                x.Status == OrbitalGroupStatus.Stationed)
            .OrderByDescending(x => x.Quantity)
            .FirstAsync();
        var scoutQuantityBefore = await dbContext.Set<OrbitalGroup>()
            .AsNoTracking()
            .Where(x =>
                x.CivilizationId == SeededCivilizationId &&
                x.CurrentPlanetId == SeededOwnedPlanetId &&
                x.AssetType == SpaceAssetType.ScoutCraft)
            .SumAsync(x => x.Quantity);

        using (var splitResponse = await client.PostAsJsonAsync("/api/dev/fleets/orbital-groups/split", new
        {
            civilizationId = SeededCivilizationId,
            sourceOrbitalGroupId = sourceScout.Id,
            quantity = 1
        }))
        {
            var splitPayload = await splitResponse.Content.ReadFromJsonAsync<SplitOrbitalGroupResponse>();
            Assert.Equal(HttpStatusCode.Created, splitResponse.StatusCode);
            Assert.NotNull(splitPayload);
            Assert.Equal(0, splitPayload.Status);
            Assert.True(splitPayload.Succeeded);
            Assert.Equal(sourceScout.Id, splitPayload.SourceOrbitalGroupId);
            Assert.Equal(2, splitPayload.SourceQuantity);
            Assert.Equal(1, splitPayload.NewQuantity);
            Assert.NotNull(splitPayload.NewOrbitalGroupId);

            using var mergeResponse = await client.PostAsJsonAsync("/api/dev/fleets/orbital-groups/merge", new
            {
                civilizationId = SeededCivilizationId,
                targetOrbitalGroupId = sourceScout.Id,
                sourceOrbitalGroupId = splitPayload.NewOrbitalGroupId.Value
            });
            var mergePayload = await mergeResponse.Content.ReadFromJsonAsync<MergeOrbitalGroupsResponse>();

            Assert.Equal(HttpStatusCode.OK, mergeResponse.StatusCode);
            Assert.NotNull(mergePayload);
            Assert.Equal(0, mergePayload.Status);
            Assert.True(mergePayload.Succeeded);
            Assert.Equal(sourceScout.Id, mergePayload.TargetOrbitalGroupId);
            Assert.Equal(splitPayload.NewOrbitalGroupId, mergePayload.SourceOrbitalGroupId);
            Assert.Equal(3, mergePayload.TargetQuantity);
        }

        var stationedScouts = await dbContext.Set<OrbitalGroup>()
            .AsNoTracking()
            .Where(x =>
                x.CivilizationId == SeededCivilizationId &&
                x.CurrentPlanetId == SeededOwnedPlanetId &&
                x.AssetType == SpaceAssetType.ScoutCraft &&
                x.Status == OrbitalGroupStatus.Stationed)
            .OrderByDescending(x => x.Quantity)
            .ToListAsync();

        Assert.Equal(2, stationedScouts.Count);
        Assert.Equal(scoutQuantityBefore, stationedScouts.Sum(x => x.Quantity));
        Assert.Equal([3, 2], stationedScouts.Select(x => x.Quantity).ToArray());
    }

    [Fact]
    public async Task SeededSplitAndMergeRejectionsDoNotMutateGroups()
    {
        const string databaseName = "dev-orbital-group-split-merge-seeded-rejections";
        using var configuredFactory = factory.WithInMemoryPersistence(databaseName: databaseName);
        using var client = configuredFactory.CreateClient();

        await ApplyMinimalValidationSeedAsync(client);

        using var scope = configuredFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VoidEmpiresDbContext>();
        var activeTransferGroup = await dbContext.Set<OrbitalGroup>()
            .AsNoTracking()
            .SingleAsync(x =>
                x.CivilizationId == SeededCivilizationId &&
                x.CurrentPlanetId == SeededOwnedPlanetId &&
                x.AssetType == SpaceAssetType.CargoCraft &&
                x.Status == OrbitalGroupStatus.Reserved);
        var stationedScout = await dbContext.Set<OrbitalGroup>()
            .AsNoTracking()
            .Where(x =>
                x.CivilizationId == SeededCivilizationId &&
                x.CurrentPlanetId == SeededOwnedPlanetId &&
                x.AssetType == SpaceAssetType.ScoutCraft &&
                x.Status == OrbitalGroupStatus.Stationed)
            .OrderByDescending(x => x.Quantity)
            .FirstAsync();
        var groupsBefore = await ReadGroupSnapshotAsync(dbContext);

        using (var splitResponse = await client.PostAsJsonAsync("/api/dev/fleets/orbital-groups/split", new
        {
            civilizationId = SeededCivilizationId,
            sourceOrbitalGroupId = activeTransferGroup.Id,
            quantity = 1
        }))
        {
            var splitPayload = await splitResponse.Content.ReadFromJsonAsync<SplitOrbitalGroupResponse>();
            Assert.Equal(HttpStatusCode.Conflict, splitResponse.StatusCode);
            Assert.NotNull(splitPayload);
            Assert.Equal(3, splitPayload.Status);
            Assert.False(splitPayload.Succeeded);
            Assert.Contains("Source orbital group already has an active transfer.", splitPayload.Errors);
        }

        using (var mergeResponse = await client.PostAsJsonAsync("/api/dev/fleets/orbital-groups/merge", new
        {
            civilizationId = SeededCivilizationId,
            targetOrbitalGroupId = stationedScout.Id,
            sourceOrbitalGroupId = activeTransferGroup.Id
        }))
        {
            var mergePayload = await mergeResponse.Content.ReadFromJsonAsync<MergeOrbitalGroupsResponse>();
            Assert.Equal(HttpStatusCode.Conflict, mergeResponse.StatusCode);
            Assert.NotNull(mergePayload);
            Assert.Equal(3, mergePayload.Status);
            Assert.False(mergePayload.Succeeded);
            Assert.Contains("Source orbital group already has an active transfer.", mergePayload.Errors);
        }

        var groupsAfter = await ReadGroupSnapshotAsync(dbContext);
        Assert.Equal(groupsBefore, groupsAfter);
    }

    private static async Task ApplyMinimalValidationSeedAsync(HttpClient client)
    {
        using var seedResponse = await client.PostAsJsonAsync("/api/dev/seeds/apply", new { profile = "minimal-validation" });
        Assert.Equal(HttpStatusCode.OK, seedResponse.StatusCode);
    }

    private static Task<List<GroupSnapshot>> ReadGroupSnapshotAsync(VoidEmpiresDbContext dbContext) =>
        dbContext.Set<OrbitalGroup>()
            .AsNoTracking()
            .Where(x => x.CivilizationId == SeededCivilizationId)
            .OrderBy(x => x.Id)
            .Select(x => new GroupSnapshot(x.Id, x.CurrentPlanetId, x.AssetType, x.Quantity, x.Status))
            .ToListAsync();

    private sealed record GroupSnapshot(
        Guid Id,
        Guid CurrentPlanetId,
        SpaceAssetType AssetType,
        int Quantity,
        OrbitalGroupStatus Status);

    private sealed record SplitOrbitalGroupResponse(
        int Status,
        bool Succeeded,
        Guid? SourceOrbitalGroupId,
        Guid? NewOrbitalGroupId,
        int SourceQuantity,
        int NewQuantity,
        string[] Errors);

    private sealed record MergeOrbitalGroupsResponse(
        int Status,
        bool Succeeded,
        Guid? TargetOrbitalGroupId,
        Guid? SourceOrbitalGroupId,
        int TargetQuantity,
        string[] Errors);
}
