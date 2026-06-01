import type { ReactNode } from "react";
import { DevEndpointNotice } from "./DevEndpointNotice";
import { SidebarNav, type SidebarNavItem } from "./SidebarNav";
import { TopResourceBar, type TopResourceItem } from "./TopResourceBar";
import { UiCard } from "./UiCard";

interface AppShellProps {
  apiBaseUrl: string;
  backendProfile: string;
  children: ReactNode;
  resources: TopResourceItem[];
  sidebarItems: SidebarNavItem[];
  userLabel: string;
}

export function AppShell({
  apiBaseUrl,
  backendProfile,
  children,
  resources,
  sidebarItems,
  userLabel,
}: AppShellProps) {
  return (
    <div className="app-shell">
      <header className="app-topbar">
        <div className="app-topbar-brand">
          <span className="app-topbar-brand-mark">VE</span>
          <div>
            <strong>VoidEmpires</strong>
            <p>Figma shell alignment prototype</p>
          </div>
        </div>
        <TopResourceBar resources={resources} userLabel={userLabel} />
      </header>

      <div className="app-shell-frame">
        <aside className="app-sidebar">
          <div className="app-sidebar-head">
            <p className="eyebrow">Empire surface</p>
            <h2>Command map</h2>
            <p>Only implemented read routes are enabled.</p>
          </div>
          <SidebarNav items={sidebarItems} />
        </aside>

        <div className="app-main-column">
          <section className="shell-intro-grid">
            <UiCard className="shell-intro-card">
              <p className="eyebrow">VoidEmpires frontend prototype</p>
              <h1>Development-only command surface</h1>
              <p className="lede">
                Figma-aligned shell for inspecting current backend readiness
                contracts without enabling gameplay mutations or production
                authentication.
              </p>
            </UiCard>
            <DevEndpointNotice
              apiBaseUrl={apiBaseUrl}
              backendProfile={backendProfile}
            />
          </section>

          <main className="page-frame">{children}</main>
        </div>
      </div>
    </div>
  );
}
