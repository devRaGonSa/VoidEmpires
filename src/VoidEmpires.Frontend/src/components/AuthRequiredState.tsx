import type { ReactNode } from "react";
import { Link, Navigate, useLocation, useSearchParams } from "react-router-dom";
import { CockpitHero } from "./CockpitHero";
import { UiBadge } from "./ui/UiBadge";
import { UiCard } from "./ui/UiCard";
import { canEnterAccountRoute, createAccountWorldRouteSearch } from "../utils/currentAccountSession";
import { isOperatorMode } from "../utils/playableSession";
import { buildLoginUrl, buildRegisterUrl } from "../utils/routeUrls";
import { useCurrentAccountSession } from "../utils/useCurrentAccountSession";

interface AuthRequiredStateProps {
  children: ReactNode;
}

export function AuthRequiredState({ children }: AuthRequiredStateProps) {
  const location = useLocation();
  const [searchParams] = useSearchParams();
  const currentAccountSession = useCurrentAccountSession();
  const operatorAccess = isOperatorMode(searchParams);
  const canEnter = canEnterAccountRoute(currentAccountSession);
  const shouldResolveWorldContext = location.pathname !== "/account-settings";

  if (!operatorAccess && canEnter && shouldResolveWorldContext) {
    const accountWorldSearch = createAccountWorldRouteSearch(currentAccountSession.session, searchParams);
    if (accountWorldSearch) {
      return <Navigate to={`${location.pathname}?${accountWorldSearch.toString()}`} replace />;
    }
  }

  if (operatorAccess || canEnter) {
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
            ? "Estamos revisando si ya hay una cuenta activa para abrir esta pagina de juego."
            : "Entra con tu cuenta o crea una cuenta antes de abrir las paginas de juego."
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
                ? "La pagina se abrira automaticamente si la cuenta esta lista."
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
            Crear cuenta
          </Link>
        </div>
      </UiCard>
    </section>
  );
}
