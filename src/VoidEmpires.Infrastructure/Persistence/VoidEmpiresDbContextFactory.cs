using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VoidEmpires.Infrastructure.Persistence;

public sealed class VoidEmpiresDbContextFactory : IDesignTimeDbContextFactory<VoidEmpiresDbContext>
{
    public VoidEmpiresDbContext CreateDbContext(string[] args)
    {
        var provider =
            Environment.GetEnvironmentVariable("VoidEmpires__Persistence__Provider")
            ?? Environment.GetEnvironmentVariable("VOIDEMPIRES_DATABASE_PROVIDER");

        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? Environment.GetEnvironmentVariable("VOIDEMPIRES_CONNECTION_STRING")
            ?? "Host=localhost;Database=voidempires_design";

        var optionsBuilder = new DbContextOptionsBuilder<VoidEmpiresDbContext>();

        if (string.Equals(provider, "sqlserver", StringComparison.OrdinalIgnoreCase))
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
        else
        {
            optionsBuilder.UseNpgsql(connectionString);
        }

        var options = optionsBuilder.Options;

        return new VoidEmpiresDbContext(options);
    }
}
