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

  const planetLabel = session.planetName ?? "Planeta principal";
  const civilizationLabel = session.civilizationName ?? "Civilizacion activa";
  const playerLabel = session.playerDisplayName ?? "Cuenta";
  const planetUrl = buildPlanetUrl(session.civilizationId, session.planetId);

  function handleClearSavedContext() {
    clearPlayableSession();
    onClear?.();
  }

  return (
    <UiCard className="panel">
      <div className="figma-section-header">
        <div>
          <p className="eyebrow">Continuar mundo</p>
          <h3>{planetLabel}</h3>
          <p>
            {civilizationLabel} bajo {playerLabel}. Continua desde el mundo principal guardado.
          </p>
        </div>
        <UiBadge tone="good">Cuenta activa</UiBadge>
      </div>
      <p className="figma-panel-note">
        La cuenta activa actualiza recursos, colas y ordenes antes de mostrar acciones.
      </p>
      <div className="selection-chip-row">
        <Link className="selection-chip selection-chip-active" to={planetUrl}>
          Continuar
        </Link>
        <Link className="selection-chip" to="/register">
          Crear cuenta
        </Link>
        <button type="button" className="selection-chip" onClick={handleClearSavedContext}>
          Limpiar seleccion
        </button>
      </div>
    </UiCard>
  );
}
