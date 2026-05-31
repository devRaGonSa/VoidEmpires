namespace VoidEmpires.Application.StrategicMap;

public interface IStrategicMapService
{
    Task<GetStrategicMapResult> GetAsync(
        GetStrategicMapRequest request,
        CancellationToken cancellationToken = default);
}
