using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VoidEmpires.Infrastructure.Persistence;

public sealed class VoidEmpiresDbContextFactory : IDesignTimeDbContextFactory<VoidEmpiresDbContext>
{
    public VoidEmpiresDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? Environment.GetEnvironmentVariable("VOIDEMPIRES_CONNECTION_STRING")
            ?? "Host=localhost;Database=voidempires_design";

        var options = new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new VoidEmpiresDbContext(options);
    }
}
