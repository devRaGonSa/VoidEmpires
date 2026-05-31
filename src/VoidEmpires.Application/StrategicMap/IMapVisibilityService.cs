namespace VoidEmpires.Application.StrategicMap;

public interface IMapVisibilityService
{
    Task<GetMapVisibilityResult> GetAsync(
        GetMapVisibilityRequest request,
        CancellationToken cancellationToken = default);
}
