using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Development;
using VoidEmpires.Application.Players;
using VoidEmpires.Domain.Buildings;
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
        Assert.True(await dbContext.PlanetResourceStockpiles.AnyAsync(x => x.PlanetId == result.HomePlanetId && x.Metal == StartingHomeWorldBaseline.StartingMetal));
        Assert.True(await dbContext.PlanetProductionProfiles.AnyAsync(x => x.PlanetId == result.HomePlanetId && x.CreditsPerHour == StartingHomeWorldBaseline.BaseCreditsPerHour));
        Assert.True(await dbContext.PlanetBuildingCapacities.AnyAsync(x => x.PlanetId == result.HomePlanetId && x.BaseCapacity == 120));
    }

    [Fact]
    public async Task CreateAsyncStartsWithBasicConstructionOptionsAffordableAndCapacityBacked()
    {
        await using var dbContext = CreateDbContext();
        var service = new InitialPlayerWorldBootstrapService(dbContext);

        var result = await service.CreateAsync(ValidRequest());

        Assert.True(result.Succeeded);
        var stockpile = await dbContext.PlanetResourceStockpiles.SingleAsync(x => x.PlanetId == result.HomePlanetId);
        var capacity = await dbContext.PlanetBuildingCapacities.SingleAsync(x => x.PlanetId == result.HomePlanetId);
        Assert.True(capacity.TotalCapacity >= 20);
        foreach (var buildingType in new[]
        {
            BuildingType.MetalMine,
            BuildingType.CrystalMine,
            BuildingType.GasExtractor,
            BuildingType.SolarPlant
        })
        {
            var definition = BuildingCatalog.Get(buildingType);
            Assert.True(stockpile.CanSpend(
                definition.Cost.Credits,
                definition.Cost.Metal,
                definition.Cost.Crystal,
                definition.Cost.Gas));
        }
    }

    [Fact]
    public async Task CreateAsyncUsesEqualBaselineForIndependentUsers()
    {
        await using var dbContext = CreateDbContext();
        var service = new InitialPlayerWorldBootstrapService(dbContext);

        var first = await service.CreateAsync(ValidRequest());
        var second = await service.CreateAsync(ValidRequest(
            "identity-user-2",
            "Commander Orion",
            "Azure League",
            "Azure Prime"));

        Assert.True(first.Succeeded);
        Assert.True(second.Succeeded);
        Assert.Equal(first.StartingResources, second.StartingResources);

        var firstProduction = await dbContext.PlanetProductionProfiles.SingleAsync(x => x.PlanetId == first.HomePlanetId!.Value);
        var secondProduction = await dbContext.PlanetProductionProfiles.SingleAsync(x => x.PlanetId == second.HomePlanetId!.Value);
        Assert.Equal(
            (StartingHomeWorldBaseline.BaseCreditsPerHour, StartingHomeWorldBaseline.BaseMetalPerHour, StartingHomeWorldBaseline.BaseCrystalPerHour, StartingHomeWorldBaseline.BaseGasPerHour),
            (firstProduction.CreditsPerHour, firstProduction.MetalPerHour, firstProduction.CrystalPerHour, firstProduction.GasPerHour));
        Assert.Equal(
            (firstProduction.CreditsPerHour, firstProduction.MetalPerHour, firstProduction.CrystalPerHour, firstProduction.GasPerHour),
            (secondProduction.CreditsPerHour, secondProduction.MetalPerHour, secondProduction.CrystalPerHour, secondProduction.GasPerHour));
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

    private static InitialPlayerWorldBootstrapRequest ValidRequest(
        string userId = "identity-user-1",
        string displayName = "Commander Vega",
        string civilizationName = "Solar Dominion",
        string homePlanetName = "Nova Prime") => new(
            userId,
            displayName,
            civilizationName,
            homePlanetName);

    private static VoidEmpiresDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}
