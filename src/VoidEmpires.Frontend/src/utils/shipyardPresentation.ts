import { formatSpaceAssetType } from "./domainPresentation";
import { formatResourceAmountList } from "./resourceDisplay";
import { formatProductActionLabel } from "./cockpitStatus";

type ShipyardValue = string | number | null | undefined;

interface ShipyardLabelCatalogEntry {
  key: string;
  label: string;
}

export interface ShipyardAssetPresentationEntry {
  key: string;
  label: string;
  description: string;
  imageKey: string;
  categoryKey: string;
  categoryLabel: string;
  roleKey: string;
  roleLabel: string;
}

const uncategorizedAssetFallback = "Activo orbital no catalogado";
const unknownActionFallback = "Accion de astillero no catalogada";
const unknownStatusFallback = "Estado de produccion no catalogado";

const shipyardAssetCatalog: readonly ShipyardAssetPresentationEntry[] = [
  {
    key: "ScoutCraft",
    label: "Nave exploradora",
    description: "Reconocimiento ligero para lectura de rutas y presencia orbital temprana.",
    imageKey: "ship.scout-craft",
    categoryKey: "Exploracion",
    categoryLabel: "Exploracion",
    roleKey: "Reconocimiento",
    roleLabel: "Reconocimiento rapido",
  },
  {
    key: "CargoCraft",
    label: "Nave de carga",
    description: "Transporte de suministros y soporte logico sin convertir stock en flota activa.",
    imageKey: "ship.cargo-craft",
    categoryKey: "Logistica",
    categoryLabel: "Logistica",
    roleKey: "Transporte",
    roleLabel: "Transporte de suministros",
  },
  {
    key: "EscortCraft",
    label: "Nave de escolta",
    description: "Cobertura orbital de escolta; esta build solo expone produccion y readiness.",
    imageKey: "ship.escort-craft",
    categoryKey: "Escolta",
    categoryLabel: "Escolta",
    roleKey: "Cobertura",
    roleLabel: "Cobertura orbital",
  },
  {
    key: "ColonyCraft",
    label: "Nave colonial",
    description: "Expansion colonial preparada como catalogo; la ejecucion final sigue fuera de alcance.",
    imageKey: "ship.colony-craft",
    categoryKey: "Colonial",
    categoryLabel: "Colonial",
    roleKey: "Expansion",
    roleLabel: "Expansion y asentamiento",
  },
] as const;

const assetProductionStatusCatalog: readonly ShipyardLabelCatalogEntry[] = [
  { key: "Pending", label: "Pendiente" },
  { key: "Active", label: "Activa" },
  { key: "Completed", label: "Completada" },
  { key: "Cancelled", label: "Cancelada" },
] as const;

const shipyardActionCatalog: readonly ShipyardLabelCatalogEntry[] = [
  { key: "catalog.read", label: formatProductActionLabel("review", "catalogo orbital") },
  { key: "stock.read", label: formatProductActionLabel("review", "stock orbital") },
  { key: "queue.read", label: formatProductActionLabel("review", "cola de produccion") },
  { key: "production.enqueue", label: formatProductActionLabel("produce", "activo orbital") },
  { key: "production.completeDue", label: formatProductActionLabel("confirm", "produccion vencida") },
  { key: "fleet.link", label: formatProductActionLabel("open", "Flotas") },
] as const;

function normalizeValue(value: ShipyardValue) {
  if (typeof value === "number") {
    return String(value);
  }

  if (typeof value !== "string") {
    return null;
  }

  const trimmed = value.trim();
  return trimmed.length > 0 ? trimmed : null;
}

function findAssetEntry(value: ShipyardValue) {
  const normalizedValue = normalizeValue(value);
  if (!normalizedValue) {
    return null;
  }

  return shipyardAssetCatalog.find((entry) => entry.key.toLowerCase() === normalizedValue.toLowerCase()) ?? null;
}

function resolveCatalogLabel(
  value: ShipyardValue,
  catalog: readonly ShipyardLabelCatalogEntry[],
  fallback: string,
) {
  const normalizedValue = normalizeValue(value);
  if (!normalizedValue) {
    return fallback;
  }

  return catalog.find((entry) => entry.key.toLowerCase() === normalizedValue.toLowerCase())?.label ?? fallback;
}

export function getShipyardAssetCatalog() {
  return shipyardAssetCatalog;
}

export function getShipyardProductionStatusCatalog() {
  return assetProductionStatusCatalog;
}

export function getShipyardActionCatalog() {
  return shipyardActionCatalog;
}

export function getAssetTypeLabel(value: ShipyardValue) {
  return findAssetEntry(value)?.label ?? formatSpaceAssetType(value, uncategorizedAssetFallback);
}

export function getAssetCategoryLabel(value: ShipyardValue) {
  return findAssetEntry(value)?.categoryLabel ?? "Orbital";
}

export function getAssetRoleLabel(value: ShipyardValue) {
  return findAssetEntry(value)?.roleLabel ?? uncategorizedAssetFallback;
}

export function getAssetDescription(value: ShipyardValue) {
  return findAssetEntry(value)?.description ?? uncategorizedAssetFallback;
}

export function getAssetImageKey(value: ShipyardValue) {
  return findAssetEntry(value)?.imageKey ?? null;
}

export function getAssetProductionStatusLabel(value: ShipyardValue) {
  return resolveCatalogLabel(value, assetProductionStatusCatalog, unknownStatusFallback);
}

export function getShipyardActionLabel(value: ShipyardValue) {
  return resolveCatalogLabel(value, shipyardActionCatalog, unknownActionFallback);
}

export function formatAssetQuantity(quantity: number, assetType?: ShipyardValue) {
  const label = getAssetTypeLabel(assetType);
  if (!Number.isFinite(quantity)) {
    return `Cantidad no disponible para ${label.toLowerCase()}`;
  }

  return `${quantity} ${quantity === 1 ? label.toLowerCase() : `${label.toLowerCase()}s`}`;
}

export function formatAssetProductionCost(
  cost: ReadonlyArray<{ resourceType: ShipyardValue; quantity: number }>,
  fallback = "Sin coste orbital visible",
) {
  const visibleCost = cost.filter((entry) => entry.quantity > 0);
  if (visibleCost.length === 0) {
    return fallback;
  }

  return formatResourceAmountList(visibleCost, { fallback, positiveOnly: true });
}

export function formatAssetProductionDuration(value: string | number | null | undefined) {
  if (typeof value === "number" && Number.isFinite(value)) {
    if (value < 60) {
      return `${value} min`;
    }

    const hours = Math.floor(value / 60);
    const minutes = value % 60;
    return minutes > 0 ? `${hours} h ${minutes} min` : `${hours} h`;
  }

  const normalizedValue = normalizeValue(value);
  return normalizedValue ?? "Duracion orbital no disponible";
}
