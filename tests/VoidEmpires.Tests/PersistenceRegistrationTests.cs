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
