using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VoidEmpires.Application.Buildings;

namespace VoidEmpires.Infrastructure.Buildings;

public sealed class ConstructionQueueWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<ConstructionQueueWorkerOptions> options,
    ILogger<ConstructionQueueWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!options.Value.Enabled)
        {
            logger.LogInformation("Construction queue worker is disabled.");
            return;
        }

        using var timer = new PeriodicTimer(options.Value.GetInterval());

        await CompleteDueOrdersAsync(stoppingToken);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await CompleteDueOrdersAsync(stoppingToken);
        }
    }

    private async Task CompleteDueOrdersAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<IConstructionOrderCompletionService>();
        var result = await service.CompleteDueOrdersAsync(DateTime.UtcNow, cancellationToken);

        if (result.CompletedCount > 0)
        {
            logger.LogInformation("Completed {CompletedCount} construction orders.", result.CompletedCount);
        }
    }
}
