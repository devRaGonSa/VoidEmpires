type MarketValue = string | number | null | undefined;

interface MarketLabelEntry {
  key: string;
  label: string;
}

const unknownResourceFallback = "Recurso no clasificado";
const unknownSignalFallback = "Senal economica no clasificada";
const unknownConfidenceFallback = "Confianza orientativa no clasificada";
const unknownTradeStateFallback = "Estado no clasificado";
const unknownActionFallback = "Lectura de mercado no clasificada";

const marketResourceCatalog: readonly MarketLabelEntry[] = [
  { key: "Credits", label: "Creditos" },
  { key: "Metal", label: "Metal" },
  { key: "Crystal", label: "Cristal" },
  { key: "Gas", label: "Gas" },
  { key: "Deuterium", label: "Deuterio" },
  { key: "Energy", label: "Energia" },
] as const;

const marketSignalCatalog: readonly MarketLabelEntry[] = [
  { key: "LocalReserve", label: "Reserva local" },
  { key: "EstimatedProduction", label: "Produccion estimada" },
  { key: "DemandPressure", label: "Presion de demanda" },
  { key: "VisibleSurplus", label: "Excedente visible" },
  { key: "ReferencePrice", label: "Referencia de intercambio" },
  { key: "FutureTradeRoute", label: "Ruta comercial futura" },
] as const;

const priceConfidenceCatalog: readonly MarketLabelEntry[] = [
  { key: "High", label: "Alta" },
  { key: "Medium", label: "Media" },
  { key: "Low", label: "Baja" },
  { key: "Derived", label: "Derivada" },
  { key: "Advisory", label: "Orientativa" },
] as const;

const tradeStateCatalog: readonly MarketLabelEntry[] = [
  { key: "AdvisoryOnly", label: "Solo lectura" },
  { key: "LocalReserve", label: "Reserva local" },
  { key: "DemandPressure", label: "Presion de demanda" },
  { key: "VisibleSurplus", label: "Excedente visible" },
  { key: "FutureTradeRoute", label: "Ruta comercial futura" },
  { key: "OperationUnavailable", label: "No disponible en esta version" },
] as const;

const marketActionCatalog: readonly MarketLabelEntry[] = [
  { key: "market.read", label: "Lectura economica" },
  { key: "market.signal.read", label: "Lectura economica prioritaria" },
  { key: "market.reference.read", label: "Referencias orientativas" },
  { key: "market.route.future", label: "Crear ruta comercial no disponible" },
  { key: "market.buy.future", label: "Compra no disponible en esta version" },
  { key: "market.sell.future", label: "Venta no disponible en esta version" },
  { key: "market.transfer.future", label: "Transferencia de recursos no disponible" },
  { key: "market.auction.future", label: "Subasta no disponible en esta version" },
] as const;

function normalizeValue(value: MarketValue) {
  if (typeof value === "number") {
    return String(value);
  }

  if (typeof value !== "string") {
    return null;
  }

  const trimmed = value.trim();
  return trimmed.length > 0 ? trimmed : null;
}

function normalizeLookup(value: string) {
  return value.replace(/[\s_.-]+/g, "").toLowerCase();
}

function resolveCatalogLabel(
  value: MarketValue,
  catalog: readonly MarketLabelEntry[],
  fallback: string,
) {
  const normalizedValue = normalizeValue(value);
  if (!normalizedValue) {
    return fallback;
  }

  const direct = catalog.find((entry) => entry.key === normalizedValue);
  if (direct) {
    return direct.label;
  }

  const normalizedLookup = normalizeLookup(normalizedValue);
  return catalog.find((entry) => normalizeLookup(entry.key) === normalizedLookup)?.label ?? fallback;
}

function formatMarketNumber(value: number, maximumFractionDigits = 0) {
  return new Intl.NumberFormat("es-ES", {
    maximumFractionDigits,
    minimumFractionDigits: maximumFractionDigits > 0 ? 0 : undefined,
  }).format(value);
}

export function getMarketResourceLabel(value: MarketValue, fallback = unknownResourceFallback) {
  return resolveCatalogLabel(value, marketResourceCatalog, fallback);
}

export function getMarketSignalLabel(value: MarketValue, fallback = unknownSignalFallback) {
  return resolveCatalogLabel(value, marketSignalCatalog, fallback);
}

export function getPriceConfidenceLabel(value: MarketValue, fallback = unknownConfidenceFallback) {
  return resolveCatalogLabel(value, priceConfidenceCatalog, fallback);
}

export function getTradeStateLabel(value: MarketValue, fallback = unknownTradeStateFallback) {
  return resolveCatalogLabel(value, tradeStateCatalog, fallback);
}

export function getMarketActionLabel(value: MarketValue, fallback = unknownActionFallback) {
  return resolveCatalogLabel(value, marketActionCatalog, fallback);
}

export function formatMarketResourceAmount(
  quantity: number | null | undefined,
  resourceType?: MarketValue,
) {
  const resourceLabel = getMarketResourceLabel(resourceType);
  if (typeof quantity !== "number" || !Number.isFinite(quantity)) {
    return `${resourceLabel}: sin lectura visible`;
  }

  return `${resourceLabel}: ${formatMarketNumber(quantity, Number.isInteger(quantity) ? 0 : 2)}`;
}

export function formatMarketRatio(
  value: number | null | undefined,
  fallback = "Sin referencia visible",
) {
  if (typeof value !== "number" || !Number.isFinite(value)) {
    return fallback;
  }

  return `${formatMarketNumber(value, 2)}x`;
}

export function getMarketResourceCatalog() {
  return marketResourceCatalog;
}

export function getMarketSignalCatalog() {
  return marketSignalCatalog;
}

export function getPriceConfidenceCatalog() {
  return priceConfidenceCatalog;
}

export function getTradeStateCatalog() {
  return tradeStateCatalog;
}

export function getMarketActionCatalog() {
  return marketActionCatalog;
}
