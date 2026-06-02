import { appConfig } from "../config";
import type { ActionManifestResponse } from "./actionManifestTypes";
import type { FleetUiStateResponse } from "./fleetTypes";
import type { StrategicMapResponse } from "./strategicMapTypes";
function buildUrl(path: string, query?: Record<string, string>) {
  const url = new URL(path, appConfig.apiBaseUrl);

  if (query) {
    Object.entries(query).forEach(([key, value]) => {
      url.searchParams.set(key, value);
    });
  }

  return url.toString();
}

async function requestJson<T>(path: string, query?: Record<string, string>): Promise<T> {
  const response = await fetch(buildUrl(path, query), {
    headers: {
      Accept: "application/json",
    },
  });

  if (!response.ok) {
    throw new Error(`Request failed with status ${response.status}.`);
  }

  return (await response.json()) as T;
}

export interface HealthResponse {
  status: string;
  service: string;
  persistence: {
    configured: boolean;
    provider: string;
  };
  auth: {
    configured: boolean;
    provider: string;
  };
}

export interface SystemVisualStateResponse {
  succeeded: boolean;
  visualState: {
    systemId: string;
    systemName: string;
    star?: { starType?: string; visualClass?: string };
    layoutHints?: Array<{ planetId: string }>;
    orbitalGroupMarkers?: Array<{ orbitalGroupId: string }>;
    transferOverlays?: Array<{ transferId: string }>;
  } | null;
  errors: string[];
}

export interface PlanetVisualStateResponse {
  succeeded: boolean;
  visualState: {
    planetId: string;
    planetName: string;
    planetType?: string;
    colonizationStatus?: string;
    visualSeed?: number;
    colonizationIntensity?: number;
    urbanIntensity?: number;
    industrialIntensity?: number;
    terraformingIntensity?: number;
    militaryIntensity?: number;
    orbitalPresenceIntensity?: number;
    profile?: {
      paletteKey?: string;
      surfaceProfile?: string;
      lightDistributionMode?: string;
      platformMode?: string;
      atmosphereProfile?: string;
      cloudProfile?: string;
      supportsNightLights?: boolean;
      supportsSurfacePlatforms?: boolean;
      supportsOrbitalMegastructureHints?: boolean;
    };
  } | null;
  errors: string[];
}

export const voidEmpiresApi = {
  getHealth() {
    return requestJson<HealthResponse>("/health");
  },
  getStrategicMap(civilizationId: string) {
    return requestJson<StrategicMapResponse>("/api/dev/strategic-map", {
      civilizationId,
    });
  },
  getFleetUiState(civilizationId: string) {
    return requestJson<FleetUiStateResponse>("/api/dev/fleets/ui-state", {
      civilizationId,
    });
  },
  getFleetActionManifest() {
    return requestJson<ActionManifestResponse>("/api/dev/fleets/action-manifest");
  },
  getStrategicMapActionManifest() {
    return requestJson<ActionManifestResponse>("/api/dev/strategic-map/action-manifest");
  },
  getSystemVisualState(systemId: string) {
    return requestJson<SystemVisualStateResponse>(`/api/dev/solar-systems/${systemId}/visual-state`);
  },
  getPlanetVisualState(planetId: string) {
    return requestJson<PlanetVisualStateResponse>(`/api/dev/planets/${planetId}/visual-state`);
  },
};
