import { useState } from "react";
import { Link } from "react-router-dom";
import { CockpitHero } from "../components/CockpitHero";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { loadPlayableSession } from "../utils/playableSession";
import { buildPlanetUrl } from "../utils/routeUrls";

const disabledAccountActions = [
  {
    label: "Cambiar email",
    detail: "Pendiente de activar cuando exista un flujo seguro de verificacion.",
  },
  {
    label: "Cambiar contrasena",
    detail: "Pendiente de activar cuando exista recuperacion y confirmacion segura.",
  },
];

export function AccountSettingsPage() {
  const [session] = useState(() => loadPlayableSession());
  const hasSession = Boolean(session);
  const commanderLabel = session?.playerDisplayName ?? "Comandante sin cargar";
  const civilizationLabel = session?.civilizationName ?? "Civilizacion sin cargar";
  const planetLabel = session?.planetName ?? "Planeta natal sin cargar";
  const planetUrl = session ? buildPlanetUrl(session.civilizationId, session.planetId) : "/onboarding";

  return (
    <section className="page-grid account-settings-page">
      <CockpitHero
        versionLabel="Cuenta"
        title="Ajustes de cuenta"
        description="Resumen seguro del comandante y de la partida local, con acciones sensibles preparadas pero todavia desactivadas."
        developmentNote="Cuenta y seguridad."
        badges={
          <>
            <UiBadge tone={hasSession ? "good" : "warn"}>
              {hasSession ? "Sesion local" : "Sin sesion local"}
            </UiBadge>
            <UiBadge>Datos seguros</UiBadge>
          </>
        }
      />

      <UiCard className="panel account-settings-summary-panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Perfil visible</p>
            <h3>{hasSession ? commanderLabel : "Cuenta sin partida cargada"}</h3>
            <p>
              Esta superficie solo muestra nombres de juego guardados localmente y evita exponer
              identificadores internos o datos sensibles.
            </p>
          </div>
          <UiBadge tone={hasSession ? "good" : "warn"}>
            {hasSession ? "Resumen disponible" : "Pendiente"}
          </UiBadge>
        </div>

        <div className="account-settings-summary-grid">
          <div className="account-settings-summary-card">
            <span>Comandante</span>
            <strong>{commanderLabel}</strong>
          </div>
          <div className="account-settings-summary-card">
            <span>Civilizacion</span>
            <strong>{civilizationLabel}</strong>
          </div>
          <div className="account-settings-summary-card">
            <span>Planeta natal</span>
            <strong>{planetLabel}</strong>
          </div>
        </div>

        <div className="selection-chip-row">
          {hasSession ? (
            <Link className="selection-chip selection-chip-active" to={planetUrl}>
              Abrir planeta
            </Link>
          ) : (
            <Link className="selection-chip selection-chip-active" to="/onboarding">
              Crear partida
            </Link>
          )}
          <Link className="selection-chip" to="/">
            Volver a inicio
          </Link>
        </div>
      </UiCard>

      <UiCard className="panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Seguridad</p>
            <h3>Acciones sensibles preparadas</h3>
            <p>
              Email y contrasena quedan visibles como capacidades futuras, sin ejecutar cambios
              hasta que el backend exponga contratos seguros.
            </p>
          </div>
          <UiBadge tone="warn">No disponible</UiBadge>
        </div>

        <div className="account-settings-action-grid">
          {disabledAccountActions.map((action) => (
            <div className="account-settings-action-card" key={action.label}>
              <div>
                <strong>{action.label}</strong>
                <p>{action.detail}</p>
              </div>
              <button type="button" disabled>
                No disponible
              </button>
            </div>
          ))}
        </div>
      </UiCard>
    </section>
  );
}
