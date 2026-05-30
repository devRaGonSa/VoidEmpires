using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Visuals;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.Visuals;

namespace VoidEmpires.Tests;

public class SystemVisualStateServiceTests
{
    private static readonly Guid SystemId = Guid.Parse("4cfb3562-0e91-4cf0-98bd-44309f2ff98e");
    private static readonly Guid GalaxyId = Guid.Parse("ac6f0941-bbe4-4778-86fa-03eba119a5c0");
    private static readonly Guid StarId = Guid.Parse("cc31bf95-ee0c-49bd-a7f4-d3cc26f53455");
    private static readonly Guid PlanetId = Guid.Parse("aa6c3794-2fa5-4567-85a8-e71690657f98");

    [Fact]
    public async Task GetAsyncReturnsFailureForEmptySystemId()
    {
        await using var dbContext = CreateDbContext();
        var service = new SystemVisualStateService(dbContext, new FakePlanetVisualStateService([]));

        var result = await service.GetAsync(new(Guid.Empty));

        Assert.False(result.Succeeded);
        Assert.Null(result.VisualState);
        Assert.Contains("System id is required.", result.Errors);
    }

    [Fact]
    public async Task GetAsyncReturnsFailureForUnknownSystem()
    {
        await using var dbContext = CreateDbContext();
        var service = new SystemVisualStateService(dbContext, new FakePlanetVisualStateService([]));

        var result = await service.GetAsync(new(SystemId));

        Assert.False(result.Succeeded);
        Assert.Null(result.VisualState);
        Assert.Contains("System was not found.", result.Errors);
    }

    [Fact]
    public async Task GetAsyncReturnsSystemMetadataStarAndPlanetVisualStatesOrderedByOrbitalSlot()
    {
        await using var dbContext = CreateDbContext();
        var firstPlanetId = Guid.Parse("1fb55841-4efe-4683-8e6a-5f7b653baab1");
        var secondPlanetId = Guid.Parse("67da4061-dc4f-4290-a376-a50830a8409e");
        dbContext.Set<SolarSystem>().Add(CreateSystem());
        dbContext.Set<Planet>().Add(new Planet(secondPlanetId, SystemId, "Second", 2, PlanetType.Ice, 90));
        dbContext.Set<Planet>().Add(new Planet(firstPlanetId, SystemId, "First", 1, PlanetType.Terran, 100));
        await dbContext.SaveChangesAsync();
        var service = new SystemVisualStateService(dbContext, new FakePlanetVisualStateService([
            CreatePlanetState(firstPlanetId, "First", PlanetType.Terran),
            CreatePlanetState(secondPlanetId, "Second", PlanetType.Ice)
        ]));

        var result = await service.GetAsync(new(SystemId));

        Assert.True(result.Succeeded);
        Assert.NotNull(result.VisualState);
        Assert.Equal(SystemId, result.VisualState.SystemId);
        Assert.Equal(GalaxyId, result.VisualState.GalaxyId);
        Assert.Equal("Helios Prime", result.VisualState.SystemName);
        Assert.Equal(1, result.VisualState.CoordinateX);
        Assert.Equal(2, result.VisualState.CoordinateY);
        Assert.Equal(3, result.VisualState.CoordinateZ);
        Assert.Equal(StarId, result.VisualState.Star.StarId);
        Assert.Equal("Helios", result.VisualState.Star.StarName);
        Assert.Equal(StarType.YellowDwarf, result.VisualState.Star.StarType);
        Assert.Equal("yellow_dwarf", result.VisualState.Star.VisualClass);
        Assert.Equal(0.75f, result.VisualState.Star.LightIntensity);
        Assert.Empty(result.Errors);
        Assert.Collection(
            result.VisualState.Planets,
            planet => Assert.Equal(firstPlanetId, planet.PlanetId),
            planet => Assert.Equal(secondPlanetId, planet.PlanetId));
    }

