using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Fleets;

namespace VoidEmpires.Application.Fleets;

public sealed record GetDevFleetUiStateResult(
    Guid CivilizationId,
    IReadOnlyList<DevFleetUiGroupDto> Groups,
    IReadOnlyList<DevFleetUiResourceContextDto> ResourceContexts,
    IReadOnlyList<DevFleetUiActionHintDto> ActionHints);

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
    DevFleetUiCommandAvailabilityDto Commands);

public sealed record DevFleetUiTransferDto(
    Guid Id,
    Guid DestinationPlanetId,
    int AbstractDistanceUnits,
    DateTime DepartureAtUtc,
    DateTime ArrivalAtUtc,
    OrbitalTransferStatus Status);

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
    string Route);
