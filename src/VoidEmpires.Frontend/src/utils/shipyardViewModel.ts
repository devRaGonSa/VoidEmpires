import type {
  ShipyardActionAvailabilityDto,
  ShipyardAssetOptionDto,
  ShipyardAssetStockItemDto,
  ShipyardDiagnosticsDto,
  ShipyardPlanetContextDto,
  ShipyardQueueItemDto,
  ShipyardResourceStockpileItemDto,
  ShipyardUiStateDto,
} from "../api/shipyardTypes";
import {
  formatAssetProductionCost,
  formatAssetProductionDuration,
  formatAssetQuantity,
  getAssetCategoryLabel,
  getAssetProductionStatusLabel,
  getAssetRoleLabel,
  getAssetTypeLabel,
  getShipyardActionLabel,
} from "./shipyardPresentation";

export interface ShipyardCost {
  resourceType: string;
  quantity: number;
}

export interface ShipyardRequirement {
  buildingType: string;
  buildingLevel: number;
  operatorCapacity: number;
}

export interface ShipyardActionAvailability {
  key: string;
  label: string;
  reasonKey: string;
  reasonLabel: string;
  supported: boolean;
}

export interface ShipyardAssetOption {
  assetType: string;
  label: string;
  categoryKey: string;
  categoryLabel: string;
  roleLabel: string;
  quantityLabel: string;
  statusKey: string;
  statusLabel: string;
  reasonKey: string;
  reasonLabel: string;
  currentStock: number;
  estimatedDurationLabel: string;
  estimatedCostLabel: string;
  requirements: ShipyardRequirement;
  cost: ShipyardCost[];
}

export interface ShipyardQueueItem {
  orderId: string;
  assetType: string;
  label: string;
  quantity: number;
  quantityLabel: string;
  sequence: number;
  statusKey: string;
  statusLabel: string;
  startsAtUtc: string;
  endsAtUtc: string;
  isDue: boolean;
}

export interface ShipyardAssetStockItem {
  assetType: string;
  label: string;
  quantity: number;
  quantityLabel: string;
  categoryLabel: string;
}

export interface ShipyardResourceStockpile {
  resourceType: string;
  quantity: number;
}

export interface ShipyardDiagnostics {
  playerFacing: readonly string[];
  technical: readonly string[];
}

export interface ShipyardPlanetContext {
  planetId: string;
  planetName: string;
  solarSystemName: string;
  isOwnedByRequestingCivilization: boolean;
  ownerCivilizationName: string | null;
  controlStatus: string | null;
  stockpile: ShipyardResourceStockpile[];
  buildingReadiness: ShipyardPlanetContextDto["buildingReadiness"];
  catalog: ShipyardAssetOption[];
  queue: ShipyardQueueItem[];
  orbitalStock: ShipyardAssetStockItem[];
  actionAvailability: {
    queue: ShipyardActionAvailability;
    enqueue: ShipyardActionAvailability;
    completeDue: ShipyardActionAvailability;
  };
  diagnostics: ShipyardDiagnostics;
}

export interface ShipyardCategoryGroup {
  key: string;
  label: string;
  assets: ShipyardAssetOption[];
}

export interface ShipyardViewModel {
  civilizationId: string;
  selectedPlanetId: string | null;
  shipyard: ShipyardPlanetContext | null;
  diagnostics: ShipyardDiagnostics;
}

const availabilityReasonLabels: Record<string, string> = {
  Ready: "Lista para producir",
  PlanetIsNotOwned: "Planeta fuera de control",
  OpenProductionOrderExists: "Ya existe una orden orbital abierta",
  MissingResourceStockpile: "Sin reservas locales",
  MissingRequiredBuilding: "Falta infraestructura orbital",
  MissingPopulationProfile: "Sin perfil de tripulacion",
  InsufficientResources: "Recursos insuficientes",
  InsufficientOperatorCapacity: "Tripulacion insuficiente",
  OpenProductionQueueVisible: "La cola orbital tiene actividad",
  NoOpenOrbitalProductionQueue: "Sin cola orbital activa",
  DueOrbitalProductionExists: "Hay produccion vencida visible",
  NoDueOrbitalProduction: "Sin produccion vencida",
  CatalogUnavailable: "Catalogo orbital no disponible",
};

function getAvailabilityReasonLabel(value: string) {
  return availabilityReasonLabels[value] ?? value;
}

function normalizeAssetType(value: string | number | null | undefined) {
  return typeof value === "string" ? value : `${value ?? ""}`;
}

function mapCost(entries: readonly ShipyardResourceStockpileItemDto[]): ShipyardCost[] {
  return entries.map((entry) => ({
    resourceType: `${entry.resourceType ?? ""}`,
    quantity: entry.quantity,
  }));
}

function mapAssetOption(item: ShipyardAssetOptionDto): ShipyardAssetOption {
  const assetType = normalizeAssetType(item.assetType);

  return {
    assetType,
    label: getAssetTypeLabel(assetType),
    categoryKey: getAssetCategoryLabel(assetType),
    categoryLabel: getAssetCategoryLabel(assetType),
    roleLabel: getAssetRoleLabel(assetType),
    quantityLabel: formatAssetQuantity(item.currentStock, assetType),
    statusKey: item.availabilityStatus,
    statusLabel: item.availabilityStatus === "Available" ? "Disponible" : "Bloqueada",
    reasonKey: item.availabilityReason,
    reasonLabel: getAvailabilityReasonLabel(item.availabilityReason),
    currentStock: item.currentStock,
    estimatedDurationLabel: formatAssetProductionDuration(item.estimatedDuration),
    estimatedCostLabel: formatAssetProductionCost(item.cost),
    requirements: {
      buildingType: `${item.requiredBuildingType ?? ""}`,
      buildingLevel: item.requiredBuildingLevel,
      operatorCapacity: item.requiredOperatorCapacity,
    },
    cost: mapCost(item.cost),
  };
}

