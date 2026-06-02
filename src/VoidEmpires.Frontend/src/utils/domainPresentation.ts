type DomainValue = string | number | null | undefined;

interface LabelCatalog {
  names: Record<string, string>;
  numbers?: Record<number, string>;
}

const planetTypeLabels: LabelCatalog = {
  names: {
    Terran: "Terrano",
    Desert: "Desertico",
    Ice: "Helado",
    Volcanic: "Volcanico",
    Oceanic: "Oceanico",
    Barren: "Yermo",
    GasGiant: "Gigante gaseoso",
  },
  numbers: {
    1: "Terrano",
    2: "Desertico",
    3: "Helado",
    4: "Volcanico",
    5: "Oceanico",
    6: "Yermo",
    7: "Gigante gaseoso",
  },
};

const colonizationStatusLabels: LabelCatalog = {
  names: {
    Uncolonized: "Sin colonizar",
    Colonized: "Colonizado",
    Reserved: "Reservado",
    Ruined: "En ruinas",
  },
  numbers: {
    1: "Sin colonizar",
    2: "Colonizado",
    3: "Reservado",
    4: "En ruinas",
  },
};

const spaceAssetTypeLabels: LabelCatalog = {
  names: {
    ScoutCraft: "Nave exploradora",
    CargoCraft: "Nave de carga",
    EscortCraft: "Nave de escolta",
    ColonyCraft: "Nave colonial",
  },
  numbers: {
    1: "Nave exploradora",
    2: "Nave de carga",
    3: "Nave de escolta",
    4: "Nave colonial",
  },
};

const orbitalGroupStatusLabels: LabelCatalog = {
  names: {
    Stationed: "Estacionado",
    Reserved: "Reservado",
    Decommissioned: "Retirado",
  },
  numbers: {
    1: "Estacionado",
    2: "Reservado",
    3: "Retirado",
  },
};

const resourceTypeLabels: LabelCatalog = {
  names: {
    Credits: "Creditos",
    Metal: "Metal",
    Crystal: "Cristal",
    Gas: "Gas",
  },
  numbers: {
    1: "Creditos",
    2: "Metal",
    3: "Cristal",
    4: "Gas",
  },
};

const visibilityLevelLabels: LabelCatalog = {
  names: {
    Unknown: "Desconocido",
    Visible: "Visible",
    Owned: "Propio",
  },
  numbers: {
    0: "Desconocido",
    1: "Visible",
    2: "Propio",
  },
};

const visibilityReasonLabels: LabelCatalog = {
  names: {
    NoKnownVisibilitySource: "Sin fuente de visibilidad",
    OwnedPlanet: "Planeta propio",
    SystemContainsOwnedPlanet: "Sistema con planeta propio",
    ExploredSystem: "Sistema explorado",
    ExploredPlanet: "Planeta explorado",
  },
  numbers: {
    0: "Sin fuente de visibilidad",
    1: "Planeta propio",
    2: "Sistema con planeta propio",
    3: "Sistema explorado",
    4: "Planeta explorado",
  },
};

const commandBlockReasonLabels: LabelCatalog = {
  names: {
    None: "Sin bloqueo",
    Unknown: "Desconocido",
    NotVisible: "No visible",
    NoFleetContext: "Sin contexto de flota",
    ExplorationPreviewUnavailable: "Vista previa de exploracion no disponible",
  },
  numbers: {
    0: "Sin bloqueo",
    1: "Desconocido",
    2: "No visible",
    3: "Sin contexto de flota",
    4: "Vista previa de exploracion no disponible",
  },
};

const explorationBlockReasonLabels: LabelCatalog = {
  names: {
    None: "Sin bloqueo",
    AlreadyVisible: "Ya visible",
    AlreadyOwned: "Ya propio",
    NoKnownVisibilitySource: "Sin fuente de visibilidad",
  },
  numbers: {
    0: "Sin bloqueo",
    1: "Ya visible",
    2: "Ya propio",
    3: "Sin fuente de visibilidad",
  },
};

