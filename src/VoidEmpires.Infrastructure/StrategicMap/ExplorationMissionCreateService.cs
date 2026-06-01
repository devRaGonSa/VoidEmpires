using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Exploration;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Domain.Players;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.StrategicMap;

public sealed class ExplorationMissionCreateService(
    VoidEmpiresDbContext dbContext,
    IExplorationActionPreviewService explorationActionPreviewService) : IExplorationMissionCreateService
{
    private static readonly TimeSpan SystemMissionDuration = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan PlanetMissionDuration = TimeSpan.FromMinutes(45);

    public async Task<CreateExplorationMissionResult> CreateAsync(
        CreateExplorationMissionRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationErrors = Validate(request);
        if (validationErrors.Count > 0)
        {
            return CreateExplorationMissionResult.Invalid(validationErrors.ToArray());
        }

        var civilizationExists = await dbContext.Set<Civilization>()
            .AsNoTracking()
            .AnyAsync(x => x.Id == request.CivilizationId, cancellationToken);
        if (!civilizationExists)
        {
            return CreateExplorationMissionResult.Invalid("Civilization was not found.");
        }

        var systemExists = await dbContext.Set<SolarSystem>()
            .AsNoTracking()
            .AnyAsync(x => x.Id == request.TargetSystemId, cancellationToken);
        if (!systemExists)
        {
            return CreateExplorationMissionResult.Invalid("Target system was not found.");
        }

        if (request.TargetPlanetId is Guid targetPlanetId)
        {
            var planetBelongsToSystem = await dbContext.Set<Planet>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == targetPlanetId && x.SolarSystemId == request.TargetSystemId, cancellationToken);
            if (!planetBelongsToSystem)
            {
                return CreateExplorationMissionResult.Invalid("Target planet was not found in the target system.");
            }
        }

        var preview = await explorationActionPreviewService.GetAsync(
            new GetExplorationActionPreviewRequest(request.CivilizationId),
            cancellationToken);
        var systemPreview = preview.Systems.SingleOrDefault(x => x.SystemId == request.TargetSystemId);
        if (systemPreview is null)
        {
            return CreateExplorationMissionResult.Conflict("Target system is not eligible for exploration.");
        }

        if (request.TargetPlanetId is Guid planetId)
        {
            var planetPreview = systemPreview.Planets.SingleOrDefault(x => x.PlanetId == planetId);
            if (planetPreview is null || !planetPreview.CanPreviewPlanetExploration)
            {
                return CreateExplorationMissionResult.Conflict("Target planet is not eligible for exploration.");
            }
        }
        else if (!systemPreview.CanPreviewSystemExploration)
        {
            return CreateExplorationMissionResult.Conflict("Target system is not eligible for exploration.");
        }

        var dueAtUtc = request.RequestedAtUtc.Add(request.TargetPlanetId is null
            ? SystemMissionDuration
            : PlanetMissionDuration);
        var mission = ExplorationMission.CreatePlanned(
            request.CivilizationId,
            request.TargetSystemId,
            request.TargetPlanetId,
            request.RequestedAtUtc,
            dueAtUtc);

        dbContext.ExplorationMissions.Add(mission);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreateExplorationMissionResult.Success(new CreatedExplorationMissionDto(
            mission.Id,
            mission.CivilizationId,
            mission.TargetSystemId,
            mission.TargetPlanetId,
            mission.Status,
            mission.RequestedAtUtc,
            mission.DueAtUtc));
    }

    private static List<string> Validate(CreateExplorationMissionRequest request)
    {
        var errors = new List<string>();

        if (request.CivilizationId == Guid.Empty)
        {
            errors.Add("Civilization id is required.");
        }

        if (request.TargetSystemId == Guid.Empty)
        {
            errors.Add("Target system id is required.");
        }

        if (request.TargetPlanetId == Guid.Empty)
        {
            errors.Add("Target planet id cannot be empty.");
        }

        if (request.RequestedAtUtc.Kind != DateTimeKind.Utc)
        {
            errors.Add("Requested date must be UTC.");
        }

        return errors;
    }
}
