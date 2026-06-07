export type PlanetApiValue = string | number;

export interface PlanetCatalogEntry {
  id: number;
  key: string;
  label: string;
}

export const planetBuildingCategoryCatalog: readonly PlanetCatalogEntry[] = [
  { id: 1, key: "Civilian", label: "Civil" },
  { id: 2, key: "Industrial", label: "Industrial" },
  { id: 3, key: "Research", label: "Investigacion" },
  { id: 4, key: "MilitaryGround", label: "Militar terrestre" },
  { id: 5, key: "MilitarySpace", label: "Militar espacial" },
  { id: 6, key: "Defense", label: "Defensa" },
  { id: 7, key: "Logistics", label: "Logistica" },
] as const;

export const planetBuildingTypeCatalog: readonly PlanetCatalogEntry[] = [
  { id: 1, key: "CommandCenter", label: "Centro de mando" },
  { id: 2, key: "MetalMine", label: "Mina de metal" },
  { id: 3, key: "CrystalMine", label: "Mina de cristal" },
  { id: 4, key: "GasExtractor", label: "Extractor de gas" },
  { id: 5, key: "SolarPlant", label: "Planta solar" },
  { id: 6, key: "ResearchLab", label: "Laboratorio de investigacion" },
  { id: 7, key: "Shipyard", label: "Astillero" },
  { id: 8, key: "DefenseGrid", label: "Malla defensiva" },
  { id: 9, key: "HabitationDistrict", label: "Distrito habitacional" },
  { id: 10, key: "MedicalCenter", label: "Centro medico" },
  { id: 11, key: "MilitaryAcademy", label: "Academia militar" },
  { id: 12, key: "Barracks", label: "Barracones" },
  { id: 13, key: "CrewAcademy", label: "Academia de tripulacion" },
  { id: 14, key: "FleetCommandCenter", label: "Mando de flota" },
  { id: 15, key: "LogisticsHub", label: "Centro logistico" },
] as const;

export const planetConstructionActionCatalog: readonly PlanetCatalogEntry[] = [
  { id: 1, key: "Construct", label: "Construir" },
  { id: 2, key: "Upgrade", label: "Mejorar" },
] as const;

export type PlanetModule =
  | "PlanetOverview"
  | "GeneralConstruction"
  | "Research"
  | "GroundArmy"
  | "Shipyard"
  | "Defenses"
  | "Logistics"
  | "UnknownOrDiagnostics";

// Sidebar-aligned module labels used by the planet cockpit surfaces.
export const planetModuleCatalog: readonly PlanetCatalogEntry[] = [
  { id: 1, key: "PlanetOverview", label: "Planeta" },
  { id: 2, key: "GeneralConstruction", label: "Construccion" },
  { id: 3, key: "Research", label: "Investigacion" },
  { id: 4, key: "GroundArmy", label: "Ejercito Tierra" },
  { id: 5, key: "Shipyard", label: "Astillero" },
  { id: 6, key: "Defenses", label: "Defensas" },
  { id: 7, key: "Logistics", label: "Logistica" },
  { id: 8, key: "UnknownOrDiagnostics", label: "Diagnostico" },
] as const;

export interface PlanetUiStateResponse {
  succeeded: boolean;
  uiState: PlanetUiStateResult | null;
  errors: string[];
}

export interface PlanetUiStateResult {
  civilizationId: string;
  selectedPlanetId: string | null;
  knownPlanets: PlanetOptionDto[];
  planet: PlanetCockpitDto | null;
  errors: string[];
}

export interface PlanetOptionDto {
  planetId: string;
  planetName: string;
  solarSystemId: string;
  solarSystemName: string;
  isOwnedByRequestingCivilization: boolean;
}

export interface PlanetCockpitDto {
  planetId: string;
  planetName: string;
  solarSystemId: string;
  solarSystemName: string;
  orbitalSlot: number;
  planetType: PlanetApiValue;
  size: number;
  colonizationStatus: PlanetApiValue;
  isOwnedByRequestingCivilization: boolean;
  ownerCivilizationId: string | null;
  ownerCivilizationName: string | null;
  controlStatus: PlanetApiValue | null;
  stockpile: PlanetResourceBalanceDto[];
  productionSummary: PlanetProductionSummaryDto | null;
  buildingCapacity: PlanetBuildingCapacityDto | null;
  buildings: PlanetBuildingDto[];
  constructionQueue: PlanetConstructionQueueItemDto[];
  actionSummary: PlanetConstructionActionSummaryDto;
  constructionActions: PlanetConstructionActionDto[];
  orbitalContext: PlanetOrbitalContextDto;
  diagnostics: PlanetDiagnosticsDto;
}

