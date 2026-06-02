export type FleetCommandResultStatus = number | string;
export interface FleetCommandApiResult<TResponse> {
  httpStatus: number;
  response: TResponse | null;
}
export interface EstimateOrbitalTravelRequest {
  civilizationId: string;
  orbitalGroupId: string;
  destinationPlanetId: string;
}
export interface CreateOrbitalTransferRequest {
  civilizationId: string;
  orbitalGroupId: string;
  destinationPlanetId: string;
  requestedAtUtc: string;
}
export interface CancelOrbitalTransferRequest {
  civilizationId: string;
  orbitalTransferId: string;
}
export interface CompleteOrbitalTransfersRequest {
  nowUtc: string;
}
export interface SplitOrbitalGroupRequest {
  civilizationId: string;
  sourceOrbitalGroupId: string;
  quantity: number;
}
export interface MergeOrbitalGroupsRequest {
  civilizationId: string;
  targetOrbitalGroupId: string;
  sourceOrbitalGroupId: string;
}
interface FleetCommandResponseBase {
  succeeded: boolean;
  errors: string[];
}
export interface OrbitalTravelCostComponent {
  resourceType: string;
  quantity: number;
}
export interface OrbitalTravelInsufficientResource {
  resourceType: string;
  requiredQuantity: number;
  availableQuantity: number;
}
export interface OrbitalRouteProfile {
  routeClass: string | number;
  distanceBand: number;
  riskBand: string | number;
  fuelMultiplier: number;
  complexityNotes: string[];
  isSupported: boolean;
}
export interface OrbitalFuelReadiness {
  estimatedFuelUnitsRequired: number;
  estimatedRangeUnitsAvailable: number;
  isFuelReady: boolean;
  notReadyReason?: string | null;
  policy: string | number;
}
export interface EstimateOrbitalTravelResponse extends FleetCommandResponseBase {
  status: FleetCommandResultStatus;
  orbitalGroupId?: string | null;
  currentPlanetId?: string | null;
  destinationPlanetId?: string | null;
  abstractDistanceUnits: number;
  estimatedDuration?: string | null;
  routeProfile?: OrbitalRouteProfile | null;
  fuelReadiness?: OrbitalFuelReadiness | null;
  resourceCosts: OrbitalTravelCostComponent[];
  canAfford: boolean;
  insufficientResources: OrbitalTravelInsufficientResource[];
}
export interface CreateOrbitalTransferResponse extends FleetCommandResponseBase {
  status: FleetCommandResultStatus;
  orbitalTransferId?: string | null;
  orbitalGroupId?: string | null;
  originPlanetId?: string | null;
  destinationPlanetId?: string | null;
  abstractDistanceUnits: number;
  departureAtUtc?: string | null;
  arrivalAtUtc?: string | null;
}
export interface CancelOrbitalTransferResponse extends FleetCommandResponseBase {
  status: FleetCommandResultStatus;
  orbitalTransferId?: string | null;
  orbitalGroupId?: string | null;
}
export interface CompleteOrbitalTransfersResponse extends FleetCommandResponseBase {
  completedCount: number;
  completedTransferIds: string[];
  completedOrbitalGroupIds: string[];
}
export interface SplitOrbitalGroupResponse extends FleetCommandResponseBase {
  status: FleetCommandResultStatus;
  sourceOrbitalGroupId?: string | null;
  newOrbitalGroupId?: string | null;
  sourceQuantity: number;
  newQuantity: number;
}
export interface MergeOrbitalGroupsResponse extends FleetCommandResponseBase {
  status: FleetCommandResultStatus;
  targetOrbitalGroupId?: string | null;
  sourceOrbitalGroupId?: string | null;
  targetQuantity: number;
}
