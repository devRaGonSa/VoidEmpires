import { appConfig } from "../config";
import type { ShipyardUiStateResponse } from "./shipyardTypes";

interface JsonCommandResult<T> {
  httpStatus: number;
  hasJsonBody: boolean;
  bodyParseFailed: boolean;
  response: T | null;
}

interface EnqueueShipyardProductionRequest {
  civilizationId: string;
  planetId: string;
  assetType: string;
  quantity: number;
  requestedAtUtc: string;
}

interface EnqueueShipyardProductionResponse {
  succeeded: boolean;
  orderId: string | null;
  startsAtUtc: string | null;
  endsAtUtc: string | null;
  errors: readonly string[];
}

const orbitalTarget = 2;
const spaceAssetTypeMap: Record<string, number> = {
  ScoutCraft: 1,
  CargoCraft: 2,
  EscortCraft: 3,
  ColonyCraft: 4,
};

function buildUrl(path: string, query?: Record<string, string>) {
  const url = new URL(path, appConfig.apiBaseUrl);

  if (query) {
    Object.entries(query).forEach(([key, value]) => {
      url.searchParams.set(key, value);
    });
  }

  return url.toString();
}

async function requestJson<T>(path: string, query?: Record<string, string>) {
  const response = await fetch(buildUrl(path, query), {
    headers: { Accept: "application/json" },
  });

  if (!response.ok) {
    let detail: string | null = null;
    const contentType = response.headers.get("content-type") ?? "";

    if (contentType.includes("application/json")) {
      try {
        const payload = await response.json() as { errors?: readonly string[] };
        detail = payload.errors?.[0]?.trim() ?? null;
      } catch {
        detail = null;
      }
    }

    throw new Error(detail ?? `Request failed with status ${response.status}.`);
  }

  return response.json() as Promise<T>;
}

async function requestCommandJson<T>(path: string, body: unknown): Promise<JsonCommandResult<T>> {
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

export function fetchShipyardUiState(civilizationId: string, planetId?: string | null) {
  return requestJson<ShipyardUiStateResponse>("/api/dev/shipyard/ui-state", {
    civilizationId,
    ...(planetId ? { planetId } : {}),
  });
}

export function enqueueShipyardProduction(request: EnqueueShipyardProductionRequest) {
  return requestCommandJson<EnqueueShipyardProductionResponse>(
    "/api/dev/assets/production/enqueue",
    {
      civilizationId: request.civilizationId,
      planetId: request.planetId,
      target: orbitalTarget,
      spaceAssetType: spaceAssetTypeMap[request.assetType] ?? null,
      quantity: request.quantity,
      requestedAtUtc: request.requestedAtUtc,
    },
  );
}
