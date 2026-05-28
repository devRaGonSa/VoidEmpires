namespace VoidEmpires.Application.Fleets;

public interface IOrbitalTransferPersistenceService
{
    Task<PersistOrbitalTransferResult> PersistAsync(
        PersistOrbitalTransferRequest request,
        CancellationToken cancellationToken = default);
}
