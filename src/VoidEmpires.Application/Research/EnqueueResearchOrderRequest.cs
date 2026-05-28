using VoidEmpires.Domain.Research;

namespace VoidEmpires.Application.Research;

public sealed record EnqueueResearchOrderRequest(
    Guid CivilizationId,
    Guid SourcePlanetId,
    ResearchType ResearchType,
    DateTime RequestedAtUtc);
