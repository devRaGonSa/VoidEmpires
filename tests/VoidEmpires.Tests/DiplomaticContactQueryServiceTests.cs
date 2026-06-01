using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Diplomacy;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.StrategicMap;

namespace VoidEmpires.Tests;

public class DiplomaticContactQueryServiceTests
{
    [Fact]
    public async Task GetAsyncReturnsInvalidForEmptyCivilizationId()
    {
        await using var dbContext = CreateDbContext();
        var result = await CreateService(dbContext).GetAsync(new GetDiplomaticContactsRequest(Guid.Empty));

        Assert.False(result.Succeeded);
        Assert.Contains("Civilization id is required.", result.Errors);
    }

    [Fact]
    public async Task GetAsyncReturnsOnlyRequestingCivilizationContactsInDeterministicOrder()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var firstContactId = Guid.NewGuid();
        var secondContactId = Guid.NewGuid();
        var otherCivilizationId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        dbContext.DiplomaticContacts.AddRange(
            DiplomaticContact.Create(civilizationId, secondContactId, DiplomaticContactStatus.Neutral, now.AddMinutes(2), "manual-dev"),
            DiplomaticContact.Create(civilizationId, firstContactId, DiplomaticContactStatus.Contacted, now.AddMinutes(1), "exploration"),
            DiplomaticContact.Create(otherCivilizationId, civilizationId, DiplomaticContactStatus.Hostile, now, "foreign"));
        await dbContext.SaveChangesAsync();

        var result = await CreateService(dbContext).GetAsync(new GetDiplomaticContactsRequest(civilizationId));

        Assert.True(result.Succeeded);
        Assert.Equal(civilizationId, result.CivilizationId);
        Assert.Equal([firstContactId, secondContactId], result.Contacts.Select(x => x.ContactedCivilizationId));
        Assert.All(result.Contacts, x => Assert.Equal(civilizationId, x.CivilizationId));
    }

    [Fact]
    public async Task GetAsyncRemainsReadOnly()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        dbContext.DiplomaticContacts.Add(DiplomaticContact.Create(
            civilizationId,
            Guid.NewGuid(),
            DiplomaticContactStatus.Contacted,
            DateTime.UtcNow,
            "manual-dev"));
        await dbContext.SaveChangesAsync();
        var count = await dbContext.DiplomaticContacts.CountAsync();

        _ = await CreateService(dbContext).GetAsync(new GetDiplomaticContactsRequest(civilizationId));

        Assert.Equal(0, dbContext.ChangeTracker.Entries().Count(x => x.State != EntityState.Unchanged));
        Assert.Equal(count, await dbContext.DiplomaticContacts.CountAsync());
    }

    private static DiplomaticContactQueryService CreateService(VoidEmpiresDbContext dbContext) => new(dbContext);

    private static VoidEmpiresDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<VoidEmpiresDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
}
