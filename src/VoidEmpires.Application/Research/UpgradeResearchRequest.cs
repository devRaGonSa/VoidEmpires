using VoidEmpires.Domain.Research;

namespace VoidEmpires.Application.Research;

public sealed record UpgradeResearchRequest(
    Guid CivilizationId,
    Guid SourcePlanetId,
    ResearchType ResearchType);
