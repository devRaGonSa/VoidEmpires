type RankingValue = string | number | null | undefined;

interface RankingLabelEntry {
  key: string;
  label: string;
}

const unknownMetricFallback = "Metrica pendiente de clasificar";
const unknownCategoryFallback = "Categoria de poder pendiente de clasificar";
const unknownComparisonFallback = "Comparativa demo";
const unknownStateFallback = "Clasificacion no publicada";
const unknownActionFallback = "Operacion futura no disponible";

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
  demoComparison: "Comparativa demo",
  unpublishedRanking: "Clasificacion no publicada",
  futureSeason: "Temporada futura",
  rewardsUnavailable: "Recompensas no disponibles",
  unclassifiedMetric: "Metrica pendiente de clasificar",
} as const;

const rankingCategoryCatalog: readonly RankingLabelEntry[] = [
  { key: "powerIndex", label: rankingStaticLabels.powerIndex },
  { key: "economy", label: rankingStaticLabels.economyPower },
  { key: "economicPower", label: rankingStaticLabels.economyPower },
  { key: "colonies", label: rankingStaticLabels.colonialDevelopment },
  { key: "colonyDevelopment", label: rankingStaticLabels.colonialDevelopment },
  { key: "research", label: rankingStaticLabels.technologyProgress },
  { key: "technology", label: rankingStaticLabels.technologyProgress },
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
  { key: "futureSeason", label: rankingStaticLabels.futureSeason },
  { key: "rewardsDisabled", label: rankingStaticLabels.rewardsUnavailable },
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

export function getRankingStaticLabels() {
  return rankingStaticLabels;
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
