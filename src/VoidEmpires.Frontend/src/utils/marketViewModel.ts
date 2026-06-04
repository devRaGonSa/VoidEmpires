import type {
  MarketCockpitDto,
  MarketFutureActionDto,
  MarketPlanetOptionDto,
  MarketProductionSummaryDto,
  MarketReferenceRatioDto,
  MarketResourceReserveDto,
  MarketSignalDto,
  MarketUiStateDto,
} from "../api/marketTypes";
import {
  formatMarketRatio,
  formatMarketResourceAmount,
  getMarketActionLabel,
  getMarketResourceLabel,
  getMarketSignalLabel,
  getPriceConfidenceLabel,
  getTradeStateLabel,
} from "./marketPresentation";

export interface MarketResourceReserve {
  resourceType: string;
  label: string;
  quantity: number;
  quantityLabel: string;
}

export interface MarketProductionFlow {
  resourceType: string;
  label: string;
  quantityPerHour: number;
  quantityLabel: string;
}

export interface MarketReferencePrice {
  resourceType: string;
  label: string;
  advisoryRatio: number;
  advisoryRatioLabel: string;
  confidenceKey: string;
  confidenceLabel: string;
  basisKey: string;
  isAdvisory: boolean;
}

export interface MarketSignal {
  signalKey: string;
  signalLabel: string;
  tradeStateLabel: string;
  resourceType: string | null;
  resourceLabel: string | null;
  quantity: number;
  quantityLabel: string;
  productionPerHour: number | null;
  productionLabel: string;
  severityKey: string;
  reasonKey: string;
}

export interface MarketFutureAction {
  actionKey: string;
  label: string;
  isEnabled: boolean;
  stateKey: string;
  stateLabel: string;
  reasonKey: string;
  reasonLabel: string;
}

export interface MarketRoutePlaceholder {
  actionKey: string;
  label: string;
  reasonLabel: string;
}

export interface MarketReferenceComparison {
  key: string;
  pairLabel: string;
  ratioLabel: string;
  advisoryLabel: string;
  executionLabel: string;
}

export interface MarketDiagnostics {
  playerFacing: readonly string[];
  technical: readonly string[];
  limitations: readonly string[];
}

export interface MarketEconomySummary {
  totalReserveTypes: number;
  activeSignalCount: number;
  availableFutureActions: number;
  reservePosture: string;
  productionPosture: string;
  tradePotential: string;
  referenceAvailability: string;
  recommendedFocus: string;
  primaryActionLabel: string;
}

export interface MarketPlanetOption {
  planetId: string;
  planetName: string;
  solarSystemName: string;
  isOwnedByRequestingCivilization: boolean;
  visibleReserves: MarketResourceReserve[];
  hasProductionProfile: boolean;
}

export interface MarketCockpit {
  selectedPlanetId: string | null;
  selectedPlanetName: string | null;
  selectedSolarSystemName: string | null;
  civilizationReserves: MarketResourceReserve[];
  selectedPlanetReserves: MarketResourceReserve[];
  production: MarketProductionFlow[];
  references: MarketReferencePrice[];
  signals: MarketSignal[];
  futureActions: MarketFutureAction[];
  referenceComparisons: MarketReferenceComparison[];
  routePlaceholders: MarketRoutePlaceholder[];
  logistics: MarketCockpitDto["logistics"];
  summary: MarketEconomySummary;
  diagnostics: MarketDiagnostics;
}

export interface MarketUiState {
  civilizationId: string;
  selectedPlanetId: string | null;
  knownPlanets: MarketPlanetOption[];
  market: MarketCockpit | null;
  diagnostics: MarketDiagnostics;
}

export const marketResourceOrder = ["Credits", "Metal", "Crystal", "Gas", "Deuterium", "Energy"] as const;

function normalizeValue(value: string | number | null | undefined) {
  return typeof value === "string" ? value : `${value ?? ""}`;
}

function mapReserves(entries: readonly MarketResourceReserveDto[]): MarketResourceReserve[] {
  return entries.map((entry) => {
    const resourceType = normalizeValue(entry.resourceType);
    return {
      resourceType,
      label: getMarketResourceLabel(resourceType),
      quantity: entry.quantity,
      quantityLabel: formatMarketResourceAmount(entry.quantity, resourceType),
    };
  });
}

