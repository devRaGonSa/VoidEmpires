namespace VoidEmpires.Application.Rankings;

public interface IDevRankingUiStateService
{
    Task<GetDevRankingUiStateResult> GetAsync(
        GetDevRankingUiStateRequest request,
        CancellationToken cancellationToken = default);
}

public sealed record GetDevRankingUiStateRequest(Guid CivilizationId);

public sealed record GetDevRankingUiStateResult(
    Guid CivilizationId,
    DevRankingIdentityDto? Identity,
    DevRankingSummaryDto? Summary,
    IReadOnlyList<DevRankingComparisonRowDto> DemoComparisons,
    DevRankingPublicationStateDto? Publication,
    IReadOnlyList<DevRankingFuturePlaceholderDto> FuturePlaceholders,
    IReadOnlyList<DevRankingDisabledActionDto> DisabledActions,
    DevRankingDiagnosticsDto? Diagnostics,
    IReadOnlyList<string> Limitations,
    IReadOnlyList<string> Errors);

public sealed record DevRankingIdentityDto(
    Guid CivilizationId,
    string CivilizationName,
    string DisplayName,
    Guid? HomePlanetId);

public sealed record DevRankingSummaryDto(
    int TotalPowerIndex,
    IReadOnlyList<DevRankingCategoryScoreDto> Categories,
    string RecommendationKey);

public sealed record DevRankingCategoryScoreDto(
    string CategoryKey,
    int Score,
    int Weight,
    string SourceNote);

public sealed record DevRankingComparisonRowDto(
    string RowKey,
    string DisplayName,
    int TotalPowerIndex,
    int DeltaFromCurrent,
    bool IsCurrentCivilization,
    bool IsDemoOnly);

public sealed record DevRankingPublicationStateDto(
    string StateKey,
    bool IsPublished,
    string SummaryKey);

public sealed record DevRankingFuturePlaceholderDto(
    string PlaceholderKey,
    bool IsAvailable,
    string StateKey,
    string ReasonKey);

public sealed record DevRankingDisabledActionDto(
    string ActionKey,
    bool IsAvailable,
    string ReasonKey);

public sealed record DevRankingDiagnosticsDto(
    int OwnedPlanetCount,
    int VisibleSystemCount,
    int DiplomaticContactCount,
    int ActiveTransferCount,
    IReadOnlyList<string> Notes);
