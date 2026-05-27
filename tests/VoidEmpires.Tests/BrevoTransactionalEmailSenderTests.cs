using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using VoidEmpires.Application.Email;
using VoidEmpires.Infrastructure.Email;

namespace VoidEmpires.Tests;

public class BrevoTransactionalEmailSenderTests
{
    [Fact]
    public async Task DisabledBrevoSenderSkipsWithoutNetworkCall()
    {
        using var httpClient = new HttpClient(new ThrowingHandler());
        var sender = new BrevoTransactionalEmailSender(
            Options.Create(new BrevoEmailOptions { Enabled = false }),
            httpClient);

        var result = await sender.SendAsync(CreateMessage());

        Assert.Equal(TransactionalEmailDeliveryStatus.Skipped, result.Status);
        Assert.Contains("Brevo is disabled.", result.Errors);
    }

    [Fact]
    public async Task ConfiguredBrevoSenderPostsProviderRequest()
    {
        using var handler = new CapturingHandler();
        using var httpClient = new HttpClient(handler);
        var sender = new BrevoTransactionalEmailSender(
            Options.Create(new BrevoEmailOptions
            {
                Enabled = true,
                ApiBaseUrl = "https://api.brevo.test/v3",
                ApiKey = "test-api-key",
                SenderEmail = "sender@example.test",
                SenderName = "VoidEmpires"
            }),
            httpClient);

        var result = await sender.SendAsync(CreateMessage());

        Assert.Equal(TransactionalEmailDeliveryStatus.Accepted, result.Status);
        Assert.Equal("message-123", result.ProviderMessageId);
        Assert.Equal(HttpMethod.Post, handler.Request?.Method);
        Assert.Equal("https://api.brevo.test/v3/smtp/email", handler.Request?.RequestUri?.ToString());
        Assert.NotNull(handler.Request);
        Assert.True(handler.Request.Headers.TryGetValues("api-key", out var apiKeys));
        Assert.Equal("test-api-key", Assert.Single(apiKeys!));
        Assert.Contains("player@example.test", handler.Body);
        Assert.Contains("sender@example.test", handler.Body);
    }

    private static TransactionalEmailMessage CreateMessage() =>
        new(new TransactionalEmailRecipient("player@example.test", "Player"), "Confirm your account", "Use the confirmation link.");

    private sealed class ThrowingHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            throw new InvalidOperationException("Network calls are not allowed in this test.");
    }

    private sealed class CapturingHandler : HttpMessageHandler
    {
        public HttpRequestMessage? Request { get; private set; }
        public string Body { get; private set; } = string.Empty;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            Body = await request.Content!.ReadAsStringAsync(cancellationToken);

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(JsonSerializer.Serialize(new { messageId = "message-123" }))
            };
        }
    }
}
