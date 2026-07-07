using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Development;
using VoidEmpires.Application.Players;
using VoidEmpires.Infrastructure.Development;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.Players;

namespace VoidEmpires.Tests;

public class InitialPlayerWorldBootstrapServiceTests
{
    private static readonly Guid SeedOwnedPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");

    [Fact]
    public async Task CreateAsyncPersistsInitialWorldForUser()
    {
        await using var dbContext = CreateDbContext();
        var service = new InitialPlayerWorldBootstrapService(dbContext);

        var result = await service.CreateAsync(ValidRequest());

        Assert.True(result.Succeeded);
        Assert.NotNull(result.PlayerProfileId);
        Assert.NotNull(result.CivilizationId);
        Assert.NotNull(result.HomePlanetId);
        Assert.Equal("Nova Prime", result.HomePlanetName);
        Assert.Equal(220, result.StartingResources?.Credits);
        Assert.True(await dbContext.PlayerProfiles.AnyAsync(x => x.UserId == "identity-user-1"));
        Assert.True(await dbContext.PlanetOwnerships.AnyAsync(x => x.PlanetId == result.HomePlanetId && x.CivilizationId == result.CivilizationId));
        Assert.True(await dbContext.PlanetResourceStockpiles.AnyAsync(x => x.PlanetId == result.HomePlanetId && x.Metal == 320));
        Assert.True(await dbContext.PlanetProductionProfiles.AnyAsync(x => x.PlanetId == result.HomePlanetId && x.CreditsPerHour == 18));
    }

    [Fact]
    public async Task CreateAsyncRejectsDuplicateUserBootstrap()
    {
        await using var dbContext = CreateDbContext();
        var service = new InitialPlayerWorldBootstrapService(dbContext);

        var first = await service.CreateAsync(ValidRequest());
        var duplicate = await service.CreateAsync(ValidRequest() with { DisplayName = "Other Commander", CivilizationName = "Other Dominion" });

        Assert.True(first.Succeeded);
        Assert.False(duplicate.Succeeded);
        Assert.Equal(["Player profile already exists for this user."], duplicate.Errors);
        Assert.Equal(1, await dbContext.PlayerProfiles.CountAsync(x => x.UserId == "identity-user-1"));
    }

    [Fact]
    public async Task CreateAsyncDoesNotReuseSeedValidationHomePlanet()
    {
        await using var dbContext = CreateDbContext();
        await new DevelopmentSeedService(dbContext).ApplyAsync(new ApplyDevelopmentSeedRequest("minimal-validation"));
        var service = new InitialPlayerWorldBootstrapService(dbContext);

        var result = await service.CreateAsync(ValidRequest());

        Assert.True(result.Succeeded);
        Assert.NotEqual(SeedOwnedPlanetId, result.HomePlanetId);
        Assert.True(await dbContext.PlanetOwnerships.AnyAsync(x => x.PlanetId == SeedOwnedPlanetId));
    }

    private static InitialPlayerWorldBootstrapRequest ValidRequest() => new(
        "identity-user-1",
        "Commander Vega",
        "Solar Dominion",
        "Nova Prime");

    private static VoidEmpiresDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}
