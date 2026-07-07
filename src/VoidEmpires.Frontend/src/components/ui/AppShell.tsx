import type { ReactNode } from "react";
import { useMemo, useState } from "react";
import { useLocation } from "react-router-dom";
import { voidEmpiresApi } from "../../api/voidEmpiresApi";
import { getCurrentAccountDisplay } from "../../utils/currentAccountSession";
import { buildLoginUrl, buildRegisterUrl } from "../../utils/routeUrls";
import { useCurrentAccountSession } from "../../utils/useCurrentAccountSession";
import { SidebarNav, type SidebarNavItem } from "./SidebarNav";
import { TopStatusBar, type TopBarStatusItem } from "./TopResourceBar";
import { UiCard } from "./UiCard";

interface AppShellProps {
  children: ReactNode;
  sidebarItems: SidebarNavItem[];
  statusItems: TopBarStatusItem[];
}

export function AppShell({
  children,
  sidebarItems,
  statusItems,
}: AppShellProps) {
  const location = useLocation();
  const currentAccountSession = useCurrentAccountSession();
  const accountDisplay = getCurrentAccountDisplay(currentAccountSession);
  const [isLoggingOut, setIsLoggingOut] = useState(false);
  const isHomeRoute = location.pathname === "/";
  const isFleetRoute = location.pathname === "/fleets";
  const isStrategicMapRoute = location.pathname === "/galaxy";
  const isCompactIntro = isHomeRoute || isFleetRoute || isStrategicMapRoute;

  const introTitle = isHomeRoute
    ? "Inicio del imperio"
    : isFleetRoute
      ? "Readiness orbital"
      : isStrategicMapRoute
        ? "Mapa estrategico"
        : "Centro de mando imperial";

  const introDescription = isHomeRoute
    ? "Accede con tu cuenta, registra un comandante o continua hacia el centro de mando disponible."
    : isFleetRoute
      ? "Flotas muestra preparacion, carga y ordenes confirmadas para el contexto seleccionado."
      : isStrategicMapRoute
        ? "La galaxia prioriza mapa, seleccion y contexto tactico para continuar hacia las cabinas del imperio."
        : "Cabinas de colonia, investigacion, astillero y flotas con lectura de estado y confirmaciones explicitas cuando la pagina lo permite.";

  const isSignedIn = currentAccountSession.status === "ready";
  const filteredStatusItems = useMemo(
    () => [
      ...statusItems,
      {
        label: "Mando",
        value: isSignedIn ? accountDisplay.planetLabel : accountDisplay.statusLabel,
      },
    ],
    [accountDisplay.planetLabel, accountDisplay.statusLabel, isSignedIn, statusItems],
  );

  async function handleLogout() {
    setIsLoggingOut(true);
    try {
      await voidEmpiresApi.account.logout();
      await currentAccountSession.refresh();
    } finally {
      setIsLoggingOut(false);
    }
  }

  return (
    <div className="app-shell">
      <a className="skip-link" href="#main-content">
        Saltar al contenido principal
      </a>
      <header className="app-topbar">
        <div className="app-topbar-brand">
          <span className="app-topbar-brand-mark">VE</span>
          <div>
            <strong>VoidEmpires</strong>
            <p>Imperio espacial persistente</p>
          </div>
        </div>
        <TopStatusBar
          account={{
            detail: accountDisplay.detailLabel,
            isBusy: isLoggingOut,
            isSignedIn,
            loginUrl: buildLoginUrl(),
            registerUrl: buildRegisterUrl(),
            value: isSignedIn ? accountDisplay.commanderLabel : accountDisplay.statusLabel,
            onLogout: handleLogout,
          }}
          items={filteredStatusItems}
        />
      </header>

      <div className="app-shell-frame">
        <aside className="app-sidebar" aria-label="Navegacion de cabinas">
          <div className="app-sidebar-head">
            <p className="eyebrow">Superficie imperial</p>
            <h2>Mapa de mando</h2>
            <p>Colonias, flotas y rutas estrategicas del imperio.</p>
          </div>
          <SidebarNav accountStatus={currentAccountSession.status} items={sidebarItems} />
          <div className="app-sidebar-status" aria-label="Estado del producto">
            <span>{accountDisplay.statusLabel}</span>
            <strong>{isSignedIn ? accountDisplay.civilizationLabel : "Entrada de cuenta"}</strong>
            <p>{accountDisplay.detailLabel}</p>
          </div>
        </aside>

        <div className="app-main-column">
          <section
            className={
              isCompactIntro
                ? "shell-intro-grid shell-intro-grid-compact"
                : "shell-intro-grid"
            }
          >
            <UiCard
              className={
                isCompactIntro
                  ? "shell-intro-card shell-intro-card-compact"
                  : "shell-intro-card"
              }
            >
              <div className="shell-intro-copy">
                <p className="eyebrow">VoidEmpires</p>
                <h1>{introTitle}</h1>
                <p className="lede">{introDescription}</p>
              </div>
            </UiCard>
          </section>

          <main className="page-frame" id="main-content" tabIndex={-1}>{children}</main>
        </div>
      </div>
    </div>
  );
}