function mapQueueItem(item: ShipyardQueueItemDto): ShipyardQueueItem {
  const assetType = normalizeAssetType(item.assetType);
  const statusKey = `${item.status ?? ""}`;

  return {
    orderId: item.orderId,
    assetType,
    label: getAssetTypeLabel(assetType),
    quantity: item.quantity,
    quantityLabel: formatAssetQuantity(item.quantity, assetType),
    sequence: item.sequence,
    statusKey,
    statusLabel: getAssetProductionStatusLabel(item.status),
    startsAtUtc: item.startsAtUtc,
    endsAtUtc: item.endsAtUtc,
    isDue: item.isDue,
  };
}

function mapStockItem(item: ShipyardAssetStockItemDto): ShipyardAssetStockItem {
  const assetType = normalizeAssetType(item.assetType);

  return {
    assetType,
    label: getAssetTypeLabel(assetType),
    quantity: item.quantity,
    quantityLabel: formatAssetQuantity(item.quantity, assetType),
    categoryLabel: getAssetCategoryLabel(assetType),
  };
}

function mapActionAvailability(
  actionKey: string,
  supported: boolean,
  statusKey: string,
  reasonKey: string,
): ShipyardActionAvailability {
  return {
    key: actionKey,
    label: getShipyardActionLabel(actionKey),
    reasonKey: supported ? statusKey : reasonKey,
    reasonLabel: getAvailabilityReasonLabel(supported ? statusKey : reasonKey),
    supported,
  };
}

function mapDiagnostics(diagnostics: ShipyardDiagnosticsDto): ShipyardDiagnostics {
  return {
    playerFacing: diagnostics.notes.filter((note) => !note.toLowerCase().includes("development-only")),
    technical: diagnostics.notes,
  };
}

function mapShipyardContext(shipyard: ShipyardPlanetContextDto): ShipyardPlanetContext {
  const catalog = shipyard.catalog.map(mapAssetOption);
  const queue = shipyard.queue.map(mapQueueItem);
  const stock = shipyard.orbitalStock.map(mapStockItem);
  const diagnostics = mapDiagnostics(shipyard.diagnostics);

  return {
    planetId: shipyard.planetId,
    planetName: shipyard.planetName,
    solarSystemName: shipyard.solarSystemName,
    isOwnedByRequestingCivilization: shipyard.isOwnedByRequestingCivilization,
    ownerCivilizationName: shipyard.ownerCivilizationName,
    controlStatus: shipyard.controlStatus ? `${shipyard.controlStatus}` : null,
    stockpile: shipyard.resourceStockpile.map((entry) => ({
      resourceType: `${entry.resourceType ?? ""}`,
      quantity: entry.quantity,
    })),
    buildingReadiness: shipyard.buildingReadiness,
    catalog,
    queue,
    orbitalStock: stock,
    actionAvailability: {
      queue: mapActionAvailability(
        "queue.read",
        shipyard.actionSummary.openQueueCount > 0,
        shipyard.actionSummary.queueActionStatus,
        shipyard.actionSummary.queueActionReason,
      ),
      enqueue: mapActionAvailability(
        "production.enqueue",
        shipyard.actionSummary.enqueueSupported,
        shipyard.actionSummary.enqueueActionStatus,
        shipyard.actionSummary.enqueueActionReason,
      ),
      completeDue: mapActionAvailability(
        "production.completeDue",
        shipyard.actionSummary.completeDueSupported,
        shipyard.actionSummary.completeDueActionStatus,
        shipyard.actionSummary.completeDueActionReason,
      ),
    },
    diagnostics,
  };
}

export function groupAssetOptionsByCategory(assets: readonly ShipyardAssetOption[]): ShipyardCategoryGroup[] {
  const grouped = assets.reduce<Map<string, ShipyardCategoryGroup>>((accumulator, asset) => {
    const current = accumulator.get(asset.categoryKey);
    if (current) {
      current.assets.push(asset);
      return accumulator;
    }

    accumulator.set(asset.categoryKey, {
      key: asset.categoryKey,
      label: asset.categoryLabel,
      assets: [asset],
    });
    return accumulator;
  }, new Map());

  return [...grouped.values()];
}

export function selectRecommendedAssetProduction(assets: readonly ShipyardAssetOption[]) {
  return assets.find((asset) => asset.statusKey === "Available") ?? assets[0] ?? null;
}

export function getShipyardPrimaryAction(asset: ShipyardAssetOption | null | undefined) {
  if (!asset) {
    return getShipyardActionLabel("catalog.read");
  }

  return asset.statusKey === "Available"
    ? getShipyardActionLabel("production.enqueue")
    : getShipyardActionLabel("catalog.read");
}

export function mapShipyardUiStateToViewModel(state: ShipyardUiStateDto): ShipyardViewModel {
  const shipyard = state.shipyard ? mapShipyardContext(state.shipyard) : null;

  return {
    civilizationId: state.civilizationId,
    selectedPlanetId: state.selectedPlanetId,
    shipyard,
    diagnostics: shipyard?.diagnostics ?? {
      playerFacing: state.errors.length > 0 ? [state.errors[0]] : [],
      technical: [...state.errors],
    },
  };
}
