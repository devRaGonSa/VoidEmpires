import { appConfig } from "../config";

export type RankingApiValue = string | number | null | undefined;

export interface RankingIdentityDto {
  civilizationId: string;
  civilizationName: string;
  displayName: string;
  homePlanetId: string | null;
}

export interface RankingCategoryScoreDto {
  categoryKey: string;
  score: number;
  weight: number;
  sourceNote: string;
}

export interface RankingPowerSummaryDto {
  totalPowerIndex: number;
  categories: readonly RankingCategoryScoreDto[];
  recommendationKey: string;
}

export interface RankingComparisonRowDto {
  rowKey: string;
  displayName: string;
  totalPowerIndex: number;
  deltaFromCurrent: number;
  isCurrentCivilization: boolean;
  isDemoOnly: boolean;
}

export interface RankingPublicationStateDto {
  stateKey: string;
  isPublished: boolean;
  summaryKey: string;
}

export interface RankingFuturePlaceholderDto {
  placeholderKey: string;
  isAvailable: boolean;
  stateKey: string;
  reasonKey: string;
}

export interface RankingDisabledActionDto {
  actionKey: string;
  isAvailable: boolean;
  reasonKey: string;
}

export interface RankingDiagnosticsDto {
  ownedPlanetCount: number;
  visibleSystemCount: number;
  diplomaticContactCount: number;
  activeTransferCount: number;
  notes: readonly string[];
}

export interface RankingUiStateDto {
  civilizationId: string;
  identity: RankingIdentityDto | null;
  summary: RankingPowerSummaryDto | null;
  demoComparisons: readonly RankingComparisonRowDto[];
  publication: RankingPublicationStateDto | null;
  futurePlaceholders: readonly RankingFuturePlaceholderDto[];
  disabledActions: readonly RankingDisabledActionDto[];
  diagnostics: RankingDiagnosticsDto | null;
  limitations: readonly string[];
  errors: readonly string[];
}

export interface RankingUiStateResponse {
  succeeded: boolean;
  uiState: RankingUiStateDto | null;
  errors: readonly string[];
}

export type RankingRequestFailureCode =
  | "invalidCivilizationId"
  | "civilizationNotFound"
  | "rankingReadUnavailable"
  | "endpointUnavailableOutsideDevelopment"
  | "unsupportedFutureAction"
  | "unexpectedError";

export class RankingRequestError extends Error {
  constructor(
    readonly code: RankingRequestFailureCode,
    readonly detail: string,
    readonly status: number,
  ) {
    super(detail);
    this.name = "RankingRequestError";
  }
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

    if (!detail && response.status === 400) {
      detail = "Civilization id is required.";
    }

    const normalizedDetail = detail ?? `Request failed with status ${response.status}.`;
    let code: RankingRequestFailureCode = "unexpectedError";

    switch (normalizedDetail) {
      case "Civilization id is required.":
        code = "invalidCivilizationId";
        break;
      case "Civilization was not found.":
        code = "civilizationNotFound";
        break;
      case "Request failed with status 404.":
        code = "endpointUnavailableOutsideDevelopment";
        break;
      case "Request failed with status 503.":
        code = "rankingReadUnavailable";
        break;
      default:
        if (normalizedDetail.toLowerCase().includes("not available in this version")) {
          code = "unsupportedFutureAction";
        }
        break;
    }

    throw new RankingRequestError(code, normalizedDetail, response.status);
  }

  return response.json() as Promise<T>;
}

export function fetchRankingUiState(civilizationId: string) {
  return requestJson<RankingUiStateResponse>("/api/dev/ranking/ui-state", { civilizationId });
}
