import { appConfig } from "../config";
import type { ActionManifestResponse } from "./actionManifestTypes";
import type {
  CancelOrbitalTransferRequest,
  CancelOrbitalTransferResponse,
  CompleteOrbitalTransfersRequest,
  CompleteOrbitalTransfersResponse,
  CreateOrbitalTransferRequest,
  CreateOrbitalTransferResponse,
  EstimateOrbitalTravelRequest,
  EstimateOrbitalTravelResponse,
  FleetCommandApiResult,
  MergeOrbitalGroupsRequest,
  MergeOrbitalGroupsResponse,
  SplitOrbitalGroupRequest,
  SplitOrbitalGroupResponse,
} from "./fleetCommandTypes";
import type { FleetUiStateResponse } from "./fleetTypes";
import type {
  EnqueuePlanetConstructionCommandResult,
  EnqueuePlanetConstructionFailureResponse,
  EnqueuePlanetConstructionRequest,
  EnqueuePlanetConstructionResponse,
  PlanetUiStateResponse,
} from "./planetTypes";
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

interface JsonRequestOptions {
  body?: unknown;
  method?: "GET" | "POST";
  query?: Record<string, string>;
}

export interface PlayableStartRequest {
  displayName: string;
  civilizationName: string;
  homePlanetName?: string;
}

export interface PlayableStartResponse {
  succeeded: boolean;
  userId: string | null;
  playerProfileId: string | null;
  civilizationId: string | null;
  homePlanetId: string | null;
  homePlanetName: string | null;
  homeSystemId: string | null;
  homeSystemName: string | null;
  startingResources: {
    credits: number;
    metal: number;
    crystal: number;
    gas: number;
  } | null;
  limitations: string[];
  errors: string[];
}

async function requestJson<T>(path: string, options?: JsonRequestOptions): Promise<T> {
  const response = await fetch(buildUrl(path, options?.query), {
    body: options?.body ? JSON.stringify(options.body) : undefined,
    headers: {
      Accept: "application/json",
      ...(options?.body ? { "Content-Type": "application/json" } : {}),
    },
    method: options?.method ?? "GET",
  });

  if (!response.ok) {
    throw new Error(`Request failed with status ${response.status}.`);
  }

  return (await response.json()) as T;
}

async function requestCommandJson<T>(path: string, body: unknown): Promise<FleetCommandApiResult<T>> {
  const response = await fetch(buildUrl(path), {
    body: JSON.stringify(body),
    headers: {
      Accept: "application/json",
      "Content-Type": "application/json",
    },
    method: "POST",
  });

  const contentType = response.headers.get("content-type") ?? "";
  const hasJsonBody = contentType.includes("application/json");
  let payload: T | null = null;

  if (hasJsonBody) {
    try {
      payload = (await response.json()) as T;
    } catch {
      return {
        httpStatus: response.status,
        hasJsonBody: true,
        bodyParseFailed: true,
        response: null,
      };
    }
  }

  return {
    httpStatus: response.status,
    hasJsonBody,
    bodyParseFailed: false,
    response: payload,
  };
}

async function requestActionJson<T>(path: string, body: unknown): Promise<FleetCommandApiResult<T>> {
  const response = await fetch(buildUrl(path), {
    body: JSON.stringify(body),
    headers: {
      Accept: "application/json",
      "Content-Type": "application/json",
    },
    method: "POST",
  });

  const contentType = response.headers.get("content-type") ?? "";
  const hasJsonBody = contentType.includes("application/json");
  let payload: T | null = null;

  if (hasJsonBody) {
    try {
      payload = (await response.json()) as T;
    } catch {
      return {
        httpStatus: response.status,
        hasJsonBody: true,
        bodyParseFailed: true,
        response: null,
      };
    }
  }

  return {
    httpStatus: response.status,
    hasJsonBody,
    bodyParseFailed: false,
    response: payload,
  };
}

