namespace VoidEmpires.Application.Buildings;

public interface IPlanetConstructionQueueService
{
    Task<EnqueueConstructionOrderResult> EnqueueAsync(
        EnqueueConstructionOrderRequest request,
        CancellationToken cancellationToken = default);
}
