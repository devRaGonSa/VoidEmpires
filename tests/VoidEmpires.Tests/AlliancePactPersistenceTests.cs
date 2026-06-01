using Microsoft.EntityFrameworkCore;
using VoidEmpires.Domain.Diplomacy;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class AlliancePactPersistenceTests
{
    [Fact]
    public async Task DbContextPersistsAndLoadsAlliancePacts()
    {
        await using var dbContext = CreateDbContext();
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
        var pact = AlliancePact.Create(
            sourceAlliance.Id,
            targetAlliance.Id,
            AlliancePactType.TradeIntent,
            AlliancePactStatus.Proposed,
            new DateTime(2026, 6, 1, 12, 5, 0, DateTimeKind.Utc));

        dbContext.Alliances.AddRange(sourceAlliance, targetAlliance);
        dbContext.AlliancePacts.Add(pact);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var storedPact = await dbContext.AlliancePacts.SingleAsync();

        Assert.Equal(pact.Id, storedPact.Id);
        Assert.Equal(sourceAlliance.Id, storedPact.SourceAllianceId);
        Assert.Equal(targetAlliance.Id, storedPact.TargetAllianceId);
        Assert.Equal(AlliancePactType.TradeIntent, storedPact.PactType);
        Assert.Equal(AlliancePactStatus.Proposed, storedPact.Status);
    }

    private static VoidEmpiresDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}
