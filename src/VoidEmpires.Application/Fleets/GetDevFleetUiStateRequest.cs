namespace VoidEmpires.Application.Fleets;

public sealed record GetDevFleetUiStateRequest(Guid CivilizationId, Guid? PlanetId = null);
