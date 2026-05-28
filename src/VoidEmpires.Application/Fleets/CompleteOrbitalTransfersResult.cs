namespace VoidEmpires.Application.Fleets;

public sealed record CompleteOrbitalTransfersResult(
    int CompletedCount,
    IReadOnlyList<Guid> CompletedTransferIds,
    IReadOnlyList<Guid> CompletedOrbitalGroupIds);
