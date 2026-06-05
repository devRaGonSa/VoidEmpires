using VoidEmpires.Domain.Diplomacy;
using VoidEmpires.Domain.Players;

namespace VoidEmpires.Application.StrategicMap;

public sealed record GetDevAllianceUiStateRequest(Guid CivilizationId);

public sealed record GetDevAllianceUiStateResult(
    Guid CivilizationId,
    DevAllianceIdentityDto? Identity,
    DevAllianceStatusDto? Alliance,
    IReadOnlyList<DevAllianceContactDto> KnownContacts,
    IReadOnlyList<DevAllianceFuturePactDto> FuturePacts,
    IReadOnlyList<DevAllianceFutureActionDto> FutureActions,
    DevAllianceActionSummaryDto? ActionSummary,
    DevAllianceDiagnosticsDto? Diagnostics,
    IReadOnlyList<string> Limitations,
    IReadOnlyList<string> Errors);

public sealed record DevAllianceIdentityDto(
    Guid CivilizationId,
    string CivilizationName,
    CivilizationArchetype Archetype,
    CivilizationStatus Status,
    Guid? HomePlanetId,
    Guid PlayerProfileId,
    string PlayerDisplayName);

public sealed record DevAllianceStatusDto(
    string StateKey,
    bool HasActiveAlliance,
    int ActiveAllianceCount,
    int HistoricalAllianceCount,
    int KnownContactCount,
    int ActivePactCount,
    AllianceReadinessDto? PrimaryAlliance);

public sealed record DevAllianceContactDto(
    Guid ContactedCivilizationId,
    DiplomaticContactStatus Status,
    DateTime DiscoveredAtUtc,
    string Source);

public sealed record DevAllianceFuturePactDto(
    string PactTypeKey,
    bool IsAvailable,
    string StateKey,
    string ReasonKey);

public sealed record DevAllianceFutureActionDto(
    string ActionKey,
    bool IsAvailable,
    string StateKey,
    string ReasonKey);

public sealed record DevAllianceActionSummaryDto(
    int DisabledActionCount,
    string SummaryKey,
    bool HasInvitationPlaceholder,
    bool HasApplicationPlaceholder,
    bool HasPactPlaceholders);

public sealed record DevAllianceDiagnosticsDto(
    Guid? HomePlanetId,
    Guid PlayerProfileId,
    int AllianceRowCount,
    int ContactCount,
    int ActivePactCount,
    IReadOnlyList<string> Notes);

public interface IDevAllianceUiStateService
{
    Task<GetDevAllianceUiStateResult> GetAsync(
        GetDevAllianceUiStateRequest request,
        CancellationToken cancellationToken = default);
}