function mapProduction(summary: MarketProductionSummaryDto | null): MarketProductionFlow[] {
  if (!summary) {
    return [];
  }

  return [
    { resourceType: "Credits", quantityPerHour: summary.creditsPerHour },
    { resourceType: "Metal", quantityPerHour: summary.metalPerHour },
    { resourceType: "Crystal", quantityPerHour: summary.crystalPerHour },
    { resourceType: "Gas", quantityPerHour: summary.gasPerHour },
  ].map((entry) => ({
    resourceType: entry.resourceType,
    label: getMarketSignalLabel("EstimatedProduction"),
    quantityPerHour: entry.quantityPerHour,
    quantityLabel: `${getMarketResourceLabel(entry.resourceType)} +${entry.quantityPerHour}/h`,
  }));
}

function mapReferences(entries: readonly MarketReferenceRatioDto[]): MarketReferencePrice[] {
  return entries.map((entry) => {
    const resourceType = normalizeValue(entry.resourceType);
    return {
      resourceType,
      label: getMarketResourceLabel(resourceType),
      advisoryRatio: entry.advisoryRatio,
      advisoryRatioLabel: formatMarketRatio(entry.advisoryRatio),
      confidenceKey: entry.confidenceKey,
      confidenceLabel: getPriceConfidenceLabel(entry.confidenceKey),
      basisKey: entry.basisKey,
      isAdvisory: entry.isAdvisory,
    };
  });
}

function mapSignals(entries: readonly MarketSignalDto[]): MarketSignal[] {
  return entries.map((entry) => {
    const resourceType = entry.resourceType === null || entry.resourceType === undefined
      ? null
      : normalizeValue(entry.resourceType);

    return {
      signalKey: entry.signalKey,
      signalLabel: getMarketSignalLabel(entry.signalKey),
      tradeStateLabel: getTradeStateLabel(entry.signalKey),
      resourceType,
      resourceLabel: resourceType ? getMarketResourceLabel(resourceType) : null,
      quantity: entry.quantity,
      quantityLabel: resourceType
        ? formatMarketResourceAmount(entry.quantity, resourceType)
        : "Sin cantidad asociada",
      productionPerHour: entry.productionPerHour,
      productionLabel: entry.productionPerHour === null
        ? "Produccion no visible"
        : `Produccion estimada ${entry.productionPerHour}/h`,
      severityKey: entry.severityKey,
      reasonKey: entry.reasonKey,
    };
  });
}

function mapFutureActions(entries: readonly MarketFutureActionDto[]): MarketFutureAction[] {
  return entries.map((entry) => ({
    actionKey: entry.actionKey,
    label: getMarketActionLabel(entry.actionKey),
    isEnabled: entry.isEnabled,
    stateKey: entry.stateKey,
    stateLabel: entry.isEnabled ? "Disponible" : "No disponible en esta version",
    reasonKey: entry.reasonKey,
    reasonLabel: getTradeStateLabel(entry.reasonKey),
  }));
}

function mapPlanetOption(entry: MarketPlanetOptionDto): MarketPlanetOption {
  return {
    planetId: entry.planetId,
    planetName: entry.planetName,
    solarSystemName: entry.solarSystemName,
    isOwnedByRequestingCivilization: entry.isOwnedByRequestingCivilization,
    visibleReserves: mapReserves(entry.visibleReserves),
    hasProductionProfile: entry.hasProductionProfile,
  };
}

function mapDiagnostics(
  market: MarketCockpitDto | null,
  errors: readonly string[],
): MarketDiagnostics {
  if (!market) {
    return {
      playerFacing: errors.length > 0 ? [errors[0]] : [],
      technical: [...errors],
      limitations: [],
    };
  }

  return {
    playerFacing: market.diagnostics.notes.filter((note) => !note.toLowerCase().includes("development-only")),
    technical: [
      ...market.diagnostics.notes,
      `OwnedPlanets=${market.diagnostics.ownedPlanetCount}`,
      `HasSelectedPlanetStockpile=${market.diagnostics.hasSelectedPlanetStockpile}`,
      `HasSelectedPlanetProduction=${market.diagnostics.hasSelectedPlanetProduction}`,
    ],
    limitations: [...market.limitations],
  };
}

