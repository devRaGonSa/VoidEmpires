import { formatResourceType } from "./domainPresentation";
import type {
  ResearchCost as ResearchCostDto,
  ResearchDefinitionDto,
  ResearchProjectDto,
  ResearchQueueItemDto,
  ResearchTechnologyHintDto,
  ResearchUiStateDto,
} from "../api/researchTypes";

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

export interface ResearchActionAvailability {
  key: string;
  label: string;
  reasonKey: string;
  reasonLabel: string;
  canEnqueue: boolean;
  canCompleteDue: boolean;
}

export interface ResearchRequirement {
  key: string;
  label: string;
}

export interface ResearchTechnology {
  researchType: string;
  label: string;
  categoryKey: string;
  categoryLabel: string;
  bonusKey: string;
  bonusLabel: string;
  currentLevel: number;
  nextLevel: number;
  availability: ResearchActionAvailability;
  estimatedDurationLabel: string;
  estimatedCostLabel: string;
  requirements: ResearchRequirement[];
  primaryActionLabel: string;
}

export interface ResearchCategory {
  key: string;
  label: string;
  technologies: ResearchTechnology[];
}

export interface ResearchQueueItem {
  orderId: string;
  civilizationId: string;
  sourcePlanetId: string;
  researchType: string;
  label: string;
  targetLevel: number;
  sequence: number;
  startsAtUtc: string;
  endsAtUtc: string;
  statusKey: string;
  statusLabel: string;
  isDue: boolean;
}

export interface ResearchDiagnostics {
  lines: string[];
  limitations: string[];
}

export interface ResearchUiState {
  civilizationId: string;
  selectedPlanetId: string | null;
  selectedPlanetName: string | null;
  catalog: ResearchTechnology[];
  queue: ResearchQueueItem[];
  projects: ResearchTechnology[];
  technologyHints: ResearchTechnology[];
  categories: ResearchCategory[];
  diagnostics: ResearchDiagnostics;
}

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

