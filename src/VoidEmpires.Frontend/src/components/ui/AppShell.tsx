import type { ReactNode } from "react";
import { useLocation } from "react-router-dom";
import { DevEndpointNotice } from "./DevEndpointNotice";
import { SidebarNav, type SidebarNavItem } from "./SidebarNav";
import { TopStatusBar, type TopBarStatusItem } from "./TopResourceBar";
import { UiCard } from "./UiCard";

interface AppShellProps {
  apiBaseUrl: string;
  backendProfile: string;
  children: ReactNode;
  sidebarItems: SidebarNavItem[];
  statusItems: TopBarStatusItem[];
}

export function AppShell({
  apiBaseUrl,
  backendProfile,
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
            <p>Prototipo jugable local</p>
          </div>
        </div>
        <TopStatusBar items={statusItems} />
      </header>

      <div className="app-shell-frame">
        <aside className="app-sidebar" aria-label="Navegacion de cabinas">
          <div className="app-sidebar-head">
            <p className="eyebrow">Superficie imperial</p>
            <h2>Mapa de mando</h2>
            <p>Bucle jugable local, preparacion y lecturas de soporte.</p>
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
                <p className="eyebrow">VoidEmpires local</p>
                <h1>
                  {isFleetRoute
                    ? "Readiness orbital"
                    : isStrategicMapRoute
                      ? "Cabina estrategica de lectura"
                      : "Bucle jugable Development"}
                </h1>
                <p className="lede">
                  {isFleetRoute
                    ? "Flotas muestra readiness, carga y ordenes confirmadas sin ocultar el alcance local."
                    : isStrategicMapRoute
                      ? "La galaxia prioriza mapa, seleccion y contexto tactico. Los detalles tecnicos siguen disponibles, pero pasan a una segunda capa."
                      : "Cabinas con lecturas backend y mutaciones Development confirmadas cuando la pagina lo permite; sin autenticacion de produccion."}
                </p>
              </div>
            </UiCard>
            <DevEndpointNotice
              apiBaseUrl={apiBaseUrl}
              backendProfile={backendProfile}
              compact={isFleetRoute}
            />
          </section>

          <main className="page-frame" id="main-content" tabIndex={-1}>{children}</main>
        </div>
      </div>
    </div>
  );
}
