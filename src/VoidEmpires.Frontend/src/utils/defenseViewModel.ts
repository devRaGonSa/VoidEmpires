import type {
  DefenseActionSummaryDto,
  DefenseCatalogItemDto,
  DefenseDiagnosticsDto,
  DefenseOptionDto,
  DefensePlanetContextDto,
  DefenseQueueItemDto,
  DefenseResourceStockpileItemDto,
  DefenseStructureDto,
  DefensesUiStateDto,
} from "../api/defenseTypes";
import {
  formatDefenseCost,
  formatDefenseDuration,
  getDefenseActionLabel,
  getDefenseCategoryLabel,
  getDefenseReadinessLabel,
  getDefenseStatusLabel,
  getDefenseStructureLabel,
} from "./defensePresentation";
import { formatResourceType } from "./domainPresentation";

export interface DefenseCost {
  resourceType: string;
  quantity: number;
}

export interface DefenseStructure {
  buildingType: string;
  label: string;
  categoryKey: string;
  categoryLabel: string;
  level: number;
  footprint: number;
}

export interface DefenseOption {
  actionKey: string;
  actionLabel: string;
  assetType: string | null;
  productionModel: "unit" | "level";
  buildingType: string;
  structureLabel: string;
  categoryKey: string;
  categoryLabel: string;
  currentLevel: number;
  targetLevel: number;
  statusKey: string;
  statusLabel: string;
  reasonKey: string;
  reasonLabel: string;
  estimatedDurationLabel: string;
  estimatedCostLabel: string;
  affordabilityLabel: string | null;
  requirementLabel: string | null;
  cost: DefenseCost[];
}

export interface DefenseQueueItem {
  orderId: string;
  actionKey: string;
  actionLabel: string;
  buildingType: string;
  productionModel: "unit" | "level";
  structureLabel: string;
  statusKey: string;
  statusLabel: string;
  targetLevel: number;
  sequence: number;
  startsAtUtc: string;
  endsAtUtc: string;
  isDue: boolean;
  estimatedCostLabel: string;
}

export interface DefenseActionAvailability {
  key: string;
  label: string;
  statusKey: string;
  statusLabel: string;
  reasonKey: string;
  reasonLabel: string;
  supported: boolean;
}

export interface DefenseDiagnostics {
  playerFacing: readonly string[];
  technical: readonly string[];
  limitations: readonly string[];
}

export interface DefensePlanetContext {
  planetId: string;
  planetName: string;
  solarSystemId: string;
  solarSystemName: string;
  isOwnedByRequestingCivilization: boolean;
  ownerCivilizationId: string | null;
  ownerCivilizationName: string | null;
  controlStatus: string | null;
  stockpile: DefenseCost[];
  structures: DefenseStructure[];
  options: DefenseOption[];
  queue: DefenseQueueItem[];
  protectionSummary: DefensePlanetContextDto["protectionSummary"];
  actionAvailability: {
    queue: DefenseActionAvailability;
    enqueue: DefenseActionAvailability;
    completeDue: DefenseActionAvailability;
  };
  diagnostics: DefenseDiagnostics;
}

export interface DefenseOptionGroup {
  key: string;
  label: string;
  options: DefenseOption[];
}

export interface DefensesViewModel {
  civilizationId: string;
  selectedPlanetId: string | null;
  defenses: DefensePlanetContext | null;
  diagnostics: DefenseDiagnostics;
}

const reasonLabels: Record<string, string> = {
  "Ready for explicit development confirmation.": "Lista para abrir Construccion",
  "Planet already has an open construction order.": "La cola de Construccion ya tiene una obra abierta",
  "Planet resource stockpile was not found.": "La colonia no expone reservas locales utilizables",
  "Planet building capacity was not found.": "Sin campos disponibles",
  "Planet building capacity would be exceeded.": "Sin campos disponibles",
  "Insufficient resources.": "Recursos insuficientes",
  "Planet is not controlled by the requesting civilization.": "La colonia no esta bajo control local",
  MissingRequiredBuilding: "Requiere Malla defensiva",
  InsufficientPopulationCapacity: "Capacidad local insuficiente",
  InsufficientResources: "Recursos insuficientes",
};

const unitDefenseAssetTypes: Record<string, string> = {
  MissileBattery: "MissileBattery",
  LaserTurret: "LaserTurret",
  IonCannon: "IonCannon",
  PlasmaCannon: "PlasmaCannon",
};

function getDefenseProductionModel(buildingType: string): "unit" | "level" {
  return unitDefenseAssetTypes[buildingType] ? "unit" : "level";
}

function mapCost(entries: readonly DefenseResourceStockpileItemDto[]): DefenseCost[] {
  return entries.map((entry) => ({
    resourceType: `${entry.resourceType ?? ""}`,
    quantity: entry.quantity,
  }));
}

function getMissingResourceLabel(
  cost: DefenseCost[],
  stockpile: DefenseCost[],
) {
  const missing = cost
    .map((entry) => {
      const available = stockpile.find((item) => item.resourceType === entry.resourceType)?.quantity ?? 0;
      return {
        resourceType: entry.resourceType,
        quantity: Math.max(0, entry.quantity - available),
      };
    })
    .filter((entry) => entry.quantity > 0)
    .map((entry) => `Falta ${formatResourceType(entry.resourceType)} ${entry.quantity}`);

  return missing.length > 0 ? missing.join(" | ") : null;
}

