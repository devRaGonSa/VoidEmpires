export type ShipyardApiValue = string | number | null | undefined;

export interface ShipyardResourceStockpileItemDto {
  resourceType: ShipyardApiValue;
  quantity: number;
}

export interface ShipyardBuildingReadinessDto {
  shipyardLevel: number;
  fleetCommandCenterLevel: number;
  logisticsHubLevel: number;
  hasPopulationProfile: boolean;
}

export interface ShipyardAssetOptionDto {
  assetType: ShipyardApiValue;
  requiredBuildingType: ShipyardApiValue;
  requiredBuildingLevel: number;
  requiredOperatorCapacity: number;
  estimatedDuration: string;
  cost: readonly ShipyardResourceStockpileItemDto[];
  currentStock: number;
  availabilityStatus: string;
  availabilityReason: string;
  enqueueCommand?: ShipyardEnqueueCommandDto | null;
}

export interface ShipyardEnqueueCommandDto {
  actionKey: string;
  method: string;
  route: string;
  civilizationId: string;
  planetId: string;
  target: ShipyardApiValue;
  spaceAssetType: ShipyardApiValue;
  quantity: number;
}

export interface ShipyardQueueItemDto {
  orderId: string;
  assetType: ShipyardApiValue;
  quantity: number;
  sequence: number;
  status: ShipyardApiValue;
  startsAtUtc: string;
  endsAtUtc: string;
  isDue: boolean;
}

export interface ShipyardAssetStockItemDto {
  assetType: ShipyardApiValue;
  quantity: number;
}

export interface ShipyardActionAvailabilityDto {
  queueActionStatus: string;
  queueActionReason: string;
  enqueueSupported: boolean;
  enqueueActionStatus: string;
  enqueueActionReason: string;
  completeDueSupported: boolean;
  completeDueActionStatus: string;
  completeDueActionReason: string;
  openQueueCount: number;
  dueQueueCount: number;
}

export interface ShipyardDiagnosticsDto {
  requestPlanetId: string | null;
  homePlanetId: string | null;
  hasResourceStockpile: boolean;
  hasOwnedShipyardBuilding: boolean;
  hasPopulationProfile: boolean;
  notes: readonly string[];
}

export interface ShipyardPlanetContextDto {
  planetId: string;
  planetName: string;
  solarSystemId: string;
  solarSystemName: string;
  isOwnedByRequestingCivilization: boolean;
  ownerCivilizationId: string | null;
  ownerCivilizationName: string | null;
  controlStatus: ShipyardApiValue;
  resourceStockpile: readonly ShipyardResourceStockpileItemDto[];
  buildingReadiness: ShipyardBuildingReadinessDto;
  catalog: readonly ShipyardAssetOptionDto[];
  queue: readonly ShipyardQueueItemDto[];
  orbitalStock: readonly ShipyardAssetStockItemDto[];
  actionSummary: ShipyardActionAvailabilityDto;
  diagnostics: ShipyardDiagnosticsDto;
}

export interface ShipyardPlanetOptionDto {
  planetId: string;
  planetName: string;
  solarSystemId: string;
  solarSystemName: string;
  isOwnedByRequestingCivilization: boolean;
}

export interface ShipyardUiStateDto {
  civilizationId: string;
  selectedPlanetId: string | null;
  knownPlanets: readonly ShipyardPlanetOptionDto[];
  shipyard: ShipyardPlanetContextDto | null;
  errors: readonly string[];
}

export interface ShipyardUiStateResponse {
  succeeded: boolean;
  uiState: ShipyardUiStateDto | null;
  errors: readonly string[];
}

export interface EnqueueShipyardProductionRequest {
  civilizationId: string;
  planetId: string;
  assetType: string;
  quantity: number;
  requestedAtUtc: string;
  route?: string | null;
  target?: ShipyardApiValue;
}

export type ShipyardApiErrorCode =
  | "MissingCivilizationId"
  | "MissingPlanetId"
  | "MissingAssetType"
  | "InvalidAssetType"
  | "MissingQuantity"
  | "MissingRequestedAtUtc"
  | "RequestedAtUtcNotUtc"
  | "PlanetNotOwned"
  | "OpenProductionOrderExists"
  | "PlanetStockpileMissing"
  | "InsufficientResources"
  | "MissingRequiredBuilding"
  | "PopulationProfileMissing"
  | "InsufficientOperatorCapacity"
  | "UnknownValidationFailure";

export interface ShipyardApiErrorEntry {
  code: ShipyardApiErrorCode;
  message: string;
  rawMessage: string;
}

export interface EnqueueShipyardProductionSuccessResponse {
  succeeded: true;
  orderId: string;
  startsAtUtc: string;
  endsAtUtc: string;
  errors: readonly [];
}

export interface EnqueueShipyardProductionFailureResponse {
  succeeded: false;
  orderId: string | null;
  startsAtUtc: string | null;
  endsAtUtc: string | null;
  errors: readonly string[];
  errorEntries: readonly ShipyardApiErrorEntry[];
  failureKind: "validation" | "conflict" | "unknown";
}

export type EnqueueShipyardProductionResponse =
  | EnqueueShipyardProductionSuccessResponse
  | EnqueueShipyardProductionFailureResponse;

export interface EnqueueShipyardProductionCommandResult {
  httpStatus: number;
  hasJsonBody: boolean;
  bodyParseFailed: boolean;
  response: EnqueueShipyardProductionResponse | null;
}
