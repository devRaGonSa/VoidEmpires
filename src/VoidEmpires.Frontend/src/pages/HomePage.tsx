import { useState } from "react";
import { Link } from "react-router-dom";
import { CockpitHero } from "../components/CockpitHero";
import { PlayableSessionBanner } from "../components/PlayableSessionBanner";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { loadPlayableSession } from "../utils/playableSession";
import { buildGalaxyUrl } from "../utils/routeUrls";

export function HomePage() {
  const [session, setSession] = useState(() => loadPlayableSession());
  const hasSession = Boolean(session);

  return (
    <section className="page-grid">
      <CockpitHero
        versionLabel={hasSession ? "Continuar partida" : "Nueva partida"}
        title={hasSession ? "Retomar imperio" : "Comenzar VoidEmpires"}
        description={hasSession
          ? "Vuelve a tu colonia guardada o empieza una partida nueva desde el mismo lugar."
          : "Crea tu comandante, funda una colonia inicial y entra al hub de Planeta para administrar el imperio."}
        developmentNote="Inicio de juego."
        badges={
          <>
            <UiBadge tone="good">{hasSession ? "Partida guardada" : "Primer mundo"}</UiBadge>
            <UiBadge>Planeta como hub</UiBadge>
          </>
        }
      />

      {session ? (
        <PlayableSessionBanner session={session} onClear={() => setSession(null)} />
      ) : (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Nueva partida</p>
              <h3>Funda tu primera colonia</h3>
              <p>El inicio crea el contexto de juego y te lleva a Planeta como primera cabina.</p>
            </div>
            <UiBadge tone="good">Listo para empezar</UiBadge>
          </div>
          <div className="selection-chip-row">
            <Link className="selection-chip selection-chip-active" to="/onboarding">
              Nueva partida
            </Link>
            <Link className="selection-chip" to={buildGalaxyUrl()}>
              Ver galaxia
            </Link>
          </div>
        </UiCard>
      )}
    </section>
  );
}
