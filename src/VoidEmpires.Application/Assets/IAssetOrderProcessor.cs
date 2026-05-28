namespace VoidEmpires.Application.Assets;

public interface IAssetOrderProcessor
{
    Task<CompleteAssetProductionOrdersResult> ProcessAsync(
        DateTime nowUtc,
        CancellationToken cancellationToken = default);
}
