import { formatResourceType } from "./domainPresentation";

type ResearchValue = string | number | null | undefined;

interface LabelCatalog { names: Record<string, string>; numbers?: Record<number, string>; }

export interface ResearchCatalogEntry {
  id: number;
  key: string;
  label: string;
  categoryKey: string;
  categoryLabel: string;
}

export interface ResearchResourceCost { resourceType: ResearchValue; quantity: number; }

export const researchTechnologyCatalog: readonly ResearchCatalogEntry[] = [
  { id: 1, key: "PlanetaryEngineering", label: "Ingenieria planetaria", categoryKey: "Colonizacion", categoryLabel: "Colonizacion" },
  { id: 2, key: "ResourceExtraction", label: "Extraccion de recursos", categoryKey: "Economia", categoryLabel: "Economia" },
  { id: 3, key: "EnergySystems", label: "Sistemas energeticos", categoryKey: "Energia", categoryLabel: "Energia" },
  { id: 4, key: "ConstructionAutomation", label: "Automatizacion de construccion", categoryKey: "Administracion", categoryLabel: "Administracion" },
  { id: 5, key: "Propulsion", label: "Propulsion", categoryKey: "Logistica", categoryLabel: "Logistica" },
  { id: 6, key: "ShipWeapons", label: "Armas de nave", categoryKey: "Militar espacial", categoryLabel: "Militar espacial" },
  { id: 7, key: "Shielding", label: "Escudos", categoryKey: "Defensa", categoryLabel: "Defensa" },
  { id: 8, key: "Espionage", label: "Espionaje", categoryKey: "Exploracion", categoryLabel: "Exploracion" },
] as const;

const researchTechnologyLabelCatalog: LabelCatalog = {
  names: Object.fromEntries(researchTechnologyCatalog.map((entry) => [entry.key, entry.label])),
  numbers: Object.fromEntries(researchTechnologyCatalog.map((entry) => [entry.id, entry.label])),
};

const researchCategoryCatalog: LabelCatalog = {
  names: { Economia: "Economia", Energia: "Energia", Logistica: "Logistica", Exploracion: "Exploracion", MilitaryGround: "Militar terrestre", MilitarySpace: "Militar espacial", Defensa: "Defensa", Colonizacion: "Colonizacion", Administracion: "Administracion" },
  numbers: { 1: "Economia", 2: "Energia", 3: "Logistica", 4: "Exploracion", 5: "Militar terrestre", 6: "Militar espacial", 7: "Defensa", 8: "Colonizacion", 9: "Administracion" },
};

const researchStatusCatalog: LabelCatalog = {
  names: { Available: "Disponible", InResearch: "En investigacion", Pending: "En investigacion", Active: "En investigacion", Completed: "Completada", Blocked: "Bloqueada", Cancelled: "Bloqueada", InsufficientResources: "Recursos insuficientes", RequirementPending: "Requisito pendiente", NotAvailableInThisBuild: "No disponible en esta build" },
  numbers: { 1: "Disponible", 2: "En investigacion", 3: "Completada", 4: "Bloqueada", 5: "Recursos insuficientes", 6: "Requisito pendiente", 7: "No disponible en esta build" },
};

const researchRequirementCatalog: LabelCatalog = {
  names: { SourcePlanet: "Planeta de origen", Civilization: "Civilizacion", ResearchLab: "Laboratorio de investigacion", ResourceStockpile: "Reservas de recursos", ResearchQueueSlot: "Hueco en cola", PlanetaryEngineering: "Ingenieria planetaria", ResourceExtraction: "Extraccion de recursos", EnergySystems: "Sistemas energeticos", ConstructionAutomation: "Automatizacion de construccion", Propulsion: "Propulsion", ShipWeapons: "Armas de nave", Shielding: "Escudos", Espionage: "Espionaje" },
  numbers: { 1: "Planeta de origen", 2: "Civilizacion", 3: "Laboratorio de investigacion", 4: "Reservas de recursos", 5: "Hueco en cola", 6: "Ingenieria planetaria", 7: "Extraccion de recursos", 8: "Sistemas energeticos", 9: "Automatizacion de construccion", 10: "Propulsion", 11: "Armas de nave", 12: "Escudos", 13: "Espionaje" },
};