const researchBonusCatalog: LabelCatalog = {
  names: { planet_capacity: "Capacidad planetaria", resource_output: "Produccion de recursos", energy_output: "Produccion energetica", build_speed: "Velocidad de construccion", fleet_speed: "Velocidad de flota", weapon_damage: "Danio de armamento", shield_strength: "Potencia de escudos", intel_strength: "Capacidad de espionaje" },
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

export function getResearchBonusLabel(value: ResearchValue, fallback = "Beneficio pendiente de clasificar") {
  return resolveLabel(value, researchBonusCatalog, fallback, true);
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

export function formatResearchCost(
  cost: ResearchCostDto | ReadonlyArray<ResearchResourceCost>,
  fallback = "Sin coste",
) {
  const researchCost = cost as ResearchCostDto;
  const entries = Array.isArray(cost)
    ? cost
    : [
      { resourceType: "Credits", quantity: researchCost.credits },
      { resourceType: "Metal", quantity: researchCost.metal },
      { resourceType: "Crystal", quantity: researchCost.crystal },
      { resourceType: "Gas", quantity: researchCost.gas },
    ];

  return entries.length
    ? entries.filter((item) => item.quantity > 0).map((item) => `${formatResourceType(item.resourceType)} ${item.quantity}`).join(", ")
    : fallback;
}

function findResearchTechnology(value: ResearchValue) {
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

  return researchTechnologyCatalog.find((entry) => entry.key === trimmed || normalizeName(entry.key) === normalizeName(trimmed)) ?? null;
}

function findResearchHint<T extends ResearchTechnologyHintDto | ResearchDefinitionDto | ResearchProjectDto>(value: ResearchValue, items: readonly T[]) {
  return items.find((item) => item.researchType === value || `${item.researchType}` === `${value}`) ?? null;
}

export function getResearchPrimaryAction(technology: ResearchTechnology) {
  if (technology.availability.canCompleteDue) {
    return "Completar ahora";
  }

  if (technology.availability.canEnqueue) {
    return technology.currentLevel > 0 ? "Reinvestigar" : "Iniciar investigacion";
  }

  return "Revisar requisitos";
}

export function selectRecommendedResearch(technologies: readonly ResearchTechnology[]) {
  return technologies.find((item) => item.availability.canEnqueue)
    ?? technologies.find((item) => item.availability.canCompleteDue)
    ?? technologies[0]
    ?? null;
}

export function groupResearchTechnologiesByCategory(technologies: readonly ResearchTechnology[]) {
  return researchTechnologyCatalog.map((entry) => ({
    key: entry.categoryKey,
    label: entry.categoryLabel,
    technologies: technologies.filter((technology) => technology.categoryKey === entry.categoryKey),
  })).filter((group) => group.technologies.length > 0);
}

function mapResearchTechnology(
  definition: ResearchDefinitionDto,
  hint: ResearchTechnologyHintDto | null,
  projectLevel: number | null,
) {
  const catalogEntry = findResearchTechnology(definition.researchType);
  const researchType = `${definition.researchType ?? ""}`.trim() || "Unknown";
  const label = getResearchTechnologyLabel(definition.researchType);
  const categoryKey = catalogEntry?.categoryKey ?? "Unknown";
  const categoryLabel = catalogEntry?.categoryLabel ?? getResearchCategoryLabel(categoryKey);
  const bonusKey = `${definition.bonusKey ?? ""}`.trim() || "Unknown";
  const bonusLabel = getResearchBonusLabel(bonusKey);
  const currentLevel = hint?.currentLevel ?? projectLevel ?? 0;
  const nextLevel = hint?.nextLevel ?? currentLevel + 1;
  const availability: ResearchActionAvailability = {
    key: hint?.statusKey ?? (projectLevel ? "Completed" : "Blocked"),
    label: getResearchStatusLabel(hint?.statusKey ?? (projectLevel ? "Completed" : "Blocked")),
    reasonKey: hint?.availabilityReasonKey ?? "SourcePlanet",
    reasonLabel: getResearchRequirementLabel(hint?.availabilityReasonKey ?? "SourcePlanet"),
    canEnqueue: hint?.canEnqueue ?? false,
    canCompleteDue: hint?.canCompleteDue ?? false,
  };

  return {
    researchType,
    label,
    categoryKey,
    categoryLabel,
    bonusKey,
    bonusLabel,
    currentLevel,
    nextLevel,
    availability,
    estimatedDurationLabel: formatResearchDuration(hint?.estimatedDuration),
    estimatedCostLabel: formatResearchCost(hint?.estimatedCost ?? definition.baseCost),
    requirements: (hint?.requirementKeys ?? ["SourcePlanet", "ResearchQueueSlot", "ResourceStockpile"])
      .map((key) => ({ key: `${key}`, label: getResearchRequirementLabel(key) })),
    primaryActionLabel: getResearchPrimaryAction({
      researchType,
      label,
      categoryKey,
      categoryLabel,
      bonusKey,
      bonusLabel,
      currentLevel,
      nextLevel,
      availability,
      estimatedDurationLabel: formatResearchDuration(hint?.estimatedDuration),
      estimatedCostLabel: formatResearchCost(hint?.estimatedCost ?? definition.baseCost),
      requirements: (hint?.requirementKeys ?? ["SourcePlanet", "ResearchQueueSlot", "ResourceStockpile"])
        .map((key) => ({ key: `${key}`, label: getResearchRequirementLabel(key) })),
      primaryActionLabel: "No disponible",
    }),
  } satisfies ResearchTechnology;
}

function mapResearchQueueItem(item: ResearchQueueItemDto) {
  const statusKey = `${item.status}`;
  const statusLabel = getResearchStatusLabel(item.status);
  const endsAt = Date.parse(item.endsAtUtc);

  return {
    orderId: item.orderId,
    civilizationId: item.civilizationId,
    sourcePlanetId: item.sourcePlanetId,
    researchType: `${item.researchType ?? ""}`.trim() || "Unknown",
    label: getResearchTechnologyLabel(item.researchType),
    targetLevel: item.targetLevel,
    sequence: item.sequence,
    startsAtUtc: item.startsAtUtc,
    endsAtUtc: item.endsAtUtc,
    statusKey,
    statusLabel,
    isDue: statusKey === "Active" && !Number.isNaN(endsAt) && endsAt <= Date.now(),
  } satisfies ResearchQueueItem;
}

function mapResearchProject(item: ResearchProjectDto) {
  const catalogEntry = findResearchTechnology(item.researchType);
  return {
    researchType: `${item.researchType ?? ""}`.trim() || "Unknown",
    label: getResearchTechnologyLabel(item.researchType),
    categoryKey: catalogEntry?.categoryKey ?? "Unknown",
    categoryLabel: catalogEntry?.categoryLabel ?? getResearchCategoryLabel(catalogEntry?.categoryKey ?? "Unknown"),
    bonusKey: catalogEntry?.key ?? "Completed",
    bonusLabel: catalogEntry ? catalogEntry.label : "Completada",
    currentLevel: item.level,
    nextLevel: item.level + 1,
    availability: {
      key: "Completed",
      label: "Completada",
      reasonKey: "ResearchQueueSlot",
      reasonLabel: getResearchRequirementLabel("ResearchQueueSlot"),
      canEnqueue: false,
      canCompleteDue: false,
    },
    estimatedDurationLabel: "No disponible",
    estimatedCostLabel: "Sin coste",
    requirements: [],
    primaryActionLabel: "Completada",
  } satisfies ResearchTechnology;
}

export function mapResearchUiStateToViewModel(state: ResearchUiStateDto): ResearchUiState {
  const hintByType = new Map(state.technologyHints.map((hint) => [`${hint.researchType}`, hint]));
  const projectLevels = new Map(state.projects.map((project) => [`${project.researchType}`, project.level]));
  const catalog = state.catalog.map((definition) => mapResearchTechnology(
    definition,
    hintByType.get(`${definition.researchType}`) ?? null,
    projectLevels.get(`${definition.researchType}`) ?? null,
  ));
  const projects = state.projects.map((project) => mapResearchProject(project));

  return {
    civilizationId: state.civilizationId,
    selectedPlanetId: state.selectedPlanetId,
    selectedPlanetName: state.selectedPlanetName,
    catalog,
    queue: state.queue.map(mapResearchQueueItem),
    projects,
    technologyHints: catalog,
    categories: groupResearchTechnologiesByCategory(catalog),
    diagnostics: {
      lines: [
        `Catalogo: ${catalog.length} tecnologias.`,
        `Cola activa: ${state.queue.filter((item) => `${item.status}` === "1" || `${item.status}` === "2").length} ordenes.`,
        `Proyectos completados: ${projects.length}.`,
      ],
      limitations: [...state.limitations],
    },
  };
}
