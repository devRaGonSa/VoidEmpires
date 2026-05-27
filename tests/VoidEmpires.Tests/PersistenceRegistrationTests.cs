using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Infrastructure;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class PersistenceRegistrationTests
{
    [Fact]
    public void EmptyConnectionStringDoesNotRegisterDbContext()
    {
        var services = new ServiceCollection();

        services.AddVoidEmpiresPersistence("");

        using var provider = services.BuildServiceProvider();

        Assert.Null(provider.GetService<VoidEmpiresDbContext>());
    }

    [Fact]
    public void ConfiguredConnectionStringRegistersNpgsqlDbContext()
    {
        var services = new ServiceCollection();

        services.AddVoidEmpiresPersistence("Host=localhost;Database=voidempires_test");

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<DbContextOptions<VoidEmpiresDbContext>>();

        Assert.Contains(
            options.Extensions,
            extension => extension.GetType().FullName == "Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal.NpgsqlOptionsExtension");
    }
}
