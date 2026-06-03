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
