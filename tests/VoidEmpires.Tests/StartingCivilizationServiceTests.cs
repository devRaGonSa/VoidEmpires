using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Players;
using VoidEmpires.Domain.Players;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.Players;

namespace VoidEmpires.Tests;

public class StartingCivilizationServiceTests
{
    [Fact]
    public async Task CreateAsyncCreatesPlayerProfileAndCivilization()
    {
        await using var dbContext = CreateDbContext();
        var service = new StartingCivilizationService(dbContext);

        var result = await service.CreateAsync(new CreateStartingCivilizationRequest(
            "user-1",
            "Player One",
            "Solar Dominion",
            CivilizationArchetype.Industrial));

        Assert.True(result.Succeeded);
        Assert.NotNull(result.PlayerProfileId);
        Assert.NotNull(result.CivilizationId);
        Assert.Empty(result.Errors);
        Assert.Equal(1, await dbContext.PlayerProfiles.CountAsync());
        Assert.Equal(1, await dbContext.Civilizations.CountAsync());
    }

    [Fact]
    public async Task CreateAsyncPersistsHomePlanetId()
    {
        await using var dbContext = CreateDbContext();
        var service = new StartingCivilizationService(dbContext);
        var planetId = Guid.NewGuid();

        var result = await service.CreateAsync(new CreateStartingCivilizationRequest(
            "user-1",
            "Player One",
            "Solar Dominion",
            CivilizationArchetype.Exploratory,
            planetId));

        var civilization = await dbContext.Civilizations.SingleAsync();

        Assert.True(result.Succeeded);
        Assert.Equal(planetId, result.HomePlanetId);
        Assert.Equal(planetId, civilization.HomePlanetId);
    }

    [Fact]
    public async Task CreateAsyncRejectsDuplicateUserId()
    {
        await using var dbContext = CreateDbContext();
        var service = new StartingCivilizationService(dbContext);
        var request = new CreateStartingCivilizationRequest(
            "user-1",
            "Player One",
            "Solar Dominion",
            CivilizationArchetype.Balanced);

        var first = await service.CreateAsync(request);
        var duplicate = await service.CreateAsync(request with { CivilizationName = "Second Dominion" });

        Assert.True(first.Succeeded);
        Assert.False(duplicate.Succeeded);
        Assert.Equal(["Player profile already exists for this user."], duplicate.Errors);
        Assert.Equal(1, await dbContext.PlayerProfiles.CountAsync());
    }

    [Fact]
    public async Task CreateAsyncRejectsInvalidRequest()
    {
        await using var dbContext = CreateDbContext();
        var service = new StartingCivilizationService(dbContext);

        var result = await service.CreateAsync(new CreateStartingCivilizationRequest(
            " ",
            " ",
            " ",
            CivilizationArchetype.Balanced));

        Assert.False(result.Succeeded);
        Assert.Contains("User id is required.", result.Errors);
        Assert.Contains("Display name is required.", result.Errors);
        Assert.Contains("Civilization name is required.", result.Errors);
        Assert.Equal(0, await dbContext.PlayerProfiles.CountAsync());
    }

    private static VoidEmpiresDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new VoidEmpiresDbContext(options);
    }
}
