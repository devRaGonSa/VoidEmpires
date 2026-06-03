using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Domain.Players;
using VoidEmpires.Domain.Research;
using VoidEmpires.Infrastructure.Persistence;

internal static class DevResearchUiStateEndpoints
{
    public static void MapDevResearchUiStateEndpoints(this WebApplication app)
    {
        app.MapGet("/api/dev/research/ui-state", async (
            Guid? civilizationId,
            Guid? planetId,
            [FromServices] VoidEmpiresDbContext dbContext,
            [FromServices] IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection")))
            {
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (civilizationId is null || civilizationId == Guid.Empty)
            {
                return Results.BadRequest(new DevResearchUiStateApiResponse(false, null, ["Civilization id is required."]));
            }

            var civilization = await dbContext.Set<Civilization>()
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == civilizationId.Value, cancellationToken);

            if (civilization is null)
            {
                return Results.NotFound(new DevResearchUiStateApiResponse(false, null, ["Civilization was not found."]));
            }

            var ownedPlanetQuery = from planet in dbContext.Set<Planet>().AsNoTracking()
                                   join ownership in dbContext.Set<PlanetOwnership>().AsNoTracking() on planet.Id equals ownership.PlanetId
                                   where ownership.CivilizationId == civilizationId.Value && ownership.Status == PlanetControlStatus.Active
                                   orderby planet.Id == civilization.HomePlanetId descending, planet.Name, planet.OrbitalSlot
                                   select new DevResearchPlanetDto(planet.Id, planet.Name);

            DevResearchPlanetDto? selectedPlanet;
            if (planetId is not null)
            {
                selectedPlanet = await (from planet in dbContext.Set<Planet>().AsNoTracking()
                                        join ownership in dbContext.Set<PlanetOwnership>().AsNoTracking() on planet.Id equals ownership.PlanetId
                                        where planet.Id == planetId.Value &&
                                            ownership.CivilizationId == civilizationId.Value &&
                                            ownership.Status == PlanetControlStatus.Active
                                        select new DevResearchPlanetDto(planet.Id, planet.Name))
                    .SingleOrDefaultAsync(cancellationToken);

                if (selectedPlanet is null)
                {
                    return Results.NotFound(new DevResearchUiStateApiResponse(false, null, ["Planet was not found."]));
                }
            }
            else
            {
                selectedPlanet = await ownedPlanetQuery.FirstOrDefaultAsync(cancellationToken);
            }

            var queue = await dbContext.ResearchOrders
                .AsNoTracking()
                .Where(x => x.CivilizationId == civilizationId.Value)
                .OrderBy(x => x.Status == ResearchQueueItemStatus.Pending || x.Status == ResearchQueueItemStatus.Active ? 0 : 1)
                .ThenBy(x => x.Sequence)
                .ToListAsync(cancellationToken);

            var projects = await dbContext.ResearchProjects
                .AsNoTracking()
                .Where(x => x.CivilizationId == civilizationId.Value)
                .OrderBy(x => x.ResearchType)
                .ToListAsync(cancellationToken);

            var projectLevels = projects.ToDictionary(x => x.ResearchType, x => x.Level);
            var energySystemsLevel = projectLevels.GetValueOrDefault(ResearchType.EnergySystems);
            var stockpile = selectedPlanet is null
                ? null
                : await dbContext.PlanetResourceStockpiles.AsNoTracking().SingleOrDefaultAsync(x => x.PlanetId == selectedPlanet.Id, cancellationToken);

            var queueDtos = queue.Select(x => new DevResearchOrderDto(
                x.Id,
                x.CivilizationId,
                x.SourcePlanetId,
                x.ResearchType,
                x.TargetLevel,
                x.Sequence,
                x.StartsAtUtc,
                x.EndsAtUtc,
                x.Status)).ToArray();

            var projectDtos = projects.Select(x => new DevResearchProjectDto(x.CivilizationId, x.ResearchType, x.Level)).ToArray();

            var technologyHints = Enum.GetValues<ResearchType>()
                .Select(researchType =>
                {
                    var definition = ResearchCatalog.Get(researchType);
                    var currentLevel = projectLevels.GetValueOrDefault(researchType);
                    var nextLevel = currentLevel + 1;
                    var cost = ScaleCost(definition.BaseCost, nextLevel);
                    var estimatedDuration = ResearchDurationCalculator.CalculateDuration(TimeSpan.FromMinutes(10 * nextLevel), energySystemsLevel);
                    var openOrder = queueDtos.FirstOrDefault(x => x.ResearchType == researchType && x.Status is ResearchQueueItemStatus.Pending or ResearchQueueItemStatus.Active);
                    var canAfford = stockpile is not null && stockpile.CanSpend(cost.Credits, cost.Metal, cost.Crystal, cost.Gas);
                    var status = openOrder is not null
                        ? "InResearch"
                        : selectedPlanet is null
                            ? "Blocked"
                            : stockpile is null
                                ? "RequirementPending"
                                : canAfford
                                    ? "Available"
                                    : "InsufficientResources";

                    return new DevResearchTechnologyHintDto(
                        researchType,
                        currentLevel,
                        nextLevel,
                        status,
                        openOrder is not null ? "OpenQueueSlot" : canAfford ? "Ready" : selectedPlanet is null ? "SourcePlanetMissing" : "InsufficientResources",
                        selectedPlanet is not null && openOrder is null && canAfford,
                        openOrder is not null && openOrder.EndsAtUtc <= DateTime.UtcNow,
                        estimatedDuration,
                        cost,
                        selectedPlanet is not null
                            ? new DevResearchEnqueueCommandDto(
                                "research.order.enqueue",
                                "POST",
                                "/api/dev/research/orders/enqueue",
                                civilizationId.Value,
                                selectedPlanet.Id,
                                researchType,
                                nextLevel)
                            : null,
                        ["SourcePlanet", "ResearchQueueSlot", "ResourceStockpile"]);
                })
                .ToArray();

            return Results.Ok(new DevResearchUiStateApiResponse(
                true,
                new DevResearchUiStateResult(
                    civilizationId.Value,
                    selectedPlanet?.Id,
                    selectedPlanet?.Name,
                    Enum.GetValues<ResearchType>().Select(ResearchCatalog.Get).ToArray(),
                    queueDtos,
                    projectDtos,
                    technologyHints,
                    [
                        $"Catalog entries: {technologyHints.Length}.",
                        $"Open queue items: {queueDtos.Count(x => x.Status is ResearchQueueItemStatus.Pending or ResearchQueueItemStatus.Active)}.",
                        $"Completed research projects: {projectDtos.Length}.",
                    ],
                    [
                        "Read-only dev research cockpit state.",
                        "No research effects are executed by this endpoint.",
                        "Only source planet, queue slot, and stockpile readiness are derived here.",
                    ]),
                []));
        });
    }

