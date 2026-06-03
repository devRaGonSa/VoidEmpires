import { FormEvent, useEffect, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import type { PlanetUiStateResult } from "../api/planetTypes";
import type { PlanetModuleRouteInfo } from "../utils/planetPresentation";
import { voidEmpiresApi } from "../api/voidEmpiresApi";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { formatPlanetIdentity, formatPlanetOverviewLine, formatPlanetOwnerLabel, formatPlanetShortReference } from "../utils/planetPresentation";

interface PlanetDataRowProps {
  label: string;
  value: string;
}

interface ModuleCabinPageProps {
  route: PlanetModuleRouteInfo;
}

function PlanetDataRow({ label, value }: PlanetDataRowProps) {
  return (
    <div className="figma-data-row">
      <span>{label}</span>
      <strong>{value}</strong>
    </div>
  );
}

export function ModuleCabinPage({ route }: ModuleCabinPageProps) {
  const [searchParams, setSearchParams] = useSearchParams();
  const [civilizationIdInput, setCivilizationIdInput] = useState(
    searchParams.get("civilizationId") ?? "",
  );
  const [planetIdInput, setPlanetIdInput] = useState(
    searchParams.get("planetId") ?? "",
  );
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [uiState, setUiState] = useState<PlanetUiStateResult | null>(null);

  const queryCivilizationId = searchParams.get("civilizationId") ?? "";
  const queryPlanetId = searchParams.get("planetId");
  const planet = uiState?.planet ?? null;
  const activeCivilizationId = uiState?.civilizationId ?? queryCivilizationId;

  useEffect(() => {
    setCivilizationIdInput(queryCivilizationId);
    setPlanetIdInput(queryPlanetId ?? "");

    async function load() {
      if (!queryCivilizationId) {
        setUiState(null);
        setError(null);
        return;
      }

      setIsLoading(true);
      setError(null);

      try {
        const response = await voidEmpiresApi.getPlanetUiState(
          queryCivilizationId,
          queryPlanetId || undefined,
        );

        if (!response.succeeded || !response.uiState) {
          setUiState(null);
          setError(response.errors[0] ?? "La cabina especializada no pudo cargarse.");
          return;
        }

        setUiState(response.uiState);
      } catch (requestError) {
        setUiState(null);
        setError(
          requestError instanceof Error
            ? requestError.message
            : "La cabina especializada no pudo cargarse.",
        );
      } finally {
        setIsLoading(false);
      }
    }

    void load();
  }, [queryCivilizationId, queryPlanetId]);

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const trimmedCivilizationId = civilizationIdInput.trim();
    if (!trimmedCivilizationId) {
      setError("El id de civilizacion es obligatorio.");
      setUiState(null);
      return;
    }

    const nextParams = new URLSearchParams();
    nextParams.set("civilizationId", trimmedCivilizationId);

    const trimmedPlanetId = planetIdInput.trim();
    if (trimmedPlanetId) {
      nextParams.set("planetId", trimmedPlanetId);
    }

    setSearchParams(nextParams);
  }

  return (
    <section className="page-grid">
      <UiCard className="panel panel-hero figma-hero-card">
        <div className="figma-hero-copy">
          <UiBadge tone="resource">{route.label} v1</UiBadge>
          <h2>{route.title}</h2>
          <p>{route.purpose}</p>
        </div>
        <div className="figma-badge-row">
          <UiBadge>Cabina preparada para futura implementación.</UiBadge>
          <UiBadge tone="warn">Esta sección no ejecuta órdenes todavía.</UiBadge>
          <UiBadge tone="warn">Modulo especializado y seguro</UiBadge>
        </div>
      </UiCard>

      <div className="strategic-cockpit-top">
        <UiCard className="panel strategic-loader-panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Enlace de modulo</p>
              <h3>Cargar contexto especializado</h3>
            </div>
            <UiBadge>Uso local</UiBadge>
          </div>
          <form className="query-form" onSubmit={handleSubmit}>
            <label className="field">
              <span>Id de civilizacion</span>
              <input
                type="text"
                value={civilizationIdInput}
                onChange={(event) => setCivilizationIdInput(event.target.value)}
                placeholder="00000000-0000-0000-0000-000000000000"
                spellCheck={false}
              />
            </label>
            <label className="field">
              <span>Id de planeta opcional</span>
              <input
                type="text"
                value={planetIdInput}
                onChange={(event) => setPlanetIdInput(event.target.value)}
                placeholder="40000000-0000-0000-0000-000000000000"
                spellCheck={false}
              />
            </label>
            <button type="submit" disabled={isLoading}>
              {isLoading ? "Cargando..." : "Abrir cabina"}
            </button>
          </form>
          {error ? <p className="error-text">{error}</p> : null}
          {!queryCivilizationId ? (
            <p className="figma-panel-note">
              Introduce un `civilizationId` valido para cargar el contexto de la cabina especializada.
            </p>
          ) : null}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Estado actual</p>
              <h3>Contexto de planeta</h3>
            </div>
            <UiBadge>{planet ? formatPlanetShortReference(planet.planetId) : "Sin planeta"}</UiBadge>
          </div>
          {planet ? (
            <div className="figma-data-list">
              <PlanetDataRow label="Mundo" value={formatPlanetIdentity(planet)} />
              <PlanetDataRow label="Linea tactica" value={formatPlanetOverviewLine(planet)} />
              <PlanetDataRow label="Propiedad" value={formatPlanetOwnerLabel(planet)} />
              <PlanetDataRow label="Modulo" value={route.label} />
            </div>
          ) : (
            <p className="figma-panel-note">
              La cabina mostrara el planeta seleccionado cuando el contexto incluya `civilizationId` y, si aplica, `planetId`.
            </p>
          )}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Que pertenece aqui</p>
              <h3>Limite de la cabina</h3>
            </div>
            <UiBadge tone="good">Sin ordenes activas</UiBadge>
          </div>
          <ul className="stack-list strategic-rules-list">
            {route.belongsTo.map((item) => (
              <li key={item}>{item}</li>
            ))}
          </ul>
          <div className="figma-section-header module-boundary-spacer">
            <div>
              <p className="eyebrow">Que no pertenece aqui</p>
              <h4>Fuera de alcance</h4>
            </div>
          </div>
          <ul className="stack-list strategic-rules-list">
            {route.excludes.map((item) => (
              <li key={item}>{item}</li>
            ))}
          </ul>
        </UiCard>
      </div>

      <UiCard className="panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Navegacion</p>
            <h3>Siguientes cabinas</h3>
          </div>
          <UiBadge tone="warn">Contexto conservado</UiBadge>
        </div>
        <div className="selection-chip-row">
          <Link
            className="selection-chip selection-chip-active"
            to={`/planet?civilizationId=${activeCivilizationId}${planet ? `&planetId=${planet.planetId}` : queryPlanetId ? `&planetId=${queryPlanetId}` : ""}`}
          >
            Volver a Planeta
          </Link>
          <Link
            className="selection-chip"
            to={`/construction?civilizationId=${activeCivilizationId}${planet ? `&planetId=${planet.planetId}` : queryPlanetId ? `&planetId=${queryPlanetId}` : ""}`}
          >
            Abrir Construccion
          </Link>
          {route.module === "Shipyard" ? (
            <Link
              className="selection-chip"
              to={`/fleets?civilizationId=${activeCivilizationId}${planet ? `&planetId=${planet.planetId}` : queryPlanetId ? `&planetId=${queryPlanetId}` : ""}`}
            >
              Abrir Flotas
            </Link>
          ) : null}
        </div>
      </UiCard>
    </section>
  );
}