function getReasonLabel(rawReason: string, fallbackLabel?: string | null) {
  if (fallbackLabel?.trim()) {
    return fallbackLabel;
  }

  return reasonLabels[rawReason] ?? "Preparacion defensiva no catalogada";
}

function buildCatalogMetadataMap(catalog: readonly DefenseCatalogItemDto[]) {
  return catalog.reduce<Map<string, DefenseCatalogItemDto>>((accumulator, item) => {
    accumulator.set(`${item.buildingType ?? ""}`, item);
    return accumulator;
  }, new Map());
}

function mapStructure(item: DefenseStructureDto, metadataByBuildingType: ReadonlyMap<string, DefenseCatalogItemDto>): DefenseStructure {
  const buildingType = `${item.buildingType ?? ""}`;
  const categoryKey = `${item.category ?? ""}`;
  const metadata = metadataByBuildingType.get(buildingType);

  return {
    buildingType,
    label: metadata?.displayName ?? item.display?.buildingTypeLabel ?? getDefenseStructureLabel(buildingType),
    categoryKey: metadata?.categoryKey ?? categoryKey,
    categoryLabel: metadata?.categoryLabel ?? item.display?.categoryLabel ?? getDefenseCategoryLabel(categoryKey),
    level: item.level,
    footprint: item.footprint,
  };
}

function mapOption(item: DefenseOptionDto, stockpile: DefenseCost[], metadataByBuildingType: ReadonlyMap<string, DefenseCatalogItemDto>): DefenseOption {
  const actionKey = `${item.action ?? ""}`;
  const buildingType = `${item.buildingType ?? ""}`;
  const categoryKey = `${item.category ?? ""}`;
  const metadata = metadataByBuildingType.get(buildingType);

  const cost = mapCost(item.cost);

  return {
    actionKey,
    actionLabel: item.display?.actionLabel ?? getDefenseActionLabel(actionKey),
    assetType: unitDefenseAssetTypes[buildingType] ?? null,
    productionModel: getDefenseProductionModel(buildingType),
    buildingType,
    structureLabel: metadata?.displayName ?? item.display?.buildingTypeLabel ?? getDefenseStructureLabel(buildingType),
    categoryKey: metadata?.categoryKey ?? categoryKey,
    categoryLabel: metadata?.categoryLabel ?? item.display?.categoryLabel ?? getDefenseCategoryLabel(categoryKey),
    currentLevel: item.currentLevel,
    targetLevel: item.targetLevel,
    statusKey: item.availabilityStatus,
    statusLabel: item.display?.availabilityLabel ?? getDefenseReadinessLabel(item.availabilityStatus),
    reasonKey: item.availabilityReason,
    reasonLabel: getReasonLabel(item.availabilityReason, item.display?.availabilityReasonLabel),
    estimatedDurationLabel: formatDefenseDuration(item.estimatedDuration),
    estimatedCostLabel: formatDefenseCost(item.cost),
    affordabilityLabel: item.availabilityReason === "Insufficient resources."
      ? getMissingResourceLabel(cost, stockpile)
      : null,
    requirementLabel: item.availabilityReason === "Planet already has an open construction order."
      ? "La cola de Construccion debe quedar libre antes de preparar otra defensa"
      : item.availabilityReason === "Planet is not controlled by the requesting civilization."
        ? "Defensas solo revisa colonias propias en esta fase"
        : item.availabilityReason === "Planet building capacity was not found."
          ? "Sin campos disponibles"
          : item.availabilityReason === "Planet building capacity would be exceeded."
            ? "Sin campos disponibles"
            : item.availabilityReason === "Planet resource stockpile was not found."
              ? "No hay reservas locales suficientes para validar esta preparacion"
        : null,
    cost,
  };
}

function mapQueueItem(item: DefenseQueueItemDto, metadataByBuildingType: ReadonlyMap<string, DefenseCatalogItemDto>): DefenseQueueItem {
  const actionKey = `${item.action ?? ""}`;
  const buildingType = `${item.buildingType ?? ""}`;
  const statusKey = `${item.status ?? ""}`;
  const metadata = metadataByBuildingType.get(buildingType);

  return {
    orderId: item.orderId,
    actionKey,
    actionLabel: item.display?.actionLabel ?? getDefenseActionLabel(actionKey),
    buildingType,
    productionModel: getDefenseProductionModel(buildingType),
    structureLabel: metadata?.displayName ?? item.display?.buildingTypeLabel ?? getDefenseStructureLabel(buildingType),
    statusKey,
    statusLabel: item.display?.statusLabel ?? getDefenseStatusLabel(statusKey),
    targetLevel: item.targetLevel,
    sequence: item.sequence,
    startsAtUtc: item.startsAtUtc,
    endsAtUtc: item.endsAtUtc,
    isDue: item.isDue,
    estimatedCostLabel: formatDefenseCost(item.cost),
  };
}

