import type {
  RankingCategoryScoreDto,
  RankingComparisonRowDto,
  RankingDiagnosticsDto,
  RankingDisabledActionDto,
  RankingFuturePlaceholderDto,
  RankingIdentityDto,
  RankingPowerSummaryDto,
  RankingPublicationStateDto,
  RankingRequestFailureCode,
  RankingUiStateDto,
} from "../api/rankingApi";
import { RankingRequestError } from "../api/rankingApi";
import { formatPlanetPrimaryLabel } from "./domainPresentation";

type RankingValue = string | number | null | undefined;

interface RankingLabelEntry {
  key: string;
  label: string;
}

const unknownMetricFallback = "Área pendiente de lectura";
const unknownCategoryFallback = "Categoría por consolidar";
const unknownComparisonFallback = "Referencia de validación";
const unknownStateFallback = "Lectura interna no publicada";
const unknownActionFallback = "Acción futura no disponible";

const rankingStaticLabels = {
  powerIndex: "Indice de poder",
  economyPower: "Potencia economica",
  colonialDevelopment: "Desarrollo colonial",
  technologyProgress: "Progreso tecnologico",
  orbitalCapacity: "Capacidad orbital",
  defensiveReadiness: "Preparacion defensiva",
  groundGarrison: "Guarnicion terrestre",
  strategicIntelligence: "Inteligencia estrategica",
  diplomacy: "Diplomacia",
  demoComparison: "Comparativa de demostración",
  unpublishedRanking: "Lectura interna no publicada",
  readOnly: "Solo lectura",
  demoScenarioReference: "Referencia de validación",
  globalLeaderboard: "Tabla global",
  allianceLeaderboard: "Resumen de alianzas",
  futureSeason: "Temporada futura",
  rewardsUnavailable: "Recompensas desactivadas",
  unavailable: "No disponible",
  unclassifiedMetric: "Lectura estratégica incompleta",
} as const;

const rankingCategoryCatalog: readonly RankingLabelEntry[] = [
  { key: "powerIndex", label: rankingStaticLabels.powerIndex },
  { key: "economy", label: rankingStaticLabels.economyPower },
  { key: "economicPower", label: rankingStaticLabels.economyPower },
  { key: "colonies", label: rankingStaticLabels.colonialDevelopment },
  { key: "colonyDevelopment", label: rankingStaticLabels.colonialDevelopment },
  { key: "research", label: rankingStaticLabels.technologyProgress },
  { key: "technology", label: rankingStaticLabels.technologyProgress },
  { key: "technologyProgress", label: rankingStaticLabels.technologyProgress },
  { key: "orbital", label: rankingStaticLabels.orbitalCapacity },
  { key: "orbitalCapacity", label: rankingStaticLabels.orbitalCapacity },
  { key: "defense", label: rankingStaticLabels.defensiveReadiness },
  { key: "defensiveReadiness", label: rankingStaticLabels.defensiveReadiness },
  { key: "groundArmy", label: rankingStaticLabels.groundGarrison },
  { key: "groundGarrison", label: rankingStaticLabels.groundGarrison },
  { key: "intelligence", label: rankingStaticLabels.strategicIntelligence },
  { key: "strategicIntelligence", label: rankingStaticLabels.strategicIntelligence },
  { key: "alliance", label: rankingStaticLabels.diplomacy },
  { key: "diplomacy", label: rankingStaticLabels.diplomacy },
  { key: "comparison", label: rankingStaticLabels.demoComparison },
  { key: "demoComparison", label: rankingStaticLabels.demoComparison },
] as const;

const rankingStateCatalog: readonly RankingLabelEntry[] = [
  { key: "readOnly", label: rankingStaticLabels.unpublishedRanking },
  { key: "private", label: rankingStaticLabels.unpublishedRanking },
  { key: "unpublished", label: rankingStaticLabels.unpublishedRanking },
  { key: "notPublic", label: rankingStaticLabels.unpublishedRanking },
  { key: "future", label: rankingStaticLabels.futureSeason },
  { key: "futureSeason", label: rankingStaticLabels.futureSeason },
  { key: "rewardsUnavailable", label: rankingStaticLabels.rewardsUnavailable },
  { key: "demoComparison", label: rankingStaticLabels.demoComparison },
] as const;

