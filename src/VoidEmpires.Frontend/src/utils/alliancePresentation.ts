type AllianceValue = string | number | null | undefined;

interface AllianceLabelEntry {
  key: string;
  label: string;
}

export interface AllianceFutureActionPlaceholder {
  key: string;
  label: string;
}

export interface AllianceErrorFeedback {
  primaryMessage: string;
  followUp: string | null;
  technicalDetail: string | null;
}

const unknownAllianceStateFallback = "Lectura diplomatica pendiente de clasificar";
const unknownContactFallback = "Lectura diplomatica pendiente de clasificar";
const unknownPactFallback = "Alianza futura";
const unknownActionFallback = "Diplomacia solo lectura";
const unknownRoleFallback = "Rol pendiente de clasificar";
const unknownMembershipFallback = "Membresia pendiente de clasificar";
const unknownConfidenceFallback = "Lectura diplomatica pendiente de clasificar";

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
  noOtherCivilization: "No hay otra civilizacion diplomatica activa en esta version",
  futurePactsPrepared: "Los pactos quedan preparados para una fase futura",
  limitedReadNote: "La lectura diplomatica sigue limitada a metadata de desarrollo",
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

const allianceRoleCatalog: readonly AllianceLabelEntry[] = [
  { key: "Leader", label: "Lider" },
  { key: "Officer", label: "Oficial" },
  { key: "Member", label: "Miembro" },
  { key: "Unknown", label: unknownRoleFallback },
] as const;

const allianceMembershipCatalog: readonly AllianceLabelEntry[] = [
  { key: "Active", label: "Membresia activa" },
  { key: "Departed", label: "Historial diplomatico" },
  { key: "Pending", label: "Lectura diplomatica pendiente de clasificar" },
  { key: "Unknown", label: unknownMembershipFallback },
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

export function getAllianceRoleLabel(value: AllianceValue, fallback = unknownRoleFallback) {
  return resolveCatalogLabel(value, allianceRoleCatalog, fallback);
}

export function getAllianceMembershipLabel(value: AllianceValue, fallback = unknownMembershipFallback) {
  return resolveCatalogLabel(value, allianceMembershipCatalog, fallback);
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

export function getAllianceContactConfidenceLabel(value: AllianceValue, hasTechnicalSource = false) {
  const normalizedValue = normalizeValue(value);
  const normalizedLookup = normalizedValue ? normalizeLookup(normalizedValue) : null;

  if (normalizedLookup && ["friendly", "neutral", "contacted", "known"].includes(normalizedLookup)) {
    return hasTechnicalSource ? "Lectura conocida" : allianceStaticLabels.knownContact;
  }

  if (normalizedLookup && ["hostile"].includes(normalizedLookup)) {
    return "Contacto conocido";
  }

  if (normalizedLookup && ["unknown", "unconfirmed", "pending", "tentative"].includes(normalizedLookup)) {
    return allianceStaticLabels.unconfirmedContact;
  }

  return unknownConfidenceFallback;
}

export function formatAllianceDiagnostics(
  notes: readonly string[] | null | undefined,
  limitations: readonly string[] | null | undefined,
) {
  const technical = [...(notes ?? [])];
  const playerFacing = technical.filter((note) => !note.toLowerCase().includes("development-only"));

  return {
    playerFacing,
    technical,
    limitations: [...(limitations ?? [])],
  };
}

export function getAllianceReadOnlyStatement() {
  return "La cabina de Alianzas permanece en solo lectura durante esta fase.";
}

export function getAllianceCatalogPlaceholder(kind: "known" | "potential" | "future" | "limited") {
  switch (kind) {
    case "known":
    case "potential":
      return allianceStaticLabels.noOtherCivilization;
    case "future":
      return allianceStaticLabels.futurePactsPrepared;
    case "limited":
      return allianceStaticLabels.limitedReadNote;
    default:
      return allianceStaticLabels.unclassifiedRead;
  }
}

export function getAllianceContactCardTitle(group: "confirmed" | "unconfirmed", index: number) {
  const prefix = group === "confirmed" ? "Contacto diplomatico conocido" : "Contacto diplomatico potencial";
  return `${prefix} ${String(index + 1).padStart(2, "0")}`;
}

export function getAllianceContactReadinessLabel(group: "confirmed" | "unconfirmed") {
  return group === "confirmed" ? "Listo para seguimiento entre cabinas" : "Pendiente de confirmacion";
}

export function getAllianceNextCockpitHint(group: "confirmed" | "unconfirmed", sourceLabel: string) {
  if (group === "confirmed") {
    return sourceLabel === "Contacto observado" ? "Seguir desde Galaxia o Espionaje" : "Seguir desde Espionaje";
  }

  return "Mantener seguimiento en Espionaje";
}

export function getAllianceFutureActionPlaceholders(): readonly AllianceFutureActionPlaceholder[] {
  return [
    { key: "create-alliance", label: "Crear alianza" },
    { key: "request-membership", label: "Solicitar entrada" },
    { key: "invite-civilization", label: "Invitar civilizacion" },
    { key: "propose-defense-pact", label: "Proponer pacto defensivo" },
    { key: "propose-trade-pact", label: "Proponer pacto comercial" },
    { key: "manage-roles", label: "Gestionar roles" },
  ];
}

export function formatAllianceRequestFailure(rawError: string | null | undefined): AllianceErrorFeedback {
  const technicalDetail = rawError?.trim() || null;

  switch (technicalDetail) {
    case "Civilization id is required.":
      return {
        primaryMessage: "No hay contexto de civilizacion.",
        followUp: "Introduce un id valido o vuelve a entrar desde otra cabina para conservar el contexto.",
        technicalDetail,
      };
    case "Civilization was not found.":
      return {
        primaryMessage: "No se pudo cargar la lectura diplomatica.",
        followUp: "La civilizacion solicitada no existe en el contexto visible.",
        technicalDetail,
      };
    case "Alliance read is not available for this civilization.":
      return {
        primaryMessage: "No se pudo cargar la lectura diplomatica.",
        followUp: "La lectura de alianzas no esta disponible para esta civilizacion en el estado actual de desarrollo.",
        technicalDetail,
      };
    case "Request failed with status 404.":
      return {
        primaryMessage: "No se pudo cargar la lectura diplomatica.",
        followUp: "La ruta de Alianzas no esta disponible fuera del entorno de desarrollo.",
        technicalDetail,
      };
    case "Alliance actions are not supported in this version.":
    case "Request failed with status 405.":
    case "Request failed with status 501.":
      return {
        primaryMessage: "Las acciones diplomaticas no estan disponibles en esta version.",
        followUp: "La cabina mantiene una lectura segura y no expone acciones ejecutables.",
        technicalDetail,
      };
    case "Request failed with status 503.":
      return {
        primaryMessage: "No se pudo cargar la lectura diplomatica.",
        followUp: "La persistencia de desarrollo no esta disponible ahora mismo.",
        technicalDetail,
      };
    default:
      return {
        primaryMessage: "No se pudo cargar la lectura diplomatica.",
        followUp: "Revisa el contexto actual y abre el diagnostico secundario si el problema persiste.",
        technicalDetail,
      };
  }
}
