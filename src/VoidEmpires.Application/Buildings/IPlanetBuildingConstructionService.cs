namespace VoidEmpires.Application.Buildings;

public interface IPlanetBuildingConstructionService
{
    Task<ConstructBuildingResult> ConstructAsync(
        ConstructBuildingRequest request,
        CancellationToken cancellationToken = default);
}
