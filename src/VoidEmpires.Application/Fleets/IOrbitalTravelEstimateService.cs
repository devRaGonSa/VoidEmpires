namespace VoidEmpires.Application.Fleets;

public interface IOrbitalTravelEstimateService
{
    Task<EstimateOrbitalTravelResult> EstimateAsync(
        EstimateOrbitalTravelRequest request,
        CancellationToken cancellationToken = default);
}
