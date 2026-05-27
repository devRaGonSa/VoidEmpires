using VoidEmpires.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddVoidEmpiresPersistence(builder.Configuration.GetConnectionString("DefaultConnection"));

var app = builder.Build();

app.MapGet("/", () => "VoidEmpires");
app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    service = "VoidEmpires.Web"
}));

app.Run();

public partial class Program
{
}
