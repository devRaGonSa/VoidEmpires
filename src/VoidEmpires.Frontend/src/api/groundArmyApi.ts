import { appConfig } from "../config";
import type { GroundArmyUiStateResponse } from "./groundArmyTypes";

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
