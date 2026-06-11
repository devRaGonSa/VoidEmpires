using VoidEmpires.Domain.Players;

namespace VoidEmpires.Application.Players;

public sealed record CreateStartingCivilizationRequest(
    string DisplayName,
    string CivilizationName,
    string? HomePlanetName = null,
    string? UserId = null,
    CivilizationArchetype Archetype = CivilizationArchetype.Balanced);
