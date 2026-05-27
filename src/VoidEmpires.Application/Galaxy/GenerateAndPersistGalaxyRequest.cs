namespace VoidEmpires.Application.Galaxy;

public sealed record GenerateAndPersistGalaxyRequest(
    string Name,
    string Seed,
    int SolarSystemCount,
    int MinPlanetsPerSystem,
    int MaxPlanetsPerSystem);
