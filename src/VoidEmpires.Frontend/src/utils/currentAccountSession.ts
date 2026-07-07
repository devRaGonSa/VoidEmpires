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

export interface CurrentAccountDisplay {
  statusLabel: string;
  commanderLabel: string;
  civilizationLabel: string;
  planetLabel: string;
  detailLabel: string;
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

export function getCurrentAccountDisplay(state: CurrentAccountSessionState): CurrentAccountDisplay {
  const worldEntry = getCurrentAccountWorldEntry(state.session);

  if (state.status === "loading") {
    return {
      statusLabel: "Comprobando cuenta",
      commanderLabel: "Comandante pendiente",
      civilizationLabel: "Civilizacion pendiente",
      planetLabel: "Planeta pendiente",
      detailLabel: "Revisando la cuenta actual.",
    };
  }

  if (state.status !== "ready") {
    return {
      statusLabel: "Cuenta requerida",
      commanderLabel: "Sin cuenta activa",
      civilizationLabel: "Acceso pendiente",
      planetLabel: "Sin planeta activo",
      detailLabel: "Entra o registra un comandante.",
    };
  }

  return {
    statusLabel: worldEntry ? "Cuenta activa" : "Cuenta incompleta",
    commanderLabel: "Comandante activo",
    civilizationLabel: state.session?.civilizationId ? "Civilizacion activa" : "Civilizacion pendiente",
    planetLabel: worldEntry?.planetName ?? "Planeta pendiente",
    detailLabel: worldEntry ? `Mando listo en ${worldEntry.planetName ?? "tu planeta principal"}.` : "Falta el mundo inicial.",
  };
}
