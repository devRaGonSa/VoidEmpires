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

        if (!TryBuildConfirmationUrl("configuration-check", "configuration-check", out _, out var configurationError))
        {
            return RegisterUserResult.Failure(configurationError);
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
        if (!TryBuildConfirmationUrl(user.Id, confirmationToken, out var confirmationUrl, out var urlError))
        {
            return RegisterUserResult.Failure(urlError);
        }

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

    private bool TryBuildConfirmationUrl(string userId, string token, out Uri? confirmationUrl, out string error)
    {
        confirmationUrl = null;
        error = string.Empty;

        var configuredBaseUrl = emailOptions.Value.ConfirmationBaseUrl;
        if (string.IsNullOrWhiteSpace(configuredBaseUrl))
        {
            error = "Email confirmation base URL is not configured.";
            return false;
        }

        if (!Uri.TryCreate(configuredBaseUrl, UriKind.Absolute, out var baseUri))
        {
            error = "Email confirmation base URL is invalid.";
            return false;
        }

        var baseUrl = baseUri.ToString();
        var separator = baseUrl.Contains('?') ? '&' : '?';
        confirmationUrl = new Uri($"{baseUrl}{separator}userId={Uri.EscapeDataString(userId)}&token={Uri.EscapeDataString(token)}");
        return true;
    }
}
