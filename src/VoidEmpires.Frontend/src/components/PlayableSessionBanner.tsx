import { Link } from "react-router-dom";
import type { PlayableSession } from "../utils/playableSession";
import { clearPlayableSession } from "../utils/playableSession";
import { buildPlanetUrl } from "../utils/routeUrls";
import { UiBadge } from "./ui/UiBadge";
import { UiCard } from "./ui/UiCard";

interface PlayableSessionBannerProps {
  session: PlayableSession | null;
  onClear?: () => void;
}

export function PlayableSessionBanner({ session, onClear }: PlayableSessionBannerProps) {
  if (!session) {
    return null;
  }

  const planetLabel = session.planetName ?? "Colonia guardada";
  const civilizationLabel = session.civilizationName ?? "Civilizacion guardada";
  const playerLabel = session.playerDisplayName ?? "Comandante";
  const planetUrl = buildPlanetUrl(session.civilizationId, session.planetId);

  function handleClearLocalSession() {
    clearPlayableSession();
    onClear?.();
  }

  return (
    <UiCard className="panel">
      <div className="figma-section-header">
        <div>
          <p className="eyebrow">Partida local</p>
          <h3>{planetLabel}</h3>
          <p>
            {civilizationLabel} bajo {playerLabel}. Continua tu partida desde la colonia guardada.
          </p>
        </div>
        <UiBadge tone="good">Partida guardada</UiBadge>
      </div>
      <p className="figma-panel-note">
        Cada cabina vuelve a comprobar el estado de juego antes de mostrar recursos, colas u ordenes disponibles.
      </p>
      <div className="selection-chip-row">
        <Link className="selection-chip selection-chip-active" to={planetUrl}>
          Continuar partida
        </Link>
        <Link className="selection-chip" to="/onboarding">
          Nueva partida
        </Link>
        <button type="button" className="selection-chip" onClick={handleClearLocalSession}>
          Olvidar partida
        </button>
      </div>
    </UiCard>
  );
}
