import type { ReactNode } from "react";
import { useLocation } from "react-router-dom";
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
  const isFleetRoute = location.pathname === "/fleets";
  const isStrategicMapRoute =
    location.pathname === "/" || location.pathname === "/galaxy";
  const isCompactIntro = isFleetRoute || isStrategicMapRoute;

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
        <TopStatusBar items={statusItems} />
      </header>

      <div className="app-shell-frame">
        <aside className="app-sidebar" aria-label="Navegacion de cabinas">
          <div className="app-sidebar-head">
            <p className="eyebrow">Superficie imperial</p>
            <h2>Mapa de mando</h2>
            <p>Colonias, flotas y rutas estrategicas del imperio.</p>
          </div>
          <SidebarNav items={sidebarItems} />
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
                <h1>
                  {isFleetRoute
                    ? "Readiness orbital"
                    : isStrategicMapRoute
                      ? "Mapa estrategico"
                      : "Centro de mando imperial"}
                </h1>
                <p className="lede">
                  {isFleetRoute
                    ? "Flotas muestra preparacion, carga y ordenes confirmadas para el contexto seleccionado."
                    : isStrategicMapRoute
                      ? "La galaxia prioriza mapa, seleccion y contexto tactico para continuar hacia las cabinas del imperio."
                      : "Cabinas de colonia, investigacion, astillero y flotas con lectura de estado y confirmaciones explicitas cuando la pagina lo permite."}
                </p>
              </div>
            </UiCard>
          </section>

          <main className="page-frame" id="main-content" tabIndex={-1}>{children}</main>
        </div>
      </div>
    </div>
  );
}