function normalizeName(value: string) {
  return value.replace(/[\s_-]+/g, "").toLowerCase();
}

function resolveLabel(value: ResearchValue, catalog: LabelCatalog, fallback = "No disponible", strict = false) {
  if (typeof value === "number") {
    return catalog.numbers?.[value] ?? (strict ? fallback : String(value));
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
    return catalog.numbers?.[numeric] ?? (strict ? fallback : trimmed);
  }
  const direct = catalog.names[trimmed];
  if (direct) {
    return direct;
  }
  const normalized = normalizeName(trimmed);
  for (const [key, label] of Object.entries(catalog.names)) {
    if (normalizeName(key) === normalized) {
      return label;
    }
  }
  return strict ? fallback : trimmed;
}

function findTechnology(value: ResearchValue) {
  if (typeof value === "number") {
    return researchTechnologyCatalog.find((entry) => entry.id === value) ?? null;
  }
  if (typeof value !== "string") {
    return null;
  }
  const trimmed = value.trim();
  if (!trimmed) {
    return null;
  }
  const direct = researchTechnologyCatalog.find((entry) => entry.key === trimmed);
  if (direct) {
    return direct;
  }
  const normalized = normalizeName(trimmed);
  return researchTechnologyCatalog.find((entry) => normalizeName(entry.key) === normalized) ?? null;
}

function formatResearchMinutes(totalMinutes: number) {
  const normalized = Math.max(0, Math.round(totalMinutes));
  const hours = Math.floor(normalized / 60);
  const minutes = normalized % 60;
  if (hours === 0) {
    return `${minutes} min`;
  }
  if (minutes === 0) {
    return `${hours} h`;
  }
  return `${hours} h ${minutes} min`;
}

function parseDurationText(value: string) {
  const trimmed = value.trim();
  if (!trimmed) {
    return null;
  }
  const iso = /^P(?:T(?:(\d+)H)?(?:(\d+)M)?(?:(\d+)S)?)$/i.exec(trimmed);
  if (iso) {
    const totalMinutes = (Number.parseInt(iso[1] ?? "0", 10) * 60) + Number.parseInt(iso[2] ?? "0", 10) + (Number.parseInt(iso[3] ?? "0", 10) > 0 ? 1 : 0);
    return formatResearchMinutes(totalMinutes);
  }
  const clock = /^(\d+):(\d{2})(?::(\d{2}))?$/.exec(trimmed);
  if (clock) {
    const totalMinutes = (Number.parseInt(clock[1], 10) * 60) + Number.parseInt(clock[2], 10) + (Number.parseInt(clock[3] ?? "0", 10) > 0 ? 1 : 0);
    return formatResearchMinutes(totalMinutes);
  }
  return trimmed;
}

export function getResearchTechnologyLabel(value: ResearchValue, fallback = "Tecnologia pendiente de clasificar") {
  return findTechnology(value)?.label ?? resolveLabel(value, researchTechnologyLabelCatalog, fallback, true);
}

export function getResearchCategoryLabel(value: ResearchValue, fallback = "Categoria pendiente de clasificar") {
  const technology = findTechnology(value);
  return technology ? technology.categoryLabel : resolveLabel(value, researchCategoryCatalog, fallback, true);
}

export function getResearchStatusLabel(value: ResearchValue, fallback = "Estado pendiente de clasificar") {
  return resolveLabel(value, researchStatusCatalog, fallback, true);
}

export function getResearchRequirementLabel(value: ResearchValue, fallback = "Requisito pendiente de clasificar") {
  return resolveLabel(value, researchRequirementCatalog, fallback, true);
}

export function formatResearchDuration(value: string | number | null | undefined, fallback = "Duracion no disponible") {
  if (typeof value === "number") {
    return formatResearchMinutes(value);
  }
  if (typeof value !== "string") {
    return fallback;
  }
  return parseDurationText(value) ?? fallback;
}

export function formatResearchCost(cost: ReadonlyArray<ResearchResourceCost>, fallback = "Sin coste") {
  return cost.length ? cost.map((item) => `${formatResourceType(item.resourceType)} ${item.quantity}`).join(", ") : fallback;
}

