import { FormEvent, useEffect, useMemo, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { fetchAllianceUiState } from "../api/allianceApi";
import { CockpitHero } from "../components/CockpitHero";
import { PageContextStrip } from "../components/PageContextStrip";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { cockpitStatusLabels } from "../utils/cockpitStatus";
import {
  formatAllianceRequestFailure,
  getAllianceCatalogPlaceholder,
  getAllianceContactCardTitle,
  getAllianceContactReadinessLabel,
  getAllianceNextCockpitHint,
  getAllianceReadOnlyStatement,
  getAllianceStaticLabels,
} from "../utils/alliancePresentation";
import {
  getAlliancePrimaryAction,
  groupAllianceContacts,
  mapAllianceUiStateToViewModel,
  selectRecommendedDiplomacyFocus,
  type AllianceUiState,
} from "../utils/allianceViewModel";
import {
  buildDevelopmentHelperUrl,
  buildEspionageUrl,
  buildGalaxyUrl,
  buildMarketUrl,
  buildRankingUrl,
} from "../utils/routeUrls";
import { formatCompactGuid } from "../utils/domainPresentation";

const allianceLabels = getAllianceStaticLabels();

interface AllianceCatalogCard {
  key: string;
  eyebrow: string;
  title: string;
  badgeLabel: string;
  badgeTone: "neutral" | "good" | "warn";
  facts: Array<{ label: string; value: string }>;
  note: string;
}

interface AllianceCatalogSection {
  key: "known" | "potential" | "future" | "limited";
  label: string;
  description: string;
  badgeLabel: string;
  badgeTone: "neutral" | "good" | "warn";
  cards: AllianceCatalogCard[];
}

interface AllianceHandoffCard {
  key: "galaxy" | "market" | "espionage" | "ranking";
  label: string;
  title: string;
  description: string;
  ctaLabel: string;
  unavailableMessage?: string;
}

const allianceHandoffCards: readonly AllianceHandoffCard[] = [
  {
    key: "galaxy",
    label: "Galaxia",
    title: "Vista estrategica",
    description: "Mantiene el mapa, el contexto planetario conocido y la continuidad del frente sin convertir Alianzas en una cabina de mando.",
    ctaLabel: "Volver a Galaxia",
  },
  {
    key: "market",
    label: "Mercado",
    title: "Economia visible",
    description: "Recupera reservas, produccion y presion comercial usando la misma civilizacion y el mundo base cuando existe.",
    ctaLabel: "Abrir Mercado",
  },
  {
    key: "espionage",
    label: "Espionaje",
    title: "Seguimiento de contactos",
    description: "Permite seguir contactos parciales y lecturas incompletas sin abrir mensajes, pactos ni invitaciones.",
    ctaLabel: "Abrir Espionaje",
  },
  {
    key: "ranking",
    label: "Ranking",
    title: "Referencia de poder",
    description: "Permite comparar poder relativo y contexto diplomatico sin convertir Alianzas en una superficie de pactos ejecutables.",
    ctaLabel: "Abrir Ranking",
  },
] as const;

function getAllianceReadinessStatus(uiState: AllianceUiState | null) {
  if (!uiState) return "Esperando lectura";
  if (uiState.status?.hasActiveAlliance) return "Metadata de alianza";
  if (uiState.contacts.length > 0) return "Contactos preparados";
  return "Sin alianza activa";
}

export function AlliancePage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [civilizationIdInput, setCivilizationIdInput] = useState(searchParams.get("civilizationId") ?? "");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [errorFollowUp, setErrorFollowUp] = useState<string | null>(null);
  const [technicalErrorDetail, setTechnicalErrorDetail] = useState<string | null>(null);
  const [uiState, setUiState] = useState<AllianceUiState | null>(null);

  const queryCivilizationId = searchParams.get("civilizationId") ?? "";
  const groupedContacts = useMemo(() => groupAllianceContacts(uiState?.contacts ?? []), [uiState?.contacts]);
  const recommendedFocus = useMemo(
    () => (uiState ? selectRecommendedDiplomacyFocus(uiState) : allianceLabels.noActiveAlliance),
    [uiState],
  );
  const primaryAction = useMemo(
    () => (uiState ? getAlliancePrimaryAction(uiState) : allianceLabels.futureAlliance),
    [uiState],
  );
  const pactFutureCount = uiState?.futurePacts.length ?? 0;
  const contactKnownCount = uiState?.contacts.length ?? 0;
  const readOnlyStatement = getAllianceReadOnlyStatement();
  const activeCivilizationId = uiState?.civilizationId ?? queryCivilizationId;
  const activeHomePlanetId = uiState?.identity?.homePlanetId ?? null;
  const allianceReadinessStatus = getAllianceReadinessStatus(uiState);
  const catalogSections = useMemo<AllianceCatalogSection[]>(() => {
    const confirmedCards = groupedContacts.confirmed.map((contact, index) => ({
      key: `known-${contact.contactedCivilizationId}`,
      eyebrow: "Contacto conocido",
      title: getAllianceContactCardTitle("confirmed", index),
      badgeLabel: contact.statusLabel,
      badgeTone: "good" as const,
      facts: [
        { label: "Estado", value: contact.statusLabel },
        { label: "Confianza", value: contact.confidenceLabel },
        { label: "Lectura", value: getAllianceContactReadinessLabel("confirmed") },
        { label: "Siguiente cabina", value: getAllianceNextCockpitHint("confirmed", contact.sourceLabel) },
      ],
      note: `${contact.sourceLabel} | ${contact.discoveredAtLabel}`,
    }));

    const potentialCards = groupedContacts.unconfirmed.map((contact, index) => ({
      key: `potential-${contact.contactedCivilizationId}`,
      eyebrow: "Contacto potencial",
      title: getAllianceContactCardTitle("unconfirmed", index),
      badgeLabel: contact.statusLabel,
      badgeTone: "warn" as const,
      facts: [
        { label: "Estado", value: contact.statusLabel },
        { label: "Confianza", value: contact.confidenceLabel },
        { label: "Lectura", value: getAllianceContactReadinessLabel("unconfirmed") },
        { label: "Siguiente cabina", value: getAllianceNextCockpitHint("unconfirmed", contact.sourceLabel) },
      ],
      note: `${contact.sourceLabel} | ${contact.discoveredAtLabel}`,
    }));

    const futureCards = [
      ...((uiState?.futurePacts ?? []).map((pact) => ({
        key: `pact-${pact.pactTypeKey}`,
        eyebrow: "Pacto futuro",
        title: pact.pactLabel,
        badgeLabel: pact.stateLabel,
        badgeTone: "warn" as const,
        facts: [
          { label: "Estado", value: pact.stateLabel },
          { label: "Preparacion", value: "Preparado para fase futura" },
          { label: "Disponibilidad", value: pact.isAvailable ? "Visible" : "Bloqueado" },
          { label: "Siguiente cabina", value: "Seguir desde Espionaje o Mercado" },
        ],
        note: pact.reasonLabel,
      })) ?? []),
      ...((uiState?.futureActions ?? []).map((action) => ({
        key: `action-${action.actionKey}`,
        eyebrow: "Accion futura",
        title: action.label,
        badgeLabel: action.stateLabel,
        badgeTone: "warn" as const,
        facts: [
          { label: "Estado", value: action.stateLabel },
          { label: "Preparacion", value: "Bloqueado en esta version" },
          { label: "Disponibilidad", value: action.isAvailable ? "Visible" : "No ejecutable" },
          { label: "Siguiente cabina", value: "Mantener lectura desde Alianzas" },
        ],
        note: action.reasonLabel,
      })) ?? []),
    ];

    const limitedCards: AllianceCatalogCard[] = [
      {
        key: "limited-readiness",
        eyebrow: "Limite actual",
        title: "Lectura diplomatica limitada",
        badgeLabel: cockpitStatusLabels.readOnly,
        badgeTone: "warn",
        facts: [
          { label: "Estado", value: uiState?.actionSummary?.summaryLabel ?? allianceLabels.readOnlyDiplomacy },
          { label: "Confianza", value: "Metadata visible" },
          { label: "Pactos activos", value: String(uiState?.status?.activePactCount ?? 0) },
          { label: "Siguiente cabina", value: "Volver a Galaxia, Mercado o Espionaje" },
        ],
        note: readOnlyStatement,
      },
    ];

    if (uiState?.diagnostics.limitations[0]) {
      limitedCards.push({
        key: "limited-diagnostics",
        eyebrow: "Limitacion visible",
        title: "Estado honesto del modulo",
        badgeLabel: cockpitStatusLabels.diagnostics,
        badgeTone: "warn",
        facts: [
          { label: "Estado", value: "Sin ejecucion diplomatica" },
          { label: "Confianza", value: "Lectura de desarrollo" },
          { label: "Notas", value: `${uiState.diagnostics.limitations.length} limitaciones` },
          { label: "Siguiente cabina", value: "Conservar contexto entre cabinas" },
        ],
        note: uiState.diagnostics.limitations[0],
      });
    }

    return [
      {
        key: "known",
        label: "Contactos conocidos",
        description: "Lecturas diplomaticas ya asentadas. La cabina no inventa participantes y solo muestra contexto verificable.",
        badgeLabel: confirmedCards.length > 0 ? `${confirmedCards.length} lecturas` : "Sin contactos",
        badgeTone: confirmedCards.length > 0 ? "good" : "warn",
        cards: confirmedCards.length > 0 ? confirmedCards : [{
          key: "known-empty",
          eyebrow: "Contacto conocido",
          title: "Sin otras civilizaciones visibles",
          badgeLabel: allianceLabels.knownContact,
          badgeTone: "warn",
          facts: [
            { label: "Estado", value: allianceLabels.noActiveAlliance },
            { label: "Confianza", value: "Lectura determinista" },
            { label: "Lectura", value: "Sin participantes confirmados" },
            { label: "Siguiente cabina", value: "Seguir desde Galaxia o Espionaje" },
          ],
          note: getAllianceCatalogPlaceholder("known"),
        }],
      },
      {
        key: "potential",
        label: "Contactos potenciales",
        description: "Datos por confirmar que siguen visibles sin elevarse a relacion real ni a invitacion diplomatica.",
        badgeLabel: potentialCards.length > 0 ? `${potentialCards.length} lecturas` : "Sin potencial",
        badgeTone: "warn",
        cards: potentialCards.length > 0 ? potentialCards : [{
          key: "potential-empty",
          eyebrow: "Contacto potencial",
          title: "Sin contacto por confirmar",
          badgeLabel: allianceLabels.unconfirmedContact,
          badgeTone: "warn",
          facts: [
            { label: "Estado", value: allianceLabels.unconfirmedContact },
            { label: "Confianza", value: "Sin evidencia adicional" },
            { label: "Lectura", value: "No hay otra civilizacion visible" },
            { label: "Siguiente cabina", value: "Mantener seguimiento en Espionaje" },
          ],
          note: getAllianceCatalogPlaceholder("potential"),
        }],
      },
      {
        key: "future",
        label: "Pactos futuros",
        description: "La hoja de ruta mantiene pactos y acciones futuras como referencia visual, siempre bloqueados en esta fase.",
        badgeLabel: futureCards.length > 0 ? `${futureCards.length} referencias` : "Sin pactos",
        badgeTone: "warn",
        cards: futureCards.length > 0 ? futureCards : [{
          key: "future-empty",
          eyebrow: "Pacto futuro",
          title: "Sin pactos visibles",
          badgeLabel: allianceLabels.futureAlliance,
          badgeTone: "warn",
          facts: [
            { label: "Estado", value: allianceLabels.futureAlliance },
            { label: "Preparacion", value: "Fase futura" },
            { label: "Disponibilidad", value: "No ejecutable" },
            { label: "Siguiente cabina", value: "Conservar lectura en Alianzas" },
          ],
          note: getAllianceCatalogPlaceholder("future"),
        }],
      },
      {
        key: "limited",
        label: "Lectura diplomatica limitada",
        description: "La cabina muestra hasta donde llega la metadata actual y deriva el seguimiento a las superficies ya implementadas.",
        badgeLabel: `${limitedCards.length} notas`,
        badgeTone: "warn",
        cards: limitedCards.length > 0 ? limitedCards : [{
          key: "limited-empty",
          eyebrow: "Lectura limitada",
          title: "Sin notas adicionales",
          badgeLabel: cockpitStatusLabels.readOnly,
          badgeTone: "warn",
          facts: [
            { label: "Estado", value: allianceLabels.readOnlyDiplomacy },
            { label: "Confianza", value: "Lectura de desarrollo" },
            { label: "Notas", value: "Sin diagnosticos visibles" },
            { label: "Siguiente cabina", value: "Volver a Galaxia o Mercado" },
          ],
          note: getAllianceCatalogPlaceholder("limited"),
        }],
      },
    ];
  }, [groupedContacts.confirmed, groupedContacts.unconfirmed, readOnlyStatement, uiState]);

  useEffect(() => {
    setCivilizationIdInput(queryCivilizationId);

    async function load() {
      if (!queryCivilizationId) {
        setUiState(null);
        setError(null);
        setErrorFollowUp(null);
        setTechnicalErrorDetail(null);
        return;
      }

      setIsLoading(true);
      setError(null);
      setErrorFollowUp(null);
      setTechnicalErrorDetail(null);

      try {
        const response = await fetchAllianceUiState(queryCivilizationId);
        if (!response.succeeded || !response.uiState) {
          const failure = formatAllianceRequestFailure(response.errors[0] ?? null);
          setUiState(null);
          setError(failure.primaryMessage);
          setErrorFollowUp(failure.followUp);
          setTechnicalErrorDetail(failure.technicalDetail);
          return;
        }

        setUiState(mapAllianceUiStateToViewModel(response.uiState));
      } catch (requestError) {
        const failure = formatAllianceRequestFailure(requestError instanceof Error ? requestError.message : null);
        setUiState(null);
        setError(failure.primaryMessage);
        setErrorFollowUp(failure.followUp);
        setTechnicalErrorDetail(failure.technicalDetail);
      } finally {
        setIsLoading(false);
      }
    }

    void load();
  }, [queryCivilizationId]);

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const trimmedCivilizationId = civilizationIdInput.trim();
    if (!trimmedCivilizationId) {
      const failure = formatAllianceRequestFailure("Civilization id is required.");
      setUiState(null);
      setError(failure.primaryMessage);
      setErrorFollowUp(failure.followUp);
      setTechnicalErrorDetail(failure.technicalDetail);
      return;
    }

    const nextParams = new URLSearchParams();
    nextParams.set("civilizationId", trimmedCivilizationId);
    setSearchParams(nextParams);
  }

  return (
    <section className="page-grid">
      <CockpitHero
        versionLabel="Alianzas v1"
        title="Alianzas"
        description="Cabina diplomatica de solo lectura para identidad, estado actual, contactos conocidos y pactos futuros todavia bloqueados."
        developmentNote={`${readOnlyStatement} Conserva el contexto de lectura y no abre acciones de invitación o gestión.`}
        badges={(
          <>
            <UiBadge tone="resource">{uiState?.status?.stateLabel ?? "Estado diplomatico"}</UiBadge>
            <UiBadge>{uiState?.identity?.civilizationName ?? "Civilizacion propia"}</UiBadge>
            <UiBadge tone="warn">{recommendedFocus}</UiBadge>
          </>
        )}
      />

      {queryCivilizationId ? (
        <PageContextStrip
          eyebrow="Cabina diplomatica"
          title={uiState?.identity?.civilizationName ?? "Lectura de alianzas"}
          purpose="Identidad, contactos y pactos futuros como metadata de solo lectura, sin invitaciones, permisos compartidos ni gestion de miembros."
          statusLabel={allianceReadinessStatus}
          statusTone={uiState ? "good" : "warn"}
          contextItems={[
            { label: "Civilizacion", value: formatCompactGuid(activeCivilizationId) },
            {
              label: "Estado",
              value: uiState?.status?.stateLabel ?? allianceLabels.noActiveAlliance,
              detail: uiState?.status?.supportText ?? "Carga pendiente",
            },
            {
              label: "Contactos",
              value: String(contactKnownCount),
              detail: contactKnownCount > 0 ? allianceLabels.knownContact : "Sin contactos conocidos",
            },
            {
              label: "Mundo base",
              value: uiState?.identity?.homePlanetLabel ?? formatCompactGuid(activeHomePlanetId) ?? "Sin mundo base",
              detail: activeHomePlanetId ? "Contexto conservado" : undefined,
            },
          ]}
          resourceItems={[
            { label: "Diplomacia", value: "Solo lectura", tone: "good" },
            { label: "Mutaciones", value: "Bloqueadas", tone: "warn" },
            { label: "Visibilidad", value: "No compartida", tone: "neutral" },
          ]}
          primaryAction={
            <div className="selection-chip-row">
              <Link className="selection-chip selection-chip-active" to={buildGalaxyUrl(activeCivilizationId, undefined, activeHomePlanetId)}>
                Abrir Galaxia
              </Link>
              <Link className="selection-chip" to={buildMarketUrl(activeCivilizationId, activeHomePlanetId)}>
                Abrir Mercado
              </Link>
              <Link className="selection-chip" to={buildEspionageUrl(activeCivilizationId, undefined, activeHomePlanetId)}>
                Abrir Espionaje
              </Link>
            </div>
          }
        />
      ) : null}

      <UiCard className="panel alliance-summary-panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Resumen diplomatico</p>
            <h3>Lectura rapida de alianzas</h3>
            <p>{readOnlyStatement}</p>
          </div>
          <UiBadge tone="warn">{cockpitStatusLabels.readOnly}</UiBadge>
        </div>
        <div className="alliance-summary-grid">
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">Estado diplomatico</p>
            <strong>{uiState?.status?.stateLabel ?? allianceLabels.noActiveAlliance}</strong>
            <span>{uiState?.status?.headline ?? allianceLabels.unclassifiedRead}</span>
          </section>
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">Civilizacion propia</p>
            <strong>{uiState?.identity?.civilizationName ?? "Sin contexto cargado"}</strong>
            <span>{uiState?.identity?.archetypeLabel ?? "Lectura diplomatica pendiente de clasificar"}</span>
          </section>
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">Sin alianza activa</p>
            <strong>{uiState?.status?.hasActiveAlliance ? "No" : "Si"}</strong>
            <span>{uiState?.status?.supportText ?? allianceLabels.futureAlliance}</span>
          </section>
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">Contactos conocidos</p>
            <strong>{contactKnownCount}</strong>
            <span>{contactKnownCount > 0 ? allianceLabels.knownContact : "Sin contactos conocidos"}</span>
          </section>
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">Pactos futuros</p>
            <strong>{pactFutureCount}</strong>
            <span>{pactFutureCount > 0 ? allianceLabels.futureAlliance : "Sin pactos futuros visibles"}</span>
          </section>
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">Foco recomendado</p>
            <strong>{recommendedFocus}</strong>
            <span>{primaryAction}</span>
          </section>
        </div>
      </UiCard>

      <div className="strategic-cockpit-top">
        <UiCard className="panel strategic-loader-panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Contexto diplomatico</p>
              <h3>Cargar lectura de alianzas</h3>
            </div>
            <UiBadge>{cockpitStatusLabels.developmentOnly}</UiBadge>
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
            <button type="submit" disabled={isLoading}>
              {isLoading ? "Cargando..." : "Abrir alianzas"}
            </button>
          </form>
          {error ? (
            <div className="subpanel figma-subpanel figma-mini-card-warn">
              <p className="error-text">{error}</p>
              {errorFollowUp ? <p className="figma-panel-note">{errorFollowUp}</p> : null}
            </div>
          ) : null}
          {!queryCivilizationId ? (
            <p className="figma-panel-note">
              Introduce un `civilizationId` valido para convertir esta ruta en una cabina diplomatica real.
            </p>
          ) : null}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Estado visible</p>
              <h3>Resumen diplomatico</h3>
            </div>
            <UiBadge>{uiState?.status?.stateLabel ?? allianceLabels.noActiveAlliance}</UiBadge>
          </div>
          {uiState?.identity && uiState?.status ? (
            <div className="figma-data-list">
              <div className="figma-data-row"><span>Civilizacion</span><strong>{uiState.identity.civilizationName}</strong></div>
              <div className="figma-data-row"><span>Foco recomendado</span><strong>{recommendedFocus}</strong></div>
              <div className="figma-data-row"><span>Estado</span><strong>{uiState.status.headline}</strong></div>
              <div className="figma-data-row"><span>Postura</span><strong>{uiState.status.supportText}</strong></div>
            </div>
          ) : (
            <p className="figma-panel-note">
              Cuando exista un contexto valido, la cabina mostrara identidad, estado diplomatico y el siguiente foco de lectura.
            </p>
          )}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Limite de la cabina</p>
              <h3>Diplomacia solo lectura</h3>
            </div>
            <UiBadge tone="warn">Sin ejecucion</UiBadge>
          </div>
          <ul className="stack-list strategic-rules-list">
            <li>Lee identidad, membresia, contactos conocidos y pactos futuros.</li>
            <li>No crea ni modifica alianzas, invitaciones o solicitudes.</li>
            <li>No comparte visibilidad, sensores, flotas, comercio ni permisos.</li>
            <li>{readOnlyStatement}</li>
          </ul>
        </UiCard>
      </div>

      {!queryCivilizationId && !isLoading ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Sin contexto</p>
              <h3>Alianzas necesita una civilizacion antes de mostrar una lectura util.</h3>
            </div>
            <UiBadge tone="warn">Contexto requerido</UiBadge>
          </div>
          <p className="figma-panel-note">
            Usa el formulario superior o entra desde Galaxia, Mercado o Espionaje para conservar `civilizationId`.
          </p>
          <div className="selection-chip-row">
            <Link className="selection-chip selection-chip-active" to={buildDevelopmentHelperUrl()}>
              Abrir contexto de desarrollo
            </Link>
          </div>
        </UiCard>
      ) : null}

      {isLoading ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Cargando</p>
              <h3>Sincronizando lectura diplomatica</h3>
            </div>
            <UiBadge>Cargando...</UiBadge>
          </div>
          <p>Consultando identidad, estado de alianza, contactos conocidos y referencias diplomaticas futuras.</p>
        </UiCard>
      ) : null}

      {uiState?.identity && uiState?.status ? (
        <>
          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Identidad diplomatica</p>
                <h3>{uiState.identity.civilizationName}</h3>
                <p>La cabina toma como fuente principal a la civilización actual y deja el perfil del jugador como soporte técnico.</p>
              </div>
              <UiBadge tone={uiState.status.hasActiveAlliance ? "good" : "warn"}>
                {uiState.status.stateLabel}
              </UiBadge>
            </div>
            <div className="readiness-grid">
              <section className="subpanel figma-subpanel">
                <div className="figma-data-list">
                  <div className="figma-data-row"><span>Arquetipo</span><strong>{uiState.identity.archetypeLabel}</strong></div>
                  <div className="figma-data-row"><span>Estado civilizatorio</span><strong>{uiState.identity.statusLabel}</strong></div>
                  <div className="figma-data-row"><span>Mundo base</span><strong>{uiState.identity.homePlanetLabel}</strong></div>
                  <div className="figma-data-row"><span>Perfil visible</span><strong>{uiState.identity.playerDisplayName}</strong></div>
                </div>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-data-list">
                  <div className="figma-data-row"><span>Alianzas activas</span><strong>{uiState.status.activeAllianceCount}</strong></div>
                  <div className="figma-data-row"><span>Historial de alianzas</span><strong>{uiState.status.historicalAllianceCount}</strong></div>
                  <div className="figma-data-row"><span>Contactos conocidos</span><strong>{uiState.status.knownContactCount}</strong></div>
                  <div className="figma-data-row"><span>Pactos activos</span><strong>{uiState.status.activePactCount}</strong></div>
                </div>
              </section>
            </div>
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Estado de alianza</p>
                <h3>{uiState.status.headline}</h3>
                <p>Si existe una alianza activa, la cabina solo muestra metadata. Si no existe, deja referencias futuras sin insinuar acciones ejecutables.</p>
              </div>
              <UiBadge tone={uiState.status.hasActiveAlliance ? "good" : "warn"}>{uiState.status.supportText}</UiBadge>
            </div>
            <div className="readiness-grid">
              <section className="subpanel figma-subpanel">
                <div className="figma-data-list">
                  <div className="figma-data-row"><span>Estado actual</span><strong>{uiState.status.stateLabel}</strong></div>
                  <div className="figma-data-row"><span>Accion principal</span><strong>{primaryAction}</strong></div>
                  <div className="figma-data-row"><span>Foco recomendado</span><strong>{recommendedFocus}</strong></div>
                </div>
              </section>
              <section className="subpanel figma-subpanel">
                {uiState.status.primaryAlliance ? (
                  <div className="figma-data-list">
                    <div className="figma-data-row"><span>Nombre</span><strong>{uiState.status.primaryAlliance.name}</strong></div>
                    <div className="figma-data-row"><span>Sigla</span><strong>{uiState.status.primaryAlliance.tag}</strong></div>
                    <div className="figma-data-row"><span>Estado visible</span><strong>{uiState.status.primaryAlliance.statusLabel}</strong></div>
                    <div className="figma-data-row"><span>Creada</span><strong>{uiState.status.primaryAlliance.createdAtLabel}</strong></div>
                    {uiState.status.membership ? (
                      <>
                        <div className="figma-data-row"><span>Rol visible</span><strong>{uiState.status.membership.roleLabel}</strong></div>
                        <div className="figma-data-row"><span>Membresia</span><strong>{uiState.status.membership.statusLabel}</strong></div>
                      </>
                    ) : null}
                  </div>
                ) : (
                  <p className="figma-panel-note">
                    {allianceLabels.noActiveAlliance}. La cabina mantiene el estado honesto y deriva cualquier futura accion a referencias desactivadas.
                  </p>
                )}
              </section>
            </div>
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Catalogo diplomatico</p>
                <h3>Contactos y preparacion diplomatica</h3>
                <p>La cabina separa contacto conocido, potencial, pactos futuros y limites de lectura sin fabricar participantes ni acuerdos activos.</p>
              </div>
              <UiBadge tone="warn">{cockpitStatusLabels.safePlaceholder}</UiBadge>
            </div>
            <div className="alliance-catalog-grid">
              {catalogSections.map((section) => (
                <section key={section.key} className="subpanel figma-subpanel alliance-catalog-section">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">{section.label}</p>
                      <h4>{section.badgeLabel}</h4>
                      <p>{section.description}</p>
                    </div>
                    <UiBadge tone={section.badgeTone}>{section.label}</UiBadge>
                  </div>
                  <div className="alliance-catalog-card-grid">
                    {section.cards.map((card) => (
                      <article key={card.key} className={`alliance-catalog-card alliance-catalog-card-${card.badgeTone}`}>
                        <div className="alliance-catalog-card-head">
                          <div>
                            <p className="eyebrow">{card.eyebrow}</p>
                            <h5>{card.title}</h5>
                          </div>
                          <UiBadge tone={card.badgeTone}>{card.badgeLabel}</UiBadge>
                        </div>
                        <div className="figma-data-list">
                          {card.facts.map((fact) => (
                            <div key={`${card.key}-${fact.label}`} className="figma-data-row">
                              <span>{fact.label}</span>
                              <strong>{fact.value}</strong>
                            </div>
                          ))}
                        </div>
                        <p className="alliance-catalog-note">{card.note}</p>
                      </article>
                    ))}
                  </div>
                </section>
              ))}
            </div>
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Preparacion diplomatica</p>
                <h3>Fundacion sin autoridad compartida</h3>
                <p>La direccion futura queda documentada como contexto secundario. Alianzas no confirma pactos, invitaciones, permisos ni miembros.</p>
              </div>
              <UiBadge tone="warn">No disponible</UiBadge>
            </div>
            <div className="readiness-grid">
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Relaciones</p>
                    <h4>Invitaciones y solicitudes diferidas</h4>
                  </div>
                  <UiBadge tone="warn">No disponible</UiBadge>
                </div>
                <p className="figma-panel-note">La cabina no abre altas, bajas, invitaciones, solicitudes ni gestion de miembros.</p>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Pactos</p>
                    <h4>Tratados como metadata futura</h4>
                  </div>
                  <UiBadge tone="warn">Fase futura</UiBadge>
                </div>
                <p className="figma-panel-note">Comercio, defensa y no agresion permanecen como lectura preparada, no como acuerdos activos.</p>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Dependencias finales</p>
                    <h4>Autoridad diplomatica pendiente</h4>
                  </div>
                  <UiBadge tone="warn">No final</UiBadge>
                </div>
                <p className="figma-panel-note">Persistencia final, autorizacion de produccion, activos finales, mercado, movimiento y combate siguen fuera de esta cabina.</p>
              </section>
            </div>
          </UiCard>
        </>
      ) : null}

      {(technicalErrorDetail || uiState?.diagnostics.technical.length || uiState?.diagnostics.limitations.length) ? (
        <details className="technical-disclosure">
          <summary>
            <div>
              <p className="eyebrow">Diagnostico secundario</p>
              <strong>Errores, limites y notas tecnicas</strong>
            </div>
            <UiBadge tone="warn">Contraido por defecto</UiBadge>
          </summary>
          <div className="technical-disclosure-body">
            <UiCard className="panel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Soporte de alianzas</p>
                  <h3>Lectura tecnica</h3>
                </div>
                <UiBadge>{cockpitStatusLabels.diagnostics}</UiBadge>
              </div>
              {technicalErrorDetail ? (
                <ul className="stack-list compact-list">
                  <li>{technicalErrorDetail}</li>
                </ul>
              ) : null}
              {uiState?.diagnostics.limitations.length ? (
                <>
                  <div className="figma-section-header module-boundary-spacer">
                    <div>
                      <p className="eyebrow">Limitaciones</p>
                      <h4>Fase actual</h4>
                    </div>
                  </div>
                  <ul className="stack-list compact-list">
                    {uiState.diagnostics.limitations.map((item) => (
                      <li key={item}>{item}</li>
                    ))}
                  </ul>
                </>
              ) : null}
              {uiState?.diagnostics.technical.length ? (
                <>
                  <div className="figma-section-header module-boundary-spacer">
                    <div>
                      <p className="eyebrow">Trazas tecnicas</p>
                      <h4>Solo soporte</h4>
                    </div>
                  </div>
                  <ul className="stack-list compact-list">
                    {uiState.diagnostics.technical.map((item) => (
                      <li key={item}>{item}</li>
                    ))}
                  </ul>
                </>
              ) : null}
            </UiCard>
          </div>
        </details>
      ) : null}

      <UiCard className="panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Pasar a otras cabinas</p>
            <h3>Handoffs relacionados</h3>
            <p>Alianzas conserva el contexto de civilizacion y solo deriva la lectura a las superficies ya implementadas, sin insinuar flujos diplomaticos ejecutables.</p>
          </div>
          <UiBadge tone="warn">{cockpitStatusLabels.contextPreserved}</UiBadge>
        </div>
        <div className="readiness-grid">
          {allianceHandoffCards.map((card) => {
            const link = card.key === "galaxy"
              ? buildGalaxyUrl(activeCivilizationId, undefined, activeHomePlanetId)
              : card.key === "market"
                ? buildMarketUrl(activeCivilizationId, activeHomePlanetId)
                : card.key === "espionage"
                  ? buildEspionageUrl(activeCivilizationId, undefined, activeHomePlanetId)
                  : buildRankingUrl(activeCivilizationId);

            return (
              <section key={card.key} className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">{card.label}</p>
                    <h4>{card.title}</h4>
                  </div>
                  <UiBadge tone={link ? "neutral" : "warn"}>{card.label}</UiBadge>
                </div>
                <p className="figma-panel-note">{card.description}</p>
                {link ? (
                  <Link
                    className={`selection-chip${card.key === "galaxy" ? " selection-chip-active" : ""}`}
                    to={link}
                  >
                    {card.ctaLabel}
                  </Link>
                ) : (
                  <>
                    <button type="button" className="planet-action-button-blocked" disabled>
                      {card.ctaLabel}
                    </button>
                    <p className="planet-action-handoff-message">
                      {card.unavailableMessage ?? "Ruta no disponible."}
                    </p>
                  </>
                )}
              </section>
            );
          })}
        </div>
        <div className="selection-chip-row">
          <Link className="selection-chip selection-chip-active" to={buildGalaxyUrl(activeCivilizationId, undefined, activeHomePlanetId)}>
            Volver a Galaxia
          </Link>
          <Link className="selection-chip" to={buildMarketUrl(activeCivilizationId, activeHomePlanetId)}>
            Abrir Mercado
          </Link>
          <Link className="selection-chip" to={buildEspionageUrl(activeCivilizationId, undefined, activeHomePlanetId)}>
            Abrir Espionaje
          </Link>
        </div>
      </UiCard>
    </section>
  );
}
