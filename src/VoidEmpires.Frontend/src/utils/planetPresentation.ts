import type {
  PlanetApiValue,
  PlanetBuildingDto,
  PlanetCockpitDto,
  PlanetCatalogEntry,
  PlanetConstructionActionDto,
  PlanetModule,
} from "../api/planetTypes";
import {
  planetBuildingCategoryCatalog,
  planetBuildingTypeCatalog,
  planetConstructionActionCatalog,
  planetModuleCatalog,
} from "../api/planetTypes";
import {
  specializedPlanetModuleRoutes,
  type PlanetModuleRouteInfo,
} from "./planetModuleRoutes";
import {
  formatCompactGuid,
  formatPlanetType,
  formatColonizationStatus,
  formatResourceType,
} from "./domainPresentation";

type PlanetValue = PlanetApiValue | null | undefined;

export interface ConstructionCommandFeedback {
  primaryMessage: string;
  technicalDetail: string | null;
}

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

function resolveModuleByCategory(category: PlanetValue): PlanetModule {
  const entry = findCatalogEntry(category, planetBuildingCategoryCatalog);

  // Some infrastructure is shared between the planet overview and construction
  // surfaces, but research, army, shipyard, defense, and logistics ownership
  // stays with their dedicated module labels.
  switch (entry?.key) {
    case "Civilian":
    case "Industrial":
      return "GeneralConstruction";
    case "Research":
      return "Research";
    case "MilitaryGround":
      return "GroundArmy";
    case "MilitarySpace":
      return "Shipyard";
    case "Defense":
      return "Defenses";
    case "Logistics":
      return "Logistics";
    default:
      return "UnknownOrDiagnostics";
  }
}

function resolveModuleByBuildingType(buildingType: PlanetValue): PlanetModule {
  const entry = findCatalogEntry(buildingType, planetBuildingTypeCatalog);

  switch (entry?.key) {
    case "CommandCenter":
    case "MetalMine":
    case "CrystalMine":
    case "GasExtractor":
    case "SolarPlant":
    case "HabitationDistrict":
    case "MedicalCenter":
    case "LogisticsHub":
      return "GeneralConstruction";
    case "ResearchLab":
      return "Research";
    case "MilitaryAcademy":
    case "Barracks":
    case "CrewAcademy":
      return "GroundArmy";
    case "Shipyard":
    case "FleetCommandCenter":
      return "Shipyard";
    case "DefenseGrid":
      return "Defenses";
    default:
      return "UnknownOrDiagnostics";
  }
}

function resolveModuleByAction(action: PlanetConstructionActionDto): PlanetModule {
  const categoryModule = resolveModuleByCategory(action.category);
  if (categoryModule !== "UnknownOrDiagnostics") {
    return categoryModule;
  }

  return resolveModuleByBuildingType(action.buildingType);
}

export function getPlanetModuleLabel(module: PlanetModule) {
  return resolveCatalogLabel(module, planetModuleCatalog, "Pendiente de clasificar");
}

export function getPlanetModuleForBuilding(
  buildingType: PlanetValue,
  category: PlanetValue,
) {
  const categoryModule = resolveModuleByCategory(category);
  if (categoryModule !== "UnknownOrDiagnostics") {
    return categoryModule;
  }

  return resolveModuleByBuildingType(buildingType);
}

export function isGeneralConstructionAction(action: PlanetConstructionActionDto) {
  const module = resolveModuleByAction(action);
  return module === "GeneralConstruction" || module === "Logistics";
}

export function isSpecializedModuleAction(action: PlanetConstructionActionDto) {
  const module = resolveModuleByAction(action);
  return (
    module === "Research"
    || module === "GroundArmy"
    || module === "Shipyard"
    || module === "Defenses"
  );
}

export function canRenderActionInModule(
  action: PlanetConstructionActionDto,
  module: PlanetModule,
) {
  const actionModule = resolveModuleByAction(action);
  if (module === "GeneralConstruction") {
    return actionModule === "GeneralConstruction" || actionModule === "Logistics";
  }

  return actionModule === module;
}

export function getActionHandoffTarget(action: PlanetConstructionActionDto) {
  const module = resolveModuleByAction(action);
  if (module === "GeneralConstruction" || module === "Logistics" || module === "UnknownOrDiagnostics") {
    return null;
  }

  return specializedPlanetModuleRoutes.find((route) => route.module === module) ?? null;
}

export function getWrongModuleMessage(action: PlanetConstructionActionDto) {
  const module = resolveModuleByAction(action);
  if (module === "UnknownOrDiagnostics") {
    return "Disponible en una cabina futura.";
  }

  return `Esta orden pertenece a ${getPlanetModuleLabel(module)}.`;
}

export function getConstructionHandoffModules() {
  return [
    "Research",
    "GroundArmy",
    "Shipyard",
    "Defenses",
  ] as const;
}

export interface ConstructionHandoffModuleInfo extends PlanetModuleRouteInfo {
  actionCount: number;
  statusLabel: string;
  summary: string;
}

export { specializedPlanetModuleRoutes };
export type { PlanetModuleRouteInfo };

