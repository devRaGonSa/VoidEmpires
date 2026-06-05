import { appConfig } from "../config";
import type { AllianceUiStateResponse } from "./allianceTypes";

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

    if (!detail) {
      if (response.status === 400) {
        detail = "Civilization id is required.";
      } else if (response.status === 405 || response.status === 501) {
        detail = "Alliance actions are not supported in this version.";
      }
    }

    throw new Error(detail ?? `Request failed with status ${response.status}.`);
  }

  return response.json() as Promise<T>;
}

export function fetchAllianceUiState(civilizationId: string) {
  return requestJson<AllianceUiStateResponse>("/api/dev/alliance/ui-state", { civilizationId });
}
