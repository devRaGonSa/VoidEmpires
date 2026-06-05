type AllianceValue = string | number | null | undefined;

interface AllianceLabelEntry {
  key: string;
  label: string;
}

const unknownAllianceStateFallback = "Lectura diplomatica pendiente de clasificar";
const unknownContactFallback = "Lectura diplomatica pendiente de clasificar";
const unknownPactFallback = "Alianza futura";
const unknownActionFallback = "Diplomacia solo lectura";

const allianceStaticLabels = {
  diplomacyStatus: "Estado diplomatico",
  noActiveAlliance: "Sin alianza activa",
  futureAlliance: "Alianza futura",
  invitationUnavailable: "Invitacion no disponible",
  requestUnavailable: "Solicitud no disponible",
  readOnlyDiplomacy: "Diplomacia solo lectura",
  knownContact: "Contacto conocido",
  unconfirmedContact: "Contacto sin confirmar",
  unclassifiedRead: "Lectura diplomatica pendiente de clasificar",
} as const;

const allianceStateCatalog: readonly AllianceLabelEntry[] = [
  { key: "None", label: allianceStaticLabels.noActiveAlliance },
  { key: "ReadOnly", label: allianceStaticLabels.readOnlyDiplomacy },
  { key: "Future", label: allianceStaticLabels.futureAlliance },
  { key: "Active", label: allianceStaticLabels.readOnlyDiplomacy },
  { key: "Archived", label: allianceStaticLabels.readOnlyDiplomacy },
  { key: "Pending", label: allianceStaticLabels.unclassifiedRead },
] as const;

const alliancePactCatalog: readonly AllianceLabelEntry[] = [
  { key: "MutualDefenseIntent", label: "Pacto defensivo futuro" },
  { key: "TradeIntent", label: "Pacto comercial futuro" },
  { key: "NonAggression", label: "Pacto de no agresion futuro" },
  { key: "Cooperation", label: allianceStaticLabels.futureAlliance },
  { key: "Unknown", label: allianceStaticLabels.futureAlliance },
] as const;

const allianceActionCatalog: readonly AllianceLabelEntry[] = [
  { key: "alliance.read", label: allianceStaticLabels.readOnlyDiplomacy },
  { key: "alliance.summary.read", label: allianceStaticLabels.readOnlyDiplomacy },
  { key: "alliance.invitation.future", label: allianceStaticLabels.invitationUnavailable },
  { key: "alliance.application.future", label: allianceStaticLabels.requestUnavailable },
  { key: "alliance.membership.future", label: allianceStaticLabels.futureAlliance },
  { key: "alliance.pact.future", label: allianceStaticLabels.futureAlliance },
  { key: "alliance.pact.trade.future", label: "Pacto comercial futuro" },
  { key: "alliance.pact.defense.future", label: "Pacto defensivo futuro" },
  { key: "alliance.pact.nonAggression.future", label: "Pacto de no agresion futuro" },
] as const;

function normalizeValue(value: AllianceValue) {
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
  value: AllianceValue,
  catalog: readonly AllianceLabelEntry[],
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

export function getAllianceStaticLabels() {
  return allianceStaticLabels;
}

export function getAllianceStateLabel(value: AllianceValue, fallback = unknownAllianceStateFallback) {
  return resolveCatalogLabel(value, allianceStateCatalog, fallback);
}

export function getAlliancePactLabel(value: AllianceValue, fallback = unknownPactFallback) {
  return resolveCatalogLabel(value, alliancePactCatalog, fallback);
}

export function getAllianceActionLabel(value: AllianceValue, fallback = unknownActionFallback) {
  return resolveCatalogLabel(value, allianceActionCatalog, fallback);
}

export function getAllianceContactLabel(value: AllianceValue, fallback = unknownContactFallback) {
  const normalizedValue = normalizeValue(value);
  if (!normalizedValue) {
    return fallback;
  }

  const normalizedLookup = normalizeLookup(normalizedValue);
  if (["contacted", "friendly", "neutral", "hostile", "known"].includes(normalizedLookup)) {
    return allianceStaticLabels.knownContact;
  }

  if (["unknown", "unconfirmed", "pending", "tentative"].includes(normalizedLookup)) {
    return allianceStaticLabels.unconfirmedContact;
  }

  return fallback;
}

export function getAllianceFutureActionReasonLabel(actionKey: AllianceValue) {
  const normalizedValue = normalizeValue(actionKey);
  if (!normalizedValue) {
    return allianceStaticLabels.readOnlyDiplomacy;
  }

  switch (normalizeLookup(normalizedValue)) {
    case "allianceinvitationfuture":
      return `${allianceStaticLabels.invitationUnavailable}. ${allianceStaticLabels.readOnlyDiplomacy}.`;
    case "allianceapplicationfuture":
      return `${allianceStaticLabels.requestUnavailable}. ${allianceStaticLabels.readOnlyDiplomacy}.`;
    case "alliancepacttradefuture":
      return "Pacto comercial futuro. Diplomacia solo lectura.";
    case "alliancepactdefensefuture":
      return "Pacto defensivo futuro. Diplomacia solo lectura.";
    case "alliancepactnonaggressionfuture":
      return "Pacto de no agresion futuro. Diplomacia solo lectura.";
    default:
      return `${allianceStaticLabels.futureAlliance}. ${allianceStaticLabels.readOnlyDiplomacy}.`;
  }
}