    private static ResearchCost ScaleCost(ResearchCost baseCost, int level) =>
        new(baseCost.Credits * level, baseCost.Metal * level, baseCost.Crystal * level, baseCost.Gas * level);
}

internal sealed record DevResearchUiStateApiResponse(
    bool Succeeded,
    DevResearchUiStateResult? UiState,
    IReadOnlyList<string> Errors);

internal sealed record DevResearchUiStateResult(
    Guid CivilizationId,
    Guid? SelectedPlanetId,
    string? SelectedPlanetName,
    IReadOnlyList<ResearchDefinition> Catalog,
    IReadOnlyList<DevResearchOrderDto> Queue,
    IReadOnlyList<DevResearchProjectDto> Projects,
    IReadOnlyList<DevResearchTechnologyHintDto> TechnologyHints,
    IReadOnlyList<string> Diagnostics,
    IReadOnlyList<string> Limitations);

internal sealed record DevResearchPlanetDto(Guid Id, string Name);

internal sealed record DevResearchOrderDto(
    Guid Id,
    Guid CivilizationId,
    Guid SourcePlanetId,
    ResearchType ResearchType,
    int TargetLevel,
    int Sequence,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc,
    ResearchQueueItemStatus Status);

internal sealed record DevResearchProjectDto(
    Guid CivilizationId,
    ResearchType ResearchType,
    int Level);

internal sealed record DevResearchTechnologyHintDto(
    ResearchType ResearchType,
    int CurrentLevel,
    int NextLevel,
    string StatusKey,
    string AvailabilityReasonKey,
    bool CanEnqueue,
    bool CanCompleteDue,
    TimeSpan EstimatedDuration,
    ResearchCost EstimatedCost,
    DevResearchEnqueueCommandDto? EnqueueCommand,
    IReadOnlyList<string> RequirementKeys);

internal sealed record DevResearchEnqueueCommandDto(
    string ActionKey,
    string Method,
    string Route,
    Guid CivilizationId,
    Guid SourcePlanetId,
    ResearchType ResearchType,
    int TargetLevel);
