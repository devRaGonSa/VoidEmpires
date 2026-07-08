export type DefenseApiValue = string | number | null | undefined;

export interface DefensePlanetOptionDto {
  planetId: string;
  planetName: string;
  solarSystemId: string;
  solarSystemName: string;
  isOwnedByRequestingCivilization: boolean;
}

export interface DefenseResourceStockpileItemDto {
  resourceType: DefenseApiValue;
  quantity: number;
}

export interface DefenseStructureDisplayDto {
  buildingTypeLabel: string;
  categoryLabel: string;
}

export interface DefenseStructureDto {
  buildingType: DefenseApiValue;
  category: DefenseApiValue;
  level: number;
  footprint: number;
  display?: DefenseStructureDisplayDto | null;
}

export interface DefenseCatalogItemDto {
  buildingType: DefenseApiValue;
  displayName: string;
  description: string;
  categoryKey: string;
  categoryLabel: string;
  roleKey: string;
  roleLabel: string;
  moduleKey: string;
  moduleLabel: string;
  imageKey: string;
  iconKey: string;
  sortOrder: number;
  futureProductionRequirementKey: string;
  futureProductionRequirementLabel: string;
  futureCombatDependencyKey: string;
  futureCombatDependencyLabel: string;
  tags: readonly string[];
}

export interface DefenseOptionDisplayDto {
  actionLabel: string;
  buildingTypeLabel: string;
  categoryLabel: string;
  availabilityLabel: string;
  availabilityReasonLabel: string;
}

export interface DefenseOptionDto {
  action: DefenseApiValue;
  buildingType: DefenseApiValue;
  category: DefenseApiValue;
  currentLevel: number;
  targetLevel: number;
  availabilityStatus: string;
  availabilityReason: string;
  estimatedDuration: string;
  cost: readonly DefenseResourceStockpileItemDto[];
  display?: DefenseOptionDisplayDto | null;
}

export interface DefenseQueueItemDisplayDto {
  actionLabel: string;
  statusLabel: string;
  buildingTypeLabel: string;
  categoryLabel: string;
}

export interface DefenseQueueItemDto {
  orderId: string;
  action: DefenseApiValue;
  status: DefenseApiValue;
  buildingType: DefenseApiValue;
  category: DefenseApiValue;
  targetLevel: number;
  sequence: number;
  startsAtUtc: string;
  endsAtUtc: string;
  isDue: boolean;
  cost: readonly DefenseResourceStockpileItemDto[];
  display?: DefenseQueueItemDisplayDto | null;
}

export interface DefenseProtectionSummaryDto {
  structureCount: number;
  totalDefenseLevel: number;
  availableOptionCount: number;
  blockedOptionCount: number;
  queueItemCount: number;
  dueQueueItemCount: number;
}

export interface DefenseActionSummaryDisplayDto {
  queueActionStatusLabel: string;
  queueActionReasonLabel: string;
  completeDueActionStatusLabel: string;
  completeDueActionReasonLabel: string;
}

export interface DefenseActionSummaryDto {
  queueActionStatus: string;
  queueActionReason: string;
  completeDueSupported: boolean;
  completeDueActionStatus: string;
  completeDueActionReason: string;
  dueConstructionCount: number;
  display?: DefenseActionSummaryDisplayDto | null;
}

export interface DefenseDiagnosticsDto {
  requestPlanetId: string | null;
  solarSystemId: string;
  ownerCivilizationId: string | null;
  hasResourceStockpile: boolean;
  hasDefenseStructure: boolean;
  notes: readonly string[];
  limitations: readonly string[];
}

export interface DefensePlanetContextDto {
  planetId: string;
  planetName: string;
  solarSystemId: string;
  solarSystemName: string;
  isOwnedByRequestingCivilization: boolean;
  ownerCivilizationId: string | null;
  ownerCivilizationName: string | null;
  controlStatus: DefenseApiValue;
  resourceStockpile: readonly DefenseResourceStockpileItemDto[];
  catalog: readonly DefenseCatalogItemDto[];
  defenseStructures: readonly DefenseStructureDto[];
  defenseOptions: readonly DefenseOptionDto[];
  defenseQueue: readonly DefenseQueueItemDto[];
  protectionSummary: DefenseProtectionSummaryDto;
  actionSummary: DefenseActionSummaryDto;
  diagnostics: DefenseDiagnosticsDto;
}

export interface DefensesUiStateDto {
  civilizationId: string;
  selectedPlanetId: string | null;
  knownPlanets: readonly DefensePlanetOptionDto[];
  defenses: DefensePlanetContextDto | null;
  errors: readonly string[];
}

export interface DefensesUiStateResponse {
  succeeded: boolean;
  uiState: DefensesUiStateDto | null;
  errors: readonly string[];
}

export interface EnqueueDefenseProductionRequest {
  civilizationId: string;
  planetId: string;
  assetType: string;
  quantity: number;
  requestedAtUtc: string;
}

export interface EnqueueDefenseProductionResponse {
  succeeded: boolean;
  orderId: string | null;
  startsAtUtc: string | null;
  endsAtUtc: string | null;
  errors: readonly string[];
}

export interface EnqueueDefenseProductionResult {
  httpStatus: number;
  response: EnqueueDefenseProductionResponse | null;
}
