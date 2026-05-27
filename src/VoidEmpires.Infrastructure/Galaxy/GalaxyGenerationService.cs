using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Galaxy;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.GalaxyGeneration;

public sealed class GalaxyGenerationService : IGalaxyGenerationService
{
    private readonly IGalaxyGenerator _galaxyGenerator;
    private readonly VoidEmpiresDbContext _dbContext;

    public GalaxyGenerationService(
        IGalaxyGenerator galaxyGenerator,
        VoidEmpiresDbContext dbContext)
    {
        _galaxyGenerator = galaxyGenerator;
        _dbContext = dbContext;
    }

    public async Task<GenerateAndPersistGalaxyResult> GenerateAndPersistAsync(
        GenerateAndPersistGalaxyRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existingGalaxy = await _dbContext.Galaxies
            .FirstOrDefaultAsync(galaxy => galaxy.Name == request.Name, cancellationToken);

        if (existingGalaxy is not null)
        {
            if (!request.OverwriteExisting)
            {
                return GenerateAndPersistGalaxyResult.Failure($"Galaxy '{request.Name}' already exists.");
            }

            _dbContext.Galaxies.Remove(existingGalaxy);
        }

        GeneratedGalaxyResult generatedGalaxy;

        try
        {
            generatedGalaxy = _galaxyGenerator.Generate(new GenerateGalaxyRequest(
                request.Name,
                request.Seed,
                request.SolarSystemCount,
                request.MinPlanetsPerSystem,
                request.MaxPlanetsPerSystem));
        }
        catch (ArgumentException)
        {
            return GenerateAndPersistGalaxyResult.Failure("Galaxy generation request is invalid.");
        }
        catch (InvalidOperationException)
        {
            return GenerateAndPersistGalaxyResult.Failure("Galaxy generation failed.");
        }

        _dbContext.Galaxies.Add(generatedGalaxy.Galaxy);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return GenerateAndPersistGalaxyResult.Success(
            generatedGalaxy.Galaxy.Id,
            generatedGalaxy.Galaxy.Name,
            generatedGalaxy.SolarSystemCount,
            generatedGalaxy.PlanetCount);
    }
}
