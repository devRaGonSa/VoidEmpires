using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using VoidEmpires.Application.Identity;
using VoidEmpires.Application.Players;
using VoidEmpires.Infrastructure.Persistence;

internal static class AccountEndpoints
{
    public static void MapAccountEndpoints(this WebApplication app) =>
        app.MapPost("/api/accounts/register", RegisterAsync);
    private static async Task<IResult> RegisterAsync(
        AccountRegistrationRequest? request,
        [FromServices] IServiceProvider services,
        [FromServices] IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        if (!IsPersistenceConfigured(configuration)) return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
        var safeRequest = Sanitize(request);
        var registrationService = services.GetRequiredService<IAccountRegistrationService>();
        var bootstrapService = services.GetRequiredService<IInitialPlayerWorldBootstrapService>();
        await using var transaction = await BeginTransactionIfRelationalAsync(services, cancellationToken);
        var accountResult = await registrationService.RegisterAsync(safeRequest, cancellationToken);
        if (!accountResult.Succeeded || string.IsNullOrWhiteSpace(accountResult.UserId)) return ToAccountFailureResult(accountResult.Errors);
        var bootstrapResult = await bootstrapService.CreateAsync(new InitialPlayerWorldBootstrapRequest(
            accountResult.UserId,
            safeRequest.DisplayName,
            safeRequest.CivilizationName,
            safeRequest.HomePlanetName), cancellationToken);
        if (!bootstrapResult.Succeeded) return Results.Conflict(AccountRegistrationApiResponse.Failure(MapBootstrapErrors(bootstrapResult.Errors)));
        if (transaction is not null) await transaction.CommitAsync(cancellationToken);
        var nextRoute = $"/planet?civilizationId={bootstrapResult.CivilizationId}&planetId={bootstrapResult.HomePlanetId}";
        return Results.Created("/api/accounts/register", new AccountRegistrationApiResponse(
            true,
            accountResult.UserId,
            bootstrapResult.PlayerProfileId,
            bootstrapResult.CivilizationId,
            bootstrapResult.HomePlanetId,
            bootstrapResult.HomePlanetName,
            nextRoute,
            bootstrapResult.StartingResources,
            []));
    }
    private static AccountRegistrationRequest Sanitize(AccountRegistrationRequest? request) =>
        new(request?.Email ?? string.Empty, request?.Password ?? string.Empty, request?.ConfirmPassword ?? string.Empty, request?.DisplayName ?? string.Empty, request?.CivilizationName ?? string.Empty, request?.HomePlanetName);
    private static async Task<IDbContextTransaction?> BeginTransactionIfRelationalAsync(
        IServiceProvider services,
        CancellationToken cancellationToken)
    {
        var dbContext = services.GetService<VoidEmpiresDbContext>();
        return dbContext?.Database.IsRelational() == true
            ? await dbContext.Database.BeginTransactionAsync(cancellationToken)
            : null;
    }
    private static IResult ToAccountFailureResult(IReadOnlyList<AccountRegistrationError> errors)
    {
        var safeErrors = errors.Select(ToSafeAccountError).ToArray();
        var response = AccountRegistrationApiResponse.Failure(safeErrors);
        return safeErrors.Any(error => IsConflictError(error.Code)) ? Results.Conflict(response) : Results.BadRequest(response);
    }
    private static AccountRegistrationApiError ToSafeAccountError(AccountRegistrationError error) =>
        IsKnownAccountError(error.Code)
            ? new AccountRegistrationApiError(error.Code, error.Message, error.Field)
            : new AccountRegistrationApiError("RegistrationFailed", "Registration could not be completed.", null);
    private static bool IsKnownAccountError(string code) =>
        code is "EmailRequired" or "EmailInvalid" or "PasswordRequired" or "PasswordTooWeak" or "ConfirmPasswordRequired" or "PasswordMismatch" or "DisplayNameRequired" or "DisplayNameTooLong" or "CivilizationNameRequired" or "CivilizationNameTooLong" or "HomePlanetNameTooLong" or "EmailAlreadyRegistered";
    private static bool IsConflictError(string code) =>
        code is "EmailAlreadyRegistered" or "DisplayNameTaken" or "CivilizationNameTaken" or "PlayerProfileExists";
    private static AccountRegistrationApiError[] MapBootstrapErrors(IReadOnlyList<string> errors) =>
        errors.Select(MapBootstrapError).ToArray();
    private static AccountRegistrationApiError MapBootstrapError(string error) =>
        error switch
        {
            "Player profile already exists for this user." => new AccountRegistrationApiError("PlayerProfileExists", error, null),
            "Display name is already in use." => new AccountRegistrationApiError("DisplayNameTaken", error, "displayName"),
            "Civilization name is already in use." => new AccountRegistrationApiError("CivilizationNameTaken", error, "civilizationName"),
            _ => new AccountRegistrationApiError("BootstrapFailed", "Registration could not be completed.", null)
        };
    private static bool IsPersistenceConfigured(IConfiguration configuration) =>
        !string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection"));
}
internal sealed record AccountRegistrationApiResponse(bool Succeeded, string? UserId, Guid? PlayerProfileId, Guid? CivilizationId, Guid? HomePlanetId, string? HomePlanetName, string? NextRoute, CreateStartingCivilizationResourceSnapshot? StartingResources, IReadOnlyList<AccountRegistrationApiError> Errors)
{
    public static AccountRegistrationApiResponse Failure(IReadOnlyList<AccountRegistrationApiError> errors) =>
        new(false, null, null, null, null, null, null, null, errors);
}
internal sealed record AccountRegistrationApiError(string Code, string Message, string? Field);
