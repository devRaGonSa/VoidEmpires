export type PlanetApiValue = string | number;

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
}

export interface PlanetConstructionActionSummaryDto {
  queueActionStatus: string;
  queueActionReason: string;
  completeDueSupported: boolean;
  completeDueActionStatus: string;
  completeDueActionReason: string;
  dueConstructionCount: number;
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

export interface EnqueuePlanetConstructionResponse {
  succeeded: boolean;
  orderId: string | null;
  startsAtUtc: string | null;
  endsAtUtc: string | null;
  errors: string[];
}
