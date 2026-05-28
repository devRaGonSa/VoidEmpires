namespace VoidEmpires.Application.Fleets;

public interface IOrbitalGroupLookupService
{
    Task<IReadOnlyList<OrbitalGroupQueryItem>> ListAsync(
        OrbitalGroupQueryRequest request,
        CancellationToken cancellationToken = default);
}
