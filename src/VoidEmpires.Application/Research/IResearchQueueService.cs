namespace VoidEmpires.Application.Research;

public interface IResearchQueueService
{
    Task<EnqueueResearchOrderResult> EnqueueAsync(
        EnqueueResearchOrderRequest request,
        CancellationToken cancellationToken = default);
}
