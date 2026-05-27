using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Identity;

namespace VoidEmpires.Tests;

public class AuthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task RegisterReturnsBadRequestForInvalidInput()
    {
        using var client = CreateClient(new FakeRegistrationService(), new FakeConfirmationService());

        using var response = await client.PostAsJsonAsync("/api/auth/register", new { email = "", password = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RegisterInvokesRegistrationServiceForValidInput()
    {
        var registration = new FakeRegistrationService();
        using var client = CreateClient(registration, new FakeConfirmationService());

        using var response = await client.PostAsJsonAsync(
            "/api/auth/register",
            new { email = "player@example.test", password = "P@ssw0rd!23" });
        var payload = await response.Content.ReadFromJsonAsync<AuthResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.Equal("user-123", payload.UserId);
        Assert.Equal("player@example.test", registration.Request?.Email);
    }

    [Fact]
    public async Task ConfirmEmailReturnsBadRequestForMissingInput()
    {
        using var client = CreateClient(new FakeRegistrationService(), new FakeConfirmationService());

        using var response = await client.GetAsync("/api/auth/confirm-email");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ConfirmEmailInvokesConfirmationServiceForValidInput()
    {
        var confirmation = new FakeConfirmationService();
        using var client = CreateClient(new FakeRegistrationService(), confirmation);

        using var response = await client.GetAsync("/api/auth/confirm-email?userId=user-123&token=test-token");
        var payload = await response.Content.ReadFromJsonAsync<AuthResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Succeeded);
        Assert.Equal("user-123", confirmation.Request?.UserId);
        Assert.Equal("test-token", confirmation.Request?.ConfirmationToken);
    }

    private HttpClient CreateClient(
        FakeRegistrationService registration,
        FakeConfirmationService confirmation)
    {
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IUserRegistrationService>(registration);
                services.AddSingleton<IEmailConfirmationService>(confirmation);
            });
        });

        return factory.CreateClient();
    }

    private sealed class FakeRegistrationService : IUserRegistrationService
    {
        public RegisterUserRequest? Request { get; private set; }

        public Task<RegisterUserResult> RegisterAsync(
            RegisterUserRequest request,
            CancellationToken cancellationToken = default)
        {
            Request = request;

            return Task.FromResult(RegisterUserResult.Success("user-123"));
        }
    }

    private sealed class FakeConfirmationService : IEmailConfirmationService
    {
        public ConfirmEmailRequest? Request { get; private set; }

        public Task<EmailConfirmationResult> ConfirmEmailAsync(
            ConfirmEmailRequest request,
            CancellationToken cancellationToken = default)
        {
            Request = request;

            return Task.FromResult(EmailConfirmationResult.Success());
        }
    }

    private sealed record AuthResponse(bool Succeeded, string? UserId, string[] Errors);
}
