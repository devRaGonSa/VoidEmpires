import { NavLink, Route, Routes } from "react-router-dom";
import { appConfig } from "./config";
import { FleetsPage } from "./pages/FleetsPage";
import { StrategicMapPage } from "./pages/StrategicMapPage";

const navItems = [
  { to: "/", label: "Strategic Map" },
  { to: "/fleets", label: "Fleets" },
];

export default function App() {
  return (
    <div className="app-shell">
      <header className="topbar">
        <div>
          <p className="eyebrow">VoidEmpires frontend prototype</p>
          <h1>Development-only command surface</h1>
          <p className="lede">
            Conservative frontend shell for inspecting current backend readiness
            contracts without enabling gameplay mutations or production auth.
          </p>
        </div>
        <div className="environment-panel">
          <span className="badge badge-warn">Dev endpoints only</span>
          <dl>
            <div>
              <dt>Backend base URL</dt>
              <dd>{appConfig.apiBaseUrl}</dd>
            </div>
            <div>
              <dt>Expected backend profile</dt>
              <dd>{appConfig.backendProfile}</dd>
            </div>
          </dl>
        </div>
      </header>

      <nav className="nav-tabs" aria-label="Primary">
        {navItems.map((item) => (
          <NavLink
            key={item.to}
            to={item.to}
            end={item.to === "/"}
            className={({ isActive }) =>
              isActive ? "nav-tab nav-tab-active" : "nav-tab"
            }
          >
            {item.label}
          </NavLink>
        ))}
      </nav>

      <main className="page-frame">
        <Routes>
          <Route path="/" element={<StrategicMapPage />} />
          <Route path="/fleets" element={<FleetsPage />} />
        </Routes>
      </main>
    </div>
  );
}
