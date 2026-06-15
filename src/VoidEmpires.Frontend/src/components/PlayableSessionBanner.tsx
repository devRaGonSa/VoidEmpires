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
  const civilizationLabel = session.civilizationName ?? "Civilizacion local";
  const playerLabel = session.playerDisplayName ?? "Comandante local";
  const planetUrl = buildPlanetUrl(session.civilizationId, session.planetId);

  function handleClearLocalSession() {
    clearPlayableSession();
    onClear?.();
  }

  return (
    <UiCard className="panel">
      <div className="figma-section-header">
        <div>
          <p className="eyebrow">Sesion jugable local</p>
          <h3>{planetLabel}</h3>
          <p>
            {civilizationLabel} bajo {playerLabel}. Contexto Development-safe guardado solo para navegar.
          </p>
        </div>
        <UiBadge tone="good">Memoria local</UiBadge>
      </div>
      <p className="figma-panel-note">
        Esta tarjeta no autentica, no concede permisos y no sustituye el estado del backend.
      </p>
      <div className="selection-chip-row">
        <Link className="selection-chip selection-chip-active" to={planetUrl}>
          Ir al planeta
        </Link>
        <Link className="selection-chip" to="/onboarding">
          Cambiar/iniciar otra sesion
        </Link>
        <button type="button" className="selection-chip" onClick={handleClearLocalSession}>
          Limpiar sesion local
        </button>
      </div>
    </UiCard>
  );
}
