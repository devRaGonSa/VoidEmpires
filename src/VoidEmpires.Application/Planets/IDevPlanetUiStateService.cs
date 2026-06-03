using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Galaxy;

namespace VoidEmpires.Application.Planets;

public interface IDevPlanetUiStateService
{
    Task<GetDevPlanetUiStateResult> GetAsync(
        GetDevPlanetUiStateRequest request,
        CancellationToken cancellationToken = default);
}

public interface IDevDefenseUiStateService
{
    Task<GetDevDefenseUiStateResult> GetAsync(
        GetDevDefenseUiStateRequest request,
        CancellationToken cancellationToken = default);
}

public sealed record GetDevPlanetUiStateRequest(
    Guid CivilizationId,
    Guid? PlanetId = null);

public sealed record GetDevDefenseUiStateRequest(
    Guid CivilizationId,
    Guid? PlanetId = null);

public sealed record GetDevPlanetUiStateResult(
    Guid CivilizationId,
    Guid? SelectedPlanetId,
    IReadOnlyList<DevPlanetOptionDto> KnownPlanets,
    DevPlanetCockpitDto? Planet,
    IReadOnlyList<string> Errors);

public sealed record GetDevDefenseUiStateResult(
    Guid CivilizationId,
    Guid? SelectedPlanetId,
    IReadOnlyList<DevPlanetOptionDto> KnownPlanets,
    DevDefenseCockpitDto? Defenses,
    IReadOnlyList<string> Errors);

public sealed record DevPlanetOptionDto(
    Guid PlanetId,
    string PlanetName,
    Guid SolarSystemId,
    string SolarSystemName,
    bool IsOwnedByRequestingCivilization);

public sealed record DevPlanetCockpitDto(
    Guid PlanetId,
    string PlanetName,
    Guid SolarSystemId,
    string SolarSystemName,
    int OrbitalSlot,
    PlanetType PlanetType,
    int Size,
    PlanetColonizationStatus ColonizationStatus,
    bool IsOwnedByRequestingCivilization,
    Guid? OwnerCivilizationId,
    string? OwnerCivilizationName,
    PlanetControlStatus? ControlStatus,
    IReadOnlyList<DevPlanetResourceBalanceDto> Stockpile,
    DevPlanetProductionSummaryDto? ProductionSummary,
    DevPlanetBuildingCapacityDto? BuildingCapacity,
    IReadOnlyList<DevPlanetBuildingDto> Buildings,
    IReadOnlyList<DevPlanetConstructionQueueItemDto> ConstructionQueue,
    DevPlanetConstructionActionSummaryDto ActionSummary,
    IReadOnlyList<DevPlanetConstructionActionDto> ConstructionActions,
    DevPlanetOrbitalContextDto OrbitalContext,
    DevPlanetDiagnosticsDto Diagnostics);

public sealed record DevDefenseCockpitDto(
    Guid PlanetId,
    string PlanetName,
    Guid SolarSystemId,
    string SolarSystemName,
    bool IsOwnedByRequestingCivilization,
    Guid? OwnerCivilizationId,
    string? OwnerCivilizationName,
    PlanetControlStatus? ControlStatus,
    IReadOnlyList<DevPlanetResourceBalanceDto> ResourceStockpile,
    IReadOnlyList<DevPlanetBuildingDto> DefenseStructures,
    IReadOnlyList<DevPlanetConstructionActionDto> DefenseOptions,
    IReadOnlyList<DevPlanetConstructionQueueItemDto> DefenseQueue,
    DevDefenseProtectionSummaryDto ProtectionSummary,
    DevPlanetConstructionActionSummaryDto ActionSummary,
    DevDefenseDiagnosticsDto Diagnostics);

public sealed record DevPlanetResourceBalanceDto(
    ResourceType ResourceType,
    decimal Quantity);

public sealed record DevPlanetProductionSummaryDto(
    decimal CreditsPerHour,
    decimal MetalPerHour,
    decimal CrystalPerHour,
    decimal GasPerHour,
    decimal ResearchMultiplier);

public sealed record DevPlanetBuildingCapacityDto(
    int UsedCapacity,
    int BaseCapacity,
    int PersistentBonusCapacity,
    int ResearchBonusCapacity,
    int TotalAvailableCapacity);

public sealed record DevPlanetBuildingDto(
    BuildingType BuildingType,
    BuildingCategory Category,
    int Level,
    int Footprint,
    DevPlanetBuildingDisplayDto? Display = null);

public sealed record DevPlanetConstructionQueueItemDto(
    Guid OrderId,
    ConstructionQueueItemAction Action,
    ConstructionQueueItemStatus Status,
    BuildingType BuildingType,
    BuildingCategory Category,
    int TargetLevel,
    int Sequence,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc,
    bool IsDue,
    IReadOnlyList<DevPlanetResourceBalanceDto> Cost,
    DevPlanetConstructionQueueItemDisplayDto? Display = null);

public sealed record DevPlanetConstructionActionSummaryDto(
    string QueueActionStatus,
    string QueueActionReason,
    bool CompleteDueSupported,
    string CompleteDueActionStatus,
    string CompleteDueActionReason,
    int DueConstructionCount,
    DevPlanetConstructionActionSummaryDisplayDto? Display = null);

public sealed record DevPlanetConstructionActionDto(
    ConstructionQueueItemAction Action,
    BuildingType BuildingType,
    BuildingCategory Category,
    int CurrentLevel,
    int TargetLevel,
    string AvailabilityStatus,
    string AvailabilityReason,
    TimeSpan EstimatedDuration,
    IReadOnlyList<DevPlanetResourceBalanceDto> Cost,
    DevPlanetConstructionActionDisplayDto? Display = null);

public sealed record DevPlanetBuildingDisplayDto(
    string BuildingTypeLabel,
    string CategoryLabel);

public sealed record DevPlanetConstructionQueueItemDisplayDto(
    string ActionLabel,
    string StatusLabel,
    string BuildingTypeLabel,
    string CategoryLabel);

public sealed record DevPlanetConstructionActionSummaryDisplayDto(
    string QueueActionStatusLabel,
    string QueueActionReasonLabel,
    string CompleteDueActionStatusLabel,
    string CompleteDueActionReasonLabel);

public sealed record DevPlanetConstructionActionDisplayDto(
    string ActionLabel,
    string BuildingTypeLabel,
    string CategoryLabel,
    string AvailabilityLabel,
    string AvailabilityReasonLabel);

public sealed record DevPlanetOrbitalContextDto(
    int StationedGroups,
    int ActiveDepartures,
    int ActiveArrivals);

public sealed record DevPlanetDiagnosticsDto(
    Guid? RequestPlanetId,
    Guid SolarSystemId,
    Guid? OwnerCivilizationId,
    Guid? HomePlanetId,
    bool HasResourceStockpile,
    bool HasProductionProfile,
    bool HasBuildingCapacity,
    int OpenConstructionOrderCount,
    IReadOnlyList<string> Notes);

public sealed record DevDefenseProtectionSummaryDto(
    int StructureCount,
    int TotalDefenseLevel,
    int AvailableOptionCount,
    int BlockedOptionCount,
    int QueueItemCount,
    int DueQueueItemCount);

public sealed record DevDefenseDiagnosticsDto(
    Guid? RequestPlanetId,
    Guid SolarSystemId,
    Guid? OwnerCivilizationId,
    bool HasResourceStockpile,
    bool HasDefenseStructure,
    IReadOnlyList<string> Notes,
    IReadOnlyList<string> Limitations);
