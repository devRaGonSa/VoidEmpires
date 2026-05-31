namespace VoidEmpires.Application.Fleets;

public interface IOrbitalGroupSplitService
{
    Task<SplitOrbitalGroupResult> SplitAsync(
        SplitOrbitalGroupRequest request,
        CancellationToken cancellationToken = default);
}
