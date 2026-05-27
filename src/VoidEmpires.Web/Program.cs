using Microsoft.AspNetCore.Mvc;
using VoidEmpires.Application.Identity;
using VoidEmpires.Infrastructure;
using VoidEmpires.Infrastructure.Email;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<BrevoEmailOptions>(builder.Configuration.GetSection(BrevoEmailOptions.SectionName));
builder.Services.AddVoidEmpiresTransactionalEmail();

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
    [FromServices] IUserRegistrationService registrationService) =>
{
    if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
    {
        return Results.BadRequest(new AuthApiResponse(false, null, ["Email and password are required."]));
    }

    var result = await registrationService.RegisterAsync(new RegisterUserRequest(request.Email, request.Password));

    return result.Succeeded
        ? Results.Created("/api/auth/register", new AuthApiResponse(true, result.UserId, []))
        : Results.BadRequest(new AuthApiResponse(false, null, result.Errors));
});
app.MapGet("/api/auth/confirm-email", async (
    string? userId,
    string? token,
    [FromServices] IEmailConfirmationService confirmationService) =>
{
    if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
    {
        return Results.BadRequest(new AuthApiResponse(false, null, ["User id and token are required."]));
    }

    var result = await confirmationService.ConfirmEmailAsync(new ConfirmEmailRequest(userId, token));

    return result.Succeeded
        ? Results.Ok(new AuthApiResponse(true, userId, []))
        : Results.BadRequest(new AuthApiResponse(false, null, result.Errors));
});
app.MapGet("/health", () =>
{
    var persistenceConnectionString = app.Configuration.GetConnectionString("DefaultConnection");

    return Results.Ok(new
    {
        status = "ok",
        service = "VoidEmpires.Web",
        persistence = new
        {
            configured = !string.IsNullOrWhiteSpace(persistenceConnectionString),
            provider = string.IsNullOrWhiteSpace(persistenceConnectionString) ? "none" : "PostgreSQL"
        }
    });
});

app.Run();

public partial class Program
{
}

internal sealed record RegisterApiRequest(string? Email, string? Password);

internal sealed record AuthApiResponse(bool Succeeded, string? UserId, IReadOnlyList<string> Errors);
