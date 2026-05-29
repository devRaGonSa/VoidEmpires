namespace VoidEmpires.Application.Visuals;

public interface IPlanetVisualStateService
{
    Task<GetPlanetVisualStateResult> GetAsync(
        GetPlanetVisualStateRequest request,
        CancellationToken cancellationToken = default);
}
