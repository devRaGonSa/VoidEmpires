var builder = WebApplication.CreateBuilder(args);
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
