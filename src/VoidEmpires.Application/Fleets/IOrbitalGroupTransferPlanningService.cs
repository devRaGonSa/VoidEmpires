namespace VoidEmpires.Application.Fleets;

public interface IOrbitalGroupTransferPlanningService
{
    Task<PlanOrbitalGroupTransferResult> PlanAsync(
        PlanOrbitalGroupTransferRequest request,
        CancellationToken cancellationToken = default);
}
