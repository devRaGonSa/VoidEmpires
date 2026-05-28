using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VoidEmpires.Application.Research;

namespace VoidEmpires.Infrastructure.Research;

public sealed class ResearchProgressWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<ResearchQueueWorkerOptions> options,
    ILogger<ResearchProgressWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!options.Value.Enabled)
        {
            logger.LogInformation("Research progress worker is disabled.");
            return;
        }

        using var timer = new PeriodicTimer(options.Value.GetInterval());

        await RunAsync(stoppingToken);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await RunAsync(stoppingToken);
        }
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<IResearchOrderCompletionService>();
        var result = await service.CompleteDueOrdersAsync(DateTime.UtcNow, cancellationToken);

        if (result.CompletedCount > 0)
        {
            logger.LogInformation("Processed {CompletedCount} research items.", result.CompletedCount);
        }
    }
}
