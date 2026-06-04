export type MarketApiValue = string | number | null | undefined;

export interface MarketResourceReserveDto {
  resourceType: MarketApiValue;
  quantity: number;
}

export interface MarketProductionSummaryDto {
  creditsPerHour: number;
  metalPerHour: number;
  crystalPerHour: number;
  gasPerHour: number;
}

export interface MarketReferenceRatioDto {
  resourceType: MarketApiValue;
  advisoryRatio: number;
  confidenceKey: string;
  basisKey: string;
  isAdvisory: boolean;
}

export interface MarketSignalDto {
  signalKey: string;
  resourceType: MarketApiValue;
  quantity: number;
  productionPerHour: number | null;
  severityKey: string;
  reasonKey: string;
}

export interface MarketFutureActionDto {
  actionKey: string;
  isEnabled: boolean;
  stateKey: string;
  reasonKey: string;
}

export interface MarketLogisticsSummaryDto {
  stationedGroupsAtSelectedPlanet: number;
  activeDeparturesFromSelectedPlanet: number;
  activeArrivalsToSelectedPlanet: number;
  civilizationActiveTransfers: number;
  hasFutureRoutePlaceholders: boolean;
}

export interface MarketDiagnosticsDto {
  requestPlanetId: string | null;
  homePlanetId: string | null;
  ownedPlanetCount: number;
  hasSelectedPlanetStockpile: boolean;
  hasSelectedPlanetProduction: boolean;
  notes: readonly string[];
}

export interface MarketPlanetOptionDto {
  planetId: string;
  planetName: string;
  solarSystemId: string;
  solarSystemName: string;
  isOwnedByRequestingCivilization: boolean;
  visibleReserves: readonly MarketResourceReserveDto[];
  hasProductionProfile: boolean;
}

export interface MarketCockpitDto {
  selectedPlanetId: string | null;
  selectedPlanetName: string | null;
  selectedSolarSystemId: string | null;
  selectedSolarSystemName: string | null;
  civilizationReserves: readonly MarketResourceReserveDto[];
  selectedPlanetReserves: readonly MarketResourceReserveDto[];
  selectedPlanetProduction: MarketProductionSummaryDto | null;
  referenceRatios: readonly MarketReferenceRatioDto[];
  signals: readonly MarketSignalDto[];
  futureActions: readonly MarketFutureActionDto[];
  logistics: MarketLogisticsSummaryDto;
  diagnostics: MarketDiagnosticsDto;
  limitations: readonly string[];
}

export interface MarketUiStateDto {
  civilizationId: string;
  selectedPlanetId: string | null;
  knownPlanets: readonly MarketPlanetOptionDto[];
  market: MarketCockpitDto | null;
  errors: readonly string[];
}

export interface MarketUiStateResponse {
  succeeded: boolean;
  uiState: MarketUiStateDto | null;
  errors: readonly string[];
}
