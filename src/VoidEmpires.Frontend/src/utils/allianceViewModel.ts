import type {
  AllianceActionSummaryDto,
  AllianceContactDto,
  AllianceFutureActionDto,
  AllianceIdentityDto,
  AllianceMembershipDto,
  AlliancePactPlaceholderDto,
  AllianceReadinessDto,
  AllianceStatusSummaryDto,
  AllianceUiStateDto,
} from "../api/allianceTypes";
import {
  formatAllianceDiagnostics,
  getAllianceActionLabel,
  getAllianceContactConfidenceLabel,
  getAllianceContactLabel,
  getAllianceFutureActionReasonLabel,
  getAllianceMembershipLabel,
  getAlliancePactLabel,
  getAllianceRoleLabel,
  getAllianceStateLabel,
  getAllianceStaticLabels,
} from "./alliancePresentation";
import { formatPlanetPrimaryLabel } from "./domainPresentation";

export interface AllianceDiplomaticIdentity {
  civilizationId: string;
  civilizationName: string;
  archetypeKey: string;
  archetypeLabel: string;
  statusKey: string;
  statusLabel: string;
  homePlanetId: string | null;
  homePlanetLabel: string;
  playerProfileId: string;
  playerDisplayName: string;
}

export interface AllianceMembershipSummary {
  allianceMembershipId: string;
  allianceId: string;
  statusKey: string;
  statusLabel: string;
  roleKey: string;
  roleLabel: string;
  joinedAtUtc: string;
  joinedAtLabel: string;
}

export interface AllianceStatusSummary {
  stateKey: string;
  stateLabel: string;
  hasActiveAlliance: boolean;
  activeAllianceCount: number;
  historicalAllianceCount: number;
  knownContactCount: number;
  activePactCount: number;
  headline: string;
  supportText: string;
  membership: AllianceMembershipSummary | null;
  primaryAlliance: {
    allianceId: string;
    name: string;
    tag: string;
    statusKey: string;
    statusLabel: string;
    createdAtUtc: string;
    createdAtLabel: string;
  } | null;
}

export interface AllianceContact {
  contactedCivilizationId: string;
  contactLabel: string;
  confidenceLabel: string;
  statusKey: string;
  statusLabel: string;
  source: string;
  sourceLabel: string;
  discoveredAtUtc: string;
  discoveredAtLabel: string;
  recommendedGroup: "confirmed" | "unconfirmed";
  diagnostics: AllianceContactDto;
}

export interface AlliancePactPlaceholder {
  pactTypeKey: string;
  pactLabel: string;
  isAvailable: boolean;
  stateKey: string;
  stateLabel: string;
  reasonKey: string;
  reasonLabel: string;
}

export interface AllianceFutureAction {
  actionKey: string;
  label: string;
  isAvailable: boolean;
  stateKey: string;
  stateLabel: string;
  reasonKey: string;
  reasonLabel: string;
}

export interface AllianceDiagnostics {
  playerFacing: readonly string[];
  technical: readonly string[];
  limitations: readonly string[];
  counts: {
    allianceRows: number;
    contacts: number;
    activePacts: number;
  };
  ids: {
    homePlanetId: string | null;
    playerProfileId: string | null;
  };
}

export interface AllianceUiState {
  civilizationId: string;
  identity: AllianceDiplomaticIdentity | null;
  status: AllianceStatusSummary | null;
  contacts: AllianceContact[];
  futurePacts: AlliancePactPlaceholder[];
  futureActions: AllianceFutureAction[];
  actionSummary: {
    disabledActionCount: number;
    summaryKey: string;
    summaryLabel: string;
    hasInvitationPlaceholder: boolean;
    hasApplicationPlaceholder: boolean;
    hasPactPlaceholders: boolean;
  } | null;
  diagnostics: AllianceDiagnostics;
}

const allianceLabels = getAllianceStaticLabels();

function normalizeValue(value: string | number | null | undefined) {
  if (typeof value === "number") {
    return String(value);
  }

  return typeof value === "string" ? value.trim() : "";
}

function formatAllianceDate(value: string | null | undefined, fallback = "Fecha no disponible") {
  if (!value) {
    return fallback;
  }

  const parsed = new Date(value);
  if (Number.isNaN(parsed.getTime())) {
    return fallback;
  }

  return new Intl.DateTimeFormat("es-ES", {
    dateStyle: "medium",
    timeStyle: "short",
    timeZone: "UTC",
  }).format(parsed);
}

