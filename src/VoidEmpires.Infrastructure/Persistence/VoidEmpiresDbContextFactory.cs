using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VoidEmpires.Infrastructure.Persistence;

public sealed class VoidEmpiresDbContextFactory : IDesignTimeDbContextFactory<VoidEmpiresDbContext>
{
    private const string DesignTimePostgreSqlConnectionString = "Host=localhost;Database=voidempires_design";
    private const string DesignTimeSqlServerConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=VoidEmpires_GenerationOnly;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;";

    public VoidEmpiresDbContext CreateDbContext(string[] args)
    {
        var provider =
            Environment.GetEnvironmentVariable("VoidEmpires__Persistence__Provider")
            ?? Environment.GetEnvironmentVariable("VOIDEMPIRES_DATABASE_PROVIDER");

        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? Environment.GetEnvironmentVariable("VOIDEMPIRES_CONNECTION_STRING");

        var optionsBuilder = new DbContextOptionsBuilder<VoidEmpiresDbContext>();

        if (string.Equals(provider, "sqlserver", StringComparison.OrdinalIgnoreCase))
        {
            optionsBuilder
                .UseSqlServer(ResolveConnectionString(connectionString, DesignTimeSqlServerConnectionString))
                .ReplaceService<IMigrationsAssembly, SqlServerDesignTimeMigrationsAssembly>();
        }
        else
        {
            optionsBuilder.UseNpgsql(ResolveConnectionString(connectionString, DesignTimePostgreSqlConnectionString));
        }

        var options = optionsBuilder.Options;

        return new VoidEmpiresDbContext(options);
    }

    private static string ResolveConnectionString(string? configuredConnectionString, string fallbackConnectionString) =>
        string.IsNullOrWhiteSpace(configuredConnectionString)
            ? fallbackConnectionString
            : configuredConnectionString;
}
