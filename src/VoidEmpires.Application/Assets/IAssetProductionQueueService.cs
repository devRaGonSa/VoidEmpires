namespace VoidEmpires.Application.Assets;

public interface IAssetProductionQueueService
{
    Task<EnqueueAssetProductionResult> EnqueueAsync(
        EnqueueAssetProductionRequest request,
        CancellationToken cancellationToken = default);
}
