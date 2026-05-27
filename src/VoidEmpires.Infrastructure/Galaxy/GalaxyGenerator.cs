using System.Security.Cryptography;
using System.Text;
using VoidEmpires.Application.Galaxy;
using VoidEmpires.Domain.Galaxy;

namespace VoidEmpires.Infrastructure.Galaxy;

public sealed class GalaxyGenerator : IGalaxyGenerator
{
    public GeneratedGalaxyResult Generate(GenerateGalaxyRequest request)
    {
        Validate(request);

        var random = new Random(CreateStableSeed(request.Seed));
        var galaxy = Domain.Galaxy.Galaxy.Create(request.Name);
        var usedCoordinates = new HashSet<GalaxyCoordinates>();

        for (var index = 0; index < request.SolarSystemCount; index++)
        {
            var coordinates = GenerateUniqueCoordinates(random, usedCoordinates);
            var solarSystemId = Guid.NewGuid();
            var systemName = $"{request.Name} System {index + 1:0000}";
            var star = Star.Create(solarSystemId, $"{systemName} Star", PickStarType(random));
            var solarSystem = new SolarSystem(solarSystemId, galaxy.Id, systemName, coordinates, star);
            var planetCount = random.Next(request.MinPlanetsPerSystem, request.MaxPlanetsPerSystem + 1);

            for (var slot = 1; slot <= planetCount; slot++)
            {
                solarSystem.AddPlanet(Planet.Create(
                    solarSystem.Id,
                    $"{systemName} {Romanize(slot)}",
                    slot,
                    PickPlanetType(random),
                    random.Next(2_000, 24_001)));
            }

            galaxy.AddSolarSystem(solarSystem);
        }

        return new GeneratedGalaxyResult(galaxy);
    }

    private static void Validate(GenerateGalaxyRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Galaxy name must not be empty.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Seed))
        {
            throw new ArgumentException("Galaxy generation seed must not be empty.", nameof(request));
        }

        if (request.SolarSystemCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request), "Solar system count must be positive.");
        }

        if (request.MinPlanetsPerSystem <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request), "Minimum planets per system must be positive.");
        }

        if (request.MaxPlanetsPerSystem < request.MinPlanetsPerSystem)
        {
            throw new ArgumentOutOfRangeException(nameof(request), "Maximum planets per system must be greater than or equal to the minimum.");
        }
    }

    private static int CreateStableSeed(string seed)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(seed));
        return BitConverter.ToInt32(bytes, 0);
    }

    private static GalaxyCoordinates GenerateUniqueCoordinates(Random random, HashSet<GalaxyCoordinates> usedCoordinates)
    {
        for (var attempts = 0; attempts < 10_000; attempts++)
        {
            var coordinates = new GalaxyCoordinates(
                random.Next(-10_000, 10_001),
                random.Next(-10_000, 10_001),
                random.Next(-1_000, 1_001));

            if (usedCoordinates.Add(coordinates))
            {
                return coordinates;
            }
        }

        throw new InvalidOperationException("Unable to generate unique solar system coordinates.");
    }

    private static StarType PickStarType(Random random)
    {
        var values = Enum.GetValues<StarType>();
        return values[random.Next(values.Length)];
    }

    private static PlanetType PickPlanetType(Random random)
    {
        var values = Enum.GetValues<PlanetType>();
        return values[random.Next(values.Length)];
    }

    private static string Romanize(int value) => value switch
    {
        1 => "I",
        2 => "II",
        3 => "III",
        4 => "IV",
        5 => "V",
        6 => "VI",
        7 => "VII",
        8 => "VIII",
        9 => "IX",
        10 => "X",
        11 => "XI",
        12 => "XII",
        _ => value.ToString("000")
    };
}