export function groupMarketSignals(signals: readonly MarketSignal[]) {
  return signals.reduce<Record<string, MarketSignal[]>>((accumulator, signal) => {
    const key = signal.signalKey;
    accumulator[key] ??= [];
    accumulator[key].push(signal);
    return accumulator;
  }, {});
}

export function selectRecommendedMarketFocus(market: MarketCockpit | null) {
  if (!market) {
    return "Lectura economica pendiente";
  }

  const demandSignal = market.signals.find((signal) => signal.signalKey === "DemandPressure");
  if (demandSignal) {
    return demandSignal.resourceLabel
      ? `Presion de demanda en ${demandSignal.resourceLabel}`
      : "Presion de demanda visible";
  }

  const surplusSignal = market.signals.find((signal) => signal.signalKey === "VisibleSurplus");
  if (surplusSignal) {
    return surplusSignal.resourceLabel
      ? `Excedente visible de ${surplusSignal.resourceLabel}`
      : "Excedente visible";
  }

  return market.routePlaceholders.length > 0
    ? "Ruta futura en observacion"
    : "Reserva local estable";
}

export function getMarketPrimaryAction(market: MarketCockpit | null) {
  if (!market) {
    return getMarketActionLabel("market.read");
  }

  return market.futureActions.find((action) => action.isEnabled)?.label
    ?? getMarketActionLabel("market.signal.read");
}

export function getMarketResourceSignalLabel(
  resourceType: string,
  signals: readonly MarketSignal[],
) {
  const signal = signals.find((entry) => entry.resourceType === resourceType);
  if (!signal) {
    return "Sin lectura";
  }

  switch (signal.signalKey) {
    case "VisibleSurplus":
      return "Excedente visible";
    case "DemandPressure":
      return "Reserva ajustada";
    default:
      return "Estable";
  }
}

export function getMarketTradeSignalSummary(signal: MarketSignal) {
  const resourceLabel = signal.resourceLabel ?? "la economia visible";

  switch (signal.signalKey) {
    case "VisibleSurplus":
      return `${resourceLabel} mantiene excedente visible y podria requerir una salida logistica futura.`;
    case "DemandPressure":
      return `${resourceLabel} muestra tension y depende de produccion local o apoyo logistico posterior.`;
    case "FutureTradeRoute":
      return "La cabina detecta una dependencia de ruta futura, pero no puede crear ni ejecutar esa ruta desde Mercado.";
    case "EstimatedProduction":
      return signal.productionPerHour !== null
        ? `${resourceLabel} conserva una produccion estimada de ${signal.productionPerHour}/h para esta lectura.`
        : `${resourceLabel} sigue sin una produccion visible para orientar ruta o intercambio.`;
    default:
      return `${signal.signalLabel} sigue siendo una lectura orientativa sin mutacion de flotas ni recursos.`;
  }
}

function getReservePosture(civilizationReserves: readonly MarketResourceReserve[], signals: readonly MarketSignal[]) {
  if (signals.some((signal) => signal.signalKey === "DemandPressure")) {
    return "Reservas bajo presion";
  }

  const totalQuantity = civilizationReserves.reduce((sum, entry) => sum + entry.quantity, 0);
  if (signals.some((signal) => signal.signalKey === "VisibleSurplus") || totalQuantity >= 600) {
    return "Reservas holgadas";
  }

  return "Reserva local estable";
}

function getProductionPosture(production: readonly MarketProductionFlow[], signals: readonly MarketSignal[]) {
  if (production.length === 0) {
    return "Produccion no visible";
  }

  if (signals.some((signal) => signal.signalKey === "DemandPressure")) {
    return "Flujo tensionado";
  }

  return "Produccion estimada estable";
}

