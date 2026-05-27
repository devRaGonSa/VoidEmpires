using Microsoft.AspNetCore.Mvc;
using VoidEmpires.Application.Galaxy;
using VoidEmpires.Application.Identity;
using VoidEmpires.Application.Players;
using VoidEmpires.Domain.Players;
using VoidEmpires.Infrastructure;
using VoidEmpires.Infrastructure.Email;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<BrevoEmailOptions>(builder.Configuration.GetSection(BrevoEmailOptions.SectionName));
builder.Services.AddVoidEmpiresTransactionalEmail();
builder.Services.AddVoidEmpiresGalaxyGeneration();

var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddVoidEmpiresPersistence(defaultConnectionString);
if (!string.IsNullOrWhiteSpace(defaultConnectionString))
{
    builder.Services.AddVoidEmpiresIdentity();
}

var app = builder.Build();

app.MapGet("/", () => "VoidEmpires");
app.MapPost("/api/auth/register", async (
    RegisterApiRequest request,
    [FromServices] IServiceProvider services,
    [FromServices] IConfiguration configuration) =>
{
    if (!IsPersistenceConfigured(configuration))
    {
        return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
    }

    if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
    {
        return Results.BadRequest(new AuthApiResponse(false, null, ["Email and password are required."]));
    }

    var registrationService = services.GetRequiredService<IUserRegistrationService>();
    var result = await registrationService.RegisterAsync(new RegisterUserRequest(request.Email, request.Password));

    return result.Succeeded
        ? Results.Created("/api/auth/register", new AuthApiResponse(true, result.UserId, []))
        : Results.BadRequest(new AuthApiResponse(false, null, result.Errors));
});
app.MapGet("/api/auth/confirm-email", async (
    string? userId,
    string? token,
    [FromServices] IServiceProvider services,
    [FromServices] IConfiguration configuration) =>
{
    if (!IsPersistenceConfigured(configuration))
    {
        return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
    }

    if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
    {
        return Results.BadRequest(new AuthApiResponse(false, null, ["User id and token are required."]));
    }

    var confirmationService = services.GetRequiredService<IEmailConfirmationService>();
    var result = await confirmationService.ConfirmEmailAsync(new ConfirmEmailRequest(userId, token));

    return result.Succeeded
        ? Results.Ok(new AuthApiResponse(true, userId, []))
        : Results.BadRequest(new AuthApiResponse(false, null, result.Errors));
});
if (AreDevelopmentEndpointsEnabled(app.Environment, app.Configuration))
{
    app.MapPost("/api/dev/galaxies/generate", async (
        GenerateGalaxyApiRequest request,
        [FromServices] IServiceProvider services,
        [FromServices] IConfiguration configuration,
        CancellationToken cancellationToken) =>
    {
        if (!IsPersistenceConfigured(configuration))
        {
            return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
        }

        var errors = Validate(request);
        if (errors.Count > 0)
        {
            return Results.BadRequest(new GalaxyGenerationApiResponse(false, null, null, 0, 0, errors));
        }

        var generationService = services.GetRequiredService<IGalaxyGenerationService>();
        var result = await generationService.GenerateAndPersistAsync(new GenerateAndPersistGalaxyRequest(
            request.Name!,
            request.Seed!,
            request.SolarSystemCount,
            request.MinPlanetsPerSystem,
            request.MaxPlanetsPerSystem,
            request.OverwriteExisting), cancellationToken);

        var response = new GalaxyGenerationApiResponse(
            result.Succeeded,
            result.GalaxyId,
            result.GalaxyName,
            result.SolarSystemCount,
            result.PlanetCount,
            result.Errors);

        return result.Succeeded
            ? Results.Created($"/api/dev/galaxies/{result.GalaxyId}", response)
            : Results.Conflict(response);
    });

    app.MapPost("/api/dev/players/starting-civilization", async (
        CreateStartingCivilizationApiRequest request,
        [FromServices] IServiceProvider services,
        [FromServices] IConfiguration configuration,
        CancellationToken cancellationToken) =>
    {
        if (!IsPersistenceConfigured(configuration))
        {
            return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
        }

        var errors = Validate(request);
        if (errors.Count > 0)
        {
            return Results.BadRequest(new StartingCivilizationApiResponse(false, null, null, null, errors));
        }

        var service = services.GetRequiredService<IStartingCivilizationService>();
        var result = await service.CreateAsync(new CreateStartingCivilizationRequest(
            request.UserId!,
            request.DisplayName!,
            request.CivilizationName!,
            request.Archetype,
            request.HomePlanetId), cancellationToken);

        var response = new StartingCivilizationApiResponse(
            result.Succeeded,
            result.PlayerProfileId,
            result.CivilizationId,
            result.HomePlanetId,
            result.Errors);

        return result.Succeeded
            ? Results.Created($"/api/dev/players/{result.PlayerProfileId}/civilizations/{result.CivilizationId}", response)
            : Results.Conflict(response);
    });
}
app.MapGet("/health", () =>
{
    var persistenceConfigured = IsPersistenceConfigured(app.Configuration);

    return Results.Ok(new
    {
        status = "ok",
        service = "VoidEmpires.Web",
        persistence = new
        {
            configured = persistenceConfigured,
            provider = "PostgreSQL"
        },
        auth = new
        {
            configured = persistenceConfigured,
            provider = "ASP.NET Core Identity"
        }
    });
});

