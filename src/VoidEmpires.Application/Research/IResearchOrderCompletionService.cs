namespace VoidEmpires.Application.Research;

public interface IResearchOrderCompletionService
{
    Task<CompleteResearchOrdersResult> CompleteDueOrdersAsync(
        DateTime nowUtc,
        CancellationToken cancellationToken = default);
}
