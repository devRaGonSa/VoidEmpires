import { NavLink, useLocation } from "react-router-dom";
import type { CurrentAccountSessionStatus } from "../../utils/currentAccountSession";

export interface SidebarNavItem {
  label: string;
  to?: string;
  state?: "playable" | "map" | "account" | "readiness" | "readOnly" | "future";
}

interface SidebarNavProps {
  items: SidebarNavItem[];
  accountStatus?: CurrentAccountSessionStatus;
}

function getSidebarStateNote(state: SidebarNavItem["state"]) {
  switch (state) {
    case "playable":
      return "Activo";
    case "map":
      return "Mapa";
    case "account":
      return "Cuenta";
    case "readiness":
      return "Estado";
    case "readOnly":
      return "Consulta";
    case "future":
      return "Proximamente";
    default:
      return null;
  }
}

function shouldShowItem(item: SidebarNavItem, accountStatus: CurrentAccountSessionStatus | undefined) {
  if (accountStatus === "ready") {
    return item.to !== "/login" && item.to !== "/register";
  }

  if (accountStatus === "signedOut" || accountStatus === "error") {
    return item.to !== "/account-settings";
  }

  return true;
}

function getSidebarItemPathname(to: string) {
  return to.split("?")[0] || "/";
}

export function SidebarNav({ items, accountStatus }: SidebarNavProps) {
  const location = useLocation();
  const visibleItems = items.filter((item) => shouldShowItem(item, accountStatus));

  return (
    <nav className="sidebar-nav" aria-label="Navegacion principal">
      {visibleItems.map((item) => {
        const stateNote = getSidebarStateNote(item.state);
        const stateClass = `sidebar-nav-item-${item.state ?? "readiness"}`;

        if (item.to) {
          const itemTo = item.to;

          return (
          <NavLink
            key={item.label}
            to={itemTo}
            end={getSidebarItemPathname(itemTo) === "/"}
            className={({ isActive }) => {
              const itemPathname = getSidebarItemPathname(itemTo);
              const shouldHighlightHomeAlias =
                itemPathname === "/" && location.pathname === "/planet";

              return isActive || shouldHighlightHomeAlias
                ? `sidebar-nav-item ${stateClass} sidebar-nav-item-active`
                : `sidebar-nav-item ${stateClass}`;
            }}
          >
            <span className="sidebar-nav-item-label">{item.label}</span>
            {stateNote ? <small className="sidebar-nav-item-note">{stateNote}</small> : null}
          </NavLink>
          );
        }

        return (
          <span
            key={item.label}
            className="sidebar-nav-item sidebar-nav-item-future"
            aria-disabled="true"
          >
            <span className="sidebar-nav-item-label">{item.label}</span>
            {stateNote ? <small className="sidebar-nav-item-note">{stateNote}</small> : null}
          </span>
        );
      })}
    </nav>
  );
}
