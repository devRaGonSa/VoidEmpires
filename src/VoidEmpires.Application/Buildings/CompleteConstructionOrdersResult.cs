namespace VoidEmpires.Application.Buildings;

public sealed record CompleteConstructionOrdersResult(
    int CompletedCount,
    IReadOnlyList<Guid> CompletedOrderIds);
