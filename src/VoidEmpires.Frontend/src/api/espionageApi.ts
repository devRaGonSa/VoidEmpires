import { appConfig } from "../config";
import type { EspionageUiStateResponse } from "./espionageTypes";

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

export function fetchEspionageUiState(civilizationId: string) {
  return requestJson<EspionageUiStateResponse>("/api/dev/espionage/ui-state", { civilizationId });
}