    [Fact]
    public async Task GetAsyncReturnsLayoutHintsOrderedByOrbitalSlot()
    {
        await using var dbContext = CreateDbContext();
        var firstPlanetId = Guid.Parse("1fb55841-4efe-4683-8e6a-5f7b653baab1");
        var secondPlanetId = Guid.Parse("67da4061-dc4f-4290-a376-a50830a8409e");
        dbContext.Set<SolarSystem>().Add(CreateSystem());
        dbContext.Set<Planet>().Add(new Planet(secondPlanetId, SystemId, "Second", 2, PlanetType.Ice, 175));
        dbContext.Set<Planet>().Add(new Planet(firstPlanetId, SystemId, "First", 1, PlanetType.Terran, 45));
        await dbContext.SaveChangesAsync();
        var service = new SystemVisualStateService(dbContext, new FakePlanetVisualStateService([
            CreatePlanetState(firstPlanetId, "First", PlanetType.Terran),
            CreatePlanetState(secondPlanetId, "Second", PlanetType.Ice)
        ]));

        var result = await service.GetAsync(new(SystemId));

        Assert.True(result.Succeeded);
        Assert.NotNull(result.VisualState);
        Assert.Collection(
            result.VisualState.LayoutHints,
            hint =>
            {
                Assert.Equal(firstPlanetId, hint.PlanetId);
                Assert.Equal(1, hint.OrbitalSlot);
                Assert.Equal(5.75f, hint.OrbitRadius);
                Assert.Equal(47f, hint.OrbitAngleDegrees);
                Assert.Equal(0.45f, hint.VisualScale);
            },
            hint =>
            {
                Assert.Equal(secondPlanetId, hint.PlanetId);
                Assert.Equal(2, hint.OrbitalSlot);
                Assert.Equal(7.5f, hint.OrbitRadius);
                Assert.Equal(94f, hint.OrbitAngleDegrees);
                Assert.Equal(1.75f, hint.VisualScale);
            });
    }

    [Theory]
    [InlineData(StarType.RedDwarf, "red_dwarf", 0.55f)]
    [InlineData(StarType.YellowDwarf, "yellow_dwarf", 0.75f)]
    [InlineData(StarType.BlueGiant, "blue_giant", 1.00f)]
    [InlineData(StarType.WhiteDwarf, "white_dwarf", 0.85f)]
    [InlineData(StarType.NeutronStar, "neutron_star", 0.95f)]
    public async Task GetAsyncMapsStarVisualMetadata(StarType starType, string expectedVisualClass, float expectedLightIntensity)
    {
        await using var dbContext = CreateDbContext();
        var star = new Star(StarId, SystemId, "Variable", starType);
        dbContext.Set<SolarSystem>().Add(new SolarSystem(SystemId, GalaxyId, "Variable Prime", new GalaxyCoordinates(1, 2, 3), star));
        await dbContext.SaveChangesAsync();
        var service = new SystemVisualStateService(dbContext, new FakePlanetVisualStateService([]));

        var result = await service.GetAsync(new(SystemId));

        Assert.True(result.Succeeded);
        Assert.NotNull(result.VisualState);
        Assert.Equal(expectedVisualClass, result.VisualState.Star.VisualClass);
        Assert.Equal(expectedLightIntensity, result.VisualState.Star.LightIntensity);
    }

    [Fact]
    public async Task GetAsyncReturnsFailureWhenPlanetVisualStateCannotBeBuilt()
    {
        await using var dbContext = CreateDbContext();
        dbContext.Set<SolarSystem>().Add(CreateSystem());
        dbContext.Set<Planet>().Add(new Planet(PlanetId, SystemId, "Broken", 1, PlanetType.Barren, 80));
        await dbContext.SaveChangesAsync();
        var service = new SystemVisualStateService(dbContext, new FakePlanetVisualStateService([]));

        var result = await service.GetAsync(new(SystemId));

        Assert.False(result.Succeeded);
        Assert.Null(result.VisualState);
        Assert.Contains("Planet visual state was not found.", result.Errors);
    }

    private static SolarSystem CreateSystem()
    {
        var star = new Star(StarId, SystemId, "Helios", StarType.YellowDwarf);
        return new SolarSystem(SystemId, GalaxyId, "Helios Prime", new GalaxyCoordinates(1, 2, 3), star);
    }

    private static PlanetVisualStateDto CreatePlanetState(Guid planetId, string name, PlanetType planetType) =>
        new(
            planetId,
            name,
            planetType,
            100,
            PlanetColonizationStatus.Uncolonized,
            IsOwned: false,
            CivilizationId: null,
            CivilizationColor: null,
            VisualSeed: 123,
            ColonizationIntensity: 0f,
            UrbanIntensity: 0f,
            IndustrialIntensity: 0f,
            TerraformingIntensity: 0f,
            MilitaryIntensity: 0f,
            OrbitalPresenceIntensity: 0f,
            PlanetVisualProfileCatalog.GetProfile(planetType));

    private static VoidEmpiresDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new VoidEmpiresDbContext(options);
    }

    private sealed class FakePlanetVisualStateService(IReadOnlyList<PlanetVisualStateDto> states) : IPlanetVisualStateService
    {
        public Task<GetPlanetVisualStateResult> GetAsync(
            GetPlanetVisualStateRequest request,
            CancellationToken cancellationToken = default)
        {
            var state = states.SingleOrDefault(x => x.PlanetId == request.PlanetId);
            return state is null
                ? Task.FromResult(GetPlanetVisualStateResult.Failure("Planet visual state was not found."))
                : Task.FromResult(GetPlanetVisualStateResult.Success(state));
        }
    }
}
