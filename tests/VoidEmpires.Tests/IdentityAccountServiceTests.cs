using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Email;
using VoidEmpires.Application.Identity;
using VoidEmpires.Infrastructure;
using VoidEmpires.Infrastructure.Email;
using VoidEmpires.Infrastructure.Identity;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Tests;

public class IdentityAccountServiceTests
{
    [Fact]
    public async Task RegisterAsyncCreatesUserAndSendsConfirmationEmail()
    {
        await using var provider = BuildProvider();
        var registration = provider.GetRequiredService<IUserRegistrationService>();
        var users = provider.GetRequiredService<UserManager<VoidEmpiresUser>>();
        var emailSender = provider.GetRequiredService<FakeTransactionalEmailSender>();

        var result = await registration.RegisterAsync(new RegisterUserRequest("player@example.test", "P@ssw0rd!23"));

        Assert.True(result.Succeeded);
        Assert.NotNull(result.UserId);
        var user = await users.FindByIdAsync(result.UserId);
        Assert.NotNull(user);
        Assert.Equal("player@example.test", user.Email);
        Assert.Equal("player@example.test", user.UserName);
        var message = Assert.Single(emailSender.Messages);
        Assert.Equal("player@example.test", message.Recipient.Email);
        Assert.Equal("Confirm your VoidEmpires account", message.Subject);
        Assert.StartsWith("https://voidempires.example.test/confirm?", message.ActionUrl?.ToString());
    }

    [Fact]
    public async Task ConfirmEmailAsyncConfirmsExistingUserWithValidToken()
    {
        await using var provider = BuildProvider();
        var users = provider.GetRequiredService<UserManager<VoidEmpiresUser>>();
        var confirmation = provider.GetRequiredService<IEmailConfirmationService>();
        var user = new VoidEmpiresUser { UserName = "player@example.test", Email = "player@example.test" };

        var createResult = await users.CreateAsync(user, "P@ssw0rd!23");
        var token = await users.GenerateEmailConfirmationTokenAsync(user);
        var result = await confirmation.ConfirmEmailAsync(new ConfirmEmailRequest(user.Id, token));

        Assert.True(createResult.Succeeded);
        Assert.True(result.Succeeded);
        Assert.True(await users.IsEmailConfirmedAsync(user));
    }

    [Fact]
    public async Task RegisterAsyncReturnsFailureWhenEmailSenderFails()
    {
        await using var provider = BuildProvider(TransactionalEmailResult.Failed("Email service failed."));
        var registration = provider.GetRequiredService<IUserRegistrationService>();

        var result = await registration.RegisterAsync(new RegisterUserRequest("player@example.test", "P@ssw0rd!23"));

        Assert.False(result.Succeeded);
        Assert.Equal(["Email service failed."], result.Errors);
    }

    private static ServiceProvider BuildProvider(TransactionalEmailResult? emailResult = null)
    {
        var services = new ServiceCollection();
        var fakeSender = new FakeTransactionalEmailSender(emailResult ?? TransactionalEmailResult.Accepted("email-123"));

        services.AddLogging();
        services.AddDataProtection();
        services.AddDbContext<VoidEmpiresDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.Configure<BrevoEmailOptions>(options =>
        {
            options.ConfirmationBaseUrl = "https://voidempires.example.test/confirm";
        });
        services.AddVoidEmpiresIdentity();
        services.AddSingleton(fakeSender);
        services.AddSingleton<ITransactionalEmailSender>(fakeSender);

        return services.BuildServiceProvider();
    }

    private sealed class FakeTransactionalEmailSender(TransactionalEmailResult result) : ITransactionalEmailSender
    {
        public List<TransactionalEmailMessage> Messages { get; } = [];

        public Task<TransactionalEmailResult> SendAsync(
            TransactionalEmailMessage message,
            CancellationToken cancellationToken = default)
        {
            Messages.Add(message);

            return Task.FromResult(result);
        }
    }
}
