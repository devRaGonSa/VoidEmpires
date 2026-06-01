export interface StrategicMapResponse {
  succeeded: boolean;
  map: StrategicMapResult | null;
  errors: string[];
}

export interface StrategicMapResult {
  civilizationId: string;
  systems: StrategicMapSystem[];
  routeFuelNotes?: ReadinessNote[];
  sensorNotes?: ReadinessNote[];
  detectionNotes?: ReadinessNote[];
  interceptionNotes?: ReadinessNote[];
  allianceNotes?: ReadinessNote[];
  alliancePactNotes?: ReadinessNote[];
  allianceReadiness?: StrategicMapReadinessItem[];
  alliancePacts?: StrategicMapReadinessItem[];
}

export interface StrategicMapSystem {
  systemId: string;
  systemName: string | null;
  coordinateX: number | null;
  coordinateY: number | null;
  coordinateZ: number | null;
  visibilityLevel: string;
  visibilityReason: string;
  isVisible: boolean;
  planets?: StrategicMapPlanet[];
  fleetPresence?: StrategicMapReadinessItem[];
  transferOverlays?: StrategicMapReadinessItem[];
  sensorProfiles?: StrategicMapReadinessItem[];
  detectionCoverage?: StrategicMapReadinessItem[];
}

export interface StrategicMapPlanet {
  planetId: string;
  planetName: string | null;
  visibilityLevel: string;
  visibilityReason: string;
  isVisible: boolean;
}

export interface StrategicMapReadinessItem {
  note?: string | null;
  name?: string | null;
  tag?: string | null;
  [key: string]: unknown;
}

export type ReadinessNote = string | { note?: string | null; [key: string]: unknown };
