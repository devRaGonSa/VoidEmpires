import type { AccountApiError, AccountApiResult, AccountSessionResponse } from "../api/accountTypes";

export type CurrentAccountSessionStatus = "loading" | "ready" | "signedOut" | "error";

export interface CurrentAccountWorldEntry {
  civilizationId: string;
  planetId: string;
  planetName?: string;
  nextRoute?: string;
}

export interface CurrentAccountSessionState {
  status: CurrentAccountSessionStatus;
  session: AccountSessionResponse | null;
  errors: readonly AccountApiError[];
  httpStatus: number | null;
  nextRoute: string | null;
  hasWorldEntry: boolean;
}

export const initialCurrentAccountSessionState: CurrentAccountSessionState = {
  status: "loading",
  session: null,
  errors: [],
  httpStatus: null,
  nextRoute: null,
  hasWorldEntry: false,
};

function normalizeText(value: string | null | undefined) {
  const trimmed = value?.trim() ?? "";
  return trimmed.length > 0 ? trimmed : null;
}

function createEmptySessionState(
  status: CurrentAccountSessionStatus,
  httpStatus: number | null,
  errors: readonly AccountApiError[] = [],
): CurrentAccountSessionState {
  return { status, session: null, errors, httpStatus, nextRoute: null, hasWorldEntry: false };
}

export function getCurrentAccountWorldEntry(session: AccountSessionResponse | null): CurrentAccountWorldEntry | null {
  if (!session?.succeeded) {
    return null;
  }

  const civilizationId = normalizeText(session.civilizationId);
  const planetId = normalizeText(session.homePlanetId);

  if (!civilizationId || !planetId) {
    return null;
  }

  return {
    civilizationId,
    planetId,
    planetName: normalizeText(session.homePlanetName) ?? undefined,
    nextRoute: normalizeText(session.nextRoute) ?? undefined,
  };
}

export function resolveCurrentAccountSession(
  result: AccountApiResult<AccountSessionResponse>,
): CurrentAccountSessionState {
  const session = result.response;

  if (result.httpStatus === 401 || session?.succeeded === false) {
    return createEmptySessionState("signedOut", result.httpStatus, session?.errors);
  }

  if (!session?.succeeded) {
    return createEmptySessionState("error", result.httpStatus);
  }

  return {
    status: "ready",
    session,
    errors: session.errors,
    httpStatus: result.httpStatus,
    nextRoute: session.nextRoute,
    hasWorldEntry: getCurrentAccountWorldEntry(session) !== null,
  };
}

export function createCurrentAccountSessionError(message: string): CurrentAccountSessionState {
  return createEmptySessionState("error", null, [{ code: "CurrentAccountUnavailable", message, field: null }]);
}
