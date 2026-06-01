using Microsoft.EntityFrameworkCore;
using VoidEmpires.Domain.Diplomacy;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class AlliancePersistenceTests
{
    [Fact]
    public async Task DbContextPersistsAndLoadsAlliancesAndMemberships()
    {
        await using var dbContext = CreateDbContext();
        var alliance = Alliance.Create(
            "Void Council",
            "VC",
            AllianceStatus.Active,
            new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc));
        var civilizationId = Guid.NewGuid();
        var membership = AllianceMembership.Create(
            alliance.Id,
            civilizationId,
            AllianceMembershipStatus.Active,
            AllianceMembershipRole.Member,
            new DateTime(2026, 6, 1, 12, 10, 0, DateTimeKind.Utc));

        dbContext.Alliances.Add(alliance);
        dbContext.AllianceMemberships.Add(membership);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var storedAlliance = await dbContext.Alliances.SingleAsync();
        var storedMembership = await dbContext.AllianceMemberships.SingleAsync();

        Assert.Equal(alliance.Id, storedAlliance.Id);
        Assert.Equal("VC", storedAlliance.Tag);
        Assert.Equal(membership.Id, storedMembership.Id);
        Assert.Equal(alliance.Id, storedMembership.AllianceId);
        Assert.Equal(civilizationId, storedMembership.CivilizationId);
    }

    private static VoidEmpiresDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}
