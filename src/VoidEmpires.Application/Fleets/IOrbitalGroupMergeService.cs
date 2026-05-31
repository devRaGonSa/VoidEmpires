namespace VoidEmpires.Application.Fleets;

public interface IOrbitalGroupMergeService
{
    Task<MergeOrbitalGroupsResult> MergeAsync(
        MergeOrbitalGroupsRequest request,
        CancellationToken cancellationToken = default);
}
