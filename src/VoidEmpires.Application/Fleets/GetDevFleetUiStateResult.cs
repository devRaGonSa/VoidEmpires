using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Application.StrategicMap;

namespace VoidEmpires.Application.Fleets;

public sealed record GetDevFleetUiStateResult(
    Guid CivilizationId,
    IReadOnlyList<DevFleetUiGroupDto> Groups,
    IReadOnlyList<DevFleetUiResourceContextDto> ResourceContexts,
    IReadOnlyList<DevFleetUiActionHintDto> ActionHints,
    IReadOnlyList<DevFleetUiInterceptionNoteDto> InterceptionNotes)
{
    public Guid? SelectedPlanetId { get; init; }
    public IReadOnlyList<DevFleetUiPlanetDto> Planets { get; init; } = [];
    public IReadOnlyList<DevFleetUiOrbitalStockDto> LocalStock { get; init; } = [];
}

public sealed record DevFleetUiPlanetDto(Guid PlanetId, string PlanetName, bool IsOwnedByRequestingCivilization);
public sealed record DevFleetUiOrbitalStockDto(SpaceAssetType AssetType, int Quantity);

public sealed record DevFleetUiGroupDto(
    Guid Id,
    Guid CivilizationId,
    Guid OriginPlanetId,
    Guid CurrentPlanetId,
    SpaceAssetType AssetType,
    int Quantity,
    OrbitalGroupStatus Status,
    bool IsStationedAwayFromOrigin,
    bool HasActiveTransfer,
    DevFleetUiTransferDto? ActiveTransfer,
    DevFleetUiCommandAvailabilityDto Commands,
    DevFleetUiRouteFuelReadinessHintDto RouteFuelReadiness);

public sealed record DevFleetUiTransferDto(
    Guid Id,
    Guid DestinationPlanetId,
    int AbstractDistanceUnits,
    DateTime DepartureAtUtc,
    DateTime ArrivalAtUtc,
    OrbitalTransferStatus Status,
    InterceptionReadinessSummaryDto? InterceptionReadiness = null);

public sealed record DevFleetUiCommandAvailabilityDto(
    bool CanCreateTransfer,
    bool CanSplit,
    bool CanMerge,
    bool CanCancelTransfer);

public sealed record DevFleetUiResourceContextDto(
    Guid PlanetId,
    IReadOnlyList<DevFleetUiResourceBalanceDto> Balances);

public sealed record DevFleetUiResourceBalanceDto(
    ResourceType ResourceType,
    decimal Quantity);

public sealed record DevFleetUiActionHintDto(
    string ActionKey,
    string DisplayName,
    bool IsReadOnly,
    string Method,
    string Route,
    string Notes);

public sealed record DevFleetUiInterceptionNoteDto(string Note);

public sealed record DevFleetUiRouteFuelReadinessHintDto(
    bool CanRequestTravelEstimate,
    bool RequiresDestination,
    string EstimateActionKey,
    string EstimateRoute,
    OrbitalFuelReadinessPolicy FuelReadinessPolicy,
    OrbitalRouteProfileDto? RouteProfile,
    OrbitalFuelReadinessDto? FuelReadiness,
    IReadOnlyList<string> Notes);
