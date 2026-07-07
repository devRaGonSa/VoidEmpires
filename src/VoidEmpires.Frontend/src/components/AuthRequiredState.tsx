import type { ReactNode } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { CockpitHero } from "./CockpitHero";
import { UiBadge } from "./ui/UiBadge";
import { UiCard } from "./ui/UiCard";
import { canEnterAccountRoute } from "../utils/currentAccountSession";
import { isOperatorMode } from "../utils/playableSession";
import { buildLoginUrl, buildRegisterUrl } from "../utils/routeUrls";
import { useCurrentAccountSession } from "../utils/useCurrentAccountSession";

interface AuthRequiredStateProps {
  children: ReactNode;
}

export function AuthRequiredState({ children }: AuthRequiredStateProps) {
  const [searchParams] = useSearchParams();
  const currentAccountSession = useCurrentAccountSession();
  const operatorAccess = isOperatorMode(searchParams);

  if (operatorAccess || canEnterAccountRoute(currentAccountSession)) {
    return <>{children}</>;
  }

  const isLoading = currentAccountSession.status === "loading";

  return (
    <section className="page-grid">
      <CockpitHero
        versionLabel={isLoading ? "Comprobando cuenta" : "Cuenta requerida"}
        title={isLoading ? "Preparando acceso" : "Entrada necesaria"}
        description={
          isLoading
            ? "Estamos revisando si ya hay una cuenta activa para abrir esta cabina."
            : "Entra con tu cuenta o registra un comandante antes de abrir las cabinas principales."
        }
        developmentNote="Acceso de cuenta."
        badges={
          <>
            <UiBadge tone={isLoading ? "neutral" : "good"}>{isLoading ? "En curso" : "Acceso seguro"}</UiBadge>
            <UiBadge>Cuenta multijugador</UiBadge>
          </>
        }
      />

      <UiCard className="panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Acceso de cuenta</p>
            <h3>{isLoading ? "Comprobando sesion" : "Conecta tu comandante"}</h3>
            <p>
              {isLoading
                ? "La cabina se abrira automaticamente si la cuenta esta lista."
                : "Las rutas de juego usan la cuenta actual para seleccionar el imperio y el planeta inicial."}
            </p>
          </div>
          <UiBadge tone={isLoading ? "neutral" : "good"}>{isLoading ? "Espera" : "Disponible"}</UiBadge>
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
    </section>
  );
}
