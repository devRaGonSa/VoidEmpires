import { formatResourceType } from "./domainPresentation";
import { formatProductActionLabel } from "./cockpitStatus";

type GroundArmyValue = string | number | null | undefined;

interface GroundArmyLabelCatalogEntry {
  key: string;
  label: string;
}

export interface GroundArmyUnitPresentationEntry {
  key: string;
  label: string;
  categoryKey: string;
  categoryLabel: string;
  roleLabel: string;
  requiredStructureKey: string;
}

const unknownUnitFallback = "Unidad terrestre no catalogada";
const unknownStructureFallback = "Estructura terrestre no catalogada";
const unknownCategoryFallback = "Categoria terrestre no catalogada";
const unknownReadinessFallback = "Preparacion terrestre no catalogada";
const unknownActionFallback = "Accion terrestre no catalogada";

export interface GroundArmyErrorFeedback {
  primaryMessage: string;
  technicalDetail: string | null;
}

const groundArmyUnitCatalog: readonly GroundArmyUnitPresentationEntry[] = [
  { key: "PatrolGroup", label: "Patrulla de guarnicion", categoryKey: "Guarnicion", categoryLabel: "Guarnicion", roleLabel: "Vigilancia planetaria", requiredStructureKey: "Barracks" },
  { key: "ExpeditionGroup", label: "Grupo expedicionario", categoryKey: "PreparacionColonial", categoryLabel: "Preparacion colonial", roleLabel: "Despliegue terrestre preparado", requiredStructureKey: "MilitaryAcademy" },
  { key: "VehicleGroup", label: "Columna blindada", categoryKey: "Entrenamiento", categoryLabel: "Entrenamiento", roleLabel: "Choque terrestre pesado", requiredStructureKey: "Barracks" },
  { key: "SupportGroup", label: "Grupo de apoyo", categoryKey: "LogisticaTerrestre", categoryLabel: "Logistica terrestre", roleLabel: "Suministro y soporte de campo", requiredStructureKey: "LogisticsHub" },
] as const;

const groundArmyStructureCatalog: readonly GroundArmyLabelCatalogEntry[] = [
  { key: "Barracks", label: "Barracones" },
  { key: "MilitaryAcademy", label: "Academia militar" },
  { key: "LogisticsHub", label: "Centro logistico terrestre" },
  { key: "CommandCenter", label: "Centro de mando terrestre" },
] as const;

const groundArmyReadinessCatalog: readonly GroundArmyLabelCatalogEntry[] = [
  { key: "Available", label: "Lista para preparar" },
  { key: "Blocked", label: "Bloqueada" },
  { key: "InsufficientResources", label: "Recursos insuficientes" },
  { key: "CapacityExceeded", label: "Capacidad agotada" },
  { key: "MissingResourceStockpile", label: "Sin reservas" },
  { key: "MissingCapacityData", label: "Sin capacidad confirmada" },
  { key: "Unsupported", label: "Pendiente de activacion" },
  { key: "ReadOnly", label: "Consulta disponible" },
  { key: "Placeholder", label: "Vista preparada" },
  { key: "Pending", label: "Pendiente" },
  { key: "Active", label: "En preparacion" },
  { key: "Completed", label: "Completada" },
  { key: "Cancelled", label: "Cancelada" },
] as const;

const groundArmyActionCatalog: readonly GroundArmyLabelCatalogEntry[] = [
  { key: "catalog.read", label: formatProductActionLabel("review", "fuerzas terrestres") },
  { key: "stock.read", label: formatProductActionLabel("review", "guarnicion planetaria") },
  { key: "queue.read", label: formatProductActionLabel("review", "cola de entrenamiento") },
  { key: "production.enqueue", label: formatProductActionLabel("produce", "entrenamiento terrestre") },
  { key: "production.completeDue", label: formatProductActionLabel("confirm", "entrenamiento vencido") },
  { key: "construction.link", label: formatProductActionLabel("open", "Construccion") },
  { key: "defenses.link", label: formatProductActionLabel("open", "Defensas") },
  { key: "fleets.link", label: formatProductActionLabel("open", "Flotas") },
] as const;

function normalizeValue(value: GroundArmyValue) {
  if (typeof value === "number") {
    return String(value);
  }

  return typeof value === "string" && value.trim().length > 0
    ? value.trim()
    : null;
}