function getTradePotential(
  routePlaceholders: readonly MarketRoutePlaceholder[],
  references: readonly MarketReferencePrice[],
  signals: readonly MarketSignal[],
) {
  if (routePlaceholders.length > 0 && references.length > 0 && signals.length > 0) {
    return "Potencial comercial en observacion";
  }

  if (references.length > 0) {
    return "Lectura economica disponible";
  }

  return "Potencial comercial limitado";
}

function getReferenceAvailability(references: readonly MarketReferencePrice[]) {
  if (references.length === 0) {
    return "Sin referencias visibles";
  }

  const lowConfidence = references.filter((reference) => reference.confidenceKey === "Low").length;
  return lowConfidence > 0
    ? "Referencias orientativas"
    : "Referencias disponibles";
}

function mapMarketContext(market: MarketCockpitDto, errors: readonly string[]): MarketCockpit {
  const civilizationReserves = mapReserves(market.civilizationReserves);
  const selectedPlanetReserves = mapReserves(market.selectedPlanetReserves);
  const production = mapProduction(market.selectedPlanetProduction);
  const references = mapReferences(market.referenceRatios);
  const signals = mapSignals(market.signals);
  const futureActions = mapFutureActions(market.futureActions);
  const referenceComparisons = buildMarketReferenceComparisons(references);
  const routePlaceholders = futureActions
    .filter((action) => action.actionKey.includes("route"))
    .map((action) => ({
      actionKey: action.actionKey,
      label: action.label,
      reasonLabel: action.reasonLabel,
    }));
  const diagnostics = mapDiagnostics(market, errors);

  const cockpit: MarketCockpit = {
    selectedPlanetId: market.selectedPlanetId,
    selectedPlanetName: market.selectedPlanetName,
    selectedSolarSystemName: market.selectedSolarSystemName,
    civilizationReserves,
    selectedPlanetReserves,
    production,
    references,
    signals,
    futureActions,
    referenceComparisons,
    routePlaceholders,
    logistics: market.logistics,
    summary: {
      totalReserveTypes: civilizationReserves.length,
      activeSignalCount: signals.length,
      availableFutureActions: futureActions.filter((action) => action.isEnabled).length,
      reservePosture: "",
      productionPosture: "",
      tradePotential: "",
      referenceAvailability: "",
      recommendedFocus: "",
      primaryActionLabel: "",
    },
    diagnostics,
  };

  cockpit.summary.reservePosture = getReservePosture(civilizationReserves, signals);
  cockpit.summary.productionPosture = getProductionPosture(production, signals);
  cockpit.summary.tradePotential = getTradePotential(routePlaceholders, references, signals);
  cockpit.summary.referenceAvailability = getReferenceAvailability(references);
  cockpit.summary.recommendedFocus = selectRecommendedMarketFocus(cockpit);
  cockpit.summary.primaryActionLabel = getMarketPrimaryAction(cockpit);
  return cockpit;
}

export function buildMarketReferenceComparisons(
  references: readonly MarketReferencePrice[],
): MarketReferenceComparison[] {
  const pairs: Array<[string, string]> = [
    ["Metal", "Crystal"],
    ["Metal", "Gas"],
    ["Crystal", "Gas"],
    ["Credits", "Metal"],
  ];

  return pairs.flatMap(([leftResource, rightResource]) => {
    const left = references.find((entry) => entry.resourceType === leftResource);
    const right = references.find((entry) => entry.resourceType === rightResource);
    if (!left || !right || right.advisoryRatio === 0) {
      return [];
    }

    return [{
      key: `${leftResource}-${rightResource}`,
      pairLabel: `${left.label} <-> ${right.label}`,
      ratioLabel: formatMarketRatio(left.advisoryRatio / right.advisoryRatio),
      advisoryLabel: "Precio no ejecutable",
      executionLabel: "No ejecutable en esta cabina",
    }];
  });
}

export function mapMarketUiStateToViewModel(state: MarketUiStateDto): MarketUiState {
  const market = state.market ? mapMarketContext(state.market, state.errors) : null;
  const diagnostics = mapDiagnostics(state.market, state.errors);

  return {
    civilizationId: state.civilizationId,
    selectedPlanetId: state.selectedPlanetId,
    knownPlanets: state.knownPlanets.map(mapPlanetOption),
    market,
    diagnostics,
  };
}
