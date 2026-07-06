using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Email;
using VoidEmpires.Infrastructure;
using VoidEmpires.Infrastructure.Identity;
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

    [Fact]
    public void ExplicitSqlServerProviderRegistersSqlServerDbContext()
    {
        var services = new ServiceCollection();

        services.AddVoidEmpiresPersistence(
            "Server=localhost;Database=VoidEmpires;User Id=test;Password=<PASSWORD>;TrustServerCertificate=True;",
            "sqlserver");

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<DbContextOptions<VoidEmpiresDbContext>>();

        Assert.Contains(
            options.Extensions,
            extension => extension.GetType().FullName == "Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal.SqlServerOptionsExtension");
    }

    [Fact]
    public void DesignTimeFactoryDefaultsToNpgsqlWhenProviderIsNotSpecified()
    {
        var originalProvider = Environment.GetEnvironmentVariable("VoidEmpires__Persistence__Provider");
        var originalProviderAlias = Environment.GetEnvironmentVariable("VOIDEMPIRES_DATABASE_PROVIDER");
        var originalConnection = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
        var originalConnectionAlias = Environment.GetEnvironmentVariable("VOIDEMPIRES_CONNECTION_STRING");

        try
        {
            Environment.SetEnvironmentVariable("VoidEmpires__Persistence__Provider", null);
            Environment.SetEnvironmentVariable("VOIDEMPIRES_DATABASE_PROVIDER", null);
            Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", "Host=localhost;Database=voidempires_test");
            Environment.SetEnvironmentVariable("VOIDEMPIRES_CONNECTION_STRING", null);

            var context = new VoidEmpiresDbContextFactory().CreateDbContext([]);

            Assert.Equal("Npgsql.EntityFrameworkCore.PostgreSQL", context.Database.ProviderName);
        }
        finally
        {
            Environment.SetEnvironmentVariable("VoidEmpires__Persistence__Provider", originalProvider);
            Environment.SetEnvironmentVariable("VOIDEMPIRES_DATABASE_PROVIDER", originalProviderAlias);
            Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", originalConnection);
            Environment.SetEnvironmentVariable("VOIDEMPIRES_CONNECTION_STRING", originalConnectionAlias);
        }
    }

    [Fact]
    public void DesignTimeFactorySupportsExplicitSqlServerProviderSelection()
    {
        var originalProvider = Environment.GetEnvironmentVariable("VoidEmpires__Persistence__Provider");
        var originalProviderAlias = Environment.GetEnvironmentVariable("VOIDEMPIRES_DATABASE_PROVIDER");
        var originalConnection = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
        var originalConnectionAlias = Environment.GetEnvironmentVariable("VOIDEMPIRES_CONNECTION_STRING");

        try
        {
            Environment.SetEnvironmentVariable("VoidEmpires__Persistence__Provider", "sqlserver");
            Environment.SetEnvironmentVariable("VOIDEMPIRES_DATABASE_PROVIDER", null);
            Environment.SetEnvironmentVariable(
                "ConnectionStrings__DefaultConnection",
                "Server=localhost;Database=VoidEmpires;User Id=test;Password=<PASSWORD>;TrustServerCertificate=True;");
            Environment.SetEnvironmentVariable("VOIDEMPIRES_CONNECTION_STRING", null);

            var context = new VoidEmpiresDbContextFactory().CreateDbContext([]);

            Assert.Equal("Microsoft.EntityFrameworkCore.SqlServer", context.Database.ProviderName);
        }
        finally
        {
            Environment.SetEnvironmentVariable("VoidEmpires__Persistence__Provider", originalProvider);
            Environment.SetEnvironmentVariable("VOIDEMPIRES_DATABASE_PROVIDER", originalProviderAlias);
            Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", originalConnection);
            Environment.SetEnvironmentVariable("VOIDEMPIRES_CONNECTION_STRING", originalConnectionAlias);
        }
    }

    [Fact]
    public void DesignTimeFactoryUsesPasswordlessSqlServerFallbackWhenConnectionIsNotSpecified()
    {
        var originalProvider = Environment.GetEnvironmentVariable("VoidEmpires__Persistence__Provider");
        var originalProviderAlias = Environment.GetEnvironmentVariable("VOIDEMPIRES_DATABASE_PROVIDER");
        var originalConnection = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
        var originalConnectionAlias = Environment.GetEnvironmentVariable("VOIDEMPIRES_CONNECTION_STRING");

        try
        {
            Environment.SetEnvironmentVariable("VoidEmpires__Persistence__Provider", "sqlserver");
            Environment.SetEnvironmentVariable("VOIDEMPIRES_DATABASE_PROVIDER", null);
            Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", null);
            Environment.SetEnvironmentVariable("VOIDEMPIRES_CONNECTION_STRING", null);

            var context = new VoidEmpiresDbContextFactory().CreateDbContext([]);

            Assert.Equal("Microsoft.EntityFrameworkCore.SqlServer", context.Database.ProviderName);
            Assert.Contains("VoidEmpires_GenerationOnly", context.Database.GetConnectionString());
            Assert.DoesNotContain("Password", context.Database.GetConnectionString(), StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Environment.SetEnvironmentVariable("VoidEmpires__Persistence__Provider", originalProvider);
            Environment.SetEnvironmentVariable("VOIDEMPIRES_DATABASE_PROVIDER", originalProviderAlias);
            Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", originalConnection);
            Environment.SetEnvironmentVariable("VOIDEMPIRES_CONNECTION_STRING", originalConnectionAlias);
        }
    }

    [Fact]
    public void IdentityRegistrationUsesVoidEmpiresDbContextWithConservativeDefaults()
    {
        var services = new ServiceCollection();

        services.AddVoidEmpiresPersistence("Host=localhost;Database=voidempires_test");
        services.AddVoidEmpiresIdentity();

        using var provider = services.BuildServiceProvider();
        var identityOptions = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<IdentityOptions>>().Value;

        Assert.True(identityOptions.User.RequireUniqueEmail);
        Assert.True(identityOptions.SignIn.RequireConfirmedEmail);
        Assert.NotNull(provider.GetRequiredService<IUserStore<VoidEmpiresUser>>());
    }

    [Fact]
    public void IdentityRegistrationSupportsValidatedServiceProviderConstruction()
    {
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddVoidEmpiresPersistence("Host=localhost;Database=voidempires_test");
        services.AddSingleton<ITransactionalEmailSender, StubTransactionalEmailSender>();
        services.AddVoidEmpiresIdentity();

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true
        });

        Assert.NotNull(provider.GetRequiredService<DataProtectorTokenProvider<VoidEmpiresUser>>());
    }

    private sealed class StubTransactionalEmailSender : ITransactionalEmailSender
    {
        public Task<TransactionalEmailResult> SendAsync(
            TransactionalEmailMessage message,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(TransactionalEmailResult.Accepted("stub-email"));
    }
}
