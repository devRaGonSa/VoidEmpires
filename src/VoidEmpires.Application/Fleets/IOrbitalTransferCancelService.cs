namespace VoidEmpires.Application.Fleets;

public interface IOrbitalTransferCancelService
{
    Task<CancelOrbitalTransferResult> CancelAsync(
        CancelOrbitalTransferRequest request,
        CancellationToken cancellationToken = default);
}
