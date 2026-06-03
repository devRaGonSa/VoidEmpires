export type ResearchApiValue = string | number | null | undefined;

export interface ResearchCost {
  credits: number;
  metal: number;
  crystal: number;
  gas: number;
}

export interface ResearchDefinitionDto {
  researchType: ResearchApiValue;
  baseCost: ResearchCost;
  bonusKey: ResearchApiValue;
}

export interface ResearchQueueItemDto {
  orderId: string;
  civilizationId: string;
  sourcePlanetId: string;
  researchType: ResearchApiValue;
  targetLevel: number;
  sequence: number;
  startsAtUtc: string;
  endsAtUtc: string;
  status: ResearchApiValue;
}

export interface ResearchProjectDto {
  civilizationId: string;
  researchType: ResearchApiValue;
  level: number;
}

export interface ResearchTechnologyHintDto {
  researchType: ResearchApiValue;
  currentLevel: number;
  nextLevel: number;
  statusKey: string;
  availabilityReasonKey: string;
  canEnqueue: boolean;
  canCompleteDue: boolean;
  estimatedDuration: string;
  estimatedCost: ResearchCost;
  requirementKeys: readonly ResearchApiValue[];
}

export interface ResearchUiStateDto {
  civilizationId: string;
  selectedPlanetId: string | null;
  selectedPlanetName: string | null;
  catalog: readonly ResearchDefinitionDto[];
  queue: readonly ResearchQueueItemDto[];
  projects: readonly ResearchProjectDto[];
  technologyHints: readonly ResearchTechnologyHintDto[];
  diagnostics: readonly string[];
  limitations: readonly string[];
}

export interface ResearchUiStateResponse {
  succeeded: boolean;
  uiState: ResearchUiStateDto | null;
  errors: readonly string[];
}
