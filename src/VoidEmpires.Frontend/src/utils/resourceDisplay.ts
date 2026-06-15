type ResourceValue = string | number | null | undefined;

export interface ResourceAmount {
  resourceType: ResourceValue;
  quantity: number;
}

const resourceLabelsByName: Record<string, string> = {
  Credits: "Creditos",
  Metal: "Metal",
  Crystal: "Cristal",
  Gas: "Gas",
  Energy: "Energía",
};

const resourceLabelsByNumber: Record<number, string> = {
  1: "Creditos",
  2: "Metal",
  3: "Cristal",
  4: "Gas",
  5: "Energía",
};

function normalizeResourceName(value: string) {
  return value.replace(/[\s_-]+/g, "").toLowerCase();
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
