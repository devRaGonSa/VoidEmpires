import { appConfig } from "../config";
import type { GroundArmyUiStateResponse } from "./groundArmyTypes";

const planetaryTarget = 1;
const groundAssetTypeMap: Record<string, number> = {
  PatrolGroup: 1,
  ExpeditionGroup: 2,
  VehicleGroup: 3,
  SupportGroup: 4,
};

export interface EnqueueGroundTrainingRequest {
  civilizationId: string;
  planetId: string;
  assetType: string;
  quantity: number;
  requestedAtUtc: string;
}

export interface EnqueueGroundTrainingResult {
  httpStatus: number;
  response: { succeeded: boolean; orderId: string | null; startsAtUtc: string | null; endsAtUtc: string | null; errors: readonly string[] } | null;
}

function buildUrl(path: string, query?: Record<string, string>) {
  const url = new URL(path, appConfig.apiBaseUrl);
  Object.entries(query ?? {}).forEach(([key, value]) => url.searchParams.set(key, value));
  return url.toString();
}

async function requestJson<T>(path: string, query?: Record<string, string>) {
  const response = await fetch(buildUrl(path, query), { headers: { Accept: "application/json" } });
  if (!response.ok) {
    let detail: string | null = null;
    if ((response.headers.get("content-type") ?? "").includes("application/json")) {
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

export function fetchGroundArmyUiState(civilizationId: string, planetId?: string | null) {
  return requestJson<GroundArmyUiStateResponse>("/api/dev/ground-army/ui-state", {
    civilizationId,
    ...(planetId ? { planetId } : {}),
  });
}

export async function enqueueGroundTraining(request: EnqueueGroundTrainingRequest): Promise<EnqueueGroundTrainingResult> {
  const response = await fetch(buildUrl("/api/dev/assets/production/enqueue"), {
    method: "POST",
    headers: { Accept: "application/json", "Content-Type": "application/json" },
    body: JSON.stringify({
      civilizationId: request.civilizationId,
      planetId: request.planetId,
      target: planetaryTarget,
      planetaryAssetType: groundAssetTypeMap[request.assetType] ?? null,
      quantity: request.quantity,
      requestedAtUtc: request.requestedAtUtc,
    }),
  });

  let payload: EnqueueGroundTrainingResult["response"] = null;
  if ((response.headers.get("content-type") ?? "").includes("application/json")) {
    try {
      payload = await response.json();
    } catch {
      payload = null;
    }
  }

  return { httpStatus: response.status, response: payload };
}
