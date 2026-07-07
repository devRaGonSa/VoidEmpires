using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using VoidEmpires.Application.Identity;
using VoidEmpires.Application.Players;
using VoidEmpires.Infrastructure.Identity;
using VoidEmpires.Infrastructure.Persistence;

internal static class AccountEndpoints
{
    public static void MapAccountEndpoints(this WebApplication app)
    {
        app.MapPost("/api/accounts/register", RegisterAsync);
        app.MapPost("/api/accounts/login", LoginAsync);
        app.MapPost("/api/accounts/logout", (Delegate)LogoutAsync);
        app.MapGet("/api/accounts/me", MeAsync);
    }
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
    private static async Task<IResult> LoginAsync(
        AccountLoginRequest? request,
        HttpContext httpContext,
        [FromServices] UserManager<VoidEmpiresUser> userManager,
        [FromServices] VoidEmpiresDbContext dbContext,
        [FromServices] IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        if (!IsPersistenceConfigured(configuration)) return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
        var safeRequest = Sanitize(request);
        var errors = ValidateLogin(safeRequest);
        if (errors.Count > 0) return Results.BadRequest(AccountSessionResult.Failure(errors.ToArray()));
        var user = await userManager.FindByEmailAsync(safeRequest.Email);
        if (user is null || !await userManager.CheckPasswordAsync(user, safeRequest.Password)) return Results.Json(
            AccountSessionResult.Failure(new AccountSessionError("InvalidCredentials", "Email or password is incorrect.")),
            statusCode: StatusCodes.Status401Unauthorized);
        await SignInAsync(httpContext, user.Id, user.Email ?? safeRequest.Email);
        return Results.Ok(await BuildSessionAsync(dbContext, user.Id, cancellationToken));
    }
    private static async Task<IResult> LogoutAsync(HttpContext httpContext)
    {
        await httpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
        return Results.Ok(new AccountLogoutResponse(true, []));
    }
    private static async Task<IResult> MeAsync(
        HttpContext httpContext,
        [FromServices] VoidEmpiresDbContext dbContext,
        [FromServices] IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        if (!IsPersistenceConfigured(configuration)) return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Results.Json(CurrentAccountSession.Unauthenticated(), statusCode: StatusCodes.Status401Unauthorized);
        return Results.Ok(CurrentAccountSession.From(await BuildSessionAsync(dbContext, userId, cancellationToken)));
    }
    private static Task SignInAsync(HttpContext httpContext, string userId, string email)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, email)
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme));
        return httpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal, new AuthenticationProperties { IsPersistent = false });
    }
    private static AccountLoginRequest Sanitize(AccountLoginRequest? request) =>
        new((request?.Email ?? string.Empty).Trim().ToLowerInvariant(), request?.Password ?? string.Empty);
    private static List<AccountSessionError> ValidateLogin(AccountLoginRequest request)
    {
        var errors = new List<AccountSessionError>();
        if (string.IsNullOrWhiteSpace(request.Email)) errors.Add(new AccountSessionError("EmailRequired", "Email is required.", "email"));
        if (string.IsNullOrWhiteSpace(request.Password)) errors.Add(new AccountSessionError("PasswordRequired", "Password is required.", "password"));
        return errors;
    }
    private static async Task<AccountSessionResult> BuildSessionAsync(VoidEmpiresDbContext dbContext, string userId, CancellationToken cancellationToken)
    {
        var profile = await dbContext.PlayerProfiles.AsNoTracking().SingleOrDefaultAsync(x => x.UserId == userId, cancellationToken);
        if (profile is null) return AccountSessionResult.Success(userId, null, null, null, null, null);
        var civilization = await dbContext.Civilizations.AsNoTracking().OrderBy(x => x.Name).FirstOrDefaultAsync(x => x.PlayerProfileId == profile.Id, cancellationToken);
        if (civilization?.HomePlanetId is not Guid homePlanetId) return AccountSessionResult.Success(userId, profile.Id, civilization?.Id, null, null, null);
        var homePlanetName = await dbContext.Planets.AsNoTracking().Where(x => x.Id == homePlanetId).Select(x => x.Name).SingleOrDefaultAsync(cancellationToken);
        return AccountSessionResult.Success(userId, profile.Id, civilization.Id, homePlanetId, homePlanetName, $"/planet?civilizationId={civilization.Id}&planetId={homePlanetId}");
    }
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

internal sealed record AccountLogoutResponse(bool Succeeded, IReadOnlyList<string> Errors);
