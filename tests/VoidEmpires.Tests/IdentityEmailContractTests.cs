using VoidEmpires.Application.Email;
using VoidEmpires.Application.Identity;

namespace VoidEmpires.Tests;

public class IdentityEmailContractTests
{
    [Fact]
    public void RegisterUserResultSuccessCapturesUserIdWithoutErrors()
    {
        var result = RegisterUserResult.Success("user-123");

        Assert.True(result.Succeeded);
        Assert.Equal("user-123", result.UserId);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void RegisterUserResultFailureCapturesDeterministicErrors()
    {
        var result = RegisterUserResult.Failure("Email is required.", "Password is required.");

        Assert.False(result.Succeeded);
        Assert.Null(result.UserId);
        Assert.Equal(["Email is required.", "Password is required."], result.Errors);
    }

    [Fact]
    public void EmailConfirmationResultFailureCapturesErrors()
    {
        var result = EmailConfirmationResult.Failure("Invalid confirmation token.");

        Assert.False(result.Succeeded);
        Assert.Equal(["Invalid confirmation token."], result.Errors);
    }

    [Theory]
    [InlineData(TransactionalEmailDeliveryStatus.Accepted, true)]
    [InlineData(TransactionalEmailDeliveryStatus.Skipped, true)]
    [InlineData(TransactionalEmailDeliveryStatus.Invalid, false)]
    [InlineData(TransactionalEmailDeliveryStatus.Failed, false)]
    public void TransactionalEmailResultSucceededReflectsDeliveryStatus(
        TransactionalEmailDeliveryStatus status,
        bool expectedSucceeded)
    {
        var result = new TransactionalEmailResult(status, null, []);

        Assert.Equal(expectedSucceeded, result.Succeeded);
    }

    [Fact]
    public void TransactionalEmailMessageUsesProviderAgnosticShape()
    {
        var recipient = new TransactionalEmailRecipient("player@example.test", "Player");
        var message = new TransactionalEmailMessage(
            recipient,
            "Confirm your account",
            "Use the confirmation link.",
            ActionUrl: new Uri("https://voidempires.example.test/confirm"));

        Assert.Equal("player@example.test", message.Recipient.Email);
        Assert.Equal("Confirm your account", message.Subject);
        Assert.Null(message.HtmlBody);
        Assert.Equal("https://voidempires.example.test/confirm", message.ActionUrl?.ToString());
    }
}
