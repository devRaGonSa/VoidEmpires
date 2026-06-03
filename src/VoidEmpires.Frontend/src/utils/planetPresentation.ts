import type {
  PlanetApiValue,
  PlanetBuildingDto,
  PlanetCockpitDto,
  PlanetCatalogEntry,
  PlanetConstructionActionDto,
} from "../api/planetTypes";
import {
  planetBuildingCategoryCatalog,
  planetBuildingTypeCatalog,
  planetConstructionActionCatalog,
} from "../api/planetTypes";
import {
  formatCompactGuid,
  formatPlanetType,
  formatColonizationStatus,
  formatResourceType,
} from "./domainPresentation";

type PlanetValue = PlanetApiValue | null | undefined;

const constructionStatusLabels: Record<string, string> = {
  Pending: "Pendiente",
  Active: "Activa",
  Completed: "Completada",
  Cancelled: "Cancelada",
};

const constructionAvailabilityLabels: Record<string, string> = {
  Available: "Disponible",
  Blocked: "Bloqueada",
  MissingResourceStockpile: "Sin reservas",
  MissingCapacityData: "Sin capacidad",
  CapacityExceeded: "Capacidad agotada",
  InsufficientResources: "Recursos insuficientes",
  Unsupported: "No disponible",
};

const controlStatusLabels: Record<string, string> = {
  Active: "Control activo",
  Abandoned: "Abandonado",
  Lost: "Perdido",
};

function normalizeName(value: string) {
  return value.replace(/[\s_-]+/g, "").toLowerCase();
}

function resolveLabel(value: PlanetValue, labels: Record<string, string>, fallback = "No disponible") {
  if (typeof value === "number") {
    return resolveLabel(`${value}`, labels, fallback);
  }

  if (typeof value !== "string") {
    return fallback;
  }

  const trimmed = value.trim();
  if (trimmed.length === 0) {
    return fallback;
  }

  const direct = labels[trimmed];
  if (direct) {
    return direct;
  }

  const normalized = normalizeName(trimmed);
  for (const [key, label] of Object.entries(labels)) {
    if (normalizeName(key) === normalized) {
      return label;
    }
  }

  return trimmed;
}

function findCatalogEntry(
  value: PlanetValue,
  catalog: readonly PlanetCatalogEntry[],
) {
  if (typeof value === "number") {
    return catalog.find((entry) => entry.id === value) ?? null;
  }

  if (typeof value !== "string") {
    return null;
  }

  const trimmed = value.trim();
  if (trimmed.length === 0) {
    return null;
  }

  const direct = catalog.find((entry) => entry.key === trimmed);
  if (direct) {
    return direct;
  }

  const normalized = normalizeName(trimmed);
  return catalog.find((entry) => normalizeName(entry.key) === normalized) ?? null;
}

function resolveCatalogLabel(
  value: PlanetValue,
  catalog: readonly PlanetCatalogEntry[],
  fallback = "No disponible",
) {
  const entry = findCatalogEntry(value, catalog);
  if (entry) {
    return entry.label;
  }

  if (typeof value !== "string") {
    return fallback;
  }

  const trimmed = value.trim();
  if (trimmed.length === 0) {
    return fallback;
  }

  return trimmed;
}

export function formatBuildingType(value: PlanetValue) {
  return resolveCatalogLabel(value, planetBuildingTypeCatalog);
}

export function formatBuildingCategory(value: PlanetValue) {
  return resolveCatalogLabel(value, planetBuildingCategoryCatalog, "Otras");
}

export function formatConstructionAction(value: PlanetValue) {
  return resolveCatalogLabel(value, planetConstructionActionCatalog);
}

export function formatConstructionStatus(value: PlanetValue) {
  return resolveLabel(value, constructionStatusLabels);
}

export function formatConstructionQueuePhase(
  status: PlanetValue,
  isDue: boolean,
) {
  if (isDue) {
    return "Pendiente de cierre";
  }

  switch (status) {
    case "Pending":
    case 1:
      return "En espera de inicio";
    case "Active":
    case 2:
      return "En desarrollo";
    case "Completed":
    case 3:
      return "Finalizada";
    case "Cancelled":
    case 4:
      return "Cancelada";
    default:
      return "Estado no disponible";
  }
}

