import { formatResourceType } from "./domainPresentation";

type DefenseValue = string | number | null | undefined;

interface DefenseLabelCatalogEntry {
  key: string;
  label: string;
}

export interface DefenseStructurePresentationEntry {
  key: string;
  label: string;
  categoryKey: string;
  categoryLabel: string;
  readinessKey: string;
  readinessLabel: string;
}

const unknownStructureFallback = "Defensa pendiente de clasificar";
const unknownCategoryFallback = "Categoria defensiva pendiente de clasificar";
const unknownReadinessFallback = "Readiness defensivo pendiente de clasificar";
const unknownStatusFallback = "Estado defensivo pendiente de clasificar";
const unknownActionFallback = "Accion defensiva pendiente de clasificar";

const defenseStructureCatalog: readonly DefenseStructurePresentationEntry[] = [
  {
    key: "DefenseGrid",
    label: "Malla defensiva",
    categoryKey: "Defense",
    categoryLabel: "Proteccion planetaria",
    readinessKey: "ConstructionReady",
    readinessLabel: "Fortificacion lista para preparar",
  },
] as const;

const defenseCategoryCatalog: readonly DefenseLabelCatalogEntry[] = [
  { key: "Defense", label: "Proteccion planetaria" },
  { key: "Shielding", label: "Escudos" },
  { key: "OrbitalDefense", label: "Defensa orbital" },
  { key: "GroundDefense", label: "Defensa terrestre" },
  { key: "Sensors", label: "Sensores" },
  { key: "Infrastructure", label: "Infraestructura defensiva" },
] as const;

const defenseReadinessCatalog: readonly DefenseLabelCatalogEntry[] = [
  { key: "Available", label: "Lista para fortificar" },
  { key: "Blocked", label: "Bloqueada" },
  { key: "MissingResourceStockpile", label: "Sin reservas" },
  { key: "MissingCapacityData", label: "Capacidad no disponible" },
  { key: "CapacityExceeded", label: "Capacidad agotada" },
  { key: "InsufficientResources", label: "Recursos insuficientes" },
  { key: "Unsupported", label: "Solo lectura" },
  { key: "ConstructionReady", label: "Fortificacion lista para preparar" },
  { key: "ReadOnly", label: "Solo lectura" },
  { key: "Placeholder", label: "Cabina preparada" },
] as const;

const defenseStatusCatalog: readonly DefenseLabelCatalogEntry[] = [
  { key: "Pending", label: "Pendiente" },
  { key: "Active", label: "Activa" },
  { key: "Completed", label: "Completada" },
  { key: "Cancelled", label: "Cancelada" },
  { key: "ReadOnly", label: "Solo lectura" },
] as const;

const defenseActionCatalog: readonly DefenseLabelCatalogEntry[] = [
  { key: "catalog.read", label: "Revisar estructuras defensivas" },
  { key: "coverage.read", label: "Revisar cobertura protectora" },
  { key: "queue.read", label: "Revisar cola defensiva" },
  { key: "construction.enqueue", label: "Preparar fortificacion" },
  { key: "construction.completeDue", label: "Cerrar obras vencidas" },
  { key: "construction.link", label: "Abrir Construccion" },
  { key: "fleets.link", label: "Abrir Flotas" },
  { key: "Construct", label: "Levantar defensa" },
  { key: "Upgrade", label: "Reforzar defensa" },
] as const;

function normalizeValue(value: DefenseValue) {
  if (typeof value === "number") {
    return String(value);
  }

  if (typeof value !== "string") {
    return null;
  }

  const trimmed = value.trim();
  return trimmed.length > 0 ? trimmed : null;
}

function resolveCatalogLabel(
  value: DefenseValue,
  catalog: readonly DefenseLabelCatalogEntry[],
  fallback: string,
) {
  const normalizedValue = normalizeValue(value);
  if (!normalizedValue) {
    return fallback;
  }

  return catalog.find((entry) => entry.key.toLowerCase() === normalizedValue.toLowerCase())?.label ?? fallback;
}

function findDefenseStructure(value: DefenseValue) {
  const normalizedValue = normalizeValue(value);
  if (!normalizedValue) {
    return null;
  }

  return defenseStructureCatalog.find((entry) => entry.key.toLowerCase() === normalizedValue.toLowerCase()) ?? null;
}

function formatMinutes(totalMinutes: number) {
  const normalized = Math.max(0, Math.round(totalMinutes));
  if (normalized < 60) {
    return `${normalized} min`;
  }

  const hours = Math.floor(normalized / 60);
  const minutes = normalized % 60;
  return minutes > 0 ? `${hours} h ${minutes} min` : `${hours} h`;
}

function parseDurationText(value: string) {
  const trimmed = value.trim();
  if (!trimmed) {
    return null;
  }

  const iso = /^P(?:T(?:(\d+)H)?(?:(\d+)M)?(?:(\d+)S)?)$/i.exec(trimmed);
  if (iso) {
    const totalMinutes = (Number.parseInt(iso[1] ?? "0", 10) * 60)
      + Number.parseInt(iso[2] ?? "0", 10)
      + (Number.parseInt(iso[3] ?? "0", 10) > 0 ? 1 : 0);
    return formatMinutes(totalMinutes);
  }

  const clock = /^(\d+):(\d{2})(?::(\d{2}))?$/.exec(trimmed);
  if (clock) {
    const totalMinutes = (Number.parseInt(clock[1], 10) * 60)
      + Number.parseInt(clock[2], 10)
      + (Number.parseInt(clock[3] ?? "0", 10) > 0 ? 1 : 0);
    return formatMinutes(totalMinutes);
  }

  return trimmed;
}

export function getDefenseStructureCatalog() {
  return defenseStructureCatalog;
}

export function getDefenseReadinessCatalog() {
  return defenseReadinessCatalog;
}

export function getDefenseActionCatalog() {
  return defenseActionCatalog;
}

export function getDefenseStructureLabel(value: DefenseValue) {
  return findDefenseStructure(value)?.label ?? unknownStructureFallback;
}

export function getDefenseCategoryLabel(value: DefenseValue) {
  const structure = findDefenseStructure(value);
  return structure?.categoryLabel ?? resolveCatalogLabel(value, defenseCategoryCatalog, unknownCategoryFallback);
}

export function getDefenseReadinessLabel(value: DefenseValue) {
  return resolveCatalogLabel(value, defenseReadinessCatalog, unknownReadinessFallback);
}

export function getDefenseStatusLabel(value: DefenseValue) {
  return resolveCatalogLabel(value, defenseStatusCatalog, unknownStatusFallback);
}

export function getDefenseActionLabel(value: DefenseValue) {
  return resolveCatalogLabel(value, defenseActionCatalog, unknownActionFallback);
}

export function formatDefenseCost(
  cost: ReadonlyArray<{ resourceType: DefenseValue; quantity: number }>,
  fallback = "Sin coste defensivo visible",
) {
  const visibleCost = cost.filter((entry) => entry.quantity > 0);
  if (visibleCost.length === 0) {
    return fallback;
  }

  return visibleCost
    .map((entry) => `${formatResourceType(entry.resourceType)} ${entry.quantity}`)
    .join(" | ");
}

export function formatDefenseDuration(value: string | number | null | undefined, fallback = "Duracion defensiva no disponible") {
  if (typeof value === "number" && Number.isFinite(value)) {
    return formatMinutes(value);
  }

  if (typeof value !== "string") {
    return fallback;
  }

  return parseDurationText(value) ?? fallback;
}
