using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VoidEmpires.Application.Assets;

namespace VoidEmpires.Infrastructure.Assets;

public sealed class AssetProductionWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<AssetProductionWorkerOptions> options,
    ILogger<AssetProductionWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!options.Value.Enabled)
        {
            logger.LogInformation("Asset production worker is disabled.");
            return;
        }

        using var timer = new PeriodicTimer(options.Value.GetInterval());

        await ProcessAsync(stoppingToken);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await ProcessAsync(stoppingToken);
        }
    }

    private async Task ProcessAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<IAssetOrderProcessor>();
        var result = await service.ProcessAsync(DateTime.UtcNow, cancellationToken);

        if (result.CompletedCount > 0)
        {
            logger.LogInformation("Processed {CompletedCount} asset production items.", result.CompletedCount);
        }
    }
}
