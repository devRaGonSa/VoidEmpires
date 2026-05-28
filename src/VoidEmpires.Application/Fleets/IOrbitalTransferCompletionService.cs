namespace VoidEmpires.Application.Fleets;

public interface IOrbitalTransferCompletionService
{
    Task<CompleteOrbitalTransfersResult> CompleteDueAsync(
        DateTime nowUtc,
        CancellationToken cancellationToken = default);
}
