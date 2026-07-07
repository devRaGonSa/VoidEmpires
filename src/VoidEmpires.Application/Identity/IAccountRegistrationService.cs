namespace VoidEmpires.Application.Identity;

public interface IAccountRegistrationService
{
    Task<AccountRegistrationResult> RegisterAsync(
        AccountRegistrationRequest request,
        CancellationToken cancellationToken = default);
}
