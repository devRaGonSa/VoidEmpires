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
          <p className="eyebrow">Memoria local de navegacion</p>
          <h3>{planetLabel}</h3>
          <p>
            {civilizationLabel} bajo {playerLabel}. Guarda solo ids Development para reconstruir enlaces de cabina.
          </p>
        </div>
        <UiBadge tone="warn">No autentica</UiBadge>
      </div>
      <p className="figma-panel-note">
        Esta memoria no es login, cookie, token, rol ni permiso. Cada cabina vuelve a leer el backend con los ids visibles antes de mostrar estado jugable.
      </p>
      <div className="selection-chip-row">
        <Link className="selection-chip selection-chip-active" to={planetUrl}>
          Ir al planeta
        </Link>
        <Link className="selection-chip" to="/onboarding">
          Crear otro inicio
        </Link>
        <button type="button" className="selection-chip" onClick={handleClearLocalSession}>
          Limpiar memoria local
        </button>
      </div>
    </UiCard>
  );
}
