import { FormEvent, useEffect, useMemo, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { fetchEspionageUiState } from "../api/espionageApi";
import { CockpitHero } from "../components/CockpitHero";
import { PageContextStrip } from "../components/PageContextStrip";
import type { EspionageViewModel, IntelligenceSystemTargetGroup, IntelligenceTargetViewModel } from "../utils/espionageViewModel";
import { getEspionagePrimaryAction, mapEspionageUiStateToViewModel } from "../utils/espionageViewModel";
import {
  formatEspionageEmptyState,
  formatEspionageRequestFailure,
  getEspionageActionLabel,
  getEspionageCueDescription,
  getEspionageFutureActionReasonLabel,
  getEspionageMissingDataNote,
} from "../utils/espionagePresentation";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import {
  buildDevelopmentHelperUrl,
  buildFleetsUrl,
  buildGalaxyUrl,
  buildPlanetUrl,
  buildResearchUrl,
  isSuspiciousCabinContext,
} from "../utils/routeUrls";
import { formatCompactGuid } from "../utils/domainPresentation";
import { isOperatorMode } from "../utils/playableSession";

function pickFocusedGroup(groups: readonly IntelligenceSystemTargetGroup[], systemId: string | null, recommendedTarget: EspionageViewModel["recommendedTarget"]) {
  return groups.find((group) => group.systemId === systemId)
    ?? groups.find((group) => group.systemId === recommendedTarget?.systemId)
    ?? groups[0]
    ?? null;
}

function pickFocusedTarget(group: IntelligenceSystemTargetGroup | null, planetId: string | null, recommendedTarget: EspionageViewModel["recommendedTarget"]) {
  return group?.targets.find((target) => target.planetId === planetId)
    ?? group?.targets.find((target) => target.planetId === recommendedTarget?.planetId)
    ?? group?.targets[0]
    ?? null;
}

function getCoverageOverview(viewModel: EspionageViewModel | null) {
  if (!viewModel) return "Sin red";
  if (viewModel.summary.partialTargetCount >= viewModel.summary.visibleTargetCount) return "Cobertura parcial";
  if (viewModel.summary.passiveSignalCount === 0) return "Cobertura limitada";
  return "Cobertura amplia";
}

function getReadinessPosture(viewModel: EspionageViewModel | null) {
  if (!viewModel) return "Esperando red";
  if (viewModel.summary.partialTargetCount > 0 || viewModel.summary.passiveSignalCount > 0) return "Preparacion activa";
  if (viewModel.summary.visibleTargetCount > 0 || viewModel.summary.ownedTargetCount > 0) return "Red estable";
  return "Cobertura minima";
}

function formatEspionageProductLabel(label: string) {
  return label
    .replace(/solo\s+lect\S+/gi, "consulta de inteligencia")
    .replace(/lectura\s+de\s+inteligencia/gi, "red de inteligencia")
    .replace(/lectura/gi, "senal")
    .replace(/desarrollo/gi, "fase actual")
    .replace(/persistencia/gi, "estado")
    .replace(/acci\S+/gi, "operacion");
}

interface IntelligenceCatalogEntry {
  id: string;
  title: string;
  subtitle: string;
  typeLabel: string;
  statusLabel: string;
  statusTone: "neutral" | "good" | "warn";
  confidenceLabel: string;
  coverageLabel: string;
  handoffLabel: string;
  controlLabel: string | null;
  signalLabel: string;
}

interface IntelligenceCatalogSection {
  key: "owned" | "observed" | "partial" | "signal" | "unconfirmed";
  label: string;
  description: string;
  tone: "neutral" | "good" | "warn";
  entries: IntelligenceCatalogEntry[];
}

function isOwnedTarget(target: IntelligenceTargetViewModel) {
  return target.diagnostics.visibilityLevel === "Owned" || target.diagnostics.visibilityLevel === "2";
}

function isVisibleTarget(target: IntelligenceTargetViewModel) {
  return target.diagnostics.visibilityLevel === "Visible" || target.diagnostics.visibilityLevel === "1";
}

function getTargetTone(target: IntelligenceTargetViewModel): "neutral" | "good" | "warn" {
  return isOwnedTarget(target) ? "good" : isVisibleTarget(target) ? "neutral" : "warn";
}

function getTargetTypeLabel(target: IntelligenceTargetViewModel) {
  return target.kind === "System" ? "Sistema" : target.kind === "Planet" ? "Planeta" : "Objetivo";
}

function getTargetStatusLabel(target: IntelligenceTargetViewModel) {
  if (isOwnedTarget(target)) return "Control confirmado";
  if (isVisibleTarget(target)) return "Senal estable";
  return target.hasPassiveSignals ? "Contacto parcial" : "Contacto sin confirmar";
}

function getTargetHandoffLabel(target: IntelligenceTargetViewModel) {
  if (target.planetId && isOwnedTarget(target)) return "Abrir planeta";
  if (target.planetId && isVisibleTarget(target)) return "Revisar en Galaxia";
  if (target.planetId) return "Seguir desde Galaxia";
  return "Volver al mapa";
}

function getSignalLabel(signal: IntelligenceSystemTargetGroup["signals"][number]) {
  switch (signal.label) {
    case "SensorProfile":
      return "Perfil de sensores";
    case "DetectionCoverage":
      return "Cobertura de deteccion";
    case "TransferSignal":
      return "Trayectoria orbital";
    default:
      return "Senal orbital";
  }
}

function buildCatalogSections(groups: readonly IntelligenceSystemTargetGroup[]): IntelligenceCatalogSection[] {
  const sections: IntelligenceCatalogSection[] = [
    { key: "owned", label: "Sistema propio", description: "Colonias y sistemas confirmados bajo control propio.", tone: "good", entries: [] },
    { key: "observed", label: "Sistema observado", description: "Objetivos visibles o conocidos sin exagerar la certeza.", tone: "neutral", entries: [] },
    { key: "partial", label: "Contacto parcial", description: "Senales parciales con contexto suficiente para seguir observando.", tone: "warn", entries: [] },
    { key: "signal", label: "Senal orbital", description: "Pistas pasivas y trayectorias todavia no elevadas a objetivo confirmado.", tone: "warn", entries: [] },
    { key: "unconfirmed", label: "Sin confirmar", description: "Contactos sin evidencia estable y con datos incompletos.", tone: "warn", entries: [] },
  ];

  groups.forEach((group) => {
    group.targets.forEach((target) => {
      const key = isOwnedTarget(target)
        ? "owned"
        : isVisibleTarget(target)
          ? "observed"
          : target.hasPassiveSignals
            ? "partial"
            : "unconfirmed";
      sections.find((section) => section.key === key)?.entries.push({
        id: `${target.systemId}-${target.planetId ?? target.kind}`,
        title: target.label,
        subtitle: target.systemLabel,
        typeLabel: getTargetTypeLabel(target),
        statusLabel: getTargetStatusLabel(target),
        statusTone: getTargetTone(target),
        confidenceLabel: target.confidenceLabel,
        coverageLabel: target.coverageLabel,
        handoffLabel: getTargetHandoffLabel(target),
        controlLabel: isOwnedTarget(target) ? "Control propio" : null,
        signalLabel: target.hasPassiveSignals ? target.observationLabel : "Datos incompletos",
      });
    });

    group.signals.forEach((signal) => {
      sections.find((section) => section.key === "signal")?.entries.push({
        id: `${group.systemId}-${signal.planetId ?? signal.label}-${signal.summary}`,
        title: getSignalLabel(signal),
        subtitle: group.label,
        typeLabel: "Senal orbital",
        statusLabel: "Datos incompletos",
        statusTone: "warn",
        confidenceLabel: "Senal pasiva",
        coverageLabel: signal.summary,
        handoffLabel: signal.planetId ? "Revisar en Galaxia" : "Volver al mapa",
        controlLabel: null,
        signalLabel: signal.summary,
      });
    });
  });

  return sections.filter((section) => section.entries.length > 0);
}

const intelligenceLegend: Array<{
  key: "owned" | "observed" | "partial" | "signal" | "unconfirmed";
  label: string;
  badge: string;
  tone: "neutral" | "good" | "warn";
}> = [
  { key: "owned", label: "Propio / control directo", badge: "Confirmado", tone: "good" },
  { key: "observed", label: "Visible / observado", badge: "Senal estable", tone: "neutral" },
  { key: "partial", label: "Contacto parcial", badge: "Datos incompletos", tone: "warn" },
  { key: "signal", label: "Senal orbital", badge: "Senal pasiva", tone: "warn" },
  { key: "unconfirmed", label: "Sin confirmar", badge: "Sin evidencia estable", tone: "warn" },
];

const handoffCards = [
  {
    key: "galaxy",
    label: "Galaxia",
    title: "Vista estrategica",
    description: "Mantiene el mapa, el contexto del sistema y la siguiente comparacion visible.",
  },
  {
    key: "planet",
    label: "Planeta",
    title: "Detalle planetario",
    description: "Reune la superficie propia, las reservas y el estado local del mundo enfocado.",
  },
  {
    key: "fleets",
    label: "Flotas",
    title: "Movimiento orbital",
    description: "Permite revisar grupos orbitales, rutas y transferencias sin convertir Espionaje en una vista de mando.",
  },
  {
    key: "research",
    label: "Investigacion",
    title: "Progreso tecnologico",
    description: "Muestra el progreso tecnologico relacionado con futuras mejoras de sensores e inteligencia.",
  },
] as const;

const futureMissionKeys = [
  "espionage.reconnaissance.create",
  "espionage.infiltration.create",
  "espionage.sabotage.create",
  "espionage.counterintelligence.create",
  "espionage.technologyTheft.create",
] as const;

export function EspionagePage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [civilizationIdInput, setCivilizationIdInput] = useState(searchParams.get("civilizationId") ?? "");
  const [systemIdInput, setSystemIdInput] = useState(searchParams.get("systemId") ?? "");
  const [planetIdInput, setPlanetIdInput] = useState(searchParams.get("planetId") ?? "");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [technicalErrorDetail, setTechnicalErrorDetail] = useState<string | null>(null);
  const [viewModel, setViewModel] = useState<EspionageViewModel | null>(null);

  const queryCivilizationId = searchParams.get("civilizationId") ?? "";
  const querySystemId = searchParams.get("systemId");
  const queryPlanetId = searchParams.get("planetId");
  const operatorMode = isOperatorMode(searchParams);
  const isSuspiciousContext = isSuspiciousCabinContext(queryCivilizationId, queryPlanetId);

  function presentEspionageFailure(failure: ReturnType<typeof formatEspionageRequestFailure>) {
    setError(formatEspionageProductLabel(failure.primaryMessage));
    setTechnicalErrorDetail(failure.technicalDetail);
  }

  useEffect(() => {
    setCivilizationIdInput(queryCivilizationId);
    setSystemIdInput(querySystemId ?? "");
    setPlanetIdInput(queryPlanetId ?? "");

    async function load() {
      if (!queryCivilizationId) {
        setViewModel(null);
        setError(null);
        setTechnicalErrorDetail(null);
        return;
      }

      setIsLoading(true);
      setError(null);

      try {
        const response = await fetchEspionageUiState(queryCivilizationId);
        if (!response.succeeded || !response.uiState) {
          const failure = formatEspionageRequestFailure(response.errors[0] ?? null);
          setViewModel(null);
          presentEspionageFailure(failure);
          return;
        }

        const nextViewModel = mapEspionageUiStateToViewModel(response.uiState);
        if (nextViewModel.groups.length === 0 && nextViewModel.passiveSignalEntries.length === 0) {
          setError(formatEspionageEmptyState(false));
        } else {
          setError(null);
        }
        setTechnicalErrorDetail(null);
        setViewModel(nextViewModel);
      } catch (requestError) {
        const failure = formatEspionageRequestFailure(requestError instanceof Error ? requestError.message : null);
        setViewModel(null);
        presentEspionageFailure(failure);
      } finally {
        setIsLoading(false);
      }
    }

    void load();
  }, [queryCivilizationId, querySystemId, queryPlanetId]);

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const trimmedCivilizationId = civilizationIdInput.trim();
    if (!trimmedCivilizationId) {
      const failure = formatEspionageRequestFailure("Civilization id is required.");
      presentEspionageFailure(failure);
      setViewModel(null);
      return;
    }

    const nextParams = new URLSearchParams();
    nextParams.set("civilizationId", trimmedCivilizationId);
    if (systemIdInput.trim()) nextParams.set("systemId", systemIdInput.trim());
    if (planetIdInput.trim()) nextParams.set("planetId", planetIdInput.trim());
    setSearchParams(nextParams);
  }

  const focusedGroup = useMemo(
    () => pickFocusedGroup(viewModel?.groups ?? [], querySystemId, viewModel?.recommendedTarget ?? null),
    [querySystemId, viewModel],
  );
  const focusedTarget = useMemo(
    () => pickFocusedTarget(focusedGroup, queryPlanetId, viewModel?.recommendedTarget ?? null),
    [focusedGroup, queryPlanetId, viewModel],
  );
  const activePlanetId = focusedTarget?.planetId ?? queryPlanetId ?? null;
  const coverageOverview = getCoverageOverview(viewModel);
  const readinessPosture = getReadinessPosture(viewModel);
  const catalogSections = useMemo(() => buildCatalogSections(viewModel?.groups ?? []), [viewModel]);
  const futureMissionCards = useMemo(() => {
    const knownActions = new Map((viewModel?.futureActions ?? []).map((action) => [action.key, action]));
    return futureMissionKeys.map((key) => {
      const action = knownActions.get(key);
      return {
        key,
        label: formatEspionageProductLabel(getEspionageActionLabel(key) ?? "Operacion futura"),
        reason: formatEspionageProductLabel(getEspionageFutureActionReasonLabel(key, action?.reasonLabel)),
      };
    });
  }, [viewModel]);

  return (
    <section className="page-grid">
      <CockpitHero
        versionLabel="Espionaje v1"
        title="Red de inteligencia"
        description="La vista resume lo confirmado, lo observado y lo que sigue incompleto sin prometer operaciones activas."
        developmentNote="La red reutiliza la cobertura estrategica disponible y deja cualquier futura mision pendiente de activacion."
        badges={
          <>
            <UiBadge>{coverageOverview}</UiBadge>
            <UiBadge>Consulta de inteligencia</UiBadge>
            <UiBadge tone="warn">Sin operaciones activas</UiBadge>
          </>
        }
      />

      {queryCivilizationId ? (
        <PageContextStrip
          eyebrow="Vista de inteligencia"
          title={focusedTarget?.label ?? focusedGroup?.label ?? "Red de inteligencia"}
          purpose="Preparacion de objetivos, senales pasivas y handoffs sin habilitar operaciones ni inventar informacion no confirmada."
          statusLabel={readinessPosture}
          statusTone={viewModel ? "good" : "warn"}
          contextItems={[
            { label: "Civilizacion", value: formatCompactGuid(viewModel?.civilizationId ?? queryCivilizationId) },
            {
              label: "Foco",
              value: focusedTarget?.label ?? focusedGroup?.label ?? "Sin objetivo enfocado",
              detail: focusedTarget?.systemLabel ?? focusedGroup?.label,
            },
            {
              label: "Sistemas",
              value: viewModel ? String(viewModel.groups.length) : "Sin red",
              detail: viewModel ? `${viewModel.summary.visibleTargetCount} observados` : "Carga pendiente",
            },
            {
              label: "Senales",
              value: viewModel ? String(viewModel.summary.passiveSignalCount) : "Sin senales",
              detail: coverageOverview,
            },
          ]}
          resourceItems={[
            { label: "Inteligencia", value: "Red visible", tone: "good" },
            { label: "Misiones", value: "Pendientes", tone: "warn" },
            { label: "Datos", value: "Cobertura existente", tone: "neutral" },
          ]}
          primaryAction={
            <div className="selection-chip-row">
              <Link className="selection-chip selection-chip-active" to={buildGalaxyUrl(queryCivilizationId, focusedGroup?.systemId ?? querySystemId, activePlanetId)}>
                Abrir Galaxia
              </Link>
              {activePlanetId ? (
                <Link className="selection-chip" to={buildPlanetUrl(queryCivilizationId, activePlanetId)}>
                  Abrir Planeta
                </Link>
              ) : null}
              <Link className="selection-chip" to={buildResearchUrl(queryCivilizationId, activePlanetId)}>
                Abrir Investigacion
              </Link>
            </div>
          }
        />
      ) : null}

      <div className="strategic-cockpit-top">
        <UiCard className="panel strategic-loader-panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Enlace de inteligencia</p>
              <h3>Cargar cobertura</h3>
            </div>
            <UiBadge>Contexto activo</UiBadge>
          </div>
          <form className="query-form" onSubmit={handleSubmit}>
            <label className="field">
              <span>Id de civilizacion</span>
              <input type="text" value={civilizationIdInput} onChange={(event) => setCivilizationIdInput(event.target.value)} placeholder="00000000-0000-0000-0000-000000000000" spellCheck={false} />
            </label>
            <label className="field">
              <span>Id de sistema opcional</span>
              <input type="text" value={systemIdInput} onChange={(event) => setSystemIdInput(event.target.value)} placeholder="20000000-0000-0000-0000-000000000000" spellCheck={false} />
            </label>
            <label className="field">
              <span>Id de planeta opcional</span>
              <input type="text" value={planetIdInput} onChange={(event) => setPlanetIdInput(event.target.value)} placeholder="40000000-0000-0000-0000-000000000000" spellCheck={false} />
            </label>
            <button type="submit" disabled={isLoading}>{isLoading ? "Cargando..." : "Cargar espionaje"}</button>
          </form>
          {error ? <p className="error-text">{error}</p> : null}
          {operatorMode && technicalErrorDetail ? (
            <details className="json-details">
              <summary>Datos tecnicos del ultimo error</summary>
              <pre className="json-preview">{technicalErrorDetail}</pre>
            </details>
          ) : null}
          {!queryCivilizationId ? <p className="figma-panel-note">Introduce un contexto de civilizacion valido para abrir la vista de inteligencia.</p> : null}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Limite de la vista</p>
              <h3>Red de inteligencia</h3>
            </div>
            <UiBadge tone="warn">Sin ejecucion</UiBadge>
          </div>
          <ul className="stack-list strategic-rules-list">
            <li>Resume objetivos propios, visibles, conocidos y parciales.</li>
            <li>Lee senales pasivas, trayectorias y cobertura ya expuestas por la vista estrategica.</li>
            <li>No crea misiones de espionaje, infiltracion ni sabotaje.</li>
          </ul>
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Estado actual</p>
              <h3>Resumen de cobertura</h3>
            </div>
            <UiBadge>{viewModel ? `${viewModel.summary.passiveSignalCount} senales` : "Sin red"}</UiBadge>
          </div>
          {viewModel ? (
            <>
              <div className="figma-stat-grid">
                <div className="figma-stat"><strong>{viewModel.summary.ownedTargetCount}</strong><span>Confirmados</span></div>
                <div className="figma-stat"><strong>{viewModel.summary.visibleTargetCount}</strong><span>Observados</span></div>
                <div className="figma-stat"><strong>{viewModel.summary.partialTargetCount}</strong><span>Parciales</span></div>
                <div className="figma-stat"><strong>{viewModel.summary.passiveSignalCount}</strong><span>Senales</span></div>
              </div>
              <p className="figma-panel-note">
                Confirmado: control propio o senal asentada. Observado: visibilidad directa o indirecta. Incompleto: contacto que todavia necesita seguimiento.
              </p>
            </>
          ) : (
            <p className="figma-panel-note">La vista mostrara su resumen cuando exista un contexto valido de civilizacion.</p>
          )}
        </UiCard>
      </div>

      <UiCard className="panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Preparacion de producto</p>
            <h3>Frontera de inteligencia</h3>
            <p>Espionaje organiza la red disponible para futuras operaciones, pero no ejecuta misiones encubiertas ni consolida un modelo final de inteligencia.</p>
          </div>
          <UiBadge tone="warn">Fase futura</UiBadge>
        </div>
        <div className="readiness-grid">
          <section className="subpanel figma-subpanel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Entrada actual</p>
                <h4>Cobertura reutilizada</h4>
              </div>
              <UiBadge>{viewModel ? "Cargada" : "Pendiente"}</UiBadge>
            </div>
            <p className="figma-panel-note">
              La vista usa visibilidad estrategica, senales pasivas y objetivos ya expuestos por la cobertura actual.
            </p>
          </section>
          <section className="subpanel figma-subpanel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Operaciones</p>
                <h4>Misiones no ejecutables</h4>
              </div>
              <UiBadge tone="warn">Bloqueadas</UiBadge>
            </div>
            <p className="figma-panel-note">
              Reconocimiento activo, infiltracion, sabotaje, robo de tecnologia y contraespionaje permanecen como referencias desactivadas.
            </p>
          </section>
          <section className="subpanel figma-subpanel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Dependencias finales</p>
                <h4>Consolidacion pendiente</h4>
              </div>
              <UiBadge tone="warn">No final</UiBadge>
            </div>
            <p className="figma-panel-note">
              Autoridad final de misiones, permisos de inteligencia, resultados encubiertos, combate y movimiento siguen fuera de esta vista.
            </p>
          </section>
        </div>
      </UiCard>

      <UiCard className="panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Niveles de inteligencia</p>
            <h3>Como leer la inteligencia</h3>
            <p>Los datos incompletos no desbloquean acciones ofensivas. La red depende de la visibilidad estrategica actual.</p>
          </div>
          <UiBadge tone="warn">Sin precision falsa</UiBadge>
        </div>
        <div className="espionage-legend-grid">
          {intelligenceLegend.map((item) => (
            <article key={item.key} className={`subpanel figma-subpanel espionage-legend-card espionage-target-card-${item.tone}`}>
              <div className="espionage-target-card-head">
                <div>
                  <p className="eyebrow">{item.label}</p>
                  <h4>{item.badge}</h4>
                </div>
                <UiBadge tone={item.tone}>{item.badge}</UiBadge>
              </div>
              <p className="figma-panel-note">{getEspionageCueDescription(item.key)}</p>
            </article>
          ))}
        </div>
      </UiCard>

      {operatorMode && isSuspiciousContext ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Contexto sospechoso</p>
              <h3>El identificador de civilizacion no parece valido para esta vista.</h3>
            </div>
            <UiBadge tone="warn">Revisar contexto</UiBadge>
          </div>
          <p className="figma-panel-note">Revisa que no hayas usado el id del planeta como civilizacion.</p>
          <div className="selection-chip-row">
            <Link className="selection-chip selection-chip-active" to={buildDevelopmentHelperUrl()}>Abrir contexto de operador</Link>
          </div>
        </UiCard>
      ) : null}

      {viewModel ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Foco recomendado</p>
              <h3>{focusedTarget?.label ?? "Sin objetivo priorizado"}</h3>
              <p>La vista prioriza objetivos parciales con senales pasivas y sugiere donde seguir la investigacion.</p>
            </div>
            <UiBadge tone={focusedTarget?.hasPassiveSignals ? "good" : "warn"}>{getEspionagePrimaryAction(viewModel)}</UiBadge>
          </div>
          <div className="figma-detail-grid strategic-detail-grid">
            <section className="subpanel figma-subpanel">
              <div className="figma-data-list">
                <div className="figma-data-row"><span>Objetivo</span><strong>{focusedTarget?.label ?? "Sin objetivo"}</strong></div>
                <div className="figma-data-row"><span>Visibilidad</span><strong>{focusedTarget?.visibilityLabel ?? "Sin senal"}</strong></div>
                <div className="figma-data-row"><span>Inteligencia</span><strong>{focusedTarget?.intelligenceLabel ?? "Sin senal"}</strong></div>
                <div className="figma-data-row"><span>Confianza</span><strong>{focusedTarget?.confidenceLabel ?? "Sin senal"}</strong></div>
                <div className="figma-data-row"><span>Cobertura</span><strong>{focusedTarget?.coverageLabel ?? "Sin senal"}</strong></div>
              </div>
            </section>
            <section className="subpanel figma-subpanel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Sistema enfocado</p>
                  <h4>{focusedGroup?.label ?? "Sin sistema"}</h4>
                </div>
                <UiBadge>{(focusedGroup?.targets.length ?? 0) + (focusedGroup?.signals.length ?? 0)} senales</UiBadge>
              </div>
              {focusedGroup?.targets.length || focusedGroup?.signals.length ? (
                <div className="espionage-system-list">
                  {focusedGroup.targets.map((target) => (
                    <article key={`${target.systemId}-${target.planetId ?? target.kind}`} className={`espionage-target-card espionage-target-card-${getTargetTone(target)}`}>
                      <div className="espionage-target-card-head">
                        <div>
                          <p className="eyebrow">{getTargetTypeLabel(target)}</p>
                          <h5>{target.label}</h5>
                        </div>
                        <UiBadge tone={getTargetTone(target)}>{getTargetStatusLabel(target)}</UiBadge>
                      </div>
                      <div className="figma-badge-row">
                        <UiBadge>{target.intelligenceLabel}</UiBadge>
                        <UiBadge tone={target.hasPassiveSignals ? "warn" : "neutral"}>{target.observationLabel}</UiBadge>
                      </div>
                      <div className="figma-data-list espionage-data-list">
                        <div className="figma-data-row"><span>Confianza</span><strong>{target.confidenceLabel}</strong></div>
                        <div className="figma-data-row"><span>Cobertura</span><strong>{target.coverageLabel}</strong></div>
                        {isOwnedTarget(target) ? <div className="figma-data-row"><span>Control</span><strong>Control propio</strong></div> : null}
                        <div className="figma-data-row"><span>Siguiente seguimiento</span><strong>{getTargetHandoffLabel(target)}</strong></div>
                      </div>
                      {!isOwnedTarget(target) && !isVisibleTarget(target) ? (
                        <p className="espionage-target-note">{getEspionageMissingDataNote(target.hasPassiveSignals ? "partial" : "unconfirmed")}</p>
                      ) : null}
                    </article>
                  ))}

                  {focusedGroup.signals.map((signal) => (
                    <article key={`${signal.systemId}-${signal.planetId ?? signal.label}-${signal.summary}`} className="espionage-target-card espionage-target-card-warn">
                      <div className="espionage-target-card-head">
                        <div>
                          <p className="eyebrow">Senal orbital</p>
                          <h5>{getSignalLabel(signal)}</h5>
                        </div>
                        <UiBadge tone="warn">Datos incompletos</UiBadge>
                      </div>
                      <p className="figma-panel-note">{signal.summary}</p>
                      <div className="figma-data-list espionage-data-list">
                        <div className="figma-data-row"><span>Sistema</span><strong>{signal.systemLabel}</strong></div>
                        <div className="figma-data-row"><span>Siguiente seguimiento</span><strong>{signal.planetId ? "Revisar en Galaxia" : "Volver al mapa"}</strong></div>
                      </div>
                      <p className="espionage-target-note">{getEspionageMissingDataNote("signal")}</p>
                    </article>
                  ))}
                </div>
              ) : <p className="figma-panel-note">Todavia no hay objetivos agrupados para este foco.</p>}
            </section>
          </div>
        </UiCard>
      ) : null}

      <UiCard className="panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Misiones futuras</p>
            <h3>Operaciones de inteligencia</h3>
            <p>La hoja de ruta sigue visible, pero todas las operaciones permanecen desactivadas en esta version.</p>
          </div>
          <UiBadge tone="warn">Pendientes</UiBadge>
        </div>
        <div className="espionage-mission-grid">
          {futureMissionCards.map((action) => (
            <article key={action.key} className="subpanel figma-subpanel espionage-mission-card">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Operacion futura</p>
                  <h4>{action.label}</h4>
                </div>
                <UiBadge tone="warn">No disponible</UiBadge>
              </div>
              <p className="figma-panel-note">{action.reason}</p>
              <button type="button" className="planet-action-button-blocked" disabled>
                No disponible en esta version
              </button>
              <p className="espionage-target-note">Operacion pendiente de activacion en esta vista.</p>
            </article>
          ))}
        </div>
      </UiCard>

      {catalogSections.length ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Catalogo de objetivos</p>
              <h3>Red agrupada</h3>
              <p>La vista separa control confirmado, observacion estable, contacto parcial y senales sin convertirlas en operaciones.</p>
            </div>
            <UiBadge>{catalogSections.length} grupos</UiBadge>
          </div>
          <div className="espionage-catalog-grid">
            {catalogSections.map((section) => (
              <section key={section.key} className="subpanel figma-subpanel espionage-catalog-section">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">{section.label}</p>
                    <h4>{section.entries.length} senales</h4>
                    <p>{section.description}</p>
                  </div>
                  <UiBadge tone={section.tone}>{section.label}</UiBadge>
                </div>

                <div className="espionage-target-grid">
                  {section.entries.map((entry) => (
                    <article key={entry.id} className={`espionage-target-card espionage-target-card-${entry.statusTone}`}>
                      <div className="espionage-target-card-head">
                        <div>
                          <p className="eyebrow">{entry.typeLabel}</p>
                          <h5>{entry.title}</h5>
                          <p className="espionage-target-subtitle">{entry.subtitle}</p>
                        </div>
                        <UiBadge tone={entry.statusTone}>{entry.statusLabel}</UiBadge>
                      </div>

                      <div className="figma-badge-row">
                        <UiBadge>{entry.confidenceLabel}</UiBadge>
                        <UiBadge tone={entry.signalLabel === "Datos incompletos" ? "warn" : "neutral"}>{entry.signalLabel}</UiBadge>
                      </div>

                      <div className="figma-data-list espionage-data-list">
                        <div className="figma-data-row"><span>Cobertura</span><strong>{entry.coverageLabel}</strong></div>
                        {entry.controlLabel ? <div className="figma-data-row"><span>Control</span><strong>{entry.controlLabel}</strong></div> : null}
                        <div className="figma-data-row"><span>Siguiente seguimiento</span><strong>{entry.handoffLabel}</strong></div>
                      </div>
                      {entry.statusTone === "warn" ? (
                        <p className="espionage-target-note">
                          {section.key === "signal"
                            ? getEspionageMissingDataNote("signal")
                            : section.key === "partial"
                              ? getEspionageMissingDataNote("partial")
                              : getEspionageMissingDataNote("unconfirmed")}
                        </p>
                      ) : null}
                    </article>
                  ))}
                </div>
              </section>
            ))}
          </div>
        </UiCard>
      ) : null}

      {viewModel ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Senales observadas</p>
              <h3>Senales recientes</h3>
              <p>Estas pistas reutilizan cobertura pasiva ya visible. No implican seguimiento en tiempo real ni vigilancia activa.</p>
            </div>
            <UiBadge tone={viewModel.passiveSignalEntries.length > 0 ? "neutral" : "warn"}>
              {viewModel.passiveSignalEntries.length > 0 ? `${viewModel.passiveSignalEntries.length} senales` : "Sin senales"}
            </UiBadge>
          </div>

          {viewModel.passiveSignalEntries.length > 0 ? (
            <div className="espionage-signal-grid">
              {viewModel.passiveSignalEntries.map((entry) => (
                <article key={entry.id} className="espionage-target-card espionage-target-card-neutral">
                  <div className="espionage-target-card-head">
                    <div>
                      <p className="eyebrow">Senal pasiva</p>
                      <h5>{entry.title}</h5>
                      <p className="espionage-target-subtitle">{entry.systemLabel}</p>
                    </div>
                    <UiBadge>{entry.statusLabel}</UiBadge>
                  </div>
                  <p className="figma-panel-note">{entry.summary}</p>
                  <div className="figma-data-list espionage-data-list">
                    <div className="figma-data-row"><span>Siguiente seguimiento</span><strong>{entry.handoffLabel}</strong></div>
                    <div className="figma-data-row"><span>Estado</span><strong>{entry.statusLabel}</strong></div>
                  </div>
                </article>
              ))}
            </div>
          ) : (
            <div className="espionage-empty-state">
              <strong>No hay senales recientes para este contexto.</strong>
              <p>La vista seguira mostrando objetivos y niveles de certeza, pero no fabricara un historial cuando la red actual no lo ofrece.</p>
            </div>
          )}
        </UiCard>
      ) : null}

      {operatorMode && viewModel?.diagnostics.technical.length ? (
        <details className="technical-disclosure">
          <summary>
            <div>
              <p className="eyebrow">Diagnostico secundario</p>
              <strong>Lectura tecnica</strong>
            </div>
            <UiBadge tone="warn">Contraido por defecto</UiBadge>
          </summary>
          <div className="technical-disclosure-body">
            <UiCard className="panel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Limitaciones</p>
                  <h3>Estado honesto del modulo</h3>
                </div>
                <UiBadge>{viewModel.limitations.length} limites</UiBadge>
              </div>
              <ul className="stack-list compact-list">
                {viewModel.limitations.map((item) => <li key={item}>{item}</li>)}
              </ul>
              <details className="json-details">
                <summary>Datos tecnicos</summary>
                <ul className="stack-list compact-list">
                  {viewModel.diagnostics.technical.map((item) => <li key={item}>{item}</li>)}
                </ul>
              </details>
            </UiCard>
          </div>
        </details>
      ) : null}

      <UiCard className="panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Navegacion</p>
            <h3>Pasar a otras sistemas</h3>
            <p>Espionaje interpreta inteligencia. Las demas superficies siguen siendo propietarias del mapa, la superficie planetaria, el mando orbital y el progreso tecnologico.</p>
          </div>
          <UiBadge tone="warn">Contexto conservado</UiBadge>
        </div>
        <div className="espionage-handoff-grid">
          {handoffCards.map((card) => {
            const link = card.key === "galaxy"
              ? buildGalaxyUrl(queryCivilizationId, querySystemId, activePlanetId)
              : card.key === "planet"
                ? buildPlanetUrl(queryCivilizationId, activePlanetId)
                : card.key === "fleets"
                  ? buildFleetsUrl(queryCivilizationId, activePlanetId)
                  : buildResearchUrl(queryCivilizationId, activePlanetId);
            const isUnavailable = card.key === "planet" && !activePlanetId;

            return (
              <article key={card.key} className="subpanel figma-subpanel espionage-handoff-card">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">{card.label}</p>
                    <h4>{card.title}</h4>
                  </div>
                  <UiBadge tone={card.key === "galaxy" ? "neutral" : "warn"}>{card.label}</UiBadge>
                </div>
                <p className="figma-panel-note">{card.description}</p>
                {isUnavailable ? (
                  <span className="planet-action-handoff-message">Necesita un planeta enfocado para conservar contexto.</span>
                ) : (
                  <Link className={`selection-chip${card.key === "galaxy" ? " selection-chip-active" : ""}`} to={link}>
                    {card.key === "galaxy"
                      ? "Volver a Galaxia"
                      : card.key === "planet"
                        ? "Abrir Planeta"
                        : card.key === "fleets"
                          ? "Abrir Flotas"
                          : "Abrir Investigacion"}
                  </Link>
                )}
              </article>
            );
          })}
        </div>
      </UiCard>
    </section>
  );
}
