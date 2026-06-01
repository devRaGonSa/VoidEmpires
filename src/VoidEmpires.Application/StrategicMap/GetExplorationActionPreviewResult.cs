namespace VoidEmpires.Application.StrategicMap;

public sealed record GetExplorationActionPreviewResult(
    Guid CivilizationId,
    IReadOnlyList<ExplorationSystemActionPreviewDto> Systems,
    IReadOnlyList<ExplorationActionNoteDto> Notes);

public sealed record ExplorationSystemActionPreviewDto(
    Guid SystemId,
    MapVisibilityLevel VisibilityLevel,
    bool CanPreviewSystemExploration,
    ExplorationActionBlockReason BlockReason,
    string Note,
    IReadOnlyList<ExplorationPlanetActionPreviewDto> Planets);

public sealed record ExplorationPlanetActionPreviewDto(
    Guid PlanetId,
    MapVisibilityLevel VisibilityLevel,
    bool CanPreviewPlanetExploration,
    ExplorationActionBlockReason BlockReason,
    string Note);

public sealed record ExplorationActionNoteDto(
    string ActionKey,
    bool IsReadOnly,
    string Note);

public enum ExplorationActionBlockReason
{
    None = 0,
    AlreadyVisible = 1,
    AlreadyOwned = 2,
    NoKnownVisibilitySource = 3
}
