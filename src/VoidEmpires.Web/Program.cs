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
