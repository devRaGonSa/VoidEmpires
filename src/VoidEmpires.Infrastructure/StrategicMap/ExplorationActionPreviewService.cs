using VoidEmpires.Application.StrategicMap;

namespace VoidEmpires.Infrastructure.StrategicMap;

public sealed class ExplorationActionPreviewService(IMapVisibilityService mapVisibilityService) : IExplorationActionPreviewService
{
    public async Task<GetExplorationActionPreviewResult> GetAsync(
        GetExplorationActionPreviewRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CivilizationId == Guid.Empty)
        {
            return new GetExplorationActionPreviewResult(request.CivilizationId, [], CreateNotes());
        }

        var visibility = await mapVisibilityService.GetAsync(
            new GetMapVisibilityRequest(request.CivilizationId),
            cancellationToken);

        var systems = visibility.Systems
            .Select(CreateSystemPreview)
            .ToArray();

        return new GetExplorationActionPreviewResult(
            request.CivilizationId,
            systems,
            CreateNotes());
    }

    private static ExplorationSystemActionPreviewDto CreateSystemPreview(MapSystemVisibilityDto system)
    {
        var preview = CreatePreview(system.VisibilityLevel);

        return new ExplorationSystemActionPreviewDto(
            system.SystemId,
            system.VisibilityLevel,
            preview.CanPreviewExploration,
            preview.BlockReason,
            preview.Note,
            system.Planets.Select(CreatePlanetPreview).ToArray());
    }

    private static ExplorationPlanetActionPreviewDto CreatePlanetPreview(MapPlanetVisibilityDto planet)
    {
        var preview = CreatePreview(planet.VisibilityLevel);

        return new ExplorationPlanetActionPreviewDto(
            planet.PlanetId,
            planet.VisibilityLevel,
            preview.CanPreviewExploration,
            preview.BlockReason,
            preview.Note);
    }

    internal static StrategicMapExplorationPreviewDto CreatePreview(MapVisibilityLevel visibilityLevel) => visibilityLevel switch
    {
        MapVisibilityLevel.Unknown => new StrategicMapExplorationPreviewDto(
            true,
            ExplorationActionBlockReason.None,
            "Exploration preview is available as read-only UI metadata; no mission, sensor, or fog-of-war state is created."),
        MapVisibilityLevel.Visible => new StrategicMapExplorationPreviewDto(
            false,
            ExplorationActionBlockReason.AlreadyVisible,
            "Target is already visible in the current derived visibility projection."),
        MapVisibilityLevel.Owned => new StrategicMapExplorationPreviewDto(
            false,
            ExplorationActionBlockReason.AlreadyOwned,
            "Target is already owned by the requesting civilization."),
        _ => new StrategicMapExplorationPreviewDto(
            false,
            ExplorationActionBlockReason.NoKnownVisibilitySource,
            "No supported exploration preview is available for this visibility state.")
    };

    private static IReadOnlyList<ExplorationActionNoteDto> CreateNotes() =>
    [
        new ExplorationActionNoteDto(
            "exploration.preview",
            true,
            "Read-only exploration readiness metadata. It does not create missions, scanners, persisted fog-of-war, sensor data, espionage, diplomacy, combat, or route graph state.")
    ];
}
