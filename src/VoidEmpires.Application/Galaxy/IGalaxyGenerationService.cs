namespace VoidEmpires.Application.Galaxy;

public interface IGalaxyGenerationService
{
    Task<GenerateAndPersistGalaxyResult> GenerateAndPersistAsync(
        GenerateAndPersistGalaxyRequest request,
        CancellationToken cancellationToken = default);
}
