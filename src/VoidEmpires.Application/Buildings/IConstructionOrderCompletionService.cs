namespace VoidEmpires.Application.Buildings;

public interface IConstructionOrderCompletionService
{
    Task<CompleteConstructionOrdersResult> CompleteDueOrdersAsync(
        DateTime nowUtc,
        CancellationToken cancellationToken = default);
}
