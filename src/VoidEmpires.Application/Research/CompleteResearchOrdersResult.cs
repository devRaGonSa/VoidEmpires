namespace VoidEmpires.Application.Research;

public sealed record CompleteResearchOrdersResult(
    int CompletedCount,
    IReadOnlyList<Guid> CompletedOrderIds);
