namespace VoidEmpires.Application.Fleets;

public interface IDevFleetUiStateService
{
    Task<GetDevFleetUiStateResult> GetAsync(
        GetDevFleetUiStateRequest request,
        CancellationToken cancellationToken = default);
}