export function formatConstructionEnqueueSuccess(
  buildingType: PlanetValue,
  targetLevel: number,
) {
  return `${formatBuildingType(buildingType)} en cola para nivel ${targetLevel}. La cabina se actualizo con el estado confirmado por la API.`;
}

export function formatConstructionAvailability(value: string) {
  return constructionAvailabilityLabels[value] ?? value;
}

export function formatConstructionActionButtonLabel(
  availabilityStatus: string,
  isPrepared: boolean,
) {
  if (availabilityStatus === "Available") {
    return isPrepared ? "Orden preparada" : "Preparar construccion";
  }

  switch (availabilityStatus) {
    case "InsufficientResources":
      return "Faltan recursos";
    case "CapacityExceeded":
      return "Sin capacidad";
    case "MissingResourceStockpile":
      return "Sin reservas";
    case "MissingCapacityData":
      return "Capacidad no disponible";
    case "Blocked":
      return "Accion bloqueada";
    case "Unsupported":
      return "Solo consulta";
    default:
      return "No disponible";
  }
}

export function formatPlanetControlStatus(value: PlanetValue) {
  return resolveLabel(value, controlStatusLabels, "Sin control");
}

export function toPlanetCatalogId(
  value: PlanetApiValue,
  catalog: readonly PlanetCatalogEntry[],
) {
  const entry = findCatalogEntry(value, catalog);
  return entry?.id ?? value;
}

export function formatPlanetOwnerLabel(planet: PlanetCockpitDto) {
  if (planet.isOwnedByRequestingCivilization) {
    return "Colonia propia";
  }

  if (planet.ownerCivilizationName) {
    return planet.ownerCivilizationName;
  }

  return "Sin control confirmado";
}

export function formatPlanetIdentity(planet: PlanetCockpitDto) {
  return `${planet.planetName} | ${planet.solarSystemName}`;
}

export function formatPlanetShortReference(planetId: string) {
  return `ID ${formatCompactGuid(planetId)}`;
}

export function formatPlanetOverviewLine(planet: PlanetCockpitDto) {
  return `${formatPlanetType(planet.planetType)} | ${formatColonizationStatus(planet.colonizationStatus)} | Orbita ${planet.orbitalSlot}`;
}

export function formatResourceBalanceLine(resourceType: PlanetValue, quantity: number) {
  return `${formatResourceType(resourceType)} ${quantity}`;
}

export function formatCompactResourceCost(
  cost: ReadonlyArray<{ resourceType: PlanetValue; quantity: number }>,
) {
  return cost.length
    ? cost
      .filter((item) => item.quantity > 0)
      .map((item) => `${formatResourceType(item.resourceType)} ${item.quantity}`)
      .join(" | ")
    : "Sin coste";
}

export function formatMissingPlanetResources(
  stockpile: ReadonlyArray<{ resourceType: PlanetValue; quantity: number }>,
  cost: ReadonlyArray<{ resourceType: PlanetValue; quantity: number }>,
) {
  const missingResources = cost
    .map((item) => {
      const available = stockpile.find((balance) => balance.resourceType === item.resourceType)?.quantity ?? 0;
      const missing = Math.max(0, item.quantity - available);

      return {
        resourceType: item.resourceType,
        missing,
      };
    })
    .filter((item) => item.missing > 0);

  return missingResources.length > 0
    ? `Faltan ${missingResources.map((item) => `${formatResourceType(item.resourceType)} ${item.missing}`).join(" | ")}`
    : null;
}

export function groupBuildingsByCategory(buildings: PlanetBuildingDto[]) {
  return buildings.reduce<Record<string, PlanetBuildingDto[]>>((accumulator, building) => {
    const key = formatBuildingCategory(building.category);
    accumulator[key] ??= [];
    accumulator[key].push(building);
    return accumulator;
  }, {});
}

export function groupActionsByCategory(actions: PlanetConstructionActionDto[]) {
  return actions.reduce<Record<string, PlanetConstructionActionDto[]>>((accumulator, action) => {
    const key = formatBuildingCategory(action.category);
    accumulator[key] ??= [];
    accumulator[key].push(action);
    return accumulator;
  }, {});
}
