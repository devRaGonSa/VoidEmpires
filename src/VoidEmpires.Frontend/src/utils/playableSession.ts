const playableSessionStorageKey = "voidempires.playableSession.v1";
const operatorModeStorageKey = "voidempires.operatorMode";

export interface PlayableSessionInput {
  civilizationId: string | null | undefined;
  planetId: string | null | undefined;
  playerDisplayName?: string | null;
  civilizationName?: string | null;
  planetName?: string | null;
}

export interface PlayableSession {
  civilizationId: string;
  planetId: string;
  playerDisplayName?: string;
  civilizationName?: string;
  planetName?: string;
  createdAt: string;
  updatedAt: string;
}

function getLocalStorage(): Storage | null {
  return typeof globalThis.localStorage === "undefined" ? null : globalThis.localStorage;
}

function normalizeRequiredId(value: string | null | undefined) {
  const trimmed = value?.trim() ?? "";
  return trimmed.length > 0 ? trimmed : null;
}

function normalizeOptionalText(value: unknown) {
  return typeof value === "string" && value.trim().length > 0 ? value.trim() : undefined;
}

function isIsoDate(value: unknown): value is string {
  return typeof value === "string" && value.trim().length > 0 && !Number.isNaN(Date.parse(value));
}

function readRawSession(value: unknown): PlayableSession | null {
  if (!value || typeof value !== "object") {
    return null;
  }

  const raw = value as Record<string, unknown>;
  const civilizationId = normalizeRequiredId(typeof raw.civilizationId === "string" ? raw.civilizationId : null);
  const planetId = normalizeRequiredId(typeof raw.planetId === "string" ? raw.planetId : null);

  if (!civilizationId || !planetId || !isIsoDate(raw.createdAt) || !isIsoDate(raw.updatedAt)) {
    return null;
  }

  return {
    civilizationId,
    planetId,
    playerDisplayName: normalizeOptionalText(raw.playerDisplayName),
    civilizationName: normalizeOptionalText(raw.civilizationName),
    planetName: normalizeOptionalText(raw.planetName),
    createdAt: raw.createdAt,
    updatedAt: raw.updatedAt,
  };
}

export function savePlayableSession(input: PlayableSessionInput): PlayableSession | null {
  const civilizationId = normalizeRequiredId(input.civilizationId);
  const planetId = normalizeRequiredId(input.planetId);

  if (!civilizationId || !planetId) {
    return null;
  }

  const storage = getLocalStorage();
  if (!storage) {
    return null;
  }

  const existing = loadPlayableSession();
  const now = new Date().toISOString();
  const session: PlayableSession = {
    civilizationId,
    planetId,
    playerDisplayName: normalizeOptionalText(input.playerDisplayName),
    civilizationName: normalizeOptionalText(input.civilizationName),
    planetName: normalizeOptionalText(input.planetName),
    createdAt: existing?.createdAt ?? now,
    updatedAt: now,
  };

  try {
    storage.setItem(playableSessionStorageKey, JSON.stringify(session));
    return session;
  } catch {
    return null;
  }
}

export function loadPlayableSession(): PlayableSession | null {
  const storage = getLocalStorage();
  if (!storage) {
    return null;
  }

  try {
    const rawValue = storage.getItem(playableSessionStorageKey);
    if (!rawValue) {
      return null;
    }

    return readRawSession(JSON.parse(rawValue));
  } catch {
    return null;
  }
}

export function clearPlayableSession() {
  const storage = getLocalStorage();
  if (!storage) {
    return;
  }

  try {
    storage.removeItem(playableSessionStorageKey);
  } catch {
    // Local navigation memory is optional; storage failures must not break cockpit routing.
  }
}

export function hasPlayableSession() {
  return loadPlayableSession() !== null;
}

export function isOperatorMode(searchParams?: URLSearchParams | null) {
  if (searchParams?.get("operator") === "1") {
    return true;
  }

  const storage = getLocalStorage();
  if (!storage) {
    return false;
  }

  try {
    return storage.getItem(operatorModeStorageKey) === "1";
  } catch {
    return false;
  }
}
