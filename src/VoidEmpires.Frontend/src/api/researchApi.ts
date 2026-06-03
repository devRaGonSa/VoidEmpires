import { appConfig } from "../config";
import type { ResearchUiStateResponse } from "./researchTypes";

interface JsonCommandResult<T> {
  httpStatus: number;
  hasJsonBody: boolean;
  bodyParseFailed: boolean;
  response: T | null;
}

interface EnqueueResearchOrderRequest {
  civilizationId: string;
  sourcePlanetId: string;
  researchType: string;
  requestedAtUtc: string;
}

interface EnqueueResearchOrderResponse {
  succeeded: boolean;
  orderId: string | null;
  startsAtUtc: string | null;
  endsAtUtc: string | null;
  errors: readonly string[];
}

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

export function fetchResearchUiState(civilizationId: string, planetId?: string | null) {
  return requestJson<ResearchUiStateResponse>("/api/dev/research/ui-state", {
    civilizationId,
    ...(planetId ? { planetId } : {}),
  });
}

export function enqueueResearchOrder(request: EnqueueResearchOrderRequest) {
  return requestCommandJson<EnqueueResearchOrderResponse>(
    "/api/dev/research/orders/enqueue",
    request,
  );
}