function formatAllianceArchetype(value: string | number | null | undefined) {
  switch (normalizeValue(value).toLowerCase()) {
    case "balanced":
    case "1":
      return "Balanceada";
    case "industrial":
    case "2":
      return "Industrial";
    case "militarist":
    case "3":
      return "Militar";
    case "exploratory":
    case "4":
      return "Exploratoria";
    default:
      return "Arquetipo por validar";
  }
}

function formatCivilizationStatus(value: string | number | null | undefined) {
  switch (normalizeValue(value).toLowerCase()) {
    case "active":
    case "1":
      return "Activa";
    case "archived":
    case "2":
      return "Archivada";
    case "suspended":
    case "3":
      return "Suspendida";
    default:
      return "Estado en revisión";
  }
}

function formatSourceLabel(value: string | null | undefined) {
  const normalized = normalizeValue(value).toLowerCase();
  switch (normalized) {
    case "manual-dev":
      return "Lectura manual de desarrollo";
    case "exploration":
      return "Contacto observado";
    case "foreign":
      return "Registro externo";
  default:
    return value?.trim() || "Origen no visible";
  }
}

function mapIdentity(identity: AllianceIdentityDto | null): AllianceDiplomaticIdentity | null {
  if (!identity) {
    return null;
  }

  return {
    civilizationId: identity.civilizationId,
    civilizationName: identity.civilizationName,
    archetypeKey: normalizeValue(identity.archetype),
    archetypeLabel: formatAllianceArchetype(identity.archetype),
    statusKey: normalizeValue(identity.status),
    statusLabel: formatCivilizationStatus(identity.status),
    homePlanetId: identity.homePlanetId,
    homePlanetLabel: formatPlanetPrimaryLabel(identity.homePlanetId),
    playerProfileId: identity.playerProfileId,
    playerDisplayName: identity.playerDisplayName,
  };
}

function mapMembership(membership: AllianceMembershipDto | null | undefined): AllianceMembershipSummary | null {
  if (!membership) {
    return null;
  }

  return {
    allianceMembershipId: membership.allianceMembershipId,
    allianceId: membership.allianceId,
    statusKey: normalizeValue(membership.status),
    statusLabel: getAllianceMembershipLabel(membership.status),
    roleKey: normalizeValue(membership.role),
    roleLabel: getAllianceRoleLabel(membership.role),
    joinedAtUtc: membership.joinedAtUtc,
    joinedAtLabel: formatAllianceDate(membership.joinedAtUtc),
  };
}

function mapStatus(status: AllianceStatusSummaryDto | null): AllianceStatusSummary | null {
  if (!status) {
    return null;
  }

  const primaryAlliance = status.primaryAlliance
    ? {
        allianceId: status.primaryAlliance.allianceId,
        name: status.primaryAlliance.name,
        tag: status.primaryAlliance.tag,
        statusKey: normalizeValue(status.primaryAlliance.status),
        statusLabel: getAllianceStateLabel(status.primaryAlliance.status),
        createdAtUtc: status.primaryAlliance.createdAtUtc,
        createdAtLabel: formatAllianceDate(status.primaryAlliance.createdAtUtc),
      }
    : null;

  const headline = status.hasActiveAlliance
    ? `${status.primaryAlliance?.name ?? "Alianza activa"} | ${status.primaryAlliance?.tag ?? "Sin sigla"}`
    : allianceLabels.noActiveAlliance;
  const supportText = status.hasActiveAlliance
    ? allianceLabels.readOnlyDiplomacy
    : allianceLabels.futureAlliance;

  return {
    stateKey: status.stateKey,
    stateLabel: getAllianceStateLabel(status.stateKey),
    hasActiveAlliance: status.hasActiveAlliance,
    activeAllianceCount: status.activeAllianceCount,
    historicalAllianceCount: status.historicalAllianceCount,
    knownContactCount: status.knownContactCount,
    activePactCount: status.activePactCount,
    headline,
    supportText,
    membership: mapMembership(status.primaryAlliance?.membership),
    primaryAlliance,
  };
}

function mapContact(contact: AllianceContactDto): AllianceContact {
  const statusLabel = getAllianceContactLabel(contact.status);
  const confidenceLabel = getAllianceContactConfidenceLabel(contact.status, Boolean(contact.source));

  return {
    contactedCivilizationId: contact.contactedCivilizationId,
    contactLabel: statusLabel,
    confidenceLabel,
    statusKey: normalizeValue(contact.status),
    statusLabel,
    source: contact.source,
    sourceLabel: formatSourceLabel(contact.source),
    discoveredAtUtc: contact.discoveredAtUtc,
    discoveredAtLabel: formatAllianceDate(contact.discoveredAtUtc),
    recommendedGroup: confidenceLabel === allianceLabels.unconfirmedContact ? "unconfirmed" : "confirmed",
    diagnostics: contact,
  };
}

