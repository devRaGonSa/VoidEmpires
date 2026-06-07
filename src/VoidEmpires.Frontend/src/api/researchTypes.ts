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
  enqueueCommand?: ResearchEnqueueCommandDto | null;
  requirementKeys: readonly ResearchApiValue[];
}

export interface ResearchEnqueueCommandDto {
  actionKey: string;
  method: string;
  route: string;
  civilizationId: string;
  sourcePlanetId: string;
  researchType: ResearchApiValue;
  targetLevel: number;
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

export interface EnqueueResearchOrderRequest {
  civilizationId: string;
  sourcePlanetId: string;
  researchType: string;
  requestedAtUtc: string;
}

export type ResearchApiErrorCode =
  | "MissingCivilizationId"
  | "MissingSourcePlanetId"
  | "MissingResearchType"
  | "MissingRequestedAtUtc"
  | "RequestedAtUtcNotUtc"
  | "SourcePlanetNotOwned"
  | "OpenResearchOrderExists"
  | "SourcePlanetStockpileMissing"
  | "InsufficientResources"
  | "UnknownValidationFailure";

export interface ResearchApiErrorEntry {
  code: ResearchApiErrorCode;
  message: string;
  rawMessage: string;
}

export interface EnqueueResearchOrderSuccessResponse {
  succeeded: true;
  orderId: string;
  startsAtUtc: string;
  endsAtUtc: string;
  errors: readonly [];
}

export interface EnqueueResearchOrderFailureResponse {
  succeeded: false;
  orderId: string | null;
  startsAtUtc: string | null;
  endsAtUtc: string | null;
  errors: readonly string[];
  errorEntries: readonly ResearchApiErrorEntry[];
  failureKind: "validation" | "conflict" | "unknown";
  isOpenOrderNoOp: boolean;
}

export type EnqueueResearchOrderResponse =
  | EnqueueResearchOrderSuccessResponse
  | EnqueueResearchOrderFailureResponse;

export interface EnqueueResearchOrderCommandResult {
  httpStatus: number;
  hasJsonBody: boolean;
  bodyParseFailed: boolean;
  response: EnqueueResearchOrderResponse | null;
}
