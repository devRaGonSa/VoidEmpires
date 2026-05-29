using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VoidEmpires.Application.Fleets;

namespace VoidEmpires.Infrastructure.Fleets;

public sealed class OrbitalTransferWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<OrbitalTransferWorkerOptions> options,
    ILogger<OrbitalTransferWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!options.Value.Enabled)
        {
            logger.LogInformation("Orbital transfer worker is disabled.");
            return;
        }

        using var timer = new PeriodicTimer(options.Value.GetInterval());

        await CompleteDueTransfersAsync(stoppingToken);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await CompleteDueTransfersAsync(stoppingToken);
        }
    }

    private async Task CompleteDueTransfersAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<IOrbitalTransferCompletionService>();
        var result = await service.CompleteDueAsync(DateTime.UtcNow, cancellationToken);

        if (result.CompletedCount > 0)
        {
            logger.LogInformation("Completed {CompletedCount} orbital transfers.", result.CompletedCount);
        }
    }
}
