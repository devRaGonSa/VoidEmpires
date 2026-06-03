import type {
  PlanetBuildingDto,
  PlanetCockpitDto,
  PlanetConstructionActionDto,
} from "../api/planetTypes";
import {
  formatCompactGuid,
  formatPlanetType,
  formatColonizationStatus,
  formatResourceType,
} from "./domainPresentation";

type PlanetValue = string | number | null | undefined;

const buildingTypeLabels: Record<string, string> = {
  CommandCenter: "Centro de mando",
  MetalMine: "Mina de metal",
  CrystalMine: "Mina de cristal",
  GasExtractor: "Extractor de gas",
  SolarPlant: "Planta solar",
  ResearchLab: "Laboratorio de investigacion",
  Shipyard: "Astillero",
  DefenseGrid: "Malla defensiva",
  HabitationDistrict: "Distrito habitacional",
  MedicalCenter: "Centro medico",
  MilitaryAcademy: "Academia militar",
  Barracks: "Barracones",
  CrewAcademy: "Academia de tripulacion",
  FleetCommandCenter: "Mando de flota",
  LogisticsHub: "Centro logistico",
};

const buildingCategoryLabels: Record<string, string> = {
  Civilian: "Civil",
  Industrial: "Industrial",
  Research: "Investigacion",
  MilitaryGround: "Militar terrestre",
  MilitarySpace: "Militar espacial",
  Defense: "Defensa",
  Logistics: "Logistica",
};

const constructionActionLabels: Record<string, string> = {
  Construct: "Construir",
  Upgrade: "Mejorar",
};

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

export function formatBuildingType(value: PlanetValue) {
  return resolveLabel(value, buildingTypeLabels);
}

export function formatBuildingCategory(value: PlanetValue) {
  return resolveLabel(value, buildingCategoryLabels, "Otras");
}

export function formatConstructionAction(value: PlanetValue) {
  return resolveLabel(value, constructionActionLabels);
}

export function formatConstructionStatus(value: PlanetValue) {
  return resolveLabel(value, constructionStatusLabels);
}

export function formatConstructionAvailability(value: string) {
  return constructionAvailabilityLabels[value] ?? value;
}

export function formatPlanetControlStatus(value: PlanetValue) {
  return resolveLabel(value, controlStatusLabels, "Sin control");
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
