using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Development;
using VoidEmpires.Application.Visuals;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Infrastructure.Development;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.Visuals;

namespace VoidEmpires.Tests;

public class PlanetVisualStateServiceTests
{
    [Fact]
    public async Task GetAsyncReturnsFailureForEmptyPlanetId()
    {
        await using var dbContext = CreateDbContext();
        var service = new PlanetVisualStateService(dbContext);

        var result = await service.GetAsync(new(Guid.Empty));

        Assert.False(result.Succeeded);
        Assert.Null(result.VisualState);
        Assert.Contains("Planet id is required.", result.Errors);
    }

    [Fact]
    public async Task GetAsyncReturnsFailureForUnknownPlanet()
    {
        await using var dbContext = CreateDbContext();
        var service = new PlanetVisualStateService(dbContext);

        var result = await service.GetAsync(new(Guid.NewGuid()));

        Assert.False(result.Succeeded);
        Assert.Null(result.VisualState);
        Assert.Contains("Planet was not found.", result.Errors);
    }

    [Fact]
    public async Task GetAsyncReturnsUnownedPlanetVisualState()
    {
        await using var dbContext = CreateDbContext();
        var planet = Planet.Create(Guid.NewGuid(), "Nox", 1, PlanetType.Barren, 100);
        dbContext.Set<Planet>().Add(planet);
        await dbContext.SaveChangesAsync();
        var service = new PlanetVisualStateService(dbContext);

        var result = await service.GetAsync(new(planet.Id));

        Assert.True(result.Succeeded);
        Assert.NotNull(result.VisualState);
        Assert.Equal(planet.Id, result.VisualState.PlanetId);
        Assert.Equal("Nox", result.VisualState.PlanetName);
        Assert.Equal(PlanetType.Barren, result.VisualState.PlanetType);
        Assert.Equal(100, result.VisualState.Size);
        Assert.False(result.VisualState.IsOwned);
        Assert.Null(result.VisualState.CivilizationId);
        Assert.Null(result.VisualState.CivilizationColor);
        Assert.Equal(0f, result.VisualState.ColonizationIntensity);
        Assert.Equal("barren_cratered_regolith", result.VisualState.Profile.SurfaceProfile);
    }

    [Fact]
    public async Task GetAsyncAggregatesOwnershipAndBuildings()
    {
        await using var dbContext = CreateDbContext();
        var civilizationId = Guid.NewGuid();
        var planet = new Planet(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Asterion",
            2,
            PlanetType.Terran,
            100,
            PlanetColonizationStatus.Colonized);
        dbContext.Set<Planet>().Add(planet);
        dbContext.Set<PlanetOwnership>().Add(PlanetOwnership.Create(planet.Id, civilizationId));
        dbContext.Set<PlanetBuilding>().AddRange(
            PlanetBuilding.Create(planet.Id, BuildingType.CommandCenter, 5, 1),
            PlanetBuilding.Create(planet.Id, BuildingType.MetalMine, 25, 1),
            PlanetBuilding.Create(planet.Id, BuildingType.DefenseGrid, 10, 1));
        await dbContext.SaveChangesAsync();
        var service = new PlanetVisualStateService(dbContext);

        var result = await service.GetAsync(new(planet.Id));

        Assert.True(result.Succeeded);
        Assert.NotNull(result.VisualState);
        Assert.True(result.VisualState.IsOwned);
        Assert.Equal(civilizationId, result.VisualState.CivilizationId);
        Assert.StartsWith("hsl(", result.VisualState.CivilizationColor);
        Assert.Equal(0.295f, result.VisualState.ColonizationIntensity, precision: 3);
        Assert.Equal(0.05f, result.VisualState.UrbanIntensity, precision: 3);
        Assert.Equal(0.25f, result.VisualState.IndustrialIntensity, precision: 3);
        Assert.Equal(0f, result.VisualState.TerraformingIntensity);
        Assert.Equal(0.10f, result.VisualState.MilitaryIntensity, precision: 3);
        Assert.Equal(0f, result.VisualState.OrbitalPresenceIntensity);
        Assert.Equal("terran_continental", result.VisualState.Profile.SurfaceProfile);
    }

    [Fact]
    public async Task GetAsyncReturnsDistinctVisualStatesForMinimalValidationSeed()
    {
        await using var dbContext = CreateDbContext();
        await new DevelopmentSeedService(dbContext).ApplyAsync(new ApplyDevelopmentSeedRequest("minimal-validation"));
        var service = new PlanetVisualStateService(dbContext);

        var owned = AssertVisual(await service.GetAsync(new(Guid.Parse("40000000-0000-0000-0000-000000000001"))));
        var industrial = AssertVisual(await service.GetAsync(new(Guid.Parse("40000000-0000-0000-0000-000000000002"))));
        var orbital = AssertVisual(await service.GetAsync(new(Guid.Parse("40000000-0000-0000-0000-000000000003"))));
        var ownedRepeat = AssertVisual(await service.GetAsync(new(Guid.Parse("40000000-0000-0000-0000-000000000001"))));

        Assert.Equal(owned.VisualSeed, ownedRepeat.VisualSeed);
        Assert.True(owned.ColonizationIntensity > 0f);
        Assert.True(owned.UrbanIntensity > 0f);
        Assert.True(industrial.IndustrialIntensity > 0f);
        Assert.True(orbital.OrbitalPresenceIntensity > 0f);
        Assert.False(orbital.Profile.SupportsNightLights);
        Assert.NotEqual(owned.Profile.SurfaceProfile, industrial.Profile.SurfaceProfile);
        Assert.NotEqual(industrial.Profile.SurfaceProfile, orbital.Profile.SurfaceProfile);
        Assert.All(
            new[] { owned, industrial, orbital },
            visual => Assert.InRange(
                Math.Max(
                    Math.Max(visual.ColonizationIntensity, visual.UrbanIntensity),
                    Math.Max(
                        Math.Max(visual.IndustrialIntensity, visual.TerraformingIntensity),
                        Math.Max(visual.MilitaryIntensity, visual.OrbitalPresenceIntensity))),
                0f,
                1f));
    }

    private static PlanetVisualStateDto AssertVisual(GetPlanetVisualStateResult result)
    {
        Assert.True(result.Succeeded);
        return Assert.IsType<PlanetVisualStateDto>(result.VisualState);
    }

    private static VoidEmpiresDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new VoidEmpiresDbContext(options);
    }
}
