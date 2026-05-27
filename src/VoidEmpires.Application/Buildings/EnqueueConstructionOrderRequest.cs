using VoidEmpires.Domain.Buildings;

namespace VoidEmpires.Application.Buildings;

public sealed record EnqueueConstructionOrderRequest(
    Guid PlanetId,
    Guid CivilizationId,
    ConstructionQueueItemAction Action,
    BuildingType BuildingType,
    DateTime RequestedAtUtc);
