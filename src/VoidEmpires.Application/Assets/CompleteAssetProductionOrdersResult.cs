namespace VoidEmpires.Application.Assets;

public sealed record CompleteAssetProductionOrdersResult(
    int CompletedCount,
    IReadOnlyList<Guid> CompletedOrderIds);
