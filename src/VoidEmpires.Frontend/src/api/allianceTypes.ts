export type AllianceApiValue = string | number | null | undefined;

export interface AllianceMembershipDto {
  allianceMembershipId: string;
  allianceId: string;
  civilizationId: string;
  status: AllianceApiValue;
  role: AllianceApiValue;
  joinedAtUtc: string;
}

export interface AllianceReadinessDto {
  allianceId: string;
  name: string;
  tag: string;
  status: AllianceApiValue;
  createdAtUtc: string;
  membership: AllianceMembershipDto;
}

export interface AllianceIdentityDto {
  civilizationId: string;
  civilizationName: string;
  archetype: AllianceApiValue;
  status: AllianceApiValue;
  homePlanetId: string | null;
  playerProfileId: string;
  playerDisplayName: string;
}

export interface AllianceStatusSummaryDto {
  stateKey: string;
  hasActiveAlliance: boolean;
  activeAllianceCount: number;
  historicalAllianceCount: number;
  knownContactCount: number;
  activePactCount: number;
  primaryAlliance: AllianceReadinessDto | null;
}

export interface AllianceContactDto {
  contactedCivilizationId: string;
  status: AllianceApiValue;
  discoveredAtUtc: string;
  source: string;
}

export interface AlliancePactPlaceholderDto {
  pactTypeKey: string;
  isAvailable: boolean;
  stateKey: string;
  reasonKey: string;
}

export interface AllianceFutureActionDto {
  actionKey: string;
  isAvailable: boolean;
  stateKey: string;
  reasonKey: string;
}

export interface AllianceActionSummaryDto {
  disabledActionCount: number;
  summaryKey: string;
  hasInvitationPlaceholder: boolean;
  hasApplicationPlaceholder: boolean;
  hasPactPlaceholders: boolean;
}

export interface AllianceDiagnosticsDto {
  homePlanetId: string | null;
  playerProfileId: string;
  allianceRowCount: number;
  contactCount: number;
  activePactCount: number;
  notes: readonly string[];
}

export interface AllianceUiStateDto {
  civilizationId: string;
  identity: AllianceIdentityDto | null;
  alliance: AllianceStatusSummaryDto | null;
  knownContacts: readonly AllianceContactDto[];
  futurePacts: readonly AlliancePactPlaceholderDto[];
  futureActions: readonly AllianceFutureActionDto[];
  actionSummary: AllianceActionSummaryDto | null;
  diagnostics: AllianceDiagnosticsDto | null;
  limitations: readonly string[];
  errors: readonly string[];
}

export interface AllianceUiStateResponse {
  succeeded: boolean;
  uiState: AllianceUiStateDto | null;
  errors: readonly string[];
}
