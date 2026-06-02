using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VoidEmpires.Infrastructure;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

internal static class TestWebApplicationFactoryExtensions
{
    public static HttpClient CreateClientWithPersistenceDisabled(this WebApplicationFactory<Program> factory) =>
        factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = string.Empty
                });
            });
        }).CreateClient();

    public static WebApplicationFactory<Program> WithInMemoryPersistence(
        this WebApplicationFactory<Program> factory,
        string environment = "Development",
        string? databaseName = null)
    {
        var resolvedDatabaseName = databaseName ?? Guid.NewGuid().ToString("N");

        return factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment(environment);
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = $"Host=localhost;Database={resolvedDatabaseName}"
                });
            });
            builder.ConfigureTestServices(services =>
            {
                services.AddVoidEmpiresPersistence($"Host=localhost;Database={resolvedDatabaseName}");
                services.RemoveAll<DbContextOptions<VoidEmpiresDbContext>>();
                services.RemoveAll<VoidEmpiresDbContext>();
                services.AddDbContext<VoidEmpiresDbContext>(options =>
                    options.UseInMemoryDatabase(resolvedDatabaseName));
            });
        });
    }
}
