namespace VoidEmpires.Application.Fleets;

public interface IOrbitalTransferLookupService
{
    Task<IReadOnlyList<OrbitalTransferQueryItem>> ListAsync(
        OrbitalTransferQueryRequest request,
        CancellationToken cancellationToken = default);
}
