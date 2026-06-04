using VoidEmpires.Domain.Economy;

namespace VoidEmpires.Application.Markets;

public interface IDevMarketUiStateService
{
    Task<GetDevMarketUiStateResult> GetAsync(
        GetDevMarketUiStateRequest request,
        CancellationToken cancellationToken = default);
}

public sealed record GetDevMarketUiStateRequest(
    Guid CivilizationId,
    Guid? PlanetId = null);

public sealed record GetDevMarketUiStateResult(
    Guid CivilizationId,
    Guid? SelectedPlanetId,
    IReadOnlyList<DevMarketPlanetOptionDto> KnownPlanets,
    DevMarketCockpitDto? Market,
    IReadOnlyList<string> Errors);

public sealed record DevMarketPlanetOptionDto(
    Guid PlanetId,
    string PlanetName,
    Guid SolarSystemId,
    string SolarSystemName,
    bool IsOwnedByRequestingCivilization,
    IReadOnlyList<DevMarketResourceReserveDto> VisibleReserves,
    bool HasProductionProfile);

public sealed record DevMarketCockpitDto(
    Guid? SelectedPlanetId,
    string? SelectedPlanetName,
    Guid? SelectedSolarSystemId,
    string? SelectedSolarSystemName,
    IReadOnlyList<DevMarketResourceReserveDto> CivilizationReserves,
    IReadOnlyList<DevMarketResourceReserveDto> SelectedPlanetReserves,
    DevMarketProductionSummaryDto? SelectedPlanetProduction,
    IReadOnlyList<DevMarketReferenceRatioDto> ReferenceRatios,
    IReadOnlyList<DevMarketSignalDto> Signals,
    IReadOnlyList<DevMarketFutureActionDto> FutureActions,
    DevMarketLogisticsSummaryDto Logistics,
    DevMarketDiagnosticsDto Diagnostics,
    IReadOnlyList<string> Limitations);

public sealed record DevMarketResourceReserveDto(
    ResourceType ResourceType,
    decimal Quantity);

public sealed record DevMarketProductionSummaryDto(
    decimal CreditsPerHour,
    decimal MetalPerHour,
    decimal CrystalPerHour,
    decimal GasPerHour);

public sealed record DevMarketReferenceRatioDto(
    ResourceType ResourceType,
    decimal AdvisoryRatio,
    string ConfidenceKey,
    string BasisKey,
    bool IsAdvisory);

public sealed record DevMarketSignalDto(
    string SignalKey,
    ResourceType? ResourceType,
    decimal Quantity,
    decimal? ProductionPerHour,
    string SeverityKey,
    string ReasonKey);

public sealed record DevMarketFutureActionDto(
    string ActionKey,
    bool IsEnabled,
    string StateKey,
    string ReasonKey);

public sealed record DevMarketLogisticsSummaryDto(
    int StationedGroupsAtSelectedPlanet,
    int ActiveDeparturesFromSelectedPlanet,
    int ActiveArrivalsToSelectedPlanet,
    int CivilizationActiveTransfers,
    bool HasFutureRoutePlaceholders);

public sealed record DevMarketDiagnosticsDto(
    Guid? RequestPlanetId,
    Guid? HomePlanetId,
    int OwnedPlanetCount,
    bool HasSelectedPlanetStockpile,
    bool HasSelectedPlanetProduction,
    IReadOnlyList<string> Notes);
