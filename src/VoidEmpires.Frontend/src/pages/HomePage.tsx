import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import type { PlanetCockpitDto } from "../api/planetTypes";
import { voidEmpiresApi } from "../api/voidEmpiresApi";
import { CockpitHero } from "../components/CockpitHero";
import { PlanetOverviewPanel } from "../components/PlanetOverviewPanel";
import { QueueSummaryPanels } from "../components/QueueSummaryPanels";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { getCurrentAccountWorldEntry } from "../utils/currentAccountSession";
import {
  buildGalaxyUrl,
  buildLoginUrl,
  buildRegisterUrl,
} from "../utils/routeUrls";
import { useCurrentAccountSession } from "../utils/useCurrentAccountSession";

export function HomePage() {
  const currentAccountSession = useCurrentAccountSession();
  const worldEntry = getCurrentAccountWorldEntry(currentAccountSession.session);
  const isLoading = currentAccountSession.status === "loading";
  const hasCommandHub = currentAccountSession.status === "ready" && worldEntry !== null;
  const [planet, setPlanet] = useState<PlanetCockpitDto | null>(null);
  const [isPlanetLoading, setIsPlanetLoading] = useState(false);
  const [planetError, setPlanetError] = useState<string | null>(null);
  const planetLabel = worldEntry?.planetName ?? "planeta principal";

  useEffect(() => {
    if (!worldEntry) {
      setPlanet(null);
      setPlanetError(null);
      return;
    }

    let isCurrent = true;
    setIsPlanetLoading(true);
    setPlanetError(null);
    voidEmpiresApi.getPlanetUiState(worldEntry.civilizationId, worldEntry.planetId)
      .then((response) => {
        if (isCurrent) {
          setPlanet(response.uiState?.planet ?? null);
        }
      })
      .catch(() => {
        if (isCurrent) {
          setPlanet(null);
          setPlanetError("No se pudo cargar el resumen del planeta actual.");
        }
      })
      .finally(() => {
        if (isCurrent) {
          setIsPlanetLoading(false);
        }
      });

    return () => {
      isCurrent = false;
    };
  }, [worldEntry?.civilizationId, worldEntry?.planetId]);

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

      {hasCommandHub && worldEntry ? (
        planet ? (
          <div className="home-overview-layout">
            <PlanetOverviewPanel civilizationLabel="Civilizacion activa" planet={planet} />
            <QueueSummaryPanels planet={planet} />
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