export function getConstructionHandoffModuleInfo(
  actions: PlanetConstructionActionDto[],
) {
  return specializedPlanetModuleRoutes.map((route) => {
    const actionCount = actions.filter((action) => resolveModuleByAction(action) === route.module).length;

    return {
      ...route,
      actionCount,
      statusLabel: actionCount > 0 ? "Solo lectura" : "Disponible",
      summary: actionCount > 0
        ? `${actionCount} elementos gestionados en ${route.label}.`
        : `${route.label} sigue disponible aunque no tenga elementos derivados desde Construccion en esta vista.`,
    };
  });
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

export function formatConstructionCommandFailure(
  rawError: string | null | undefined,
  httpStatus?: number,
): ConstructionCommandFeedback {
  const technicalDetail = rawError?.trim() || null;

  switch (technicalDetail) {
    case "Planet already has an open construction order.":
      return {
        primaryMessage: "La colonia ya tiene una orden abierta. Espera a que la cola quede libre antes de enviar otra.",
        technicalDetail,
      };
    case "Insufficient resources.":
      return {
        primaryMessage: "No hay recursos suficientes para esta obra. Revisa el coste y acumula las reservas que faltan antes de reintentar.",
        technicalDetail,
      };
    case "Planet resource stockpile was not found.":
      return {
        primaryMessage: "La reserva del planeta no esta disponible. Recarga la cabina o revisa la seed local antes de volver a intentarlo.",
        technicalDetail,
      };
    case "Planet is not owned by the requesting civilization.":
      return {
        primaryMessage: "La colonia ya no pertenece a tu civilizacion activa. Recarga la cabina y revisa el planeta antes de volver a intentarlo.",
        technicalDetail,
      };
    case "Planet building capacity would be exceeded.":
      return {
        primaryMessage: "La colonia no tiene capacidad libre para ese edificio. Elige otra accion o amplia la infraestructura actual.",
        technicalDetail,
      };
    case "Planet building capacity was not found.":
      return {
        primaryMessage: "La capacidad de edificios no esta disponible en este entorno. Recarga la cabina o valida la seed antes de continuar.",
        technicalDetail,
      };
    case "Building was not found.":
      return {
        primaryMessage: "El edificio base ya no esta disponible para mejorar. Recarga la cabina y revisa el estado actual antes de reintentar.",
        technicalDetail,
      };
    case "Planet was not found.":
      return {
        primaryMessage: "La colonia ya no existe en la lectura actual. Recarga la cabina y vuelve a seleccionar un planeta valido.",
        technicalDetail,
      };
    case "Civilization was not found.":
      return {
        primaryMessage: "La civilizacion activa ya no esta disponible en este entorno. Revisa el contexto local antes de reintentar.",
        technicalDetail,
      };
    case "Planet id is required.":
    case "Civilization id is required.":
    case "Construction action is required.":
    case "Building type is required.":
    case "Requested date is required.":
    case "Requested date must be UTC.":
      return {
        primaryMessage: "La orden llego incompleta a la API. Revisa el contexto activo y vuelve a preparar la accion.",
        technicalDetail,
      };
    default:
      break;
  }

  if (httpStatus === 503) {
    return {
      primaryMessage: "La API de desarrollo no tiene persistencia disponible. Configura el entorno local antes de enviar la orden.",
      technicalDetail,
    };
  }

  if (httpStatus === 404) {
    return {
      primaryMessage: "La ruta de construccion no esta disponible en este entorno. Verifica que las dev endpoints sigan activas.",
      technicalDetail,
    };
  }

  if (httpStatus === 400) {
    return {
      primaryMessage: "La API rechazo la orden por validacion. Revisa el planeta activo y vuelve a preparar la accion.",
      technicalDetail,
    };
  }

  if (httpStatus === 409) {
    return {
      primaryMessage: "La orden ya no es valida con el estado actual del backend. Recarga la cabina para revisar recursos, propiedad o cola antes de reintentar.",
      technicalDetail,
    };
  }

  return {
    primaryMessage: "La orden no pudo entrar en la cola de construccion. Recarga la cabina y vuelve a intentarlo.",
    technicalDetail,
  };
}

export function formatConstructionRequestFailure(
  message: string | null | undefined,
): ConstructionCommandFeedback {
  return {
    primaryMessage: "No se pudo enviar la orden de construccion. Comprueba la conexion con la API local y vuelve a intentarlo.",
    technicalDetail: message?.trim() || null,
  };
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

export function groupBuildingsByModule(buildings: PlanetBuildingDto[]) {
  return buildings.reduce<Partial<Record<PlanetModule, PlanetBuildingDto[]>>>((accumulator, building) => {
    const module = getPlanetModuleForBuilding(building.buildingType, building.category);
    if (module === "UnknownOrDiagnostics") {
      return accumulator;
    }

    accumulator[module] ??= [];
    accumulator[module]?.push(building);
    return accumulator;
  }, {});
}

export function groupActionsByModule(actions: PlanetConstructionActionDto[]) {
  return actions.reduce<Partial<Record<PlanetModule, PlanetConstructionActionDto[]>>>((accumulator, action) => {
    const module = resolveModuleByAction(action);
    if (module === "UnknownOrDiagnostics") {
      return accumulator;
    }

    accumulator[module] ??= [];
    accumulator[module]?.push(action);
    return accumulator;
  }, {});
}
