namespace VoidEmpires.Application.Economy;

public sealed record ApplyPlanetProductionRequest(
    Guid PlanetId,
    Guid CivilizationId,
    TimeSpan Elapsed);
