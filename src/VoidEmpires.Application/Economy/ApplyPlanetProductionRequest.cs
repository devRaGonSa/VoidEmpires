namespace VoidEmpires.Application.Economy;

public sealed record ApplyPlanetProductionRequest(Guid PlanetId, TimeSpan Elapsed);
