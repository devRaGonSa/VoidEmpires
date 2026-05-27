using VoidEmpires.Domain.Galaxy;

namespace VoidEmpires.Tests;

public class GalaxyDomainTests
{
    [Fact]
    public void CoordinatesCompareByValue()
    {
        var first = new GalaxyCoordinates(1, 2, 3);
        var second = new GalaxyCoordinates(1, 2, 3);
        var different = new GalaxyCoordinates(1, 2, 4);

        Assert.Equal(first, second);
        Assert.NotEqual(first, different);
        Assert.Equal("1:2:3", first.ToString());
    }

    [Fact]
    public void GalaxyCanContainMultipleSolarSystems()
    {
        var galaxy = Galaxy.Create("Milky Way");
        var firstSystem = CreateSolarSystem(galaxy.Id, "Sol", new GalaxyCoordinates(0, 0, 0));
        var secondSystem = CreateSolarSystem(galaxy.Id, "Alpha Centauri", new GalaxyCoordinates(1, 0, 0));

        galaxy.AddSolarSystem(firstSystem);
        galaxy.AddSolarSystem(secondSystem);

        Assert.Equal(2, galaxy.SolarSystems.Count);
    }

    [Fact]
    public void GalaxyRejectsEmptyName()
    {
        Assert.Throws<ArgumentException>(() => Galaxy.Create(" "));
    }

    [Fact]
    public void GalaxyRejectsDuplicatedSolarSystemCoordinates()
    {
        var galaxy = Galaxy.Create("Milky Way");
        var coordinates = new GalaxyCoordinates(0, 0, 0);
        galaxy.AddSolarSystem(CreateSolarSystem(galaxy.Id, "Sol", coordinates));

        Assert.Throws<InvalidOperationException>(() =>
            galaxy.AddSolarSystem(CreateSolarSystem(galaxy.Id, "Sol Two", coordinates)));
    }

    [Fact]
    public void SolarSystemRejectsMissingStar()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new SolarSystem(Guid.NewGuid(), Guid.NewGuid(), "Sol", new GalaxyCoordinates(0, 0, 0), null!));
    }

    [Fact]
    public void SolarSystemRejectsDuplicatedPlanetOrbitalSlot()
    {
        var solarSystem = CreateSolarSystem(Guid.NewGuid(), "Sol", new GalaxyCoordinates(0, 0, 0));
        solarSystem.AddPlanet(Planet.Create(solarSystem.Id, "Terra", 1, PlanetType.Terran, 12000));

        Assert.Throws<InvalidOperationException>(() =>
            solarSystem.AddPlanet(Planet.Create(solarSystem.Id, "Mars", 1, PlanetType.Desert, 7000)));
    }

    [Fact]
    public void PlanetCanExistAsUncolonized()
    {
        var solarSystemId = Guid.NewGuid();
        var planet = Planet.Create(solarSystemId, "Terra", 1, PlanetType.Terran, 12000);

        Assert.Equal(solarSystemId, planet.SolarSystemId);
        Assert.Equal("Terra", planet.Name);
        Assert.Equal(1, planet.OrbitalSlot);
        Assert.Equal(PlanetType.Terran, planet.PlanetType);
        Assert.Equal(12000, planet.Size);
        Assert.Equal(PlanetColonizationStatus.Uncolonized, planet.ColonizationStatus);
    }

    [Fact]
    public void PlanetRejectsInvalidSize()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Planet.Create(Guid.NewGuid(), "Small", 1, PlanetType.Barren, 0));
    }

    [Fact]
    public void PlanetRejectsInvalidOrbitalSlot()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Planet.Create(Guid.NewGuid(), "Slot", 0, PlanetType.Barren, 1000));
    }

    [Fact]
    public void PlanetRejectsEmptyName()
    {
        Assert.Throws<ArgumentException>(() =>
            Planet.Create(Guid.NewGuid(), " ", 1, PlanetType.Barren, 1000));
    }

    private static SolarSystem CreateSolarSystem(Guid galaxyId, string name, GalaxyCoordinates coordinates)
    {
        var solarSystemId = Guid.NewGuid();
        var star = Star.Create(solarSystemId, $"{name} Star", StarType.YellowDwarf);
        return new SolarSystem(solarSystemId, galaxyId, name, coordinates, star);
    }
}
