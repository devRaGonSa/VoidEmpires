namespace VoidEmpires.Application.Players;

public interface IStartingCivilizationService
{
    Task<CreateStartingCivilizationResult> CreateAsync(
        CreateStartingCivilizationRequest request,
        CancellationToken cancellationToken = default);
}
