namespace VoidEmpires.Application.Fleets;

public interface IOrbitalGroupService
{
    Task<CreateOrbitalGroupResult> CreateFromLocalStockAsync(
        CreateOrbitalGroupRequest request,
        CancellationToken cancellationToken = default);
}
