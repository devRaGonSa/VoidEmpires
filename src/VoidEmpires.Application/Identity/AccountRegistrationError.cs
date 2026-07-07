namespace VoidEmpires.Application.Identity;

public sealed record AccountRegistrationError(
    string Code,
    string Message,
    string? Field = null);
