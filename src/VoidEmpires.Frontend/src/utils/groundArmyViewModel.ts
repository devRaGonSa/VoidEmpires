import type { GroundArmyOptionDto, GroundArmyQueueItemDto, GroundArmyUiStateDto } from "../api/groundArmyTypes";
import { formatPlanetControlStatus, formatResourceType } from "./domainPresentation";
import {
  formatGroundTrainingCost,
  formatGroundTrainingDuration,
  getGroundActionLabel,
  getGroundArmyCategoryLabel,
  getGroundArmyRoleLabel,
  getGroundReadinessLabel,
  getGroundStructureLabel,
  getGroundUnitLabel,
} from "./groundArmyPresentation";

export interface GroundArmyOption {
  assetType: string; label: string; categoryLabel: string; roleLabel: string; statusKey: string; statusLabel: string; reasonKey: string; reasonLabel: string; currentStock: number; estimatedDurationLabel: string; estimatedCostLabel: string;
}
export interface GroundArmyQueueItem {
  orderId: string; assetType: string; label: string; quantity: number; sequence: number; statusKey: string; statusLabel: string; startsAtUtc: string; endsAtUtc: string; isDue: boolean;
}
export interface GroundArmyViewModel {
  civilizationId: string; selectedPlanetId: string | null; groundArmy: null | {
    planetId: string; planetName: string; solarSystemName: string; isOwnedByRequestingCivilization: boolean; ownerCivilizationName: string | null; controlStatusLabel: string | null;
    stockpile: { resourceType: string; quantity: number; label: string; }[];
    population: { totalPopulation: number; baseRecruitablePopulation: number; buildingCapacityBonus: number; totalGroundCapacity: number; } | null;
    structures: { buildingType: string; label: string; categoryLabel: string; level: number; }[];
    garrison: { assetType: string; label: string; quantity: number; roleLabel: string; }[];
    catalog: GroundArmyOption[]; queue: GroundArmyQueueItem[];
    readinessSummary: { structureCount: number; garrisonUnitTypes: number; totalGarrisonQuantity: number; availableOptionCount: number; blockedOptionCount: number; queueItemCount: number; dueQueueItemCount: number; };
    actionAvailability: { enqueueLabel: string; enqueueSupported: boolean; enqueueStatusLabel: string; enqueueReason: string; completeDueLabel: string; completeDueSupported: boolean; completeDueStatusLabel: string; completeDueReason: string; };
    diagnostics: { playerFacing: readonly string[]; technical: readonly string[]; };
  };
  diagnostics: { playerFacing: readonly string[]; technical: readonly string[]; };
}

function mapOption(item: GroundArmyOptionDto): GroundArmyOption {
  const assetType = `${item.assetType ?? ""}`;
  return {
    assetType,
    label: getGroundUnitLabel(assetType),
    categoryLabel: getGroundArmyCategoryLabel(assetType),
    roleLabel: getGroundArmyRoleLabel(assetType),
    statusKey: item.availabilityStatus,
    statusLabel: item.availabilityStatus === "Available" ? "Disponible" : "Bloqueada",
    reasonKey: item.availabilityReason,
    reasonLabel: getGroundReadinessLabel(item.availabilityReason),
    currentStock: item.currentStock,
    estimatedDurationLabel: formatGroundTrainingDuration(item.estimatedDuration),
    estimatedCostLabel: formatGroundTrainingCost(item.cost),
  };
}

function mapQueueItem(item: GroundArmyQueueItemDto): GroundArmyQueueItem {
  const assetType = `${item.assetType ?? ""}`;
  return {
    orderId: item.orderId,
    assetType,
    label: getGroundUnitLabel(assetType),
    quantity: item.quantity,
    sequence: item.sequence,
    statusKey: `${item.status ?? ""}`,
    statusLabel: getGroundReadinessLabel(item.status),
    startsAtUtc: item.startsAtUtc,
    endsAtUtc: item.endsAtUtc,
    isDue: item.isDue,
  };
}

