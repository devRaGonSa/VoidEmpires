namespace VoidEmpires.Application.Colonization;

public interface IPlanetColonizationService
{
    Task<ColonizePlanetResult> ColonizeAsync(
        ColonizePlanetRequest request,
        CancellationToken cancellationToken = default);
}
