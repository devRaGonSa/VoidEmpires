using System.Net.Http.Json;
using System.Text.Json;
using VoidEmpires.Application.Email;

namespace VoidEmpires.Infrastructure.Email;

public sealed class BrevoEmailOptions
{
    public const string SectionName = "Brevo";

    public bool Enabled { get; set; }
    public string ApiBaseUrl { get; set; } = "https://api.brevo.com/v3";
    public string ApiKey { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = "VoidEmpires";
    public string ConfirmationBaseUrl { get; set; } = string.Empty;
}

public sealed class BrevoTransactionalEmailSender(
    Microsoft.Extensions.Options.IOptions<BrevoEmailOptions> options,
    HttpClient httpClient) : ITransactionalEmailSender
{
    private readonly BrevoEmailOptions _options = options.Value;

    public async Task<TransactionalEmailResult> SendAsync(
        TransactionalEmailMessage message,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return TransactionalEmailResult.Skipped("Brevo is disabled.");
        }

        if (string.IsNullOrWhiteSpace(_options.ApiKey) || string.IsNullOrWhiteSpace(_options.SenderEmail))
        {
            return TransactionalEmailResult.Skipped("Brevo is not configured.");
        }

        if (string.IsNullOrWhiteSpace(message.Recipient.Email))
            return TransactionalEmailResult.Invalid("Recipient email is required.");

        if (string.IsNullOrWhiteSpace(message.Subject) || string.IsNullOrWhiteSpace(message.TextBody))
            return TransactionalEmailResult.Invalid("Email subject and text body are required.");

        if (!Uri.TryCreate($"{_options.ApiBaseUrl.TrimEnd('/')}/smtp/email", UriKind.Absolute, out var requestUri))
        {
            return TransactionalEmailResult.Invalid("Brevo API base URL is invalid.");
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
        request.Headers.Add("api-key", _options.ApiKey);
        request.Content = JsonContent.Create(new
        {
            sender = new
            {
                email = _options.SenderEmail,
                name = _options.SenderName
            },
            to = new[]
            {
                new
                {
                    email = message.Recipient.Email,
                    name = message.Recipient.DisplayName
                }
            },
            subject = message.Subject,
            htmlContent = message.HtmlBody,
            textContent = message.TextBody
        });

        using var response = await httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return TransactionalEmailResult.Failed($"Brevo request failed with status code {(int)response.StatusCode}.");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var messageId = TryReadMessageId(content);

        return TransactionalEmailResult.Accepted(messageId);
    }

    private static string? TryReadMessageId(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return null;

        using var document = JsonDocument.Parse(content);

        return document.RootElement.TryGetProperty("messageId", out var messageId)
            ? messageId.GetString()
            : null;
    }
}
