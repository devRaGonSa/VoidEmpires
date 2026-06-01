using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Diplomacy;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.StrategicMap;

namespace VoidEmpires.Tests;

public class AlliancePactReadinessQueryServiceTests
{
    [Fact]
    public async Task GetAsyncScopesByActiveAllianceMembershipsAndOrdersDeterministically()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var otherCivilizationId = Guid.NewGuid();
        var createdAtUtc = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);
        var alphaAlliance = Alliance.Create("Alpha Pact", "ALPHA", AllianceStatus.Active, createdAtUtc);
        var betaAlliance = Alliance.Create("Beta Accord", "BETA", AllianceStatus.Active, createdAtUtc.AddMinutes(1));
        var gammaAlliance = Alliance.Create("Gamma League", "GAMMA", AllianceStatus.Archived, createdAtUtc.AddMinutes(2));
        var deltaAlliance = Alliance.Create("Delta Ring", "DELTA", AllianceStatus.Active, createdAtUtc.AddMinutes(3));
        dbContext.Alliances.AddRange(alphaAlliance, betaAlliance, gammaAlliance, deltaAlliance);
        dbContext.AllianceMemberships.AddRange(
            AllianceMembership.Create(alphaAlliance.Id, civilizationId, AllianceMembershipStatus.Active, AllianceMembershipRole.Leader, createdAtUtc.AddMinutes(5)),
            AllianceMembership.Create(deltaAlliance.Id, civilizationId, AllianceMembershipStatus.Departed, AllianceMembershipRole.Member, createdAtUtc.AddMinutes(6)),
            AllianceMembership.Create(gammaAlliance.Id, otherCivilizationId, AllianceMembershipStatus.Active, AllianceMembershipRole.Member, createdAtUtc.AddMinutes(7)));

        var firstPact = AlliancePact.Create(
            gammaAlliance.Id,
            alphaAlliance.Id,
            AlliancePactType.NonAggression,
            AlliancePactStatus.Active,
            createdAtUtc.AddMinutes(20));
        var secondPact = AlliancePact.Create(
            alphaAlliance.Id,
            betaAlliance.Id,
            AlliancePactType.MutualDefenseIntent,
            AlliancePactStatus.Proposed,
            createdAtUtc.AddMinutes(30));
        var excludedPact = AlliancePact.Create(
            betaAlliance.Id,
            gammaAlliance.Id,
            AlliancePactType.Cooperation,
            AlliancePactStatus.Active,
            createdAtUtc.AddMinutes(40));
        var departedMembershipPact = AlliancePact.Create(
            deltaAlliance.Id,
            betaAlliance.Id,
            AlliancePactType.TradeIntent,
            AlliancePactStatus.Archived,
            createdAtUtc.AddMinutes(50));

        dbContext.AlliancePacts.AddRange(secondPact, excludedPact, departedMembershipPact, firstPact);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var result = await CreateService(dbContext).GetAsync(new GetAlliancePactReadinessRequest(civilizationId));

        Assert.True(result.Succeeded);
        Assert.Equal(civilizationId, result.CivilizationId);
        Assert.Equal([firstPact.Id, secondPact.Id], result.Pacts.Select(x => x.AlliancePactId).ToArray());
        Assert.Equal(
            [AlliancePactType.NonAggression, AlliancePactType.MutualDefenseIntent],
            result.Pacts.Select(x => x.PactType).ToArray());
        Assert.Equal(["GAMMA", "ALPHA"], result.Pacts.Select(x => x.SourceAlliance.Tag).ToArray());
        Assert.Contains(result.Pacts, x => x.TargetAlliance.AllianceId == alphaAlliance.Id);
        Assert.DoesNotContain(result.Pacts, x => x.AlliancePactId == excludedPact.Id || x.AlliancePactId == departedMembershipPact.Id);
    }

    [Fact]
    public async Task GetAsyncRejectsEmptyCivilizationIdAndRemainsReadOnly()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var sourceAlliance = Alliance.Create(
            "Void Council",
            "VC",
            AllianceStatus.Active,
            new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc));
        var targetAlliance = Alliance.Create(
            "Star Accord",
            "STAR",
            AllianceStatus.Active,
            new DateTime(2026, 6, 1, 12, 1, 0, DateTimeKind.Utc));
        dbContext.Alliances.AddRange(sourceAlliance, targetAlliance);
        dbContext.AllianceMemberships.Add(AllianceMembership.Create(
            sourceAlliance.Id,
            civilizationId,
            AllianceMembershipStatus.Active,
            AllianceMembershipRole.Member,
            new DateTime(2026, 6, 1, 12, 5, 0, DateTimeKind.Utc)));
        dbContext.AlliancePacts.Add(AlliancePact.Create(
            sourceAlliance.Id,
            targetAlliance.Id,
            AlliancePactType.Cooperation,
            AlliancePactStatus.Active,
            new DateTime(2026, 6, 1, 12, 10, 0, DateTimeKind.Utc)));
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
        var pactCountBefore = await dbContext.AlliancePacts.CountAsync();

        var invalidResult = await CreateService(dbContext).GetAsync(new GetAlliancePactReadinessRequest(Guid.Empty));
        var validResult = await CreateService(dbContext).GetAsync(new GetAlliancePactReadinessRequest(civilizationId));

        Assert.False(invalidResult.Succeeded);
        Assert.Contains("Civilization id is required.", invalidResult.Errors);
        Assert.True(validResult.Succeeded);
        Assert.Equal(pactCountBefore, await dbContext.AlliancePacts.CountAsync());
        Assert.Empty(dbContext.ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged));
    }

    private static AlliancePactReadinessQueryService CreateService(VoidEmpiresDbContext dbContext) => new(dbContext);

    private static VoidEmpiresDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}