function toPlanetCommandFailureResponse(
  payload: Partial<EnqueuePlanetConstructionFailureResponse> | null,
): EnqueuePlanetConstructionFailureResponse {
  const rawErrors = Array.isArray(payload?.errors)
    ? payload.errors.filter((value): value is string => typeof value === "string")
    : [];

  return {
    succeeded: false,
    orderId: null,
    startsAtUtc: null,
    endsAtUtc: null,
    errors: rawErrors,
    errorEntries: rawErrors.map((message) => ({
      message: message.trim(),
      rawMessage: message,
    })),
  };
}

async function requestConstructionCommandJson(
  path: string,
  body: EnqueuePlanetConstructionRequest,
): Promise<EnqueuePlanetConstructionCommandResult> {
  const response = await fetch(buildUrl(path), {
    body: JSON.stringify(body),
    headers: {
      Accept: "application/json",
      "Content-Type": "application/json",
    },
    method: "POST",
  });

  const contentType = response.headers.get("content-type") ?? "";
  const hasJsonBody = contentType.includes("application/json");
  let payload: EnqueuePlanetConstructionResponse | null = null;

  if (hasJsonBody) {
    try {
      const parsed = (await response.json()) as EnqueuePlanetConstructionResponse;

      payload = response.ok
        ? parsed
        : toPlanetCommandFailureResponse(parsed as EnqueuePlanetConstructionFailureResponse);
    } catch {
      return {
        httpStatus: response.status,
        hasJsonBody: true,
        bodyParseFailed: true,
        response: null,
      };
    }
  }

  return {
    httpStatus: response.status,
    hasJsonBody,
    bodyParseFailed: false,
    response: payload,
  };
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
      query: { civilizationId },
    });
  },
  getFleetUiState(civilizationId: string) {
    return requestJson<FleetUiStateResponse>("/api/dev/fleets/ui-state", {
      query: { civilizationId },
    });
  },
  getPlanetUiState(civilizationId: string, planetId?: string | null) {
    return requestJson<PlanetUiStateResponse>("/api/dev/planets/ui-state", {
      query: planetId ? { civilizationId, planetId } : { civilizationId },
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
  createPlayableStart(request: PlayableStartRequest) {
    return requestActionJson<PlayableStartResponse>("/api/dev/players/starting-civilization", request);
  },
  enqueuePlanetConstruction(request: EnqueuePlanetConstructionRequest) {
    return enqueueConstructionOrder(request);
  },
  estimateOrbitalTravel(request: EstimateOrbitalTravelRequest) {
    return requestCommandJson<EstimateOrbitalTravelResponse>(
      "/api/dev/fleets/orbital-travel/estimate",
      request,
    );
  },
  createOrbitalTransfer(request: CreateOrbitalTransferRequest) {
    return requestCommandJson<CreateOrbitalTransferResponse>(
      "/api/dev/fleets/orbital-transfers/create",
      request,
    );
  },
  cancelOrbitalTransfer(request: CancelOrbitalTransferRequest) {
    return requestCommandJson<CancelOrbitalTransferResponse>(
      "/api/dev/fleets/orbital-transfers/cancel",
      request,
    );
  },
  completeDueOrbitalTransfers(request: CompleteOrbitalTransfersRequest) {
    return requestCommandJson<CompleteOrbitalTransfersResponse>(
      "/api/dev/fleets/orbital-transfers/complete-due",
      request,
    );
  },
  splitOrbitalGroup(request: SplitOrbitalGroupRequest) {
    return requestCommandJson<SplitOrbitalGroupResponse>(
      "/api/dev/fleets/orbital-groups/split",
      request,
    );
  },
  mergeOrbitalGroups(request: MergeOrbitalGroupsRequest) {
    return requestCommandJson<MergeOrbitalGroupsResponse>(
      "/api/dev/fleets/orbital-groups/merge",
      request,
    );
  },
};

export function enqueueConstructionOrder(request: EnqueuePlanetConstructionRequest) {
  return requestConstructionCommandJson(
    "/api/dev/buildings/construction-orders/enqueue",
    request,
  );
}
