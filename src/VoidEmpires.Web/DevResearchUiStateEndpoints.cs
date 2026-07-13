using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Gameplay;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Domain.Players;
using VoidEmpires.Domain.Research;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.Research;

internal static class DevResearchUiStateEndpoints
{
    public static void MapDevResearchUiStateEndpoints(this WebApplication app)
    {
        app.MapGet("/api/dev/research/ui-state", async (
            Guid? civilizationId,
            Guid? planetId,
            [FromServices] IServiceProvider services,
            [FromServices] IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection")))
            {
                return Results.Json(
                    new DevResearchUiStateApiResponse(false, null, ["Persistence is not configured."]),
                    statusCode: StatusCodes.Status503ServiceUnavailable);
            }

            if (civilizationId is null || civilizationId == Guid.Empty)
            {
                return Results.BadRequest(new DevResearchUiStateApiResponse(false, null, ["Civilization id is required."]));
            }

            var dbContext = services.GetRequiredService<VoidEmpiresDbContext>();
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

            if (selectedPlanet is not null)
            {
                try
                {
                    var refreshService = services.GetRequiredService<IGameplayRefreshService>();
                    var refreshResult = await refreshService.RefreshAsync(new GameplayRefreshRequest(
                        civilizationId.Value,
                        selectedPlanet.Id,
                        DateTime.UtcNow,
                        IncludeResources: true,
                        IncludeConstruction: true,
                        IncludeResearch: true,
                        IncludeProduction: true), cancellationToken);

                    if (!refreshResult.Succeeded)
                    {
                        services.GetService<ILoggerFactory>()?
                            .CreateLogger(nameof(DevResearchUiStateEndpoints))
                            .LogWarning("Gameplay refresh did not succeed before the research UI-state read: {Errors}", string.Join("; ", refreshResult.Errors));
                    }
                }
                catch (Exception exception)
                {
                    services.GetService<ILoggerFactory>()?
                        .CreateLogger(nameof(DevResearchUiStateEndpoints))
                        .LogWarning(exception, "Gameplay refresh failed before the research UI-state read.");
                }
            }

            var queue = await dbContext.ResearchOrders
                .AsNoTracking()
                .Where(x =>
                    x.CivilizationId == civilizationId.Value &&
                    (x.Status == ResearchQueueItemStatus.Pending || x.Status == ResearchQueueItemStatus.Active))
                .OrderBy(x => x.Sequence)
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
            var hasOpenResearchOrder = queueDtos.Any(x =>
                x.Status is ResearchQueueItemStatus.Pending or ResearchQueueItemStatus.Active);

            var projectDtos = projects.Select(x => new DevResearchProjectDto(x.CivilizationId, x.ResearchType, x.Level)).ToArray();

            var technologyHints = Enum.GetValues<ResearchType>()
                .Select(researchType =>
                {
                    var definition = ResearchCatalog.Get(researchType);
                    var currentLevel = projectLevels.GetValueOrDefault(researchType);
                    var estimatedDuration = ResearchDurationCalculator.CalculateDuration(TimeSpan.FromMinutes(10 * (currentLevel + 1)), energySystemsLevel);
                    var openOrderForTechnology = queueDtos.FirstOrDefault(x =>
                        x.ResearchType == researchType &&
                        x.Status is ResearchQueueItemStatus.Pending or ResearchQueueItemStatus.Active);
                    var readiness = ResearchEnqueueReadinessEvaluator.Evaluate(
                        selectedPlanet is not null,
                        hasOpenResearchOrder,
                        stockpile,
                        researchType,
                        currentLevel);

                    return new DevResearchTechnologyHintDto(
                        researchType,
                        currentLevel,
                        readiness.TargetLevel,
                        readiness.StatusKey,
                        readiness.AvailabilityReasonKey,
                        readiness.CanEnqueue,
                        openOrderForTechnology is not null && openOrderForTechnology.EndsAtUtc <= DateTime.UtcNow,
                        estimatedDuration,
                        readiness.Cost,
                        selectedPlanet is not null
                            ? new DevResearchEnqueueCommandDto(
                                "research.order.enqueue",
                                "POST",
                                "/api/dev/research/orders/enqueue",
                                civilizationId.Value,
                                selectedPlanet.Id,
                                researchType,
                                readiness.TargetLevel)
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
