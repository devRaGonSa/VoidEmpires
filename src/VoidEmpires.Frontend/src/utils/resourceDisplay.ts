type ResourceValue = string | number | null | undefined;

export interface ResourceAmount {
  resourceType: ResourceValue;
  quantity: number;
}

export const resourceTerminologyV1 = {
  credits: "Créditos",
  metal: "Metal",
  crystal: "Cristal",
  gas: "Gas",
  deuterium: "Deuterio",
  energy: "Energía",
  population: "Población",
} as const;

export const resourceTerminologyNotesV1 = [
  "Créditos is the spendable economy balance.",
  "Energía is a power or infrastructure signal, not a synonym for Créditos.",
  "Deuterio remains a visible future term only and is not part of the persisted stockpile.",
  "Población is colony capacity context, not a stockpile currency.",
] as const;

const resourceLabelsByName: Record<string, string> = {
  Credit: resourceTerminologyV1.credits,
  Credits: resourceTerminologyV1.credits,
  Creditos: resourceTerminologyV1.credits,
  Créditos: resourceTerminologyV1.credits,
  Metal: resourceTerminologyV1.metal,
  Crystal: resourceTerminologyV1.crystal,
  Cristal: resourceTerminologyV1.crystal,
  Gas: resourceTerminologyV1.gas,
  Deuterium: resourceTerminologyV1.deuterium,
  Deuterio: resourceTerminologyV1.deuterium,
  Energy: resourceTerminologyV1.energy,
  Energia: resourceTerminologyV1.energy,
  Energía: resourceTerminologyV1.energy,
  Population: resourceTerminologyV1.population,
  Poblacion: resourceTerminologyV1.population,
  Población: resourceTerminologyV1.population,
};

const resourceLabelsByNumber: Record<number, string> = {
  1: resourceTerminologyV1.credits,
  2: resourceTerminologyV1.metal,
  3: resourceTerminologyV1.crystal,
  4: resourceTerminologyV1.gas,
  5: resourceTerminologyV1.energy,
};

function normalizeResourceName(value: string) {
  return value
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "")
    .replace(/[\s_-]+/g, "")
    .toLowerCase();
}

export function formatResourceLabel(value: ResourceValue, fallback = "Recurso") {
  if (typeof value === "number") {
    return resourceLabelsByNumber[value] ?? String(value);
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
    return resourceLabelsByNumber[numeric] ?? trimmed;
  }

  const direct = resourceLabelsByName[trimmed];
  if (direct) {
    return direct;
  }

  const normalized = normalizeResourceName(trimmed);
  for (const [key, label] of Object.entries(resourceLabelsByName)) {
    if (normalizeResourceName(key) === normalized) {
      return label;
    }
  }

  return trimmed;
}

export function formatResourceAmount(entry: ResourceAmount) {
  return `${formatResourceLabel(entry.resourceType)} ${entry.quantity}`;
}

export function formatResourceDelta(entry: ResourceAmount) {
  const prefix = entry.quantity > 0 ? "+" : "";
  return `${formatResourceLabel(entry.resourceType)} ${prefix}${entry.quantity}`;
}

export function formatResourceAmountList(
  entries: ReadonlyArray<ResourceAmount>,
  options?: {
    fallback?: string;
    separator?: string;
    positiveOnly?: boolean;
    limit?: number;
  },
) {
  const visibleEntries = entries
    .filter((entry) => !options?.positiveOnly || entry.quantity > 0)
    .slice(0, options?.limit);

  return visibleEntries.length > 0
    ? visibleEntries.map(formatResourceAmount).join(options?.separator ?? " | ")
    : options?.fallback ?? "Sin recursos";
}
