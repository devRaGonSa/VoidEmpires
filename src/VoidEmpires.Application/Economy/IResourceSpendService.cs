namespace VoidEmpires.Application.Economy;

public interface IResourceSpendService
{
    Task<ResourceSpendResult> CheckAffordabilityAsync(
        ResourceSpendRequest request,
        CancellationToken cancellationToken = default);

    Task<ResourceSpendResult> SpendAsync(
        ResourceSpendRequest request,
        CancellationToken cancellationToken = default);
}
