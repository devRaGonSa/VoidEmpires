import { appConfig } from "../config";
import type { DefensesUiStateResponse, EnqueueDefenseProductionRequest, EnqueueDefenseProductionResult } from "./defenseTypes";

const planetaryTarget = 1;
const planetaryDefenseAssetTypeMap: Record<string, number> = {
  MissileBattery: 10,
  LaserTurret: 11,
  IonCannon: 12,
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

async function requestCommandJson(path: string, body: unknown): Promise<EnqueueDefenseProductionResult> {
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
  let payload: EnqueueDefenseProductionResult["response"] = null;

  if (hasJsonBody) {
    try {
      payload = await response.json();
    } catch {
      return {
        httpStatus: response.status,
        response: null,
      };
    }
  }

  return {
    httpStatus: response.status,
    response: payload,
  };
}

export function fetchDefensesUiState(civilizationId: string, planetId?: string | null) {
  return requestJson<DefensesUiStateResponse>("/api/dev/defenses/ui-state", {
    civilizationId,
    ...(planetId ? { planetId } : {}),
  });
}

export function enqueueDefenseProduction(request: EnqueueDefenseProductionRequest) {
  return requestCommandJson("/api/dev/assets/production/enqueue", {
    civilizationId: request.civilizationId,
    planetId: request.planetId,
    target: planetaryTarget,
    planetaryAssetType: planetaryDefenseAssetTypeMap[request.assetType] ?? null,
    quantity: request.quantity,
    requestedAtUtc: request.requestedAtUtc,
  });
}
