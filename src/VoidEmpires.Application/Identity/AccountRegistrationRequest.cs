namespace VoidEmpires.Application.Identity;

public sealed record AccountRegistrationRequest(
    string Email,
    string Password,
    string ConfirmPassword,
    string DisplayName,
    string CivilizationName,
    string? HomePlanetName = null);
