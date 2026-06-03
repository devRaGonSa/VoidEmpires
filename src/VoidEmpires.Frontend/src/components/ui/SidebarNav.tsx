import { NavLink, useLocation } from "react-router-dom";

export interface SidebarNavItem {
  label: string;
  to?: string;
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
                ? "sidebar-nav-item sidebar-nav-item-active"
                : "sidebar-nav-item";
            }}
          >
            {item.label}
          </NavLink>
        ) : (
          <span
            key={item.label}
            className="sidebar-nav-item sidebar-nav-item-disabled"
            aria-disabled="true"
          >
            {item.label}
          </span>
        ),
      )}
    </nav>
  );
}