export function groupGroundOptionsByCategory(options: readonly GroundArmyOption[]) {
  return [...options.reduce<Map<string, { key: string; label: string; options: GroundArmyOption[] }>>((accumulator, option) => {
    const current = accumulator.get(option.categoryLabel);
    if (current) current.options.push(option);
    else accumulator.set(option.categoryLabel, { key: option.categoryLabel, label: option.categoryLabel, options: [option] });
    return accumulator;
  }, new Map()).values()];
}

export function selectRecommendedGroundArmyAction(options: readonly GroundArmyOption[]) {
  return options.find((option) => option.statusKey === "Available") ?? options[0] ?? null;
}

export function getGroundArmyPrimaryAction(option: GroundArmyOption | null | undefined) {
  return option?.statusKey === "Available" ? getGroundActionLabel("production.enqueue") : getGroundActionLabel("catalog.read");
}

export function mapGroundArmyUiStateToViewModel(state: GroundArmyUiStateDto): GroundArmyViewModel {
  const groundArmy = state.groundArmy ? {
    planetId: state.groundArmy.planetId,
    planetName: state.groundArmy.planetName,
    solarSystemName: state.groundArmy.solarSystemName,
    isOwnedByRequestingCivilization: state.groundArmy.isOwnedByRequestingCivilization,
    ownerCivilizationName: state.groundArmy.ownerCivilizationName,
    controlStatusLabel: state.groundArmy.controlStatus ? formatPlanetControlStatus(state.groundArmy.controlStatus) : null,
    stockpile: state.groundArmy.resourceStockpile.map((entry) => ({ resourceType: `${entry.resourceType ?? ""}`, quantity: entry.quantity, label: formatResourceType(entry.resourceType) })),
    population: state.groundArmy.population,
    structures: state.groundArmy.groundStructures.map((entry) => ({ buildingType: `${entry.buildingType ?? ""}`, label: getGroundStructureLabel(entry.buildingType), categoryLabel: `${entry.category ?? "Militar terrestre"}`, level: entry.level })),
    garrison: state.groundArmy.garrison.map((entry) => ({ assetType: `${entry.assetType ?? ""}`, label: getGroundUnitLabel(entry.assetType), quantity: entry.quantity, roleLabel: getGroundArmyRoleLabel(entry.assetType) })),
    catalog: state.groundArmy.catalog.map(mapOption),
    queue: state.groundArmy.queue.map(mapQueueItem),
    readinessSummary: state.groundArmy.readinessSummary,
    actionAvailability: {
      enqueueLabel: getGroundActionLabel("production.enqueue"),
      enqueueSupported: state.groundArmy.actionSummary.enqueueSupported,
      enqueueStatusLabel: getGroundReadinessLabel(state.groundArmy.actionSummary.enqueueActionStatus),
      enqueueReason: getGroundReadinessLabel(state.groundArmy.actionSummary.enqueueActionReason),
      completeDueLabel: getGroundActionLabel("production.completeDue"),
      completeDueSupported: state.groundArmy.actionSummary.completeDueSupported,
      completeDueStatusLabel: getGroundReadinessLabel(state.groundArmy.actionSummary.completeDueActionStatus),
      completeDueReason: getGroundReadinessLabel(state.groundArmy.actionSummary.completeDueActionReason),
    },
    diagnostics: {
      playerFacing: [...state.groundArmy.diagnostics.notes, ...state.groundArmy.diagnostics.limitations],
      technical: [...state.errors, ...state.groundArmy.diagnostics.notes, ...state.groundArmy.diagnostics.limitations],
    },
  } : null;

  return {
    civilizationId: state.civilizationId,
    selectedPlanetId: state.selectedPlanetId,
    groundArmy,
    diagnostics: groundArmy?.diagnostics ?? { playerFacing: state.errors.length > 0 ? [state.errors[0]] : [], technical: [...state.errors] },
  };
}
