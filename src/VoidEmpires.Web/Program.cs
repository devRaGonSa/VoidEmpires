using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using VoidEmpires.Application.Identity;
using VoidEmpires.Infrastructure;
using VoidEmpires.Infrastructure.Email;

var builder = WebApplication.CreateBuilder(args);
const string LocalFrontendDevelopmentCorsPolicy = "LocalFrontendDevelopment";

builder.Services.Configure<BrevoEmailOptions>(builder.Configuration.GetSection(BrevoEmailOptions.SectionName));
builder.Services.AddVoidEmpiresTransactionalEmail();
builder.Services.AddVoidEmpiresGalaxyGeneration();
builder.Services.AddCors(options =>
{
    options.AddPolicy(LocalFrontendDevelopmentCorsPolicy, policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://127.0.0.1:5173")
            .WithMethods(HttpMethods.Get, HttpMethods.Post)
            .WithHeaders(
                HeaderNames.Accept,
                HeaderNames.ContentType);
    });
});

var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddVoidEmpiresPersistence(defaultConnectionString);
if (!string.IsNullOrWhiteSpace(defaultConnectionString))
{
    builder.Services.AddVoidEmpiresIdentity();
    builder.Services.AddVoidEmpiresConstructionQueueWorker(builder.Configuration);
    builder.Services.AddVoidEmpiresResearchQueueWorker(builder.Configuration);
    builder.Services.AddVoidEmpiresAssetProductionWorker(builder.Configuration);
    builder.Services.AddVoidEmpiresOrbitalTransferWorker(builder.Configuration);
}

var app = builder.Build();
var developmentEndpointsEnabled = AreDevelopmentEndpointsEnabled(app.Environment, app.Configuration);

if (developmentEndpointsEnabled)
{
    app.UseCors(LocalFrontendDevelopmentCorsPolicy);
}

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
if (developmentEndpointsEnabled)
{
    app.UseStaticFiles();
    app.MapDevEndpointMappings();
    app.MapDevFleetUiStateEndpoints();
    app.MapDevPlanetUiStateEndpoints();
    app.MapDevFleetActionManifestEndpoints();
    app.MapDevSystemVisualStateEndpoints();
    app.MapDevStrategicMapEndpoints();
    app.MapDevStrategicMapActionManifestEndpoints();
    app.MapDevSensorProfileEndpoints();
    app.MapDevDetectionCoverageEndpoints();
    app.MapDevInterceptionOpportunityEndpoints();
    app.MapDevAllianceReadinessEndpoints();
    app.MapDevAlliancePactReadinessEndpoints();
    app.MapDevDiplomaticContactEndpoints();
    app.MapDevExplorationActionPreviewEndpoints();
    app.MapDevExplorationMissionEndpoints();
    app.MapDevExplorationKnowledgeEndpoints();
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

public partial class Program
{
}

internal sealed record RegisterApiRequest(string? Email, string? Password);

internal sealed record AuthApiResponse(bool Succeeded, string? UserId, IReadOnlyList<string> Errors);
