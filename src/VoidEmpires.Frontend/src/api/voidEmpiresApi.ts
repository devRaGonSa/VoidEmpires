import { appConfig } from "../config";

async function requestJson<T>(path: string): Promise<T> {
  const response = await fetch(`${appConfig.apiBaseUrl}${path}`, {
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
};