function mapPact(entry: AlliancePactPlaceholderDto): AlliancePactPlaceholder {
  return {
    pactTypeKey: entry.pactTypeKey,
    pactLabel: getAlliancePactLabel(entry.pactTypeKey),
    isAvailable: entry.isAvailable,
    stateKey: entry.stateKey,
    stateLabel: getAllianceStateLabel(entry.stateKey),
    reasonKey: entry.reasonKey,
    reasonLabel: getAllianceFutureActionReasonLabel(`alliance.pact.${entry.pactTypeKey}.future`),
  };
}

function mapAction(entry: AllianceFutureActionDto): AllianceFutureAction {
  return {
    actionKey: entry.actionKey,
    label: getAllianceActionLabel(entry.actionKey),
    isAvailable: entry.isAvailable,
    stateKey: entry.stateKey,
    stateLabel: getAllianceStateLabel(entry.stateKey),
    reasonKey: entry.reasonKey,
    reasonLabel: getAllianceFutureActionReasonLabel(entry.actionKey),
  };
}

function mapActionSummary(summary: AllianceActionSummaryDto | null) {
  if (!summary) {
    return null;
  }

  return {
    disabledActionCount: summary.disabledActionCount,
    summaryKey: summary.summaryKey,
    summaryLabel: getAllianceStateLabel(summary.summaryKey, allianceLabels.readOnlyDiplomacy),
    hasInvitationPlaceholder: summary.hasInvitationPlaceholder,
    hasApplicationPlaceholder: summary.hasApplicationPlaceholder,
    hasPactPlaceholders: summary.hasPactPlaceholders,
  };
}

export function groupAllianceContacts(contacts: readonly AllianceContact[]) {
  return contacts.reduce<Record<AllianceContact["recommendedGroup"], AllianceContact[]>>(
    (accumulator, contact) => {
      accumulator[contact.recommendedGroup].push(contact);
      return accumulator;
    },
    { confirmed: [], unconfirmed: [] },
  );
}

export function selectRecommendedDiplomacyFocus(viewModel: AllianceUiState) {
  if (viewModel.status?.hasActiveAlliance) {
    return viewModel.status.headline;
  }

  const firstConfirmedContact = viewModel.contacts.find((contact) => contact.recommendedGroup === "confirmed");
  if (firstConfirmedContact) {
    return firstConfirmedContact.contactLabel;
  }

  return viewModel.futurePacts[0]?.pactLabel
    ?? viewModel.futureActions[0]?.label
    ?? allianceLabels.noActiveAlliance;
}

export function getAlliancePrimaryAction(viewModel: AllianceUiState) {
  if (viewModel.status?.hasActiveAlliance) {
    return allianceLabels.readOnlyDiplomacy;
  }

  return viewModel.futureActions[0]?.label
    ?? viewModel.futurePacts[0]?.pactLabel
    ?? allianceLabels.futureAlliance;
}

export function mapAllianceUiStateToViewModel(state: AllianceUiStateDto): AllianceUiState {
  const contacts = state.knownContacts.map(mapContact);
  const diagnostics = formatAllianceDiagnostics(state.diagnostics?.notes, state.limitations);

  return {
    civilizationId: state.civilizationId,
    identity: mapIdentity(state.identity),
    status: mapStatus(state.alliance),
    contacts,
    futurePacts: state.futurePacts.map(mapPact),
    futureActions: state.futureActions.map(mapAction),
    actionSummary: mapActionSummary(state.actionSummary),
    diagnostics: {
      playerFacing: diagnostics.playerFacing,
      technical: [
        ...diagnostics.technical,
        ...(state.errors.length > 0 ? state.errors : []),
      ],
      limitations: diagnostics.limitations,
      counts: {
        allianceRows: state.diagnostics?.allianceRowCount ?? 0,
        contacts: state.diagnostics?.contactCount ?? contacts.length,
        activePacts: state.diagnostics?.activePactCount ?? 0,
      },
      ids: {
        homePlanetId: state.diagnostics?.homePlanetId ?? null,
        playerProfileId: state.diagnostics?.playerProfileId ?? null,
      },
    },
  };
}
