import { appConfig } from "../config";
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

export const voidEmpiresApi = {
  getHealth() {
    return requestJson<HealthResponse>("/health");
  },
  getStrategicMap(civilizationId: string) {
    return requestJson<StrategicMapResponse>("/api/dev/strategic-map", {
      civilizationId,
    });
  },
};