function resolveCatalogLabel(
  value: GroundArmyValue,
  catalog: readonly GroundArmyLabelCatalogEntry[],
  fallback: string,
) {
  const normalizedValue = normalizeValue(value);
  if (!normalizedValue) {
    return fallback;
  }

  return catalog.find((entry) => entry.key.toLowerCase() === normalizedValue.toLowerCase())?.label ?? fallback;
}

function findGroundArmyUnit(value: GroundArmyValue) {
  const normalizedValue = normalizeValue(value);
  if (!normalizedValue) {
    return null;
  }

  return groundArmyUnitCatalog.find((entry) => entry.key.toLowerCase() === normalizedValue.toLowerCase()) ?? null;
}

export function getGroundArmyUnitCatalog() {
  return groundArmyUnitCatalog;
}

export function getGroundArmyReadinessCatalog() {
  return groundArmyReadinessCatalog;
}

export function getGroundArmyActionCatalog() {
  return groundArmyActionCatalog;
}

export function getGroundUnitLabel(value: GroundArmyValue) {
  return findGroundArmyUnit(value)?.label ?? unknownUnitFallback;
}

export function getGroundArmyCategoryLabel(value: GroundArmyValue) {
  return findGroundArmyUnit(value)?.categoryLabel ?? unknownCategoryFallback;
}

export function getGroundArmyRoleLabel(value: GroundArmyValue) {
  return findGroundArmyUnit(value)?.roleLabel ?? unknownUnitFallback;
}

export function getGroundStructureLabel(value: GroundArmyValue) {
  return resolveCatalogLabel(value, groundArmyStructureCatalog, unknownStructureFallback);
}

export function getGroundReadinessLabel(value: GroundArmyValue) {
  return resolveCatalogLabel(value, groundArmyReadinessCatalog, unknownReadinessFallback);
}

export function getGroundActionLabel(value: GroundArmyValue) {
  return resolveCatalogLabel(value, groundArmyActionCatalog, unknownActionFallback);
}

export function formatGroundTrainingCost(
  cost: ReadonlyArray<{ resourceType: GroundArmyValue; quantity: number }>,
  fallback = "Sin coste terrestre visible",
) {
  const visibleCost = cost.filter((entry) => entry.quantity > 0);
  return visibleCost.length > 0
    ? visibleCost.map((entry) => `${formatResourceType(entry.resourceType)} ${entry.quantity}`).join(" | ")
    : fallback;
}

export function formatGroundTrainingDuration(value: string | number | null | undefined) {
  if (typeof value === "number" && Number.isFinite(value)) {
    return value < 60 ? `${value} min` : `${Math.floor(value / 60)} h ${value % 60} min`;
  }

  return normalizeValue(value) ?? "Duracion terrestre no disponible";
}

export function formatGroundArmyRequestFailure(rawError: string | null | undefined): GroundArmyErrorFeedback {
  const technicalDetail = rawError?.trim() || null;

  switch (technicalDetail) {
    case "Civilization id is required.":
      return { primaryMessage: "Falta el id de civilizacion. Revisa el contexto antes de cargar la vista.", technicalDetail };
    case "Planet was not found.":
      return { primaryMessage: "El planeta no existe o ya no esta visible. Revisa el contexto.", technicalDetail };
    case "Planet is not controlled by the requesting civilization.":
      return { primaryMessage: "La colonia seleccionada no pertenece a tu civilizacion. Abre una colonia propia.", technicalDetail };
    case "Request failed with status 404.":
      return { primaryMessage: "La vista terrestre no esta disponible en este entorno.", technicalDetail };
    case "Request failed with status 503.":
      return { primaryMessage: "El estado terrestre no esta disponible temporalmente. Revisa el contexto e intentalo de nuevo.", technicalDetail };
    case "MissingRequiredBuilding":
      return { primaryMessage: "Falta infraestructura terrestre. Abre Construccion para revisar barracones, academia o logistica.", technicalDetail };
    case "InsufficientResources":
      return { primaryMessage: "No hay recursos suficientes para esta preparacion terrestre.", technicalDetail };
    case "InsufficientPopulationCapacity":
      return { primaryMessage: "La capacidad de poblacion terrestre es insuficiente para esta preparacion.", technicalDetail };
    default:
      return { primaryMessage: "La vista terrestre no pudo completar la solicitud. Revisa el contexto e intentalo de nuevo.", technicalDetail };
  }
}
