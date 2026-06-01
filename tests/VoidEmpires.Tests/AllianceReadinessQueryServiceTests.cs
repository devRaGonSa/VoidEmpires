using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Diplomacy;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.StrategicMap;

namespace VoidEmpires.Tests;

public class AllianceReadinessQueryServiceTests
{
    [Fact]
    public async Task GetAsyncScopesByCivilizationAndOrdersDeterministically()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var otherCivilizationId = Guid.NewGuid();
        var createdAtUtc = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);
        var alphaAlliance = Alliance.Create("Alpha Pact", "ALPHA", AllianceStatus.Active, createdAtUtc);
        var betaAlliance = Alliance.Create("Beta Accord", "BETA", AllianceStatus.Archived, createdAtUtc.AddMinutes(1));
        dbContext.Alliances.AddRange(alphaAlliance, betaAlliance);
        dbContext.AllianceMemberships.AddRange(
            AllianceMembership.Create(betaAlliance.Id, civilizationId, AllianceMembershipStatus.Departed, AllianceMembershipRole.Member, createdAtUtc.AddMinutes(20)),
            AllianceMembership.Create(alphaAlliance.Id, civilizationId, AllianceMembershipStatus.Active, AllianceMembershipRole.Officer, createdAtUtc.AddMinutes(10)),
            AllianceMembership.Create(alphaAlliance.Id, otherCivilizationId, AllianceMembershipStatus.Active, AllianceMembershipRole.Member, createdAtUtc.AddMinutes(5)));
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var result = await CreateService(dbContext).GetAsync(new GetAllianceReadinessRequest(civilizationId));

        Assert.True(result.Succeeded);
        Assert.Equal(civilizationId, result.CivilizationId);
        Assert.Equal([alphaAlliance.Id, betaAlliance.Id], result.Alliances.Select(x => x.AllianceId).ToArray());
        Assert.All(result.Alliances, x => Assert.Equal(civilizationId, x.Membership.CivilizationId));
        Assert.Equal(["ALPHA", "BETA"], result.Alliances.Select(x => x.Tag).ToArray());
    }

    [Fact]
    public async Task GetAsyncRejectsEmptyCivilizationIdAndRemainsReadOnly()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var alliance = Alliance.Create(
            "Void Council",
            "VC",
            AllianceStatus.Active,
            new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc));
        dbContext.Alliances.Add(alliance);
        dbContext.AllianceMemberships.Add(AllianceMembership.Create(
            alliance.Id,
            civilizationId,
            AllianceMembershipStatus.Active,
            AllianceMembershipRole.Leader,
            new DateTime(2026, 6, 1, 12, 5, 0, DateTimeKind.Utc)));
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
        var countBefore = await dbContext.AllianceMemberships.CountAsync();

        var invalidResult = await CreateService(dbContext).GetAsync(new GetAllianceReadinessRequest(Guid.Empty));
        var validResult = await CreateService(dbContext).GetAsync(new GetAllianceReadinessRequest(civilizationId));

        Assert.False(invalidResult.Succeeded);
        Assert.Contains("Civilization id is required.", invalidResult.Errors);
        Assert.True(validResult.Succeeded);
        Assert.Equal(countBefore, await dbContext.AllianceMemberships.CountAsync());
        Assert.Empty(dbContext.ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged));
    }

    private static AllianceReadinessQueryService CreateService(VoidEmpiresDbContext dbContext) => new(dbContext);

    private static VoidEmpiresDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}
