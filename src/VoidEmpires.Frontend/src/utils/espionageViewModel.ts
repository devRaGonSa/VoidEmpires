import type {
  EspionageActionAvailabilityDto,
  EspionageUiStateDto,
  IntelligenceSignalDto,
  IntelligenceTargetDto,
} from "../api/espionageTypes";
import {
  getEspionageActionLabel,
  getIntelConfidenceLabel,
  getIntelligenceLevelLabel,
  getObservationStatusLabel,
  getTargetVisibilityLabel,
} from "./espionagePresentation";

export interface IntelligenceTargetViewModel {
  kind: string;
  systemId: string;
  planetId: string | null;
  label: string;
  systemLabel: string;
  visibilityLabel: string;
  intelligenceLabel: string;
  confidenceLabel: string;
  coverageLabel: string;
  observationLabel: string;
  hasPassiveSignals: boolean;
  diagnostics: IntelligenceTargetDto;
}

export interface IntelligenceSignalViewModel {
  systemId: string;
  planetId: string | null;
  label: string;
  systemLabel: string;
  summary: string;
  diagnostics: IntelligenceSignalDto;
}

export interface IntelligenceSystemTargetGroup {
  systemId: string;
  label: string;
  targets: IntelligenceTargetViewModel[];
  signals: IntelligenceSignalViewModel[];
}

export interface EspionageActionAvailability {
  key: string;
  label: string;
  supported: boolean;
  reasonLabel: string;
}

export interface EspionageViewModel {
  civilizationId: string;
  summary: {
    ownedTargetCount: number;
    visibleTargetCount: number;
    knownTargetCount: number;
    partialTargetCount: number;
    passiveSignalCount: number;
  };
  groups: IntelligenceSystemTargetGroup[];
  recommendedTarget: IntelligenceTargetViewModel | null;
  futureActions: EspionageActionAvailability[];
  diagnostics: {
    playerFacing: readonly string[];
    technical: readonly string[];
  };
  limitations: readonly string[];
}

function mapTarget(target: IntelligenceTargetDto): IntelligenceTargetViewModel {
  const signalCount = target.hasPassiveSignals ? 1 : 0;
  return {
    kind: target.targetKind,
    systemId: target.systemId,
    planetId: target.planetId,
    label: target.planetName ?? target.systemName ?? "Objetivo sin nombre",
    systemLabel: target.systemName ?? "Sistema sin identificar",
    visibilityLabel: getTargetVisibilityLabel(target.visibilityLevel, target.intelligenceLevel === "Confirmed"),
    intelligenceLabel: getIntelligenceLevelLabel(target.visibilityLevel, target.visibilityReason),
    confidenceLabel: getIntelConfidenceLabel({ visibilityLevel: target.visibilityLevel, detectionCount: signalCount }),
    coverageLabel: target.coverageSummary,
    observationLabel: getObservationStatusLabel({ visibilityLevel: target.visibilityLevel, visibilityReason: target.visibilityReason, detectionCount: signalCount }),
    hasPassiveSignals: target.hasPassiveSignals,
    diagnostics: target,
  };
}

function mapSignal(
  signal: IntelligenceSignalDto,
  targetByPlanetId: ReadonlyMap<string, IntelligenceTargetViewModel>,
  targetBySystemId: ReadonlyMap<string, IntelligenceTargetViewModel>,
): IntelligenceSignalViewModel {
  const matchedPlanetTarget = signal.planetId ? targetByPlanetId.get(signal.planetId) : null;
  const matchedSystemTarget = signal.systemId !== "00000000-0000-0000-0000-000000000000"
    ? targetBySystemId.get(signal.systemId)
    : null;
  const resolvedSystemId = matchedPlanetTarget?.systemId
    ?? matchedSystemTarget?.systemId
    ?? signal.systemId;
  const resolvedSystemLabel = matchedPlanetTarget?.systemLabel
    ?? matchedSystemTarget?.systemLabel
    ?? "Sistema sin identificar";

  return {
    systemId: resolvedSystemId,
    planetId: signal.planetId,
    label: signal.signalKind,
    systemLabel: resolvedSystemLabel,
    summary: signal.summary,
    diagnostics: signal,
  };
}

function mapAction(action: EspionageActionAvailabilityDto): EspionageActionAvailability {
  return {
    key: action.actionKey,
    label: getEspionageActionLabel(action.actionKey) ?? "Mision no disponible",
    supported: action.isAvailable,
    reasonLabel: action.reason,
  };
}

export function groupIntelTargetsBySystem(
  targets: readonly IntelligenceTargetViewModel[],
  signals: readonly IntelligenceSignalViewModel[],
) {
  const grouped = new Map<string, IntelligenceSystemTargetGroup>();

  targets.forEach((target) => {
    const current = grouped.get(target.systemId) ?? {
      systemId: target.systemId,
      label: target.systemLabel,
      targets: [],
      signals: [],
    };
    current.targets.push(target);
    grouped.set(target.systemId, current);
  });

  signals.forEach((signal) => {
    if (!grouped.has(signal.systemId)) {
      grouped.set(signal.systemId, {
        systemId: signal.systemId,
        label: "Sistema sin identificar",
        targets: [],
        signals: [],
      });
    }

    grouped.get(signal.systemId)?.signals.push(signal);
  });

  return [...grouped.values()];
}

export function selectRecommendedIntelTarget(
  targets: readonly IntelligenceTargetViewModel[],
  recommendedFocus: EspionageUiStateDto["recommendedFocus"],
) {
  if (recommendedFocus) {
    return targets.find((target) =>
      target.systemId === recommendedFocus.systemId &&
      target.planetId === recommendedFocus.planetId) ?? null;
  }

  return targets.find((target) => target.hasPassiveSignals) ?? targets[0] ?? null;
}

export function getEspionagePrimaryAction(viewModel: EspionageViewModel) {
  const firstBlockedAction = viewModel.futureActions[0];
  return firstBlockedAction?.label ?? "Reconocimiento futuro";
}

export function mapEspionageUiStateToViewModel(state: EspionageUiStateDto): EspionageViewModel {
  const targets = state.targets.map(mapTarget);
  const targetByPlanetId = new Map(
    targets.flatMap((target) => (target.planetId ? [[target.planetId, target] as const] : [])),
  );
  const targetBySystemId = new Map(targets.map((target) => [target.systemId, target] as const));
  const signals = state.passiveSignals.map((signal) => mapSignal(signal, targetByPlanetId, targetBySystemId));

  return {
    civilizationId: state.civilizationId,
    summary: { ...state.overview },
    groups: groupIntelTargetsBySystem(targets, signals),
    recommendedTarget: selectRecommendedIntelTarget(targets, state.recommendedFocus),
    futureActions: state.futureActions.map(mapAction),
    diagnostics: {
      playerFacing: state.diagnostics.filter((item) => !item.toLowerCase().includes("development")),
      technical: [...state.diagnostics],
    },
    limitations: state.limitations,
  };
}
