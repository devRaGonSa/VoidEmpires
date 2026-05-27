using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Research;
using VoidEmpires.Domain.Research;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Research;

public sealed class ResearchUpgradeService(VoidEmpiresDbContext dbContext) : IResearchUpgradeService
{
    public async Task<UpgradeResearchResult> UpgradeAsync(
        UpgradeResearchRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CivilizationId == Guid.Empty)
        {
            return UpgradeResearchResult.Failure("Civilization id is required.");
        }

        if (request.SourcePlanetId == Guid.Empty)
        {
            return UpgradeResearchResult.Failure("Source planet id is required.");
        }

        var stockpile = await dbContext.PlanetResourceStockpiles
            .SingleOrDefaultAsync(x => x.PlanetId == request.SourcePlanetId, cancellationToken);

        if (stockpile is null)
        {
            return UpgradeResearchResult.Failure("Planet resource stockpile was not found.");
        }

        var project = await dbContext.ResearchProjects
            .SingleOrDefaultAsync(
                x => x.CivilizationId == request.CivilizationId && x.ResearchType == request.ResearchType,
                cancellationToken);

        var currentLevel = project?.Level ?? 0;
        var nextLevel = currentLevel + 1;
        var definition = ResearchCatalog.Get(request.ResearchType);

        var credits = definition.BaseCost.Credits * nextLevel;
        var metal = definition.BaseCost.Metal * nextLevel;
        var crystal = definition.BaseCost.Crystal * nextLevel;
        var gas = definition.BaseCost.Gas * nextLevel;

        if (!stockpile.CanSpend(credits, metal, crystal, gas))
        {
            return UpgradeResearchResult.Failure("Insufficient resources.");
        }

        if (project is null)
        {
            project = ResearchProject.Create(request.CivilizationId, request.ResearchType);
            dbContext.ResearchProjects.Add(project);
        }
        else
        {
            project.Upgrade();
        }

        stockpile.Spend(credits, metal, crystal, gas);
        await dbContext.SaveChangesAsync(cancellationToken);

        return UpgradeResearchResult.Success(project.Level);
    }
}
