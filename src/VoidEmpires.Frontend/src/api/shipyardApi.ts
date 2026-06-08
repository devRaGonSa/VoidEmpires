import { appConfig } from "../config";
import type {
  EnqueueShipyardProductionCommandResult,
  EnqueueShipyardProductionFailureResponse,
  EnqueueShipyardProductionRequest,
  EnqueueShipyardProductionResponse,
  EnqueueShipyardProductionSuccessResponse,
  ShipyardApiErrorCode,
  ShipyardUiStateResponse,
} from "./shipyardTypes";

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

async function requestCommandJson(path: string, body: unknown): Promise<EnqueueShipyardProductionCommandResult> {
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
  let payload: EnqueueShipyardProductionResponse | null = null;

  if (hasJsonBody) {
    try {
      const parsed = (await response.json()) as EnqueueShipyardProductionSuccessResponse | Partial<EnqueueShipyardProductionFailureResponse>;
      payload = response.ok
        ? parsed as EnqueueShipyardProductionSuccessResponse
        : toShipyardCommandFailureResponse(response.status, parsed as Partial<EnqueueShipyardProductionFailureResponse>);
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

function toShipyardCommandFailureResponse(
  httpStatus: number,
  payload: Partial<EnqueueShipyardProductionFailureResponse> | null,
): EnqueueShipyardProductionFailureResponse {
  const rawErrors = Array.isArray(payload?.errors)
    ? payload.errors.filter((value): value is string => typeof value === "string")
    : [];

  return {
    succeeded: false,
    orderId: null,
    startsAtUtc: null,
    endsAtUtc: null,
    errors: rawErrors,
    errorEntries: rawErrors.map((message) => ({
      code: classifyShipyardError(message),
      message: message.trim(),
      rawMessage: message,
    })),
    failureKind: httpStatus === 400 ? "validation" : httpStatus === 409 ? "conflict" : "unknown",
  };
}

function classifyShipyardError(message: string): ShipyardApiErrorCode {
  switch (message.trim()) {
    case "Civilization id is required.":
      return "MissingCivilizationId";
    case "Planet id is required.":
      return "MissingPlanetId";
    case "Space asset type is required.":
    case "Asset type is required.":
      return "MissingAssetType";
    case "Space asset type is invalid.":
      return "InvalidAssetType";
    case "Quantity must be positive.":
      return "MissingQuantity";
    case "Requested date is required.":
      return "MissingRequestedAtUtc";
    case "Requested date must be UTC.":
      return "RequestedAtUtcNotUtc";
    case "Planet is not owned by the requesting civilization.":
      return "PlanetNotOwned";
    case "Planet already has an open asset production order.":
      return "OpenProductionOrderExists";
    case "Planet resource stockpile was not found.":
      return "PlanetStockpileMissing";
    case "Insufficient resources.":
      return "InsufficientResources";
    case "Required building is missing or below required level.":
      return "MissingRequiredBuilding";
    case "Planet population profile was not found.":
      return "PopulationProfileMissing";
    case "Insufficient local operator capacity.":
      return "InsufficientOperatorCapacity";
    default:
      return "UnknownValidationFailure";
  }
}

export function fetchShipyardUiState(civilizationId: string, planetId?: string | null) {
  return requestJson<ShipyardUiStateResponse>("/api/dev/shipyard/ui-state", {
    civilizationId,
    ...(planetId ? { planetId } : {}),
  });
}

export function enqueueShipyardProduction(request: EnqueueShipyardProductionRequest) {
  return requestCommandJson(
    request.route ?? "/api/dev/assets/production/enqueue",
    {
      civilizationId: request.civilizationId,
      planetId: request.planetId,
      target: typeof request.target === "number" ? request.target : orbitalTarget,
      spaceAssetType: spaceAssetTypeMap[request.assetType] ?? null,
      quantity: request.quantity,
      requestedAtUtc: request.requestedAtUtc,
    },
  );
}
