namespace VoidEmpires.Application.Identity
{
    public sealed record RegisterUserRequest(string Email, string Password);

    public sealed record RegisterUserResult(
        bool Succeeded,
        string? UserId,
        IReadOnlyList<string> Errors)
    {
        public static RegisterUserResult Success(string userId) => new(true, userId, []);

        public static RegisterUserResult Failure(params string[] errors) => new(false, null, errors);
    }

    public interface IUserRegistrationService
    {
        Task<RegisterUserResult> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default);
    }

    public sealed record ConfirmEmailRequest(string UserId, string ConfirmationToken);

    public sealed record EmailConfirmationResult(
        bool Succeeded,
        IReadOnlyList<string> Errors)
    {
        public static EmailConfirmationResult Success() => new(true, []);

        public static EmailConfirmationResult Failure(params string[] errors) => new(false, errors);
    }

    public interface IEmailConfirmationService
    {
        Task<EmailConfirmationResult> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken = default);
    }
}

namespace VoidEmpires.Application.Email
{
    public sealed record TransactionalEmailRecipient(string Email, string? DisplayName = null);

    public sealed record TransactionalEmailMessage(
        TransactionalEmailRecipient Recipient,
        string Subject,
        string TextBody,
        string? HtmlBody = null,
        Uri? ActionUrl = null);

    public enum TransactionalEmailDeliveryStatus
    {
        Accepted,
        Skipped,
        Invalid,
        Failed
    }

    public sealed record TransactionalEmailResult(
        TransactionalEmailDeliveryStatus Status,
        string? ProviderMessageId,
        IReadOnlyList<string> Errors)
    {
        public bool Succeeded => Status is TransactionalEmailDeliveryStatus.Accepted or TransactionalEmailDeliveryStatus.Skipped;

        public static TransactionalEmailResult Accepted(string? providerMessageId = null) =>
            new(TransactionalEmailDeliveryStatus.Accepted, providerMessageId, []);

        public static TransactionalEmailResult Skipped(params string[] reasons) =>
            new(TransactionalEmailDeliveryStatus.Skipped, null, reasons);

        public static TransactionalEmailResult Invalid(params string[] errors) =>
            new(TransactionalEmailDeliveryStatus.Invalid, null, errors);

        public static TransactionalEmailResult Failed(params string[] errors) =>
            new(TransactionalEmailDeliveryStatus.Failed, null, errors);
    }

    public interface ITransactionalEmailSender
    {
        Task<TransactionalEmailResult> SendAsync(TransactionalEmailMessage message, CancellationToken cancellationToken = default);
    }
}
