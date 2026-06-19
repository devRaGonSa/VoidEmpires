using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Economy;

namespace VoidEmpires.Application.Assets;

public interface IDevShipyardUiStateService
{
    Task<GetDevShipyardUiStateResult> GetAsync(
        GetDevShipyardUiStateRequest request,
        CancellationToken cancellationToken = default);
}

public sealed record GetDevShipyardUiStateRequest(
    Guid CivilizationId,
    Guid? PlanetId = null);

public sealed record GetDevShipyardUiStateResult(
    Guid CivilizationId,
    Guid? SelectedPlanetId,
    IReadOnlyList<DevShipyardPlanetOptionDto> KnownPlanets,
    DevShipyardCockpitDto? Shipyard,
    IReadOnlyList<string> Errors);

public sealed record DevShipyardPlanetOptionDto(
    Guid PlanetId,
    string PlanetName,
    Guid SolarSystemId,
    string SolarSystemName,
    bool IsOwnedByRequestingCivilization);

public sealed record DevShipyardCockpitDto(
    Guid PlanetId,
    string PlanetName,
    Guid SolarSystemId,
    string SolarSystemName,
    bool IsOwnedByRequestingCivilization,
    Guid? OwnerCivilizationId,
    string? OwnerCivilizationName,
    PlanetControlStatus? ControlStatus,
    IReadOnlyList<DevShipyardResourceBalanceDto> ResourceStockpile,
    DevShipyardBuildingReadinessDto BuildingReadiness,
    IReadOnlyList<DevShipyardCatalogItemDto> Catalog,
    IReadOnlyList<DevShipyardQueueItemDto> Queue,
    IReadOnlyList<DevShipyardStockItemDto> OrbitalStock,
    DevShipyardActionSummaryDto ActionSummary,
    DevShipyardDiagnosticsDto Diagnostics);

public sealed record DevShipyardResourceBalanceDto(
    ResourceType ResourceType,
    decimal Quantity);

public sealed record DevShipyardBuildingReadinessDto(
    int ShipyardLevel,
    int FleetCommandCenterLevel,
    int LogisticsHubLevel,
    bool HasPopulationProfile);

public sealed record DevShipyardCatalogItemDto(
    SpaceAssetType AssetType,
    BuildingType RequiredBuildingType,
    int RequiredBuildingLevel,
    int RequiredOperatorCapacity,
    TimeSpan EstimatedDuration,
    IReadOnlyList<DevShipyardResourceBalanceDto> Cost,
    int CurrentStock,
    string AvailabilityStatus,
    string AvailabilityReason,
    DevShipyardEnqueueCommandDto? EnqueueCommand,
    DevShipyardAssetMetadataDto? Metadata = null);

public sealed record DevShipyardAssetMetadataDto(
    string DisplayName,
    string Description,
    string CategoryKey,
    string CategoryLabel,
    string RoleKey,
    string RoleLabel,
    string ModuleKey,
    string ModuleLabel,
    string ImageKey,
    string IconKey,
    int SortOrder,
    string DurationPolicyKey,
    string DurationPolicyLabel,
    string FleetHandoffPolicyKey,
    string FleetHandoffPolicyLabel,
    string PrerequisiteSummary,
    int StorageCapacity,
    int OperatingRange,
    IReadOnlyList<string> RequirementKeys,
    IReadOnlyList<string> Tags);

public sealed record DevShipyardEnqueueCommandDto(
    string ActionKey,
    string Method,
    string Route,
    Guid CivilizationId,
    Guid PlanetId,
    AssetProductionTarget Target,
    SpaceAssetType SpaceAssetType,
    int Quantity);

public sealed record DevShipyardQueueItemDto(
    Guid OrderId,
    SpaceAssetType AssetType,
    int Quantity,
    int Sequence,
    AssetProductionOrderStatus Status,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc,
    bool IsDue);

public sealed record DevShipyardStockItemDto(
    SpaceAssetType AssetType,
    int Quantity);

public sealed record DevShipyardActionSummaryDto(
    string QueueActionStatus,
    string QueueActionReason,
    bool EnqueueSupported,
    string EnqueueActionStatus,
    string EnqueueActionReason,
    bool CompleteDueSupported,
    string CompleteDueActionStatus,
    string CompleteDueActionReason,
    int OpenQueueCount,
    int DueQueueCount);

public sealed record DevShipyardDiagnosticsDto(
    Guid? RequestPlanetId,
    Guid? HomePlanetId,
    bool HasResourceStockpile,
    bool HasOwnedShipyardBuilding,
    bool HasPopulationProfile,
    IReadOnlyList<string> Notes);
