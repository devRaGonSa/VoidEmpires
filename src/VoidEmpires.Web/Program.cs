using VoidEmpires.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

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
