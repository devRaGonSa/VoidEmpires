namespace VoidEmpires.Application.StrategicMap;

public sealed record GetDevEspionageUiStateRequest(Guid CivilizationId);

public sealed record GetDevEspionageUiStateResult(
    Guid CivilizationId,
    DevEspionageOverviewDto Overview,
    IReadOnlyList<DevEspionageTargetDto> Targets,
    IReadOnlyList<DevEspionagePassiveSignalDto> PassiveSignals,
    DevEspionageRecommendedFocusDto? RecommendedFocus,
    IReadOnlyList<DevEspionageFutureActionDto> FutureActions,
    IReadOnlyList<string> Diagnostics,
    IReadOnlyList<string> Limitations);

public sealed record DevEspionageOverviewDto(
    int OwnedTargetCount,
    int VisibleTargetCount,
    int KnownTargetCount,
    int PartialTargetCount,
    int PassiveSignalCount);

public sealed record DevEspionageTargetDto(
    string TargetKind,
    Guid SystemId,
    Guid? PlanetId,
    string? SystemName,
    string? PlanetName,
    MapVisibilityLevel VisibilityLevel,
    MapVisibilityReason VisibilityReason,
    string IntelligenceLevel,
    string ConfidenceSummary,
    string CoverageSummary,
    bool HasPassiveSignals);

public sealed record DevEspionagePassiveSignalDto(
    Guid SystemId,
    Guid? PlanetId,
    string SignalKind,
    string Summary);

public sealed record DevEspionageRecommendedFocusDto(
    Guid SystemId,
    Guid? PlanetId,
    string Reason);

public sealed record DevEspionageFutureActionDto(
    string ActionKey,
    bool IsAvailable,
    string Reason);

public interface IDevEspionageUiStateService
{
    Task<GetDevEspionageUiStateResult> GetAsync(GetDevEspionageUiStateRequest request, CancellationToken cancellationToken = default);
}
