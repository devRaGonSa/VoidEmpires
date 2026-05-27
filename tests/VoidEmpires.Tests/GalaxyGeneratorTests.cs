using VoidEmpires.Application.Galaxy;
using VoidEmpires.Infrastructure.GalaxyGeneration;

namespace VoidEmpires.Tests;

public class GalaxyGeneratorTests
{
    [Fact]
    public void SameSeedGeneratesSameUniverseSignature()
    {
        var generator = new GalaxyGenerator();
        var request = new GenerateGalaxyRequest("Void Prime", "alpha-001", 25, 2, 6);

        var first = generator.Generate(request);
        var second = generator.Generate(request);

        Assert.Equal(CreateSignature(first), CreateSignature(second));
    }

    [Fact]
    public void DifferentSeedGeneratesDifferentUniverseSignature()
    {
        var generator = new GalaxyGenerator();
        var first = generator.Generate(new GenerateGalaxyRequest("Void Prime", "alpha-001", 25, 2, 6));
        var second = generator.Generate(new GenerateGalaxyRequest("Void Prime", "beta-001", 25, 2, 6));

        Assert.NotEqual(CreateSignature(first), CreateSignature(second));
    }

    [Fact]
    public void GeneratedSolarSystemsHaveUniqueCoordinates()
    {
        var generator = new GalaxyGenerator();
        var result = generator.Generate(new GenerateGalaxyRequest("Void Prime", "alpha-001", 100, 2, 6));

        var coordinates = result.SolarSystems.Select(system => system.Coordinates).ToArray();

        Assert.Equal(coordinates.Length, coordinates.Distinct().Count());
    }

    [Fact]
    public void GeneratedPlanetCountsStayInsideConfiguredLimits()
    {
        var generator = new GalaxyGenerator();
        var result = generator.Generate(new GenerateGalaxyRequest("Void Prime", "alpha-001", 30, 3, 7));

        Assert.All(result.SolarSystems, system => Assert.InRange(system.Planets.Count, 3, 7));
    }

    [Fact]
    public void InvalidRequestIsRejected()
    {
        var generator = new GalaxyGenerator();

        Assert.Throws<ArgumentException>(() =>
            generator.Generate(new GenerateGalaxyRequest(" ", "alpha-001", 10, 2, 6)));
        Assert.Throws<ArgumentException>(() =>
            generator.Generate(new GenerateGalaxyRequest("Void Prime", " ", 10, 2, 6)));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            generator.Generate(new GenerateGalaxyRequest("Void Prime", "alpha-001", 0, 2, 6)));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            generator.Generate(new GenerateGalaxyRequest("Void Prime", "alpha-001", 10, 0, 6)));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            generator.Generate(new GenerateGalaxyRequest("Void Prime", "alpha-001", 10, 7, 6)));
    }

    private static string CreateSignature(GeneratedGalaxyResult result)
    {
        return string.Join("|", result.SolarSystems
            .OrderBy(system => system.Name)
            .Select(system => string.Join(",", new[]
            {
                system.Name,
                system.Coordinates.ToString(),
                system.Star.StarType.ToString(),
                string.Join(";", system.Planets
                    .OrderBy(planet => planet.OrbitalSlot)
                    .Select(planet => $"{planet.OrbitalSlot}:{planet.PlanetType}:{planet.Size}"))
            })));
    }
}
