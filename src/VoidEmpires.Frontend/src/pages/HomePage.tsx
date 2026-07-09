import { useEffect, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { fetchDefensesUiState } from "../api/defenseApi";
import type { PlanetCockpitDto } from "../api/planetTypes";
import { fetchResearchUiState } from "../api/researchApi";
import { fetchShipyardUiState } from "../api/shipyardApi";
import { voidEmpiresApi } from "../api/voidEmpiresApi";
import { CockpitHero } from "../components/CockpitHero";
import { PlanetOverviewPanel } from "../components/PlanetOverviewPanel";
import { QueueSummaryPanels, type QueueSummaryItem } from "../components/QueueSummaryPanels";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { getCurrentAccountWorldEntry } from "../utils/currentAccountSession";
import { getDefenseStatusLabel, getDefenseStructureLabel } from "../utils/defensePresentation";
import { getResearchStatusLabel, getResearchTechnologyLabel } from "../utils/researchPresentation";
import {
  buildLoginUrl,
  buildRegisterUrl,
} from "../utils/routeUrls";
import { getAssetProductionStatusLabel, getAssetTypeLabel } from "../utils/shipyardPresentation";
import { useCurrentAccountSession } from "../utils/useCurrentAccountSession";

function formatDateTime(value: string) {
  const parsed = Date.parse(value);
  return Number.isNaN(parsed)
    ? "No disponible"
    : new Intl.DateTimeFormat("es-ES", { dateStyle: "short", timeStyle: "short" }).format(parsed);
}

function isOpenQueueStatus(value: string | number | null | undefined) {
  const status = `${value ?? ""}`.trim().toLowerCase();
  return status === "pending" || status === "active" || status === "1" || status === "2";
}

async function fetchModuleQueueSummaries(civilizationId: string, planetId: string | null | undefined) {
  const [researchResult, shipyardResult, defensesResult] = await Promise.allSettled([
    fetchResearchUiState(civilizationId, planetId),
    fetchShipyardUiState(civilizationId, planetId),
    fetchDefensesUiState(civilizationId, planetId),
  ]);
  const summaries: QueueSummaryItem[] = [];

  if (researchResult.status === "fulfilled" && researchResult.value.succeeded && researchResult.value.uiState) {
    const queue = researchResult.value.uiState.queue.filter((item) => isOpenQueueStatus(item.status));
    const first = queue[0];
    if (first) {
      summaries.push({
        label: "Investigacion",
        value: queue.length === 1 ? "1 investigacion" : `${queue.length} investigaciones`,
        detail: `${getResearchTechnologyLabel(first.researchType)} nivel ${first.targetLevel}: ${getResearchStatusLabel(first.status)}, cierre ${formatDateTime(first.endsAtUtc)}`,
      });
    }
  }

  if (shipyardResult.status === "fulfilled" && shipyardResult.value.succeeded && shipyardResult.value.uiState?.shipyard) {
    const queue = shipyardResult.value.uiState.shipyard.queue.filter((item) => isOpenQueueStatus(item.status));
    const first = queue[0];
    if (first) {
      summaries.push({
        label: "Astillero",
        value: queue.length === 1 ? "1 produccion" : `${queue.length} producciones`,
        detail: `${first.quantity} x ${getAssetTypeLabel(first.assetType)}: ${getAssetProductionStatusLabel(first.status)}, cierre ${formatDateTime(first.endsAtUtc)}`,
      });
    }
  }

  if (defensesResult.status === "fulfilled" && defensesResult.value.succeeded && defensesResult.value.uiState?.defenses) {
    const queue = defensesResult.value.uiState.defenses.defenseQueue.filter((item) => isOpenQueueStatus(item.status));
    const first = queue[0];
    if (first) {
      summaries.push({
        label: "Defensas",
        value: queue.length === 1 ? "1 produccion" : `${queue.length} producciones`,
        detail: `${getDefenseStructureLabel(first.buildingType)}: ${getDefenseStatusLabel(first.status)}, cierre ${formatDateTime(first.endsAtUtc)}`,
      });
    }
  }

  return summaries;
}

export function HomePage() {
  const [searchParams] = useSearchParams();
  const currentAccountSession = useCurrentAccountSession();
  const worldEntry = getCurrentAccountWorldEntry(currentAccountSession.session);
  const routeCivilizationId = searchParams.get("civilizationId")?.trim() ?? "";
  const routePlanetId = searchParams.get("planetId")?.trim() ?? "";
  const selectedCivilizationId = routeCivilizationId || worldEntry?.civilizationId || "";
  const selectedPlanetId = routePlanetId || (!routeCivilizationId ? worldEntry?.planetId : null);
  const isLoading = currentAccountSession.status === "loading";
  const hasCommandHub = currentAccountSession.status === "ready" && selectedCivilizationId.length > 0;
  const [planet, setPlanet] = useState<PlanetCockpitDto | null>(null);
  const [queueSummaries, setQueueSummaries] = useState<QueueSummaryItem[]>([]);
  const [isPlanetLoading, setIsPlanetLoading] = useState(false);
  const [planetError, setPlanetError] = useState<string | null>(null);
  const planetLabel = worldEntry?.planetName ?? "planeta principal";

  useEffect(() => {
    if (!selectedCivilizationId) {
      setPlanet(null);
      setQueueSummaries([]);
      setPlanetError(null);
      return;
    }

    let isCurrent = true;
    setIsPlanetLoading(true);
    setPlanetError(null);
    async function loadHomeState() {
      try {
        const response = await voidEmpiresApi.getPlanetUiState(selectedCivilizationId, selectedPlanetId);
        const nextPlanet = response.uiState?.planet ?? null;
        const nextPlanetId = nextPlanet?.planetId ?? response.uiState?.selectedPlanetId ?? selectedPlanetId;
        const nextQueueSummaries = nextPlanet
          ? await fetchModuleQueueSummaries(selectedCivilizationId, nextPlanetId)
          : [];

        if (isCurrent) {
          setPlanet(nextPlanet);
          setQueueSummaries(nextQueueSummaries);
        }
      } catch {
        if (isCurrent) {
          setPlanet(null);
          setQueueSummaries([]);
          setPlanetError("No se pudo cargar el resumen del planeta actual.");
        }
      } finally {
        if (isCurrent) {
          setIsPlanetLoading(false);
        }
      }
    }

    void loadHomeState();

    return () => {
      isCurrent = false;
    };
  }, [selectedCivilizationId, selectedPlanetId]);

  return (
    <section className="page-grid">
      <CockpitHero
        versionLabel={hasCommandHub ? "Planeta actual" : isLoading ? "Comprobando cuenta" : "Acceso multijugador"}
        title={hasCommandHub ? planet?.planetName ?? planetLabel : "VoidEmpires online"}
        description={hasCommandHub
          ? "Inicio resume la colonia activa, sus reservas, produccion y actividad inmediata."
          : "Inicia sesion o crea una cuenta para entrar con un imperio persistente."}
        developmentNote="Acceso de cuenta."
        badges={
          <>
            <UiBadge tone="good">{hasCommandHub ? "Colonia activa" : "Cuenta requerida"}</UiBadge>
            <UiBadge>Resumen planetario</UiBadge>
          </>
        }
      />

      {hasCommandHub ? (
        planet ? (
          <div className="home-overview-layout">
            <PlanetOverviewPanel civilizationLabel="Civilizacion activa" planet={planet} />
            <QueueSummaryPanels planet={planet} moduleSummaries={queueSummaries} />
          </div>
        ) : (
          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">{isPlanetLoading ? "Cargando" : "Planeta"}</p>
                <h3>{isPlanetLoading ? "Preparando resumen" : "Resumen no disponible"}</h3>
                <p>{planetError ?? "La cuenta esta lista, pero el planeta actual todavia no devolvio datos visibles."}</p>
              </div>
              <UiBadge tone={planetError ? "warn" : "neutral"}>{isPlanetLoading ? "En curso" : "Revisar"}</UiBadge>
            </div>
          </UiCard>
        )
      ) : (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">{isLoading ? "Comprobando cuenta" : "Acceso de cuenta"}</p>
              <h3>{isLoading ? "Preparando entrada" : "Conecta tu comandante"}</h3>
              <p>
                {isLoading
                  ? "Estamos revisando si ya hay una cuenta activa en este navegador."
                  : "Entra con tu cuenta o crea una cuenta para fundar tu primer mundo."}
              </p>
            </div>
            <UiBadge tone={isLoading ? "neutral" : "good"}>{isLoading ? "En curso" : "Listo"}</UiBadge>
          </div>
          <div className="selection-chip-row">
            <Link className="selection-chip selection-chip-active" to={buildLoginUrl()}>
              Entrar
            </Link>
            <Link className="selection-chip" to={buildRegisterUrl()}>
              Crear cuenta
            </Link>
          </div>
        </UiCard>
      )}
    </section>
  );
}
