namespace VoidEmpires.Application.Fleets;

public interface IFleetOperationalOverviewService
{
    Task<GetFleetOperationalOverviewResult> GetAsync(
        GetFleetOperationalOverviewRequest request,
        CancellationToken cancellationToken = default);
}