function mapActionAvailability(
  actionKey: string,
  supported: boolean,
  summary: DefenseActionSummaryDto,
  kind: "queue" | "enqueue" | "completeDue",
): DefenseActionAvailability {
  if (kind === "queue") {
    return {
      key: actionKey,
      label: getDefenseActionLabel(actionKey),
      statusKey: summary.queueActionStatus,
      statusLabel: summary.display?.queueActionStatusLabel ?? getDefenseReadinessLabel(summary.queueActionStatus),
      reasonKey: summary.queueActionReason,
      reasonLabel: summary.display?.queueActionReasonLabel ?? getReasonLabel(summary.queueActionReason),
      supported,
    };
  }

  if (kind === "completeDue") {
    return {
      key: actionKey,
      label: getDefenseActionLabel(actionKey),
      statusKey: summary.completeDueActionStatus,
      statusLabel: summary.display?.completeDueActionStatusLabel ?? getDefenseReadinessLabel(summary.completeDueActionStatus),
      reasonKey: summary.completeDueActionReason,
      reasonLabel: summary.display?.completeDueActionReasonLabel ?? getReasonLabel(summary.completeDueActionReason),
      supported,
    };
  }

  return {
    key: actionKey,
    label: getDefenseActionLabel(actionKey),
    statusKey: summary.queueActionStatus,
    statusLabel: getDefenseReadinessLabel(summary.queueActionStatus),
    reasonKey: summary.queueActionReason,
    reasonLabel: getReasonLabel(summary.queueActionReason),
    supported,
  };
}

function mapDiagnostics(diagnostics: DefenseDiagnosticsDto): DefenseDiagnostics {
  return {
    playerFacing: diagnostics.notes.filter((note) => !note.toLowerCase().includes("development-only")),
    technical: diagnostics.notes,
    limitations: diagnostics.limitations,
  };
}

function mapDefenseContext(defenses: DefensePlanetContextDto): DefensePlanetContext {
  const stockpile = mapCost(defenses.resourceStockpile);
  const metadataByBuildingType = buildCatalogMetadataMap(defenses.catalog);
  const diagnostics = mapDiagnostics(defenses.diagnostics);

  return {
    planetId: defenses.planetId,
    planetName: defenses.planetName,
    solarSystemId: defenses.solarSystemId,
    solarSystemName: defenses.solarSystemName,
    isOwnedByRequestingCivilization: defenses.isOwnedByRequestingCivilization,
    ownerCivilizationId: defenses.ownerCivilizationId,
    ownerCivilizationName: defenses.ownerCivilizationName,
    controlStatus: defenses.controlStatus ? `${defenses.controlStatus}` : null,
    stockpile,
    structures: defenses.defenseStructures.map((item) => mapStructure(item, metadataByBuildingType)),
    options: defenses.defenseOptions.map((option) => mapOption(option, stockpile, metadataByBuildingType)),
    queue: defenses.defenseQueue.map((item) => mapQueueItem(item, metadataByBuildingType)),
    protectionSummary: defenses.protectionSummary,
    actionAvailability: {
      queue: mapActionAvailability("queue.read", defenses.defenseQueue.length > 0, defenses.actionSummary, "queue"),
      enqueue: mapActionAvailability("construction.enqueue", defenses.defenseOptions.some((option) => option.availabilityStatus === "Available"), defenses.actionSummary, "enqueue"),
      completeDue: mapActionAvailability("construction.completeDue", defenses.actionSummary.completeDueSupported, defenses.actionSummary, "completeDue"),
    },
    diagnostics,
  };
}

export function groupDefenseOptionsByCategory(options: readonly DefenseOption[]): DefenseOptionGroup[] {
  const grouped = options.reduce<Map<string, DefenseOptionGroup>>((accumulator, option) => {
    const current = accumulator.get(option.categoryKey);
    if (current) {
      current.options.push(option);
      return accumulator;
    }

    accumulator.set(option.categoryKey, {
      key: option.categoryKey,
      label: option.categoryLabel,
      options: [option],
    });
    return accumulator;
  }, new Map());

  return [...grouped.values()];
}

export function selectRecommendedDefenseAction(options: readonly DefenseOption[]) {
  return options.find((option) => option.statusKey === "Available") ?? options[0] ?? null;
}

export function getDefensePrimaryAction(option: DefenseOption | null | undefined) {
  if (!option) {
    return getDefenseActionLabel("coverage.read");
  }

  return option.statusKey === "Available"
    ? getDefenseActionLabel("construction.link")
    : getDefenseActionLabel("coverage.read");
}

export function mapDefensesUiStateToViewModel(state: DefensesUiStateDto): DefensesViewModel {
  const defenses = state.defenses ? mapDefenseContext(state.defenses) : null;

  return {
    civilizationId: state.civilizationId,
    selectedPlanetId: state.selectedPlanetId,
    defenses,
    diagnostics: defenses?.diagnostics ?? {
      playerFacing: state.errors.length > 0 ? [state.errors[0]] : [],
      technical: [...state.errors],
      limitations: [],
    },
  };
}
