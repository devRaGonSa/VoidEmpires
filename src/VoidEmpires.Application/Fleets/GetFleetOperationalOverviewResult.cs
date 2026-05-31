using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Fleets;

namespace VoidEmpires.Application.Fleets;

public sealed record GetFleetOperationalOverviewResult(
    Guid CivilizationId,
    IReadOnlyList<FleetOperationalGroupDto> Groups);

public sealed record FleetOperationalGroupDto(
    Guid Id,
    Guid CivilizationId,
    Guid OriginPlanetId,
    Guid CurrentPlanetId,
    SpaceAssetType AssetType,
    int Quantity,
    OrbitalGroupStatus Status,
    bool IsStationedAwayFromOrigin,
    bool HasActiveTransfer,
    FleetOperationalTransferDto? ActiveTransfer,
    FleetOperationalCommandAvailabilityDto Commands);

public sealed record FleetOperationalTransferDto(
    Guid Id,
    Guid DestinationPlanetId,
    int AbstractDistanceUnits,
    DateTime DepartureAtUtc,
    DateTime ArrivalAtUtc,
    OrbitalTransferStatus Status);

public sealed record FleetOperationalCommandAvailabilityDto(
    bool CanCreateTransfer,
    bool CanSplit,
    bool CanMerge,
    bool CanCancelTransfer);