app.Run();

static bool IsPersistenceConfigured(IConfiguration configuration) =>
    !string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection"));

static bool AreDevelopmentEndpointsEnabled(IHostEnvironment environment, IConfiguration configuration) =>
    environment.IsDevelopment() || configuration.GetValue<bool>("VoidEmpires:DevEndpoints:Enabled");

static IReadOnlyList<string> Validate(GenerateGalaxyApiRequest request)
{
    var errors = new List<string>();

    if (string.IsNullOrWhiteSpace(request.Name))
    {
        errors.Add("Galaxy name is required.");
    }

    if (string.IsNullOrWhiteSpace(request.Seed))
    {
        errors.Add("Galaxy seed is required.");
    }

    if (request.SolarSystemCount <= 0)
    {
        errors.Add("Solar system count must be positive.");
    }

    if (request.MinPlanetsPerSystem <= 0)
    {
        errors.Add("Minimum planets per system must be positive.");
    }

    if (request.MaxPlanetsPerSystem < request.MinPlanetsPerSystem)
    {
        errors.Add("Maximum planets per system must be greater than or equal to the minimum.");
    }

    return errors;
}

static IReadOnlyList<string> Validate(CreateStartingCivilizationApiRequest request)
{
    var errors = new List<string>();

    if (string.IsNullOrWhiteSpace(request.UserId))
    {
        errors.Add("User id is required.");
    }

    if (string.IsNullOrWhiteSpace(request.DisplayName))
    {
        errors.Add("Display name is required.");
    }

    if (string.IsNullOrWhiteSpace(request.CivilizationName))
    {
        errors.Add("Civilization name is required.");
    }

    return errors;
}

public partial class Program
{
}

internal sealed record RegisterApiRequest(string? Email, string? Password);

internal sealed record AuthApiResponse(bool Succeeded, string? UserId, IReadOnlyList<string> Errors);

internal sealed record GenerateGalaxyApiRequest(
    string? Name,
    string? Seed,
    int SolarSystemCount,
    int MinPlanetsPerSystem,
    int MaxPlanetsPerSystem,
    bool OverwriteExisting = false);

internal sealed record GalaxyGenerationApiResponse(
    bool Succeeded,
    Guid? GalaxyId,
    string? GalaxyName,
    int SolarSystemCount,
    int PlanetCount,
    IReadOnlyList<string> Errors);

internal sealed record CreateStartingCivilizationApiRequest(
    string? UserId,
    string? DisplayName,
    string? CivilizationName,
    CivilizationArchetype Archetype,
    Guid? HomePlanetId = null);

internal sealed record StartingCivilizationApiResponse(
    bool Succeeded,
    Guid? PlayerProfileId,
    Guid? CivilizationId,
    Guid? HomePlanetId,
    IReadOnlyList<string> Errors);
