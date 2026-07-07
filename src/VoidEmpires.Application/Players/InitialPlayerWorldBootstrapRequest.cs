namespace VoidEmpires.Application.Players;

public sealed record InitialPlayerWorldBootstrapRequest(
    string UserId,
    string DisplayName,
    string CivilizationName,
    string? HomePlanetName = null);
