using Microsoft.AspNetCore.Identity;
using VoidEmpires.Application.Identity;

namespace VoidEmpires.Infrastructure.Identity;

public sealed class IdentityAccountRegistrationService(
    UserManager<VoidEmpiresUser> userManager) : IAccountRegistrationService
{
    public async Task<AccountRegistrationResult> RegisterAsync(
        AccountRegistrationRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = AccountRegistrationValidator.Validate(request);
        if (!validation.Succeeded || validation.NormalizedRequest is null)
        {
            return AccountRegistrationResult.Failure(validation.Errors.ToArray());
        }

        var normalized = validation.NormalizedRequest;
        if (await userManager.FindByEmailAsync(normalized.Email) is not null)
        {
            return AccountRegistrationResult.Failure(new AccountRegistrationError(
                "EmailAlreadyRegistered",
                "Email is already registered.",
                "email"));
        }

        var user = new VoidEmpiresUser
        {
            UserName = normalized.Email,
            Email = normalized.Email
        };
        var createResult = await userManager.CreateAsync(user, normalized.Password);

        return createResult.Succeeded
            ? new AccountRegistrationResult(true, user.Id, null, null, null, null, null, [])
            : AccountRegistrationResult.Failure(MapIdentityErrors(createResult.Errors));
    }

    private static AccountRegistrationError[] MapIdentityErrors(IEnumerable<IdentityError> errors) =>
        errors
            .Select(error => new AccountRegistrationError(
                string.IsNullOrWhiteSpace(error.Code) ? "IdentityRegistrationFailed" : error.Code,
                "Registration could not be completed.",
                null))
            .ToArray();
}
