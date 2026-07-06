import { NavLink, useLocation } from "react-router-dom";

export interface SidebarNavItem {
  label: string;
  to?: string;
  state?: "playable" | "map" | "readiness" | "readOnly" | "future";
}

interface SidebarNavProps {
  items: SidebarNavItem[];
}

function getSidebarStateNote(state: SidebarNavItem["state"]) {
  switch (state) {
    case "playable":
      return "Bucle jugable";
    case "map":
      return "Mapa";
    case "readiness":
      return "Preparacion";
    case "readOnly":
      return "Disponible";
    case "future":
      return "Futuro";
    default:
      return null;
  }
}

export function SidebarNav({ items }: SidebarNavProps) {
  const location = useLocation();

  return (
    <nav className="sidebar-nav" aria-label="Navegacion principal">
      {items.map((item) => {
        const stateNote = getSidebarStateNote(item.state);
        const stateClass = `sidebar-nav-item-${item.state ?? "readiness"}`;

        return item.to ? (
          <NavLink
            key={item.label}
            to={item.to}
            end={item.to === "/"}
            className={({ isActive }) => {
              const shouldHighlightGalaxyAlias =
                item.to === "/galaxy" && location.pathname === "/";

              return isActive || shouldHighlightGalaxyAlias
                ? `sidebar-nav-item ${stateClass} sidebar-nav-item-active`
                : `sidebar-nav-item ${stateClass}`;
            }}
          >
            <span className="sidebar-nav-item-label">{item.label}</span>
            {stateNote ? <small className="sidebar-nav-item-note">{stateNote}</small> : null}
          </NavLink>
        ) : (
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