const rankingActionCatalog: readonly RankingLabelEntry[] = [
  { key: "ranking.summary.read", label: rankingStaticLabels.powerIndex },
  { key: "ranking.category.economy", label: rankingStaticLabels.economyPower },
  { key: "ranking.category.colonies", label: rankingStaticLabels.colonialDevelopment },
  { key: "ranking.category.research", label: rankingStaticLabels.technologyProgress },
  { key: "ranking.category.orbital", label: rankingStaticLabels.orbitalCapacity },
  { key: "ranking.category.defense", label: rankingStaticLabels.defensiveReadiness },
  { key: "ranking.category.ground", label: rankingStaticLabels.groundGarrison },
  { key: "ranking.category.intelligence", label: rankingStaticLabels.strategicIntelligence },
  { key: "ranking.category.diplomacy", label: rankingStaticLabels.diplomacy },
  { key: "ranking.comparison.demo", label: rankingStaticLabels.demoComparison },
  { key: "ranking.leaderboard.future", label: rankingStaticLabels.unpublishedRanking },
  { key: "ranking.season.future", label: rankingStaticLabels.futureSeason },
  { key: "ranking.rewards.future", label: rankingStaticLabels.rewardsUnavailable },
] as const;

export interface RankingIdentity {
  civilizationId: string;
  civilizationName: string;
  displayName: string;
  homePlanetId: string | null;
  homePlanetLabel: string;
}

export interface RankingCategoryScore {
  categoryKey: string;
  label: string;
  score: number;
  scoreLabel: string;
  weight: number;
  emphasis: "good" | "neutral" | "warn";
  sourceNote: string;
}

export interface RankingCategoryCard {
  key: string;
  title: string;
  scoreLabel: string;
  explanation: string;
  readinessLabel: string;
  emphasis: "good" | "neutral" | "warn";
}

export interface RankingPowerSummary {
  totalPowerIndex: number;
  totalPowerIndexLabel: string;
  categories: RankingCategoryScore[];
  recommendationKey: string;
  recommendationLabel: string;
}

export interface RankingComparisonRow {
  rowKey: string;
  displayName: string;
  totalPowerIndex: number;
  totalPowerIndexLabel: string;
  deltaFromCurrent: number;
  deltaLabel: string;
  postureLabel: string;
  isCurrentCivilization: boolean;
  isDemoOnly: boolean;
}

export interface RankingFutureAction {
  key: string;
  label: string;
  stateLabel: string;
  reasonLabel: string;
  isAvailable: boolean;
}

export interface RankingComparisonCard {
  key: string;
  title: string;
  scoreLabel: string;
  deltaLabel: string;
  postureLabel: string;
  stateLabel: string;
  visibilityLabel: string;
  note: string;
  emphasis: "good" | "neutral" | "warn";
}

export interface RankingLeaderboardPlaceholderCard {
  key: string;
  title: string;
  stateLabel: string;
  reasonLabel: string;
}

export interface RankingFutureCapabilityCard {
  key: string;
  title: string;
  stateLabel: string;
  reasonLabel: string;
}

export interface RankingDiagnostics {
  playerFacing: readonly string[];
  technical: readonly string[];
  limitations: readonly string[];
  counts: {
    ownedPlanets: number;
    visibleSystems: number;
    diplomaticContacts: number;
    activeTransfers: number;
  };
}

export interface RankingErrorPresentation {
  code: RankingRequestFailureCode;
  primaryMessage: string;
  followUp: string | null;
  technicalDetail: string | null;
}

