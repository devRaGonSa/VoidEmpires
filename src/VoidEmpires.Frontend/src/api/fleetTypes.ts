import type { ReadinessNote } from "./strategicMapTypes";

export interface FleetUiStateResponse {
  succeeded: boolean;
  uiState: FleetUiState | null;
  errors: string[];
}

export interface FleetUiState {
  civilizationId: string;
  selectedPlanetId?: string | null;
  planets?: FleetPlanetOption[];
  localStock?: FleetOrbitalStock[];
  groups: FleetGroupSummary[];
  resourceContexts?: FleetResourceContext[];
  actionHints?: FleetActionHint[];
  interceptionNotes?: ReadinessNote[];
}

export interface FleetPlanetOption { planetId: string; planetName: string; isOwnedByRequestingCivilization: boolean; }
export interface FleetOrbitalStock { assetType: string; quantity: number; }

export interface FleetGroupSummary {
  id: string;
  civilizationId: string;
  originPlanetId: string;
  currentPlanetId: string;
  assetType: string;
  quantity: number;
  status: string;
  isStationedAwayFromOrigin: boolean;
  hasActiveTransfer?: boolean;
  activeTransfer?: FleetActiveTransfer | null;
  commands?: FleetCommandAvailability | null;
  routeFuelReadiness?: FleetRouteFuelReadiness | null;
}

export interface FleetActiveTransfer {
  id: string;
  destinationPlanetId: string;
  abstractDistanceUnits: number;
  departureAtUtc: string;
  arrivalAtUtc: string;
  status: string;
  interceptionReadiness?: {
    opportunityStatus?: string;
    readinessNote?: string | null;
    detectionNote?: string | null;
    blockReasons?: string[];
  } | null;
}

export interface FleetCommandAvailability {
  canCreateTransfer?: boolean;
  canSplit?: boolean;
  canMerge?: boolean;
  canCancelTransfer?: boolean;
}

export interface FleetRouteFuelReadiness {
  canRequestTravelEstimate?: boolean;
  requiresDestination?: boolean;
  estimateActionKey?: string;
  estimateRoute?: string;
  fuelReadinessPolicy?: string;
  notes?: string[];
}

export interface FleetResourceContext {
  planetId: string;
  balances?: Array<{
    resourceType: string;
    quantity: number;
  }>;
}

export interface FleetActionHint {
  actionKey: string;
  displayName: string;
  route: string;
  method: string;
  isReadOnly: boolean;
  notes?: string[];
}
