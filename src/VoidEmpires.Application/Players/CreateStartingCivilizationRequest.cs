using VoidEmpires.Domain.Players;

namespace VoidEmpires.Application.Players;

public sealed record CreateStartingCivilizationRequest(
    string UserId,
    string DisplayName,
    string CivilizationName,
    CivilizationArchetype Archetype,
    Guid? HomePlanetId = null);
