import {
  planetBuildingCategoryCatalog,
  planetBuildingTypeCatalog,
  type PlanetCatalogEntry,
} from "../api/planetTypes";

type ApiEnumValue = string | number | null | undefined;

const queueStatusByNumber: Record<number, string> = {
  1: "Pending",
  2: "Active",
  3: "Completed",
  4: "Cancelled",
};

const queueStatusByName: Record<string, string> = {
  pending: "Pending",
  active: "Active",
  completed: "Completed",
  cancelled: "Cancelled",
  canceled: "Cancelled",
};

const unitDefenseBuildingTypes = new Set([
  "MissileBattery",
  "LaserTurret",
  "IonCannon",
  "PlasmaCannon",
]);

const specialDefenseBuildingTypes = new Set([
  "DefenseGrid",
  "PlanetaryShield",
]);

export const unitDefenseBuildingTypeIds = [16, 17, 18, 20] as const;
export const specialDefenseBuildingTypeIds = [8, 19] as const;

function normalizeText(value: string) {
  return value
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "")
    .replace(/[\s_-]+/g, "")
    .toLowerCase();
}

export function normalizeCatalogKey(value: ApiEnumValue, catalog: readonly PlanetCatalogEntry[], fallback = "") {
  if (typeof value === "number" && Number.isFinite(value)) {
    return catalog.find((entry) => entry.id === value)?.key ?? String(value);
  }

  if (typeof value !== "string") {
    return fallback;
  }

  const trimmed = value.trim();
  if (!trimmed) {
    return fallback;
  }

  const numeric = Number(trimmed);
  if (Number.isInteger(numeric) && `${numeric}` === trimmed) {
    return catalog.find((entry) => entry.id === numeric)?.key ?? trimmed;
  }

  const normalized = normalizeText(trimmed);
  return catalog.find((entry) => normalizeText(entry.key) === normalized)?.key ?? trimmed;
}

export function normalizeQueueStatus(value: ApiEnumValue) {
  if (typeof value === "number" && Number.isFinite(value)) {
    return queueStatusByNumber[value] ?? String(value);
  }

  if (typeof value !== "string") {
    return "";
  }

  const trimmed = value.trim();
  if (!trimmed) {
    return "";
  }

  const numeric = Number(trimmed);
  if (Number.isInteger(numeric) && `${numeric}` === trimmed) {
    return queueStatusByNumber[numeric] ?? trimmed;
  }

  return queueStatusByName[normalizeText(trimmed)] ?? trimmed;
}

export function isOpenQueueStatus(value: ApiEnumValue) {
  const status = normalizeQueueStatus(value);
  return status === "Pending" || status === "Active";
}

export function normalizeBuildingType(value: ApiEnumValue) {
  return normalizeCatalogKey(value, planetBuildingTypeCatalog, "Unknown");
}

export function normalizeBuildingCategory(value: ApiEnumValue) {
  return normalizeCatalogKey(value, planetBuildingCategoryCatalog, "Unknown");
}

export function isUnitDefenseBuildingType(value: ApiEnumValue) {
  return unitDefenseBuildingTypes.has(normalizeBuildingType(value));
}

export function isSpecialDefenseBuildingType(value: ApiEnumValue) {
  return specialDefenseBuildingTypes.has(normalizeBuildingType(value));
}