export interface RankingUiState {
  civilizationId: string;
  identity: RankingIdentity | null;
  summary: RankingPowerSummary | null;
  comparisons: RankingComparisonRow[];
  publication: {
    stateKey: string;
    stateLabel: string;
    isPublished: boolean;
    summaryLabel: string;
  } | null;
  futureActions: RankingFutureAction[];
  diagnostics: RankingDiagnostics;
}

function normalizeValue(value: RankingValue) {
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
  value: RankingValue,
  catalog: readonly RankingLabelEntry[],
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

function formatIndex(value: number | null | undefined) {
  if (typeof value !== "number" || !Number.isFinite(value)) {
    return "Sin lectura visible";
  }

  return new Intl.NumberFormat("es-ES", { maximumFractionDigits: 0 }).format(value);
}

function formatDelta(value: number) {
  if (!Number.isFinite(value) || value === 0) {
    return "Sin cambio";
  }

  return `${value > 0 ? "+" : ""}${formatIndex(value)}`;
}

function getComparisonPosture(deltaFromCurrent: number, isCurrentCivilization: boolean) {
  if (isCurrentCivilization) {
    return "Civilizacion actual";
  }

  if (deltaFromCurrent > 0) {
    return "Referencia superior";
  }

  if (deltaFromCurrent < 0) {
    return "Referencia conservadora";
  }

  return rankingStaticLabels.demoComparison;
}

function normalizeCurrentCivilizationLabel(value: string) {
  const normalized = normalizeLookup(value);
  if (normalized === "voidseedcivilization" || normalized === "voidseed") {
    return "Civilización semilla";
  }

  return value.trim().length > 0 ? value.trim() : "Civilización semilla";
}

function getCategoryEmphasis(score: number): "good" | "neutral" | "warn" {
  if (score >= 500) {
    return "good";
  }

  if (score >= 150) {
    return "neutral";
  }

  return "warn";
}

function mapIdentity(identity: RankingIdentityDto | null): RankingIdentity | null {
  if (!identity) {
    return null;
  }

  return {
    civilizationId: identity.civilizationId,
    civilizationName: identity.civilizationName,
    displayName: identity.displayName,
    homePlanetId: identity.homePlanetId,
    homePlanetLabel: formatPlanetPrimaryLabel(identity.homePlanetId),
  };
}

function mapCategory(entry: RankingCategoryScoreDto): RankingCategoryScore {
  return {
    categoryKey: entry.categoryKey,
    label: getRankingCategoryLabel(entry.categoryKey),
    score: entry.score,
    scoreLabel: formatIndex(entry.score),
    weight: entry.weight,
    emphasis: getCategoryEmphasis(entry.score),
    sourceNote: entry.sourceNote,
  };
}

function mapSummary(summary: RankingPowerSummaryDto | null): RankingPowerSummary | null {
  if (!summary) {
    return null;
  }

  return {
    totalPowerIndex: summary.totalPowerIndex,
    totalPowerIndexLabel: formatIndex(summary.totalPowerIndex),
    categories: summary.categories.map(mapCategory),
    recommendationKey: summary.recommendationKey,
    recommendationLabel: getRankingCategoryLabel(summary.recommendationKey),
  };
}

function mapComparison(entry: RankingComparisonRowDto): RankingComparisonRow {
  return {
    rowKey: entry.rowKey,
    displayName: entry.isCurrentCivilization
      ? normalizeCurrentCivilizationLabel(entry.displayName)
      : entry.displayName,
    totalPowerIndex: entry.totalPowerIndex,
    totalPowerIndexLabel: formatIndex(entry.totalPowerIndex),
    deltaFromCurrent: entry.deltaFromCurrent,
    deltaLabel: formatDelta(entry.deltaFromCurrent),
    postureLabel: getComparisonPosture(entry.deltaFromCurrent, entry.isCurrentCivilization),
    isCurrentCivilization: entry.isCurrentCivilization,
    isDemoOnly: entry.isDemoOnly,
  };
}

function getReasonLabel(reasonKey: string) {
  switch (normalizeLookup(reasonKey)) {
    case "notpublished":
      return "La clasificacion global permanece desactivada en esta version.";
    case "futureseason":
      return "La temporada futura sigue fuera del alcance de esta vista.";
    case "rewardsunavailable":
      return "Las recompensas no forman parte de esta fase.";
    default:
      return "La referencia futura sigue deshabilitada en esta vista.";
  }
}

function mapFutureAction(entry: RankingFuturePlaceholderDto | RankingDisabledActionDto): RankingFutureAction {
  const key = "placeholderKey" in entry ? entry.placeholderKey : entry.actionKey;

  return {
    key,
    label: getRankingActionLabel("actionKey" in entry ? entry.actionKey : `ranking.${entry.placeholderKey}.future`),
    stateLabel: getRankingStateLabel("stateKey" in entry ? entry.stateKey : "future"),
    reasonLabel: getReasonLabel(entry.reasonKey),
    isAvailable: entry.isAvailable,
  };
}

function mapPublication(publication: RankingPublicationStateDto | null) {
  if (!publication) {
    return null;
  }

  return {
    stateKey: publication.stateKey,
    stateLabel: getRankingStateLabel(publication.stateKey),
    isPublished: publication.isPublished,
    summaryLabel: getRankingStateLabel(publication.summaryKey),
  };
}

function mapDiagnostics(
  diagnostics: RankingDiagnosticsDto | null,
  limitations: readonly string[],
  errors: readonly string[],
): RankingDiagnostics {
  if (!diagnostics) {
    return {
      playerFacing: errors.length > 0 ? [errors[0]] : [],
      technical: [...errors],
      limitations: [...limitations],
      counts: {
        ownedPlanets: 0,
        visibleSystems: 0,
        diplomaticContacts: 0,
        activeTransfers: 0,
      },
    };
  }

  return {
    playerFacing: diagnostics.notes.filter((note) => !note.toLowerCase().includes("development-only")),
    technical: [
      ...diagnostics.notes,
      `OwnedPlanets=${diagnostics.ownedPlanetCount}`,
      `VisibleSystems=${diagnostics.visibleSystemCount}`,
      `DiplomaticContacts=${diagnostics.diplomaticContactCount}`,
      `ActiveTransfers=${diagnostics.activeTransferCount}`,
      ...errors,
    ],
    limitations: [...limitations],
    counts: {
      ownedPlanets: diagnostics.ownedPlanetCount,
      visibleSystems: diagnostics.visibleSystemCount,
      diplomaticContacts: diagnostics.diplomaticContactCount,
      activeTransfers: diagnostics.activeTransferCount,
    },
  };
}

export function getRankingStaticLabels() {
  return rankingStaticLabels;
}

export function formatRankingRequestFailure(error: unknown): RankingErrorPresentation {
  if (error instanceof RankingRequestError) {
    switch (error.code) {
      case "invalidCivilizationId":
        return {
          code: error.code,
          primaryMessage: "No hay contexto de civilizacion.",
          followUp: "Introduce un id valido o entra desde otra vista para conservar el contexto.",
          technicalDetail: error.detail,
        };
      case "civilizationNotFound":
        return {
          code: error.code,
          primaryMessage: "No se pudo cargar la lectura de ranking.",
          followUp: "La civilizacion solicitada no existe dentro del contexto visible.",
          technicalDetail: error.detail,
        };
      case "rankingReadUnavailable":
        return {
          code: error.code,
          primaryMessage: "No se pudo cargar la lectura de ranking.",
          followUp: "La persistencia de desarrollo no esta disponible ahora mismo.",
          technicalDetail: error.detail,
        };
      case "endpointUnavailableOutsideDevelopment":
        return {
          code: error.code,
          primaryMessage: "No se pudo cargar la lectura de ranking.",
          followUp: "La ruta de Ranking solo esta disponible dentro del entorno de desarrollo.",
          technicalDetail: error.detail,
        };
      case "unsupportedFutureAction":
        return {
          code: error.code,
          primaryMessage: "La clasificacion global no esta disponible en esta version.",
          followUp: "Ranking mantiene esa accion como referencia futura y no la ejecuta.",
          technicalDetail: error.detail,
        };
      default:
        return {
          code: error.code,
          primaryMessage: "No se pudo cargar la lectura de ranking.",
          followUp: "Revisa el contexto actual y abre el diagnostico secundario si el problema persiste.",
          technicalDetail: error.detail,
        };
    }
  }

  const detail = error instanceof Error ? error.message.trim() : null;

  if (detail === "Civilization id is required.") {
    return {
      code: "invalidCivilizationId",
      primaryMessage: "No hay contexto de civilizacion.",
      followUp: "Introduce un id valido o entra desde otra vista para conservar el contexto.",
      technicalDetail: detail,
    };
  }

  return {
    code: "unexpectedError",
    primaryMessage: "No se pudo cargar la lectura de ranking.",
    followUp: "Revisa el contexto actual y abre el diagnostico secundario si el problema persiste.",
    technicalDetail: detail,
  };
}

export function getRankingMetricLabel(value: RankingValue, fallback = unknownMetricFallback) {
  return resolveCatalogLabel(value, rankingCategoryCatalog, fallback);
}

export function getRankingCategoryLabel(value: RankingValue, fallback = unknownCategoryFallback) {
  return resolveCatalogLabel(value, rankingCategoryCatalog, fallback);
}

export function getRankingComparisonLabel(value: RankingValue, fallback = unknownComparisonFallback) {
  return resolveCatalogLabel(value, rankingStateCatalog, fallback);
}

export function getRankingStateLabel(value: RankingValue, fallback = unknownStateFallback) {
  return resolveCatalogLabel(value, rankingStateCatalog, fallback);
}

export function getRankingActionLabel(value: RankingValue, fallback = unknownActionFallback) {
  return resolveCatalogLabel(value, rankingActionCatalog, fallback);
}

export function getRankingCategoryCatalog() {
  return rankingCategoryCatalog;
}

export function getRankingStateCatalog() {
  return rankingStateCatalog;
}

export function getRankingActionCatalog() {
  return rankingActionCatalog;
}

export function groupRankingCategories(categories: readonly RankingCategoryScore[]) {
  return {
    primary: categories.filter((category) => category.weight >= 15),
    secondary: categories.filter((category) => category.weight < 15),
  };
}

export function buildRankingCategoryCards(categories: readonly RankingCategoryScore[]): RankingCategoryCard[] {
  const byKey = new Map(categories.map((category) => [category.categoryKey, category]));
  const orbital = byKey.get("orbitalCapacity");
  const buildCard = (
    key: string,
    title: string,
    explanation: string,
    category?: RankingCategoryScore,
    readinessLabel?: string,
  ): RankingCategoryCard => ({
    key,
    title,
    scoreLabel: category?.scoreLabel ?? "Sin lectura",
    explanation,
    readinessLabel: readinessLabel ?? (category ? category.label : "Lectura demo"),
    emphasis: category?.emphasis ?? "warn",
  });

  return [
    buildCard("economy", "Economia", "Resume reservas y produccion visibles de la civilizacion.", byKey.get("economicPower"), "Lectura economica"),
    buildCard("colonies", "Colonias", "Resume control planetario, edificios y desarrollo visible.", byKey.get("colonyDevelopment"), "Despliegue colonial"),
    buildCard("research", "Investigacion", "Resume progreso tecnologico y cola actual sin efectos ocultos.", byKey.get("technologyProgress"), "Progreso visible"),
    buildCard("shipyard", "Astillero", "Usa la misma lectura orbital actual hasta que el backend separe industria y flotas.", orbital, "Lectura compartida"),
    buildCard("fleets", "Flotas", "Resume capacidad orbital, grupos visibles y postura de transferencias.", orbital, "Preparacion orbital"),
    buildCard("defense", "Defensas", "Resume estructuras defensivas sin simular combate ni mitigacion real.", byKey.get("defensiveReadiness"), "Defensa estructural"),
    buildCard("ground", "Ejercito Tierra", "Resume guarnicion local y capacidad de reclutamiento visible.", byKey.get("groundGarrison"), "Preparacion terrestre"),
    buildCard("intelligence", "Inteligencia", "Resume visibilidad, sensores y deteccion ya conocidos.", byKey.get("strategicIntelligence"), "Confianza estrategica"),
    buildCard("diplomacy", "Diplomacia", "Resume contactos y metadata diplomatica ya visible.", byKey.get("diplomacy"), "Lectura diplomatica"),
  ];
}

export function buildRankingComparisonCards(comparisons: readonly RankingComparisonRow[]): RankingComparisonCard[] {
  return comparisons.map((comparison) => {
    if (comparison.isCurrentCivilization) {
      return {
        key: comparison.rowKey,
        title: comparison.displayName,
        scoreLabel: comparison.totalPowerIndexLabel,
        deltaLabel: comparison.deltaLabel,
        postureLabel: comparison.postureLabel,
        stateLabel: rankingStaticLabels.unpublishedRanking,
        visibilityLabel: rankingStaticLabels.readOnly,
        note: "Tu lectura propia sólo se compara con referencias internas de validación.",
        emphasis: "good",
      };
    }

    if (normalizeLookup(comparison.rowKey).includes("futureseason")) {
      return {
        key: comparison.rowKey,
        title: comparison.displayName,
        scoreLabel: comparison.totalPowerIndexLabel,
        deltaLabel: comparison.deltaLabel,
        postureLabel: comparison.postureLabel,
        stateLabel: rankingStaticLabels.futureSeason,
        visibilityLabel: rankingStaticLabels.demoScenarioReference,
        note: "Referencia de validación del escenario. No representa una temporada publicada.",
        emphasis: "warn",
      };
    }

    return {
      key: comparison.rowKey,
      title: comparison.displayName,
      scoreLabel: comparison.totalPowerIndexLabel,
      deltaLabel: comparison.deltaLabel,
      postureLabel: comparison.postureLabel,
      stateLabel: rankingStaticLabels.demoScenarioReference,
      visibilityLabel: rankingStaticLabels.readOnly,
      note: "Referencia del escenario de validación para contexto interno no competitivo.",
      emphasis: "neutral",
    };
  });
}

export function buildRankingFutureLeaderboardCards(actions: readonly RankingFutureAction[]): RankingLeaderboardPlaceholderCard[] {
  const findAction = (pattern: string) => actions.find((action) => normalizeLookup(action.key).includes(pattern));
  const leaderboard = findAction("leaderboard");
  const season = findAction("season");
  const rewards = findAction("rewards");

  return [
    {
      key: "global-leaderboard",
      title: rankingStaticLabels.globalLeaderboard,
      stateLabel: leaderboard?.stateLabel ?? rankingStaticLabels.unpublishedRanking,
      reasonLabel: leaderboard?.reasonLabel ?? "El ranking global permanece desactivado en esta version.",
    },
    {
      key: "alliance-leaderboard",
      title: rankingStaticLabels.allianceLeaderboard,
      stateLabel: rankingStaticLabels.unpublishedRanking,
      reasonLabel: "La clasificacion de alianzas sigue fuera del alcance de esta vista.",
    },
    {
      key: "future-season",
      title: rankingStaticLabels.futureSeason,
      stateLabel: season?.stateLabel ?? rankingStaticLabels.futureSeason,
      reasonLabel: season?.reasonLabel ?? "La temporada futura sigue fuera del alcance de esta vista.",
    },
    {
      key: "rewards",
      title: rankingStaticLabels.rewardsUnavailable,
      stateLabel: rewards?.stateLabel ?? rankingStaticLabels.rewardsUnavailable,
      reasonLabel: rewards?.reasonLabel ?? "Las recompensas no forman parte de esta fase.",
    },
  ];
}

export function buildRankingFutureCapabilityCards(actions: readonly RankingFutureAction[]): RankingFutureCapabilityCard[] {
  const findAction = (pattern: string) => actions.find((action) => normalizeLookup(action.key).includes(pattern));
  const leaderboard = findAction("leaderboard");
  const season = findAction("season");
  const rewards = findAction("rewards");

  return [
    {
      key: "view-global-leaderboard",
      title: "Ver clasificacion global",
      stateLabel: leaderboard?.stateLabel ?? rankingStaticLabels.unavailable,
      reasonLabel: leaderboard?.reasonLabel ?? "El ranking global permanece desactivado en esta version.",
    },
    {
      key: "view-alliance-leaderboard",
      title: "Ver ranking de alianzas",
      stateLabel: rankingStaticLabels.unavailable,
      reasonLabel: "La clasificacion de alianzas sigue fuera del alcance de esta vista.",
    },
    {
      key: "open-season",
      title: "Abrir temporada",
      stateLabel: season?.stateLabel ?? rankingStaticLabels.unavailable,
      reasonLabel: season?.reasonLabel ?? "La temporada futura sigue fuera del alcance de esta vista.",
    },
    {
      key: "claim-reward",
      title: "Reclamar recompensa",
      stateLabel: rewards?.stateLabel ?? rankingStaticLabels.unavailable,
      reasonLabel: rewards?.reasonLabel ?? "Las recompensas no forman parte de esta fase.",
    },
    {
      key: "publish-profile",
      title: "Publicar perfil",
      stateLabel: rankingStaticLabels.unavailable,
      reasonLabel: "Los perfiles publicos no existen dentro de Ranking v1.",
    },
    {
      key: "compare-player",
      title: "Comparar jugador",
      stateLabel: rankingStaticLabels.unavailable,
      reasonLabel: "Las comparativas entre jugadores reales permanecen fuera del alcance de esta vista.",
    },
  ];
}

export function selectRecommendedRankingFocus(viewModel: RankingUiState | null) {
  if (!viewModel?.summary) {
    return rankingStaticLabels.unclassifiedMetric;
  }

  const topCategory = [...viewModel.summary.categories].sort((left, right) => right.score - left.score)[0];
  return topCategory?.label ?? viewModel.summary.recommendationLabel;
}

export function selectDominantRankingCategory(viewModel: RankingUiState | null) {
  if (!viewModel?.summary?.categories.length) {
    return rankingStaticLabels.unclassifiedMetric;
  }

  return [...viewModel.summary.categories]
    .sort((left, right) => right.score - left.score)[0]?.label
    ?? rankingStaticLabels.unclassifiedMetric;
}

export function selectWeakestRankingFocus(viewModel: RankingUiState | null) {
  if (!viewModel?.summary?.categories.length) {
    return "Sin área crítica visible";
  }

  return [...viewModel.summary.categories]
    .sort((left, right) => left.score - right.score)[0]?.label
    ?? "Pendiente de consolidar";
}

export function getRankingPrimaryAction(viewModel: RankingUiState | null) {
  if (!viewModel) {
    return rankingStaticLabels.powerIndex;
  }

  return viewModel.futureActions.find((action) => action.isAvailable)?.label
    ?? rankingStaticLabels.demoComparison;
}

export function mapRankingUiStateToViewModel(state: RankingUiStateDto): RankingUiState {
  const futureActions = [
    ...state.futurePlaceholders.map(mapFutureAction),
    ...state.disabledActions.map(mapFutureAction),
  ];

  return {
    civilizationId: state.civilizationId,
    identity: mapIdentity(state.identity),
    summary: mapSummary(state.summary),
    comparisons: state.demoComparisons.map(mapComparison),
    publication: mapPublication(state.publication),
    futureActions,
    diagnostics: mapDiagnostics(state.diagnostics, state.limitations, state.errors),
  };
}
