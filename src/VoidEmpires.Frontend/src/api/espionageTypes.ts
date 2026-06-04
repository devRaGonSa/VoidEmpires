export interface EspionageUiStateResponse {
  succeeded: boolean;
  uiState: EspionageUiStateDto | null;
  errors: readonly string[];
}

export interface EspionageUiStateDto {
  civilizationId: string;
  overview: IntelligenceSummaryDto;
  targets: readonly IntelligenceTargetDto[];
  passiveSignals: readonly IntelligenceSignalDto[];
  recommendedFocus: IntelligenceFocusDto | null;
  futureActions: readonly EspionageActionAvailabilityDto[];
  diagnostics: readonly string[];
  limitations: readonly string[];
}

export interface IntelligenceSummaryDto {
  ownedTargetCount: number;
  visibleTargetCount: number;
  knownTargetCount: number;
  partialTargetCount: number;
  passiveSignalCount: number;
}

export interface IntelligenceTargetDto {
  targetKind: string;
  systemId: string;
  planetId: string | null;
  systemName: string | null;
  planetName: string | null;
  visibilityLevel: string;
  visibilityReason: string;
  intelligenceLevel: string;
  confidenceSummary: string;
  coverageSummary: string;
  hasPassiveSignals: boolean;
}

export interface IntelligenceSignalDto {
  systemId: string;
  planetId: string | null;
  signalKind: string;
  summary: string;
}

export interface IntelligenceFocusDto {
  systemId: string;
  planetId: string | null;
  reason: string;
}

export interface EspionageActionAvailabilityDto {
  actionKey: string;
  isAvailable: boolean;
  reason: string;
}
