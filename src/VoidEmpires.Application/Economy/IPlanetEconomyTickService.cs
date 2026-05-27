namespace VoidEmpires.Application.Economy;

public interface IPlanetEconomyTickService
{
    Task<ApplyPlanetProductionResult> ApplyProductionAsync(
        ApplyPlanetProductionRequest request,
        CancellationToken cancellationToken = default);
}
