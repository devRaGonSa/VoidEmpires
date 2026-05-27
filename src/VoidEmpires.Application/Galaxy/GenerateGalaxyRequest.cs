namespace VoidEmpires.Application.Galaxy;

public sealed record GenerateGalaxyRequest(
    string Name,
    string Seed,
    int SolarSystemCount,
    int MinPlanetsPerSystem,
    int MaxPlanetsPerSystem);
