using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using VoidEmpires.Application.Email;
using VoidEmpires.Application.Identity;
using VoidEmpires.Infrastructure.Email;

namespace VoidEmpires.Infrastructure.Identity;

public sealed class IdentityAccountService(
    UserManager<VoidEmpiresUser> userManager,
    ITransactionalEmailSender emailSender,
    IOptions<BrevoEmailOptions> emailOptions) : IUserRegistrationService, IEmailConfirmationService
{
    public async Task<RegisterUserResult> RegisterAsync(
        RegisterUserRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return RegisterUserResult.Failure("Email and password are required.");
        }

        var user = new VoidEmpiresUser
        {
            UserName = request.Email,
            Email = request.Email
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            return RegisterUserResult.Failure(createResult.Errors.Select(error => error.Description).ToArray());
        }

        var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationUrl = BuildConfirmationUrl(user.Id, confirmationToken);
        var emailResult = await emailSender.SendAsync(
            new TransactionalEmailMessage(
                new TransactionalEmailRecipient(request.Email),
                "Confirm your VoidEmpires account",
                $"Confirm your VoidEmpires account: {confirmationUrl}",
                ActionUrl: confirmationUrl),
            cancellationToken);

        return emailResult.Succeeded
            ? RegisterUserResult.Success(user.Id)
            : RegisterUserResult.Failure(emailResult.Errors.ToArray());
    }

    public async Task<EmailConfirmationResult> ConfirmEmailAsync(
        ConfirmEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.ConfirmationToken))
        {
            return EmailConfirmationResult.Failure("User id and confirmation token are required.");
        }

        var user = await userManager.FindByIdAsync(request.UserId);
        if (user is null)
        {
            return EmailConfirmationResult.Failure("User was not found.");
        }

        var result = await userManager.ConfirmEmailAsync(user, request.ConfirmationToken);

        return result.Succeeded
            ? EmailConfirmationResult.Success()
            : EmailConfirmationResult.Failure(result.Errors.Select(error => error.Description).ToArray());
    }

    private Uri BuildConfirmationUrl(string userId, string token)
    {
        var configuredBaseUrl = emailOptions.Value.ConfirmationBaseUrl;
        var baseUrl = string.IsNullOrWhiteSpace(configuredBaseUrl)
            ? "https://voidempires.local/api/auth/confirm-email"
            : configuredBaseUrl;
        var separator = baseUrl.Contains('?') ? '&' : '?';

        return new Uri($"{baseUrl}{separator}userId={Uri.EscapeDataString(userId)}&token={Uri.EscapeDataString(token)}");
    }
}
