import { Link } from "react-router-dom";
import { CockpitHero } from "../components/CockpitHero";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { getCurrentAccountWorldEntry } from "../utils/currentAccountSession";
import { buildCommandHubUrl, buildGalaxyUrl, buildLoginUrl, buildRegisterUrl } from "../utils/routeUrls";
import { useCurrentAccountSession } from "../utils/useCurrentAccountSession";

export function HomePage() {
  const currentAccountSession = useCurrentAccountSession();
  const worldEntry = getCurrentAccountWorldEntry(currentAccountSession.session);
  const isLoading = currentAccountSession.status === "loading";
  const hasCommandHub = currentAccountSession.status === "ready" && worldEntry !== null;
  const planetLabel = worldEntry?.planetName ?? "planeta principal";
  const continueUrl = worldEntry
    ? buildCommandHubUrl(worldEntry.civilizationId, worldEntry.planetId)
    : currentAccountSession.nextRoute ?? buildGalaxyUrl();

  return (
    <section className="page-grid">
      <CockpitHero
        versionLabel={hasCommandHub ? "Cuenta activa" : isLoading ? "Comprobando cuenta" : "Acceso multijugador"}
        title={hasCommandHub ? "Centro de mando" : "VoidEmpires online"}
        description={hasCommandHub
          ? `Continua desde ${planetLabel} y vuelve a las cabinas principales de tu imperio.`
          : "Inicia sesion o registra tu comandante para entrar con una cuenta multijugador persistente."}
        developmentNote="Acceso de cuenta."
        badges={
          <>
            <UiBadge tone="good">{hasCommandHub ? "Mando disponible" : "Cuenta requerida"}</UiBadge>
            <UiBadge>Planeta como hub</UiBadge>
          </>
        }
      />

      {hasCommandHub ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Cuenta activa</p>
              <h3>{planetLabel}</h3>
              <p>Tu centro de mando esta listo para continuar desde la colonia principal.</p>
            </div>
            <UiBadge tone="good">Sesion lista</UiBadge>
          </div>
          <div className="home-account-summary">
            <p>El acceso se comprueba con la cuenta actual antes de abrir las cabinas de juego.</p>
          </div>
          <div className="selection-chip-row">
            <Link className="selection-chip selection-chip-active" to={continueUrl}>
              Continuar al mando
            </Link>
            <Link className="selection-chip" to={buildGalaxyUrl(worldEntry.civilizationId, null, worldEntry.planetId)}>
              Ver galaxia
            </Link>
          </div>
        </UiCard>
      ) : (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">{isLoading ? "Comprobando cuenta" : "Acceso de cuenta"}</p>
              <h3>{isLoading ? "Preparando entrada" : "Conecta tu comandante"}</h3>
              <p>
                {isLoading
                  ? "Estamos revisando si ya hay una cuenta activa en este navegador."
                  : "Entra con tu cuenta o registra un nuevo comandante para crear tu primer mundo."}
              </p>
            </div>
            <UiBadge tone={isLoading ? "neutral" : "good"}>{isLoading ? "En curso" : "Listo"}</UiBadge>
          </div>
          <div className="selection-chip-row">
            <Link className="selection-chip selection-chip-active" to={buildLoginUrl()}>
              Entrar
            </Link>
            <Link className="selection-chip" to={buildRegisterUrl()}>
              Registrar comandante
            </Link>
          </div>
        </UiCard>
      )}
    </section>
  );
}
