using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Identity;
using VoidEmpires.Infrastructure;
using VoidEmpires.Infrastructure.Identity;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class IdentityAccountRegistrationServiceTests
{
    [Fact]
    public async Task RegisterAsyncCreatesIdentityUserWithHashedPassword()
    {
        await using var provider = BuildProvider();
        var registration = provider.GetRequiredService<IAccountRegistrationService>();
        var users = provider.GetRequiredService<UserManager<VoidEmpiresUser>>();

        var result = await registration.RegisterAsync(ValidRequest());

        Assert.True(result.Succeeded);
        Assert.NotNull(result.UserId);
        var user = await users.FindByIdAsync(result.UserId);
        Assert.NotNull(user);
        Assert.Equal("player@example.test", user.Email);
        Assert.NotEqual("P@ssw0rd!23", user.PasswordHash);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task RegisterAsyncRejectsDuplicateEmailSafely()
    {
        await using var provider = BuildProvider();
        var registration = provider.GetRequiredService<IAccountRegistrationService>();

        var first = await registration.RegisterAsync(ValidRequest());
        var duplicate = await registration.RegisterAsync(ValidRequest() with
        {
            DisplayName = "Other Commander",
            CivilizationName = "Other Dominion"
        });

        Assert.True(first.Succeeded);
        Assert.False(duplicate.Succeeded);
        var error = Assert.Single(duplicate.Errors);
        Assert.Equal("EmailAlreadyRegistered", error.Code);
        Assert.Equal("email", error.Field);
        Assert.DoesNotContain("P@ssw0rd", error.Message);
    }

    [Fact]
    public async Task RegisterAsyncRejectsInvalidPasswordBeforeIdentityCreate()
    {
        await using var provider = BuildProvider();
        var registration = provider.GetRequiredService<IAccountRegistrationService>();

        var result = await registration.RegisterAsync(ValidRequest() with
        {
            Password = "weak",
            ConfirmPassword = "weak"
        });

        Assert.False(result.Succeeded);
        Assert.Contains(result.Errors, error => error.Code == "PasswordTooWeak" && error.Field == "password");
        Assert.Null(result.UserId);
    }

    [Fact]
    public async Task RegisterAsyncReturnsStructuredValidationErrors()
    {
        await using var provider = BuildProvider();
        var registration = provider.GetRequiredService<IAccountRegistrationService>();

        var result = await registration.RegisterAsync(ValidRequest() with { Email = "not-email" });

        Assert.False(result.Succeeded);
        Assert.Contains(result.Errors, error => error.Code == "EmailInvalid" && error.Field == "email");
        Assert.DoesNotContain(result.Errors, error => error.Message.Contains("P@ssw0rd", StringComparison.Ordinal));
    }

    private static AccountRegistrationRequest ValidRequest() => new(
        "player@example.test",
        "P@ssw0rd!23",
        "P@ssw0rd!23",
        "Commander Vega",
        "Solar Dominion",
        "Nova Prime");

    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<VoidEmpiresDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddVoidEmpiresIdentity();
        return services.BuildServiceProvider();
    }
}