export interface PlanetResourceBalanceDto {
  resourceType: PlanetApiValue;
  quantity: number;
}

export interface PlanetProductionSummaryDto {
  creditsPerHour: number;
  metalPerHour: number;
  crystalPerHour: number;
  gasPerHour: number;
  researchMultiplier: number;
}

export interface PlanetBuildingCapacityDto {
  usedCapacity: number;
  baseCapacity: number;
  persistentBonusCapacity: number;
  researchBonusCapacity: number;
  totalAvailableCapacity: number;
}

export interface PlanetBuildingDto {
  buildingType: PlanetApiValue;
  category: PlanetApiValue;
  level: number;
  footprint: number;
  display?: PlanetBuildingDisplayDto | null;
}

export interface PlanetConstructionQueueItemDto {
  orderId: string;
  action: PlanetApiValue;
  status: PlanetApiValue;
  buildingType: PlanetApiValue;
  category: PlanetApiValue;
  targetLevel: number;
  sequence: number;
  startsAtUtc: string;
  endsAtUtc: string;
  isDue: boolean;
  cost: PlanetResourceBalanceDto[];
  display?: PlanetConstructionQueueItemDisplayDto | null;
}

export interface PlanetConstructionActionSummaryDto {
  queueActionStatus: string;
  queueActionReason: string;
  completeDueSupported: boolean;
  completeDueActionStatus: string;
  completeDueActionReason: string;
  dueConstructionCount: number;
  display?: PlanetConstructionActionSummaryDisplayDto | null;
}

export interface PlanetConstructionActionDto {
  action: PlanetApiValue;
  buildingType: PlanetApiValue;
  category: PlanetApiValue;
  currentLevel: number;
  targetLevel: number;
  availabilityStatus: string;
  availabilityReason: string;
  estimatedDuration: string;
  cost: PlanetResourceBalanceDto[];
  display?: PlanetConstructionActionDisplayDto | null;
}

export interface PlanetBuildingDisplayDto {
  buildingTypeLabel: string;
  categoryLabel: string;
}

export interface PlanetConstructionQueueItemDisplayDto {
  actionLabel: string;
  statusLabel: string;
  buildingTypeLabel: string;
  categoryLabel: string;
}

export interface PlanetConstructionActionSummaryDisplayDto {
  queueActionStatusLabel: string;
  queueActionReasonLabel: string;
  completeDueActionStatusLabel: string;
  completeDueActionReasonLabel: string;
}

export interface PlanetConstructionActionDisplayDto {
  actionLabel: string;
  buildingTypeLabel: string;
  categoryLabel: string;
  availabilityLabel: string;
  availabilityReasonLabel: string;
}

export interface PlanetOrbitalContextDto {
  stationedGroups: number;
  activeDepartures: number;
  activeArrivals: number;
}

export interface PlanetDiagnosticsDto {
  requestPlanetId: string | null;
  solarSystemId: string;
  ownerCivilizationId: string | null;
  homePlanetId: string | null;
  hasResourceStockpile: boolean;
  hasProductionProfile: boolean;
  hasBuildingCapacity: boolean;
  openConstructionOrderCount: number;
  notes: string[];
}

export interface EnqueuePlanetConstructionRequest {
  planetId: string;
  civilizationId: string;
  action: PlanetApiValue;
  buildingType: PlanetApiValue;
  requestedAtUtc: string;
}

export interface PlanetApiErrorEntry {
  message: string;
  rawMessage: string;
}

export interface EnqueuePlanetConstructionSuccessResponse {
  succeeded: boolean;
  orderId: string;
  startsAtUtc: string;
  endsAtUtc: string;
  errors: readonly string[];
}

export interface EnqueuePlanetConstructionFailureResponse {
  succeeded: boolean;
  orderId: string | null;
  startsAtUtc: string | null;
  endsAtUtc: string | null;
  errors: readonly string[];
  errorEntries: readonly PlanetApiErrorEntry[];
}

export type EnqueuePlanetConstructionResponse =
  | EnqueuePlanetConstructionSuccessResponse
  | EnqueuePlanetConstructionFailureResponse;

export interface EnqueuePlanetConstructionCommandResult {
  httpStatus: number;
  hasJsonBody: boolean;
  bodyParseFailed: boolean;
  response: EnqueuePlanetConstructionResponse | null;
}
