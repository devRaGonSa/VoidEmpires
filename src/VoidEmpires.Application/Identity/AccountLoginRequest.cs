namespace VoidEmpires.Application.Identity;

public sealed record AccountLoginRequest(
    string Email,
    string Password);