const transferStatusLabels: LabelCatalog = {
  names: {
    Planned: "Planificado",
    InTransit: "En transito",
    Completed: "Completado",
    Cancelled: "Cancelado",
  },
  numbers: {
    1: "Planificado",
    2: "En transito",
    3: "Completado",
    4: "Cancelado",
  },
};

const fuelReadinessPolicyLabels: LabelCatalog = {
  names: {
    PlaceholderDerived: "Perfil derivado provisional",
  },
  numbers: {
    0: "Perfil derivado provisional",
  },
};

function normalizeName(value: string) {
  return value.replace(/[\s_-]+/g, "").toLowerCase();
}

function resolveLabel(value: DomainValue, catalog: LabelCatalog, fallback = "No disponible") {
  if (typeof value === "number") {
    return catalog.numbers?.[value] ?? String(value);
  }

  if (typeof value !== "string") {
    return fallback;
  }

  const trimmed = value.trim();
  if (trimmed.length === 0) {
    return fallback;
  }

  const numeric = Number(trimmed);
  if (Number.isInteger(numeric) && `${numeric}` === trimmed) {
    return catalog.numbers?.[numeric] ?? trimmed;
  }

  const exact = catalog.names[trimmed];
  if (exact) {
    return exact;
  }

  const normalized = normalizeName(trimmed);
  for (const [key, label] of Object.entries(catalog.names)) {
    if (normalizeName(key) === normalized) {
      return label;
    }
  }

  return trimmed;
}

function resolveName(value: DomainValue) {
  if (typeof value === "number") {
    return String(value);
  }

  if (typeof value !== "string") {
    return null;
  }

  const trimmed = value.trim();
  return trimmed.length > 0 ? trimmed : null;
}

export function formatPlanetType(value: DomainValue, fallback?: string) {
  return resolveLabel(value, planetTypeLabels, fallback);
}

export function formatColonizationStatus(value: DomainValue, fallback?: string) {
  return resolveLabel(value, colonizationStatusLabels, fallback);
}

export function formatSpaceAssetType(value: DomainValue, fallback?: string) {
  return resolveLabel(value, spaceAssetTypeLabels, fallback);
}

export function formatOrbitalGroupStatus(value: DomainValue, fallback?: string) {
  return resolveLabel(value, orbitalGroupStatusLabels, fallback);
}

export function formatResourceType(value: DomainValue, fallback?: string) {
  return resolveLabel(value, resourceTypeLabels, fallback);
}

export function formatVisibilityLevel(value: DomainValue, fallback?: string) {
  return resolveLabel(value, visibilityLevelLabels, fallback);
}

export function formatVisibilityReason(value: DomainValue, fallback?: string) {
  return resolveLabel(value, visibilityReasonLabels, fallback);
}

export function formatCommandBlockReason(value: DomainValue, fallback?: string) {
  return resolveLabel(value, commandBlockReasonLabels, fallback);
}

export function formatExplorationBlockReason(value: DomainValue, fallback?: string) {
  return resolveLabel(value, explorationBlockReasonLabels, fallback);
}

export function formatTransferStatus(value: DomainValue, fallback?: string) {
  return resolveLabel(value, transferStatusLabels, fallback);
}

export function formatFuelReadinessPolicy(value: DomainValue, fallback?: string) {
  return resolveLabel(value, fuelReadinessPolicyLabels, fallback);
}

export function formatBooleanLabel(value: boolean) {
  return value ? "Si" : "No";
}

export function isOwnedVisibilityLevel(value: DomainValue) {
  const name = resolveName(value);
  return name === "Owned" || name === "2";
}

export function isVisibleVisibilityLevel(value: DomainValue) {
  const name = resolveName(value);
  return name === "Visible" || name === "1";
}

export function formatCompactGuid(value: string | null | undefined, friendlyName?: string | null) {
  if (friendlyName && friendlyName.trim().length > 0) {
    return friendlyName.trim();
  }

  if (typeof value !== "string") {
    return "No disponible";
  }

  const trimmed = value.trim();
  if (trimmed.length === 0) {
    return "No disponible";
  }

  return /^[0-9a-f]{8}-/i.test(trimmed) ? trimmed.slice(0, 8) : trimmed;
}
