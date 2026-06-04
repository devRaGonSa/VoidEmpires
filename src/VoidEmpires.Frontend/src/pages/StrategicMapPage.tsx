import { FormEvent, useEffect, useMemo, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import type {
  ReadinessNote,
  StrategicMapResult,
  StrategicMapSystem,
} from "../api/strategicMapTypes";
import type {
  PlanetVisualStateResponse,
  SystemVisualStateResponse,
} from "../api/voidEmpiresApi";
import { voidEmpiresApi } from "../api/voidEmpiresApi";
import { CockpitHero } from "../components/CockpitHero";
import { StrategicMap2DView } from "../components/StrategicMap2DView";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { UiProgressBar } from "../components/ui/UiProgressBar";
import {
  formatBooleanLabel,
  formatColonizationStatus,
  formatCommandBlockReason,
  formatCompactGuid,
  formatPlanetPrimaryLabel,
  formatPlanetType,
  formatStarType,
  formatTransferStatus,
  formatVisibilityLevel,
  formatVisibilityReason,
  isOwnedVisibilityLevel,
  isVisibleVisibilityLevel,
} from "../utils/domainPresentation";
import {
  formatIntelCoverage,
  getEspionageActionLabel,
  getIntelConfidenceLabel,
  getIntelligenceLevelLabel,
  getObservationStatusLabel,
  getTargetVisibilityLabel,
} from "../utils/espionagePresentation";
import {
  buildConstructionUrl,
  buildDevelopmentHelperUrl,
  buildFleetsUrl,
  buildPlanetUrl,
  isSuspiciousCabinContext,
} from "../utils/routeUrls";
import { cockpitStatusLabels } from "../utils/cockpitStatus";
import { getUserFacingActionLabel } from "../utils/fleetCommandPresentation";

const readinessSections = [
  { label: "Notas de ruta y combustible", key: "routeFuelNotes" },
  { label: "Notas de sensores", key: "sensorNotes" },
  { label: "Notas de deteccion", key: "detectionNotes" },
  { label: "Notas de intercepcion", key: "interceptionNotes" },
  { label: "Notas de alianzas", key: "allianceNotes" },
  { label: "Notas de pactos de alianza", key: "alliancePactNotes" },
] as const;

function formatNote(note: ReadinessNote) {
  if (typeof note === "string") {
    return note;
  }

  return note.note ?? "Hay metadatos de preparacion.";
}

function readText(value: unknown, fallback = "No disponible") {
  return typeof value === "string" && value.length > 0 ? value : fallback;
}

function readCommands(value: unknown) {
  return Array.isArray(value)
    ? value.filter(
        (
          item,
        ): item is {
          actionKey?: string;
          note?: string;
          isAvailable?: boolean;
          blockReason?: string;
        } => typeof item === "object" && item !== null,
      )
    : [];
}

function readRecord(value: unknown) {
  return value as Record<string, unknown>;
}

function formatJson(value: unknown) {
  return JSON.stringify(value, null, 2);
}

function formatCoordinate(value: number | null) {
  return value ?? "?";
}

function readDomainValue(record: Record<string, unknown>, key: string) {
  const value = record[key];
  return typeof value === "string" || typeof value === "number" ? value : null;
}

function readNumberValue(record: Record<string, unknown>, key: string) {
  const value = record[key];
  return typeof value === "number" && !Number.isNaN(value) ? value : null;
}

function readStringValue(record: Record<string, unknown>, key: string) {
  const value = record[key];
  return typeof value === "string" && value.length > 0 ? value : null;
}

function readArrayLength(record: Record<string, unknown> | null, key: string) {
  if (!record) {
    return 0;
  }

  const value = record[key];
  return Array.isArray(value) ? value.length : 0;
}

function formatReadinessItem(value: unknown) {
  if (typeof value === "string") {
    return value;
  }

  if (typeof value === "object" && value !== null) {
    const record = value as Record<string, unknown>;
    return readText(record.note ?? record.name ?? record.tag, "Hay metadatos disponibles");
  }

  return "Hay metadatos disponibles";
}

function formatStrategicCommandLabel(actionKey: unknown) {
  const key = readText(actionKey, "unknown.command");
  return getEspionageActionLabel(key) ?? getUserFacingActionLabel(key, "Accion disponible");
}

function formatStrategicCommandSummary(
  actionKey: unknown,
  isAvailable: boolean | undefined,
  blockReason: unknown,
) {
  if (isAvailable) {
    switch (readText(actionKey, "")) {
      case "strategicMap.system.view":
        return "La lectura del sistema esta disponible desde el foco actual.";
      case "strategicMap.planet.viewDetail":
        return "El detalle del planeta puede inspeccionarse desde esta cabina.";
      case "fleet.travel.estimate":
        return "La ruta puede revisarse desde la cabina de flotas sin mutar datos.";
      case "fleet.transfer.create":
        return "La cabina de flotas puede preparar esta ruta cuando exista contexto valido.";
      case "exploration.preview":
        return "La lectura previa solo anticipa reconocimiento futuro y no activa ninguna mision.";
      case "exploration.mission.create":
        return "La mision permanece fuera de alcance en esta version y debe mostrarse como hoja de ruta.";
      default:
        return "Lectura disponible en esta vista de solo inspeccion.";
    }
  }

  return `No disponible en este momento: ${formatCommandBlockReason(
    typeof blockReason === "string" ? blockReason : null,
    "Bloqueado",
  ).toLowerCase()}.`;
}

function formatTransferPlanetLabel(
  planetId: string | null,
  planetNamesById: Map<string, string>,
) {
  if (!planetId) {
    return "Destino no identificado";
  }

  return planetNamesById.get(planetId) ?? formatPlanetPrimaryLabel(planetId);
}

function getVisibilityTone(level: string): "good" | "warn" | "neutral" {
  if (isOwnedVisibilityLevel(level)) {
    return "good";
  }

  if (isVisibleVisibilityLevel(level)) {
    return "neutral";
  }

  return "warn";
}

function isOwnedRecord(value: unknown) {
  return typeof value === "object" && value !== null
    ? Boolean((value as Record<string, unknown>).isOwnedByRequestingCivilization)
    : false;
}

function pickPreferredSystem(
  systems: StrategicMapSystem[],
  requestedSystemId: string | null,
) {
  return (
    systems.find((system) => system.systemId === requestedSystemId) ??
    systems.find((system) => isOwnedRecord(system)) ??
    systems.find((system) => system.isVisible) ??
    systems[0] ??
    null
  );
}

function pickPreferredPlanet(
  planets: StrategicMapSystem["planets"],
  requestedPlanetId: string | null,
) {
  return (
    planets?.find((planet) => planet.planetId === requestedPlanetId) ??
    planets?.find((planet) => isOwnedRecord(planet)) ??
    planets?.find((planet) => planet.isVisible) ??
    planets?.[0] ??
    null
  );
}

interface SummaryMetricProps {
  label: string;
  value: number;
}

function SummaryMetric({ label, value }: SummaryMetricProps) {
  return (
    <div className="figma-stat">
      <strong>{value}</strong>
      <span>{label}</span>
    </div>
  );
}

interface DataRowProps {
  label: string;
  value: string;
}

function DataRow({ label, value }: DataRowProps) {
  return (
    <div className="figma-data-row">
      <span>{label}</span>
      <strong>{value}</strong>
    </div>
  );
}

interface IntensityRowProps {
  label: string;
  value: number | null | undefined;
}

function formatPercentage(value: number | null | undefined) {
  if (typeof value !== "number" || Number.isNaN(value)) {
    return "No disponible";
  }

  return `${Math.round(Math.max(0, Math.min(1, value)) * 100)}%`;
}

function IntensityRow({ label, value }: IntensityRowProps) {
  const percent =
    typeof value === "number" && !Number.isNaN(value)
      ? Math.max(0, Math.min(100, value * 100))
      : 0;

  return (
    <div>
      <div className="figma-data-row">
        <span>{label}</span>
        <strong>{formatPercentage(value)}</strong>
      </div>
      <UiProgressBar value={percent} tone="neutral" />
    </div>
  );
}

export function StrategicMapPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [civilizationId, setCivilizationId] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [errorTechnicalDetail, setErrorTechnicalDetail] = useState<string | null>(null);
  const [result, setResult] = useState<StrategicMapResult | null>(null);
  const [selectedSystemId, setSelectedSystemId] = useState<string | null>(null);
  const [selectedPlanetId, setSelectedPlanetId] = useState<string | null>(null);
  const [isLoadingSystemVisual, setIsLoadingSystemVisual] = useState(false);
  const [systemVisualError, setSystemVisualError] = useState<string | null>(null);
  const [systemVisualState, setSystemVisualState] =
    useState<SystemVisualStateResponse["visualState"]>(null);
  const [isLoadingPlanetVisual, setIsLoadingPlanetVisual] = useState(false);
  const [planetVisualError, setPlanetVisualError] = useState<string | null>(null);
  const [planetVisualState, setPlanetVisualState] =
    useState<PlanetVisualStateResponse["visualState"]>(null);
  const queryCivilizationId = searchParams.get("civilizationId") ?? "";
  const querySystemId = searchParams.get("systemId");
  const queryPlanetId = searchParams.get("planetId");
  const isSuspiciousContext = isSuspiciousCabinContext(queryCivilizationId, queryPlanetId);
  const hasMissingContext = !queryCivilizationId;
  const hasInvalidContext = isSuspiciousContext;
  const hasApiError = Boolean(queryCivilizationId && error && !result);
  const hasEmptyStrategicReadModel = Boolean(result && result.systems.length === 0);
  const cockpitResult = result && result.systems.length > 0 ? result : null;
  const shouldRenderDiagnostics = Boolean(
    queryCivilizationId || result || error || isLoading || hasInvalidContext,
  );
  const loadStatusLabel = isLoading
    ? "Cargando"
    : hasInvalidContext
      ? "Contexto invalido"
      : hasMissingContext
        ? "Sin contexto"
        : hasApiError
          ? "Error de API"
          : hasEmptyStrategicReadModel
            ? "Lectura vacia"
            : cockpitResult
              ? "Cabina lista"
              : "Sincronizacion pendiente";

  const summary = useMemo(() => {
    if (!result) {
      return null;
    }

    return result.systems.reduce(
      (accumulator, system) => {
        accumulator.systems += 1;
        accumulator.planets += system.planets?.length ?? 0;
        accumulator.fleets += system.fleetPresence?.length ?? 0;
        accumulator.transfers += system.transferOverlays?.length ?? 0;
        return accumulator;
      },
      { systems: 0, planets: 0, fleets: 0, transfers: 0 },
    );
  }, [result]);

  const mapReadModel = useMemo(() => {
    if (!result) {
      return null;
    }

    return result.systems.reduce(
      (accumulator, system) => {
        if (isOwnedVisibilityLevel(system.visibilityLevel)) {
          accumulator.ownedSystems += 1;
        } else if (isVisibleVisibilityLevel(system.visibilityLevel)) {
          accumulator.visibleSystems += 1;
        } else {
          accumulator.unknownSystems += 1;
        }

        if (system.isVisible) {
          accumulator.detectedSystems += 1;
        }

        accumulator.fleetMarkers += system.fleetPresence?.length ?? 0;
        accumulator.transferMarkers += system.transferOverlays?.length ?? 0;
        return accumulator;
      },
      {
        ownedSystems: 0,
        visibleSystems: 0,
        unknownSystems: 0,
        detectedSystems: 0,
        fleetMarkers: 0,
        transferMarkers: 0,
      },
    );
  }, [result]);

  const selectedSystem = useMemo(
    () => (result ? pickPreferredSystem(result.systems, selectedSystemId) : null),
    [result, selectedSystemId],
  );

  const planetNamesById = useMemo(() => {
    const entries = result?.systems.flatMap((system) =>
      (system.planets ?? []).flatMap((planet) =>
        planet.planetName ? [[planet.planetId, planet.planetName] as const] : [],
      ),
    ) ?? [];
    return new Map(entries);
  }, [result]);

  const selectedPlanet = useMemo(
    () => pickPreferredPlanet(selectedSystem?.planets, selectedPlanetId),
    [selectedPlanetId, selectedSystem],
  );

  const systemCommands = useMemo(
    () => (selectedSystem ? readCommands(readRecord(selectedSystem).commands) : []),
    [selectedSystem],
  );

  const planetCommands = useMemo(
    () => (selectedPlanet ? readCommands(readRecord(selectedPlanet).commands) : []),
    [selectedPlanet],
  );

  const selectedSystemRecord = selectedSystem ? readRecord(selectedSystem) : null;
  const selectedPlanetRecord = selectedPlanet ? readRecord(selectedPlanet) : null;

  function selectSystem(system: StrategicMapSystem) {
    const preferredPlanet = pickPreferredPlanet(system.planets, null);
    setSelectedSystemId(system.systemId);
    setSelectedPlanetId(preferredPlanet?.planetId ?? null);
    setSystemVisualState(null);
    setSystemVisualError(null);
    setPlanetVisualState(null);
    setPlanetVisualError(null);

    if (result?.civilizationId) {
      const nextParams = new URLSearchParams();
      nextParams.set("civilizationId", result.civilizationId);
      nextParams.set("systemId", system.systemId);
      if (preferredPlanet?.planetId) {
        nextParams.set("planetId", preferredPlanet.planetId);
      }
      setSearchParams(nextParams, { replace: true });
    }
  }

  useEffect(() => {
    setCivilizationId(queryCivilizationId);
  }, [queryCivilizationId]);

  useEffect(() => {
    async function loadStrategicMapFromQuery() {
      if (!queryCivilizationId) {
        setResult(null);
        setSelectedSystemId(null);
        setSelectedPlanetId(null);
        setError(null);
        setErrorTechnicalDetail(null);
        setSystemVisualState(null);
        setSystemVisualError(null);
        setPlanetVisualState(null);
        setPlanetVisualError(null);
        return;
      }

      setIsLoading(true);
      setError(null);
      setErrorTechnicalDetail(null);

      try {
        const response = await voidEmpiresApi.getStrategicMap(queryCivilizationId);
        if (!response.succeeded || !response.map) {
          setResult(null);
          setSelectedSystemId(null);
          setSelectedPlanetId(null);
          setError("No se pudo cargar el mapa de Galaxia.");
          setErrorTechnicalDetail(response.errors[0] ?? "Strategic map request failed.");
          return;
        }

        const requestedSystem = pickPreferredSystem(response.map.systems, querySystemId);
        const requestedPlanet = pickPreferredPlanet(
          requestedSystem?.planets,
          queryPlanetId,
        );

        setResult(response.map);
        setSelectedSystemId(requestedSystem?.systemId ?? null);
        setSelectedPlanetId(requestedPlanet?.planetId ?? null);
        setSystemVisualState(null);
        setSystemVisualError(null);
        setPlanetVisualState(null);
        setPlanetVisualError(null);
        setErrorTechnicalDetail(null);
      } catch (requestError) {
        const message =
          requestError instanceof Error
            ? requestError.message
            : "Strategic map request failed.";
        setResult(null);
        setSelectedSystemId(null);
        setSelectedPlanetId(null);
        setError("No se pudo cargar el mapa de Galaxia.");
        setErrorTechnicalDetail(message);
      } finally {
        setIsLoading(false);
      }
    }

    void loadStrategicMapFromQuery();
  }, [queryCivilizationId, querySystemId, queryPlanetId]);

  async function loadSystemVisualState() {
    if (!selectedSystem) {
      return;
    }

    setIsLoadingSystemVisual(true);
    setSystemVisualError(null);
    try {
      const response = await voidEmpiresApi.getSystemVisualState(selectedSystem.systemId);
      setSystemVisualState(response.succeeded ? response.visualState : null);
      setSystemVisualError(
        response.succeeded
          ? null
          : response.errors[0] ?? "La lectura del estado visual del sistema fallo.",
      );
    } catch (requestError) {
      setSystemVisualState(null);
      setSystemVisualError(
        requestError instanceof Error
          ? requestError.message
          : "La lectura del estado visual del sistema fallo.",
      );
    } finally {
      setIsLoadingSystemVisual(false);
    }
  }

  async function loadPlanetVisualState() {
    if (!selectedPlanet?.isVisible) {
      setPlanetVisualState(null);
      setPlanetVisualError("Selecciona un planeta visible para inspeccionar su estado visual.");
      return;
    }

    setIsLoadingPlanetVisual(true);
    setPlanetVisualError(null);
    try {
      const response = await voidEmpiresApi.getPlanetVisualState(selectedPlanet.planetId);
      setPlanetVisualState(response.succeeded ? response.visualState : null);
      setPlanetVisualError(
        response.succeeded
          ? null
          : response.errors[0] ?? "La lectura del estado visual del planeta fallo.",
      );
    } catch (requestError) {
      setPlanetVisualState(null);
      setPlanetVisualError(
        requestError instanceof Error
          ? requestError.message
          : "La lectura del estado visual del planeta fallo.",
      );
    } finally {
      setIsLoadingPlanetVisual(false);
    }
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const trimmedCivilizationId = civilizationId.trim();
    if (!trimmedCivilizationId) {
      setError("El id de civilizacion es obligatorio.");
      setErrorTechnicalDetail("Civilization id is required.");
      setResult(null);
      setSelectedSystemId(null);
      setSelectedPlanetId(null);
      setSystemVisualState(null);
      setSystemVisualError(null);
      setPlanetVisualState(null);
      setPlanetVisualError(null);
      return;
    }

    const nextParams = new URLSearchParams();
    nextParams.set("civilizationId", trimmedCivilizationId);
    if (selectedSystemId) {
      nextParams.set("systemId", selectedSystemId);
    }
    if (selectedPlanetId) {
      nextParams.set("planetId", selectedPlanetId);
    }
    setSearchParams(nextParams);
  }

  return (
    <section className="page-grid">
      <CockpitHero
        versionLabel="Galaxia v1"
        title="Cabina estrategica de Galaxia"
        description="La vista tactica prioriza el frente, el foco de sistema y la inteligencia planetaria antes que el detalle tecnico."
        developmentNote="La lectura sigue orientada a QA local: conserva limites seguros, no ejecuta ordenes y mantiene las rutas tecnicas fuera del foco principal."
        badges={
          <>
            <UiBadge>{cockpitStatusLabels.readOnly}</UiBadge>
            <UiBadge>Mapa tactico</UiBadge>
            <UiBadge tone="warn">Sin mutaciones jugables</UiBadge>
          </>
        }
      />

      <div className="strategic-cockpit-top">
        <UiCard className="panel strategic-loader-panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Enlace de mando</p>
              <h3>Cargar estado de cabina</h3>
            </div>
            <UiBadge>Solo lectura</UiBadge>
          </div>
          <form className="query-form" onSubmit={handleSubmit}>
            <label className="field">
              <span>Id de civilizacion</span>
              <input
                type="text"
                value={civilizationId}
                onChange={(event) => setCivilizationId(event.target.value)}
                placeholder="00000000-0000-0000-0000-000000000000"
                spellCheck={false}
                aria-label="Id de civilizacion"
              />
            </label>
            <button type="submit" disabled={isLoading}>
              {isLoading ? "Cargando..." : "Sincronizar Galaxia"}
            </button>
          </form>
          {error && <p className="error-text">{error}</p>}
          {hasMissingContext && (
            <p className="figma-panel-note">
              Carga un contexto de civilizacion para abrir Galaxia.
            </p>
          )}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Panorama estrategico</p>
              <h3>Estado del frente</h3>
            </div>
            <UiBadge>
              {summary ? formatCompactGuid(result?.civilizationId) : "Esperando mapa"}
            </UiBadge>
          </div>
          {summary ? (
            <div className="figma-stat-grid">
              <SummaryMetric label="Sistemas" value={summary.systems} />
              <SummaryMetric label="Planetas" value={summary.planets} />
              <SummaryMetric label="Marcadores de flota" value={summary.fleets} />
              <SummaryMetric label="Rutas activas" value={summary.transfers} />
            </div>
          ) : (
            <p className="figma-panel-note">
              Carga una civilizacion para poblar el resumen estrategico y activar
              los paneles de inteligencia del sistema.
            </p>
          )}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Limites actuales</p>
              <h3>Reglas de lectura</h3>
            </div>
            <UiBadge tone="warn">Inspeccion segura</UiBadge>
          </div>
          <ul className="stack-list strategic-rules-list">
            <li>El servicio puede no estar disponible fuera del entorno local preparado para esta lectura.</li>
            <li>Los datos siguen en solo lectura y no otorgan autorizacion de juego.</li>
            <li>La disponibilidad y los metadatos de mando son orientativos, no autoridad.</li>
            <li>Las ordenes directas, la actualizacion continua y la simulacion visual avanzada siguen fuera de alcance.</li>
          </ul>
        </UiCard>
      </div>

      {hasInvalidContext && (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Contexto sospechoso</p>
              <h3>El contexto de civilizacion no es valido para Galaxia.</h3>
            </div>
            <UiBadge tone="warn">Revisar contexto</UiBadge>
          </div>
          <p className="figma-panel-note">
            Revisa que no hayas usado el id del planeta como civilizacion.
          </p>
          <div className="selection-chip-row">
            <Link className="selection-chip selection-chip-active" to={buildDevelopmentHelperUrl()}>
              Abrir contexto de desarrollo
            </Link>
          </div>
        </UiCard>
      )}

      {hasMissingContext && !hasInvalidContext && (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Contexto requerido</p>
              <h3>Galaxia necesita una civilizacion activa</h3>
            </div>
            <UiBadge>Uso local</UiBadge>
          </div>
          <p className="figma-panel-note">
            Carga un contexto de civilizacion para abrir Galaxia.
          </p>
          <p className="figma-panel-note">
            Puedes introducir el `civilizationId` manualmente o llegar desde
            Planeta, Construccion, Investigacion, Astillero o Flotas para conservar
            el contexto actual.
          </p>
        </UiCard>
      )}

      {isLoading && (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Cargando</p>
              <h3>Sincronizando mapa de Galaxia</h3>
              <p>Consultando sistemas visibles, planetas conocidos y marcadores tacticos.</p>
            </div>
            <UiBadge>Cargando...</UiBadge>
          </div>
        </UiCard>
      )}

      {hasApiError && (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Fallo de carga</p>
              <h3>No se pudo cargar el mapa de Galaxia.</h3>
              <p>La cabina sigue accesible, pero el backend no devolvio un mapa util para este contexto.</p>
            </div>
            <UiBadge tone="warn">Sin mapa</UiBadge>
          </div>
          <p className="error-text">{error}</p>
          {errorTechnicalDetail ? (
            <details className="json-details">
              <summary>Detalle tecnico</summary>
              <pre className="json-preview">{errorTechnicalDetail}</pre>
            </details>
          ) : null}
        </UiCard>
      )}

      {hasEmptyStrategicReadModel && (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Estado vacio</p>
              <h3>No hay sistemas visibles para esta civilizacion.</h3>
              <p>La solicitud devolvio un contexto valido, pero el teatro visible sigue vacio para esta lectura.</p>
            </div>
            <UiBadge tone="warn">Sin sistemas</UiBadge>
          </div>
        </UiCard>
      )}

      {cockpitResult ? (
        <UiCard className="panel strategic-map-panel">
          <div className="figma-map-layout">
            <div className="figma-map-stage">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Galaxia</p>
                  <h3>Mapa principal del teatro</h3>
                  <p>
                    Las coordenadas del backend se proyectan en un plano 2D
                    determinista para que el contexto espacial siga al frente de la
                    inspeccion.
                  </p>
                </div>
                <UiBadge>Plano tactico SVG</UiBadge>
              </div>
              <StrategicMap2DView
                systems={cockpitResult.systems}
                selectedSystemId={selectedSystem?.systemId}
                onSelectSystem={(systemId) => {
                  const system = cockpitResult.systems.find((item) => item.systemId === systemId);
                  if (system) {
                    selectSystem(system);
                  }
                }}
              />
            </div>

            <aside className="figma-map-sidebar">
              <div className="figma-mini-card">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Lectura actual</p>
                    <h4>Resumen de inteligencia</h4>
                  </div>
                  <UiBadge tone="warn">Solo lectura</UiBadge>
                </div>
                <div className="figma-data-list">
                  <DataRow
                    label="Sistemas visibles"
                    value={String(mapReadModel?.detectedSystems ?? cockpitResult.systems.length)}
                  />
                  <DataRow
                    label="Bajo control"
                    value={String(mapReadModel?.ownedSystems ?? 0)}
                  />
                  <DataRow
                    label="Observados"
                    value={String(mapReadModel?.visibleSystems ?? 0)}
                  />
                  <DataRow
                    label="Sin confirmar"
                    value={String(mapReadModel?.unknownSystems ?? 0)}
                  />
                  <DataRow
                    label="Foco actual"
                    value={selectedSystem?.systemName ?? "Sin seleccion"}
                  />
                  <DataRow
                    label="Marcadores orbitales"
                    value={`${mapReadModel?.fleetMarkers ?? 0} flotas | ${mapReadModel?.transferMarkers ?? 0} rutas`}
                  />
                  <DataRow label="Malla tactica" value="64u | Proyeccion fija | Solo lectura" />
                </div>
              </div>

              <div className="figma-mini-card">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Leyenda</p>
                    <h4>Codigos del mapa</h4>
                  </div>
                  <UiBadge>{cockpitResult.systems.length} nodos</UiBadge>
                </div>
                <div className="figma-legend-grid">
                  <div className="figma-legend-item">
                    <span className="figma-legend-dot figma-legend-owned" />
                    <strong>Propio</strong>
                    <small>Circulo solido y control directo</small>
                  </div>
                  <div className="figma-legend-item">
                    <span className="figma-legend-dot figma-legend-visible" />
                    <strong>Visible</strong>
                    <small>Diamante central y sistema observado</small>
                  </div>
                  <div className="figma-legend-item">
                    <span className="figma-legend-dot figma-legend-unknown" />
                    <strong>Contacto parcial</strong>
                    <small>Cuadro central y lectura incompleta</small>
                  </div>
                  <div className="figma-legend-item">
                    <span className="figma-legend-dot figma-legend-fleet" />
                    <strong>Flota</strong>
                    <small>Baliza triangular de presencia orbital</small>
                  </div>
                  <div className="figma-legend-item">
                    <span className="figma-legend-line" />
                    <strong>Transferencia</strong>
                    <small>Doble trazo orbital superpuesto</small>
                  </div>
                </div>
              </div>

              {(mapReadModel?.detectedSystems ?? cockpitResult.systems.length) <= 2 && (
                <div className="figma-mini-card figma-mini-card-warn">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">Pista operativa</p>
                      <h4>Lectura de baja densidad</h4>
                    </div>
                    <UiBadge tone="warn">Cobertura limitada</UiBadge>
                  </div>
                  <p className="figma-panel-note">
                    La niebla actual es deliberada: con pocas firmas visibles
                    conviene validar foco, balizas orbitales y rutas antes de
                    asumir que el teatro esta vacio.
                  </p>
                </div>
              )}

              {selectedSystem && (
                <div className="figma-mini-card">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">Foco activo</p>
                      <h4>{selectedSystem.systemName ?? "Sistema desconocido"}</h4>
                    </div>
                    <UiBadge tone={getVisibilityTone(selectedSystem.visibilityLevel)}>
                      {getTargetVisibilityLabel(
                        selectedSystem.visibilityLevel,
                        Boolean(selectedSystemRecord?.isOwnedByRequestingCivilization),
                      )}
                    </UiBadge>
                  </div>
                  <div className="figma-data-list">
                    <DataRow
                      label="Coordenadas"
                      value={`${formatCoordinate(selectedSystem.coordinateX)}, ${formatCoordinate(selectedSystem.coordinateY)}, ${formatCoordinate(selectedSystem.coordinateZ)}`}
                    />
                    <DataRow
                      label="Nivel de inteligencia"
                      value={getIntelligenceLevelLabel(
                        selectedSystem.visibilityLevel,
                        selectedSystem.visibilityReason,
                      )}
                    />
                    <DataRow label="Planetas" value={String(selectedSystem.planets?.length ?? 0)} />
                    <DataRow label="Flotas" value={String(selectedSystem.fleetPresence?.length ?? 0)} />
                    <DataRow label="Transferencias" value={String(selectedSystem.transferOverlays?.length ?? 0)} />
                  </div>
                </div>
              )}

              {selectedSystem && (
                <div className="figma-mini-card">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">Transito orbital</p>
                      <h4>Rutas de transferencia</h4>
                    </div>
                    <UiBadge>{selectedSystem.transferOverlays?.length ?? 0} rutas</UiBadge>
                  </div>
                  {selectedSystem.transferOverlays?.length ? (
                    <ul className="stack-list compact-list strategic-overlay-list">
                      {selectedSystem.transferOverlays.map((overlay, index) => (
                        <li key={`transfer-overlay-${index}`}>
                          {(() => {
                            const overlayRecord = readRecord(overlay);
                            const originPlanetId = readStringValue(
                              overlayRecord,
                              "originPlanetId",
                            );
                            const destinationPlanetId = readStringValue(
                              overlayRecord,
                              "destinationPlanetId",
                            );
                            const distance = readNumberValue(
                              overlayRecord,
                              "abstractDistanceUnits",
                            );
                            const status = readStringValue(overlayRecord, "status");
                            const routeLabel = `${formatTransferPlanetLabel(
                              originPlanetId,
                              planetNamesById,
                            )} -> ${formatTransferPlanetLabel(
                              destinationPlanetId,
                              planetNamesById,
                            )}`;
                            const detailParts = [
                              formatTransferStatus(status, "Estado no disponible"),
                              typeof distance === "number"
                                ? `${distance} tramos orbitales`
                                : null,
                            ].filter((value): value is string => Boolean(value));

                            return (
                              <>
                                <strong>{routeLabel}</strong>
                                <br />
                                <span className="figma-panel-note">
                                  {detailParts.join(" · ")}. Revisa Flotas para el
                                  detalle completo.
                                </span>
                              </>
                            );
                          })()}
                        </li>
                      ))}
                    </ul>
                  ) : (
                    <p className="figma-panel-note">
                      No hay rutas activas visibles en este sistema. Si detectas una
                      salida parcial, revisa Flotas para confirmar origen, destino y
                      estado.
                    </p>
                  )}
                  <div className="selection-chip-row">
                    <Link
                      className="selection-chip"
                      to={buildFleetsUrl(
                        cockpitResult.civilizationId,
                        selectedPlanet?.planetId ?? selectedSystem?.planets?.[0]?.planetId ?? null,
                      )}
                    >
                      Revisar transferencias en Flotas
                    </Link>
                  </div>
                </div>
              )}
            </aside>
          </div>
        </UiCard>
      ) : null}

      {result && result.systems.length === 0 && (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Estado vacio de desarrollo</p>
              <h3>No hay sistemas relevantes por ahora</h3>
              <p>
                La civilizacion actual no tiene sistemas propios ni descubiertos en
                este conjunto de desarrollo, por lo que Galaxia permanece en un
                estado cero seguro.
              </p>
            </div>
            <UiBadge tone="warn">0 sistemas visibles</UiBadge>
          </div>
          <ul className="stack-list">
            <li>Los contadores de sistemas, planetas, flotas y transferencias permanecen en cero.</li>
            <li>La vista del mapa es solo visual y no crea datos.</li>
          </ul>
        </UiCard>
      )}

      {result && selectedSystem && (
        <div className="strategic-system-layout">
          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Navegacion estelar</p>
                <h3>Foco del sistema</h3>
                <p>Cambia de sistema desde la banda operativa y deja el detalle tecnico en segundo plano.</p>
              </div>
              <UiBadge tone="warn">Seleccion de lectura</UiBadge>
            </div>

            <div className="selection-chip-row">
              {result.systems.map((system) => (
                <button
                  key={system.systemId}
                  type="button"
                  className={`selection-chip${selectedSystem.systemId === system.systemId ? " selection-chip-active" : ""}`}
                  onClick={() => selectSystem(system)}
                  aria-pressed={selectedSystem.systemId === system.systemId}
                >
                  {system.systemName ?? "Sistema desconocido"}
                </button>
              ))}
            </div>

            <div className="figma-detail-grid strategic-detail-grid">
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Resumen del sistema</p>
                    <h4>{selectedSystem.systemName ?? "Sistema desconocido"}</h4>
                    <p>La lectura principal prioriza posicion, control y presencia operativa sobre los metadatos de implementacion.</p>
                  </div>
                  <UiBadge tone={getVisibilityTone(selectedSystem.visibilityLevel)}>
                    {getTargetVisibilityLabel(
                      selectedSystem.visibilityLevel,
                      Boolean(selectedSystemRecord?.isOwnedByRequestingCivilization),
                    )}
                  </UiBadge>
                </div>
                <div className="figma-data-list">
                  <DataRow
                    label="Coordenadas"
                    value={`${formatCoordinate(selectedSystem.coordinateX)}, ${formatCoordinate(selectedSystem.coordinateY)}, ${formatCoordinate(selectedSystem.coordinateZ)}`}
                  />
                  {readDomainValue(selectedSystemRecord ?? {}, "starType") && (
                    <DataRow
                      label="Tipo estelar"
                      value={formatStarType(
                        readDomainValue(selectedSystemRecord ?? {}, "starType"),
                      )}
                    />
                  )}
                  <DataRow
                    label="Control"
                    value={
                      Boolean(selectedSystemRecord?.isOwnedByRequestingCivilization)
                        ? "Sistema propio"
                        : getIntelligenceLevelLabel(
                            selectedSystem.visibilityLevel,
                            selectedSystem.visibilityReason,
                          )
                    }
                  />
                  <DataRow
                    label="Observacion"
                    value={getObservationStatusLabel({
                      visibilityLevel: selectedSystem.visibilityLevel,
                      visibilityReason: selectedSystem.visibilityReason,
                      sensorCount: selectedSystem.sensorProfiles?.length ?? 0,
                      detectionCount: selectedSystem.detectionCoverage?.length ?? 0,
                      transferCount: selectedSystem.transferOverlays?.length ?? 0,
                    })}
                  />
                  <DataRow
                    label="Confianza"
                    value={getIntelConfidenceLabel({
                      visibilityLevel: selectedSystem.visibilityLevel,
                      sensorCount: selectedSystem.sensorProfiles?.length ?? 0,
                      detectionCount: selectedSystem.detectionCoverage?.length ?? 0,
                    })}
                  />
                  <DataRow
                    label="Cobertura"
                    value={formatIntelCoverage({
                      sensorCount: selectedSystem.sensorProfiles?.length ?? 0,
                      detectionCount: selectedSystem.detectionCoverage?.length ?? 0,
                      transferCount: selectedSystem.transferOverlays?.length ?? 0,
                    })}
                  />
                  <DataRow
                    label="Presencia operativa"
                    value={`${selectedSystem.planets?.length ?? 0} planetas | ${selectedSystem.fleetPresence?.length ?? 0} flotas | ${selectedSystem.transferOverlays?.length ?? 0} rutas`}
                  />
                </div>
                <details className="json-details">
                  <summary>Diagnostico tecnico del sistema</summary>
                  <div className="figma-data-list">
                    <DataRow
                      label="Id breve"
                      value={formatCompactGuid(selectedSystem.systemId)}
                    />
                    <DataRow
                      label="Motivo de visibilidad"
                      value={formatVisibilityReason(selectedSystem.visibilityReason)}
                    />
                    <DataRow
                      label="Lecturas tacticas"
                      value={String(
                        (selectedSystem.sensorProfiles?.length ?? 0) +
                          (selectedSystem.detectionCoverage?.length ?? 0),
                      )}
                    />
                  </div>
                </details>
              </section>

              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Estado operativo</p>
                    <h4>Acciones del sistema</h4>
                  </div>
                  <UiBadge>{systemCommands.length} lecturas</UiBadge>
                </div>
                {systemCommands.length > 0 ? (
                  <div className="figma-command-list">
                    {systemCommands.map((command) => (
                      <div key={command.actionKey} className="figma-command-item">
                        <div>
                          <strong>{formatStrategicCommandLabel(command.actionKey)}</strong>
                          <p>
                            {formatStrategicCommandSummary(
                              command.actionKey,
                              command.isAvailable,
                              command.blockReason,
                            )}
                          </p>
                        </div>
                        <UiBadge tone={command.isAvailable ? "good" : "warn"}>
                          {command.isAvailable
                            ? "Disponible"
                            : formatCommandBlockReason(command.blockReason, "Bloqueado")}
                        </UiBadge>
                      </div>
                    ))}
                  </div>
                ) : (
                  <p className="figma-panel-note">
                    No hay metadatos de acciones visibles para este sistema.
                  </p>
                )}
              </section>
            </div>
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Inteligencia planetaria</p>
                <h3>Colonias y mundos</h3>
                <p>La lista planetaria deja claro que mundos puedes inspeccionar ahora y a que cabina de gestion podras saltar despues.</p>
              </div>
              <UiBadge>{selectedSystem.planets?.length ?? 0} planetas</UiBadge>
            </div>

            <div className="selection-chip-row">
              {selectedSystem.planets?.map((planet) => (
                <button
                  key={planet.planetId}
                  type="button"
                  className={`selection-chip${selectedPlanet?.planetId === planet.planetId ? " selection-chip-active" : ""}`}
                  onClick={() => {
                    setSelectedPlanetId(planet.planetId);
                    setPlanetVisualState(null);
                    setPlanetVisualError(null);
                    if (result?.civilizationId && selectedSystem) {
                      const nextParams = new URLSearchParams();
                      nextParams.set("civilizationId", result.civilizationId);
                      nextParams.set("systemId", selectedSystem.systemId);
                      nextParams.set("planetId", planet.planetId);
                      setSearchParams(nextParams, { replace: true });
                    }
                  }}
                  aria-pressed={selectedPlanet?.planetId === planet.planetId}
                  title={planet.planetId}
                >
                  {planet.planetName ?? "Planeta desconocido"} -{" "}
                  {formatPlanetType(
                    readDomainValue(readRecord(planet), "planetType"),
                    "Tipo pendiente",
                  )}{" "}
                  -{" "}
                  {Boolean(readRecord(planet).isOwnedByRequestingCivilization)
                    ? "Propio"
                    : formatVisibilityLevel(planet.visibilityLevel)}{" "}
                  -{" "}
                  {formatColonizationStatus(
                    readDomainValue(readRecord(planet), "colonizationStatus"),
                    "Colonizacion pendiente",
                  )}
                </button>
              ))}
            </div>

            {selectedPlanet ? (
              <div className="figma-detail-grid strategic-detail-grid">
                <section className="subpanel figma-subpanel">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">Planeta seleccionado</p>
                      <h4>{selectedPlanet.planetName ?? "Planeta desconocido"}</h4>
                      <p>El foco principal muestra tipo, control y estado colonial antes que claves de diagnostico.</p>
                    </div>
                    <UiBadge tone={getVisibilityTone(selectedPlanet.visibilityLevel)}>
                      {getTargetVisibilityLabel(
                        selectedPlanet.visibilityLevel,
                        Boolean(selectedPlanetRecord?.isOwnedByRequestingCivilization),
                      )}
                    </UiBadge>
                  </div>
                  <div className="figma-data-list">
                    <DataRow
                      label="Estado"
                      value={getIntelligenceLevelLabel(
                        selectedPlanet.visibilityLevel,
                        selectedPlanet.visibilityReason,
                      )}
                    />
                    <DataRow
                      label="Observacion"
                      value={getObservationStatusLabel({
                        visibilityLevel: selectedPlanet.visibilityLevel,
                        visibilityReason: selectedPlanet.visibilityReason,
                        sensorCount: readArrayLength(selectedPlanetRecord ?? {}, "sensorProfiles"),
                        detectionCount: readArrayLength(
                          selectedPlanetRecord ?? {},
                          "detectionCoverage",
                        ),
                      })}
                    />
                    <DataRow
                      label="Confianza"
                      value={getIntelConfidenceLabel({
                        visibilityLevel: selectedPlanet.visibilityLevel,
                        sensorCount: readArrayLength(selectedPlanetRecord ?? {}, "sensorProfiles"),
                        detectionCount: readArrayLength(
                          selectedPlanetRecord ?? {},
                          "detectionCoverage",
                        ),
                      })}
                    />
                    <DataRow
                      label="Tipo"
                      value={formatPlanetType(readDomainValue(selectedPlanetRecord ?? {}, "planetType"))}
                    />
                    <DataRow
                      label="Colonizacion"
                      value={formatColonizationStatus(
                        readDomainValue(selectedPlanetRecord ?? {}, "colonizationStatus"),
                      )}
                    />
                    <DataRow
                      label="Acceso"
                      value={
                        selectedPlanet.isVisible
                          ? formatIntelCoverage({
                              sensorCount: readArrayLength(
                                selectedPlanetRecord ?? {},
                                "sensorProfiles",
                              ),
                              detectionCount: readArrayLength(
                                selectedPlanetRecord ?? {},
                                "detectionCoverage",
                              ),
                            })
                          : "Objetivo fuera de alcance"
                      }
                    />
                  </div>
                  <div className="selection-chip-row">
                    <Link
                      className="selection-chip selection-chip-active"
                      to={buildFleetsUrl(result.civilizationId, selectedPlanet.planetId)}
                    >
                      Ir a Flotas
                    </Link>
                    <Link
                      className="selection-chip"
                      to={buildConstructionUrl(result.civilizationId, selectedPlanet.planetId)}
                    >
                      Abrir Construccion
                    </Link>
                    <Link
                      className="selection-chip"
                      to={buildPlanetUrl(result.civilizationId, selectedPlanet.planetId)}
                    >
                      Abrir Planeta
                    </Link>
                  </div>
                  <details className="json-details">
                    <summary>Diagnostico tecnico del planeta</summary>
                    <div className="figma-data-list">
                      <DataRow
                        label="Id breve"
                        value={formatCompactGuid(selectedPlanet.planetId)}
                      />
                      <DataRow
                        label="Motivo de visibilidad"
                        value={formatVisibilityReason(selectedPlanet.visibilityReason)}
                      />
                      <DataRow
                        label="Metadatos de mando"
                        value={`${planetCommands.length} lecturas disponibles`}
                      />
                    </div>
                  </details>
                </section>

                <section className="subpanel figma-subpanel">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">Consejo planetario</p>
                      <h4>Opciones de superficie</h4>
                    </div>
                    <UiBadge>{planetCommands.length} lecturas</UiBadge>
                  </div>
                  {planetCommands.length > 0 ? (
                    <div className="figma-command-list">
                      {planetCommands.map((command) => (
                        <div key={command.actionKey} className="figma-command-item">
                          <div>
                            <strong>{formatStrategicCommandLabel(command.actionKey)}</strong>
                            <p>
                              {formatStrategicCommandSummary(
                                command.actionKey,
                                command.isAvailable,
                                command.blockReason,
                              )}
                            </p>
                          </div>
                          <UiBadge tone={command.isAvailable ? "good" : "warn"}>
                            {command.isAvailable
                              ? "Disponible"
                              : formatCommandBlockReason(command.blockReason, "Bloqueado")}
                          </UiBadge>
                        </div>
                      ))}
                    </div>
                  ) : (
                    <p className="figma-panel-note">
                      No hay metadatos de acciones visibles para este mundo.
                    </p>
                  )}
                </section>
              </div>
            ) : (
              <p>No hay planetas disponibles para el sistema seleccionado.</p>
            )}
          </UiCard>
        </div>
      )}

      {cockpitResult && !selectedSystem && cockpitResult.systems.length > 0 && (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Sin sistema activo</p>
              <h3>Selecciona un sistema del mapa</h3>
              <p>La vista tactica esta lista, pero todavia no hay un foco de inspeccion activo.</p>
            </div>
            <UiBadge tone="warn">Sin seleccion</UiBadge>
          </div>
        </UiCard>
      )}

      {shouldRenderDiagnostics ? (
        <details className="technical-disclosure">
          <summary>
            <div>
              <p className="eyebrow">Diagnostico secundario</p>
              <strong>Resumen de estado y lecturas tecnicas</strong>
            </div>
            <UiBadge tone="warn">Contraido por defecto</UiBadge>
          </summary>

          <div className="technical-disclosure-body">
            <UiCard className="panel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Resumen de estado</p>
                  <h3>Contexto actual de Galaxia</h3>
                  <p>
                    Esta lectura compacta ayuda a distinguir contexto, carga,
                    seleccion y densidad del mapa sin abrir todos los datos tecnicos.
                  </p>
                </div>
                <UiBadge>{loadStatusLabel}</UiBadge>
              </div>
              <div className="figma-detail-grid strategic-detail-grid">
                <section className="subpanel figma-subpanel">
                  <div className="figma-data-list">
                    <DataRow
                      label="Civilizacion"
                      value={
                        queryCivilizationId
                          ? formatCompactGuid(queryCivilizationId)
                          : "Sin contexto"
                      }
                    />
                    <DataRow label="Estado de carga" value={loadStatusLabel} />
                    <DataRow
                      label="Sistemas visibles"
                      value={String(summary?.systems ?? result?.systems.length ?? 0)}
                    />
                    <DataRow
                      label="Planetas visibles"
                      value={String(summary?.planets ?? 0)}
                    />
                    <DataRow
                      label="Marcadores de flota"
                      value={String(summary?.fleets ?? 0)}
                    />
                    <DataRow
                      label="Rutas visibles"
                      value={String(summary?.transfers ?? 0)}
                    />
                  </div>
                </section>

                <section className="subpanel figma-subpanel">
                  <div className="figma-data-list">
                    <DataRow
                      label="Sistema enfocado"
                      value={selectedSystem?.systemName ?? "Sin seleccion"}
                    />
                    <DataRow
                      label="Ref. sistema"
                      value={
                        selectedSystem?.systemId
                          ? formatCompactGuid(selectedSystem.systemId)
                          : "Sin sistema"
                      }
                    />
                    <DataRow
                      label="Planeta enfocado"
                      value={selectedPlanet?.planetName ?? "Sin planeta"}
                    />
                    <DataRow
                      label="Ref. planeta"
                      value={
                        selectedPlanet?.planetId
                          ? formatCompactGuid(selectedPlanet.planetId)
                          : "Sin planeta"
                      }
                    />
                  </div>
                  {errorTechnicalDetail ? (
                    <details className="json-details">
                      <summary>Detalle tecnico del ultimo error</summary>
                      <pre className="json-preview">{errorTechnicalDetail}</pre>
                    </details>
                  ) : null}
                </section>
              </div>
            </UiCard>

            {cockpitResult ? (
            <UiCard className="panel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Alcance de preparacion</p>
                  <h3>Exposicion de metadatos</h3>
                  <p>
                    Estas pistas siguen siendo informativas y nunca saltan la
                    validacion del backend.
                  </p>
                </div>
                <UiBadge>Solo inspeccion</UiBadge>
              </div>
              <p className="figma-panel-note">
                Galaxia sigue siendo una cabina de solo lectura: estas vistas previas
                y metadatos nunca ejecutan ordenes, y cualquier accion real debe
                hacerse desde la cabina propietaria.
              </p>
              <div className="figma-detail-grid strategic-detail-grid">
                <section className="subpanel figma-subpanel">
                  <div className="figma-data-list">
                    <DataRow
                      label="Filas de alianzas"
                      value={String(cockpitResult.allianceReadiness?.length ?? 0)}
                    />
                    <DataRow
                      label="Filas de pactos"
                      value={String(cockpitResult.alliancePacts?.length ?? 0)}
                    />
                    <DataRow
                      label="Notas de flota"
                      value={String(cockpitResult.interceptionNotes?.length ?? 0)}
                    />
                    <DataRow
                      label="Notas de ruta y combustible"
                      value={String(cockpitResult.routeFuelNotes?.length ?? 0)}
                    />
                  </div>
                </section>

                <section className="subpanel figma-subpanel">
                  <h4>Anotaciones de preparacion</h4>
                  <div className="readiness-grid strategic-readiness-grid">
                    {readinessSections.map((section) => {
                      const notes = cockpitResult[section.key];
                      if (!notes?.length) {
                        return null;
                      }

                      return (
                        <section key={section.key} className="subpanel figma-subpanel">
                          <h4>{section.label}</h4>
                          <ul className="stack-list compact-list">
                            {notes.map((note, index) => (
                              <li key={`${section.key}-${index}`}>{formatNote(note)}</li>
                            ))}
                          </ul>
                        </section>
                      );
                    })}
                  </div>
                </section>
              </div>
            </UiCard>
            ) : null}

            {cockpitResult ? (
            <UiCard className="panel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Datos para vista visual</p>
                  <h3>Vista previa del estado visual</h3>
                  <p>
                    Estas lecturas de desarrollo siguen disponibles para el trabajo
                    del renderer, pero ya no compiten con el flujo principal del
                    mapa.
                  </p>
                </div>
                <UiBadge tone="warn">Datos tecnicos</UiBadge>
              </div>

              <div className="figma-detail-grid strategic-detail-grid">
                <section className="subpanel figma-subpanel">
                  <div className="figma-preview-actions">
                    <div>
                      <p className="eyebrow">Vista del sistema</p>
                      <h4>Estado visual del sistema</h4>
                    </div>
                    <button
                      type="button"
                      className="selection-chip"
                      onClick={loadSystemVisualState}
                      disabled={!selectedSystem || isLoadingSystemVisual}
                    >
                      {isLoadingSystemVisual ? "Cargando..." : "Cargar estado visual del sistema"}
                    </button>
                  </div>
                  {systemVisualError && <p className="error-text">{systemVisualError}</p>}
                  {systemVisualState && (
                    <>
                      <div className="figma-data-list">
                        <DataRow
                          label="Clase estelar"
                          value={readText(systemVisualState.star?.visualClass)}
                        />
                        <DataRow
                          label="Tipo de estrella"
                          value={formatStarType(systemVisualState.star?.starType)}
                        />
                        <DataRow
                          label="Pistas de disposicion"
                          value={String(systemVisualState.layoutHints?.length ?? 0)}
                        />
                        <DataRow
                          label="Rutas activas"
                          value={String(systemVisualState.transferOverlays?.length ?? 0)}
                        />
                      </div>
                      <details className="json-details">
                        <summary>Payload crudo del sistema</summary>
                        <pre className="json-preview">{formatJson(systemVisualState)}</pre>
                      </details>
                    </>
                  )}
                </section>

                <section className="subpanel figma-subpanel">
                  <div className="figma-preview-actions">
                    <div>
                      <p className="eyebrow">Vista del planeta</p>
                      <h4>Estado visual del planeta</h4>
                    </div>
                    <button
                      type="button"
                      className="selection-chip"
                      onClick={loadPlanetVisualState}
                      disabled={!selectedPlanet || isLoadingPlanetVisual}
                    >
                      {isLoadingPlanetVisual ? "Cargando..." : "Cargar estado visual del planeta"}
                    </button>
                  </div>
                  {!selectedPlanet?.isVisible && (
                    <p>Solo los planetas visibles deben inspeccionarse desde esta vista previa.</p>
                  )}
                  {planetVisualError && <p className="error-text">{planetVisualError}</p>}
                  {planetVisualState && (
                    <>
                      <div className="figma-data-list">
                        <DataRow
                          label="Nombre del planeta"
                          value={readText(planetVisualState.planetName)}
                        />
                        <DataRow
                          label="Tipo de planeta"
                          value={formatPlanetType(planetVisualState.planetType)}
                        />
                        <DataRow
                          label="Colonization"
                          value={formatColonizationStatus(planetVisualState.colonizationStatus)}
                        />
                        <DataRow
                          label="Semilla visual"
                          value={String(planetVisualState.visualSeed ?? "No disponible")}
                        />
                        <DataRow
                          label="Perfil de superficie"
                          value={readText(planetVisualState.profile?.surfaceProfile)}
                        />
                        <DataRow
                          label="Distribucion de luz"
                          value={readText(planetVisualState.profile?.lightDistributionMode)}
                        />
                        <DataRow
                          label="Modo de plataformas"
                          value={readText(planetVisualState.profile?.platformMode)}
                        />
                        <DataRow
                          label="Perfil atmosferico"
                          value={readText(planetVisualState.profile?.atmosphereProfile)}
                        />
                        <DataRow
                          label="Perfil de nubes"
                          value={readText(planetVisualState.profile?.cloudProfile)}
                        />
                        <DataRow
                          label="Luces nocturnas"
                          value={formatBooleanLabel(Boolean(planetVisualState.profile?.supportsNightLights))}
                        />
                        <DataRow
                          label="Plataformas de superficie"
                          value={formatBooleanLabel(
                            Boolean(planetVisualState.profile?.supportsSurfacePlatforms),
                          )}
                        />
                        <DataRow
                          label="Pistas de megastructuras orbitales"
                          value={formatBooleanLabel(
                            Boolean(
                              planetVisualState.profile?.supportsOrbitalMegastructureHints,
                            ),
                          )}
                        />
                        {planetVisualState.profile?.paletteKey && (
                          <DataRow
                            label="Perfil de paleta"
                            value={readText(planetVisualState.profile.paletteKey)}
                          />
                        )}
                      </div>
                      <div className="readiness-grid">
                        <section className="subpanel figma-subpanel">
                          <h4>Intensidades</h4>
                          <div className="stack-list compact-list">
                            <IntensityRow
                              label="Intensidad de colonizacion"
                              value={planetVisualState.colonizationIntensity}
                            />
                            <IntensityRow
                              label="Intensidad urbana"
                              value={planetVisualState.urbanIntensity}
                            />
                            <IntensityRow
                              label="Intensidad industrial"
                              value={planetVisualState.industrialIntensity}
                            />
                            <IntensityRow
                              label="Intensidad de terraformacion"
                              value={planetVisualState.terraformingIntensity}
                            />
                            <IntensityRow
                              label="Intensidad militar"
                              value={planetVisualState.militaryIntensity}
                            />
                            <IntensityRow
                              label="Intensidad de presencia orbital"
                              value={planetVisualState.orbitalPresenceIntensity}
                            />
                          </div>
                        </section>
                      </div>
                      <details className="json-details">
                        <summary>Payload crudo del planeta</summary>
                        <pre className="json-preview">{formatJson(planetVisualState)}</pre>
                      </details>
                    </>
                  )}
                </section>
              </div>
            </UiCard>
            ) : null}
          </div>
        </details>
      ) : null}
    </section>
  );
}
