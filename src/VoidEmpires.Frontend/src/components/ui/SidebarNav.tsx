import { NavLink, useLocation } from "react-router-dom";

export interface SidebarNavItem {
  label: string;
  to?: string;
  state?: "implemented" | "future";
}

interface SidebarNavProps {
  items: SidebarNavItem[];
}

export function SidebarNav({ items }: SidebarNavProps) {
  const location = useLocation();

  return (
    <nav className="sidebar-nav" aria-label="Primary">
      {items.map((item) =>
        item.to ? (
          <NavLink
            key={item.label}
            to={item.to}
            end={item.to === "/"}
            className={({ isActive }) => {
              const shouldHighlightGalaxyAlias =
                item.to === "/galaxy" && location.pathname === "/";

              return isActive || shouldHighlightGalaxyAlias
                ? "sidebar-nav-item sidebar-nav-item-implemented sidebar-nav-item-active"
                : "sidebar-nav-item sidebar-nav-item-implemented";
            }}
          >
            <span className="sidebar-nav-item-label">{item.label}</span>
          </NavLink>
        ) : (
          <span
            key={item.label}
            className="sidebar-nav-item sidebar-nav-item-future"
            aria-disabled="true"
          >
            <span className="sidebar-nav-item-label">{item.label}</span>
            <small className="sidebar-nav-item-note">Futuro</small>
          </span>
        ),
      )}
    </nav>
  );
}
