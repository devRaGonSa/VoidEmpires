using VoidEmpires.Domain.Buildings;

namespace VoidEmpires.Application.Buildings;

public sealed record ConstructBuildingRequest(
    Guid PlanetId,
    Guid CivilizationId,
    BuildingType BuildingType);
