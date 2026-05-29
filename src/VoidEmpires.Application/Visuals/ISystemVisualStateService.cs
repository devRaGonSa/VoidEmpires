namespace VoidEmpires.Application.Visuals;

public interface ISystemVisualStateService
{
    Task<GetSystemVisualStateResult> GetAsync(
        GetSystemVisualStateRequest request,
        CancellationToken cancellationToken = default);
}
