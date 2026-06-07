import { appConfig } from "../config";
import type {
  EnqueueResearchOrderCommandResult,
  EnqueueResearchOrderFailureResponse,
  EnqueueResearchOrderRequest,
  EnqueueResearchOrderResponse,
  EnqueueResearchOrderSuccessResponse,
  ResearchApiErrorCode,
  ResearchUiStateResponse,
} from "./researchTypes";

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

async function requestCommandJson<T>(path: string, body: unknown): Promise<EnqueueResearchOrderCommandResult> {
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
  let payload: EnqueueResearchOrderResponse | null = null;

  if (hasJsonBody) {
    try {
      const parsed = (await response.json()) as EnqueueResearchOrderSuccessResponse | Partial<EnqueueResearchOrderFailureResponse>;
      payload = response.ok
        ? parsed as EnqueueResearchOrderSuccessResponse
        : toResearchCommandFailureResponse(response.status, parsed as Partial<EnqueueResearchOrderFailureResponse>);
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

function toResearchCommandFailureResponse(
  httpStatus: number,
  payload: Partial<EnqueueResearchOrderFailureResponse> | null,
): EnqueueResearchOrderFailureResponse {
  const rawErrors = Array.isArray(payload?.errors)
    ? payload.errors.filter((value): value is string => typeof value === "string")
    : [];
  const errorEntries = rawErrors.map((message) => ({
    code: classifyResearchError(message),
    message: message.trim(),
    rawMessage: message,
  }));

  return {
    succeeded: false,
    orderId: null,
    startsAtUtc: null,
    endsAtUtc: null,
    errors: rawErrors,
    errorEntries,
    failureKind: httpStatus === 400 ? "validation" : httpStatus === 409 ? "conflict" : "unknown",
    isOpenOrderNoOp: errorEntries.some((entry) => entry.code === "OpenResearchOrderExists"),
  };
}

function classifyResearchError(message: string): ResearchApiErrorCode {
  switch (message.trim()) {
    case "Civilization id is required.":
      return "MissingCivilizationId";
    case "Source planet id is required.":
      return "MissingSourcePlanetId";
    case "Research type is required.":
      return "MissingResearchType";
    case "Requested date is required.":
      return "MissingRequestedAtUtc";
    case "Requested date must be UTC.":
      return "RequestedAtUtcNotUtc";
    case "Planet is not owned by the requesting civilization.":
      return "SourcePlanetNotOwned";
    case "Civilization already has an open research order.":
      return "OpenResearchOrderExists";
    case "Planet resource stockpile was not found.":
      return "SourcePlanetStockpileMissing";
    case "Insufficient resources.":
      return "InsufficientResources";
    default:
      return "UnknownValidationFailure";
  }
}
