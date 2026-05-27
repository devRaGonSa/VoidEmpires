namespace VoidEmpires.Application.Buildings;

public interface IPlanetBuildingUpgradeService
{
    Task<UpgradeBuildingResult> UpgradeAsync(
        UpgradeBuildingRequest request,
        CancellationToken cancellationToken = default);
}
