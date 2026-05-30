const modeInput = document.getElementById('visual-mode');
const renderModeInput = document.getElementById('render-mode');
const idInput = document.getElementById('visual-id');
const loadButton = document.getElementById('load-visual-state');
const statusOutput = document.getElementById('visual-status');
const previewOutput = document.getElementById('visual-preview');
const intensitiesOutput = document.getElementById('visual-intensities');
const profileOutput = document.getElementById('visual-profile');
const overlaysOutput = document.getElementById('visual-overlays');
const payloadOutput = document.getElementById('visual-payload');
const selectedPlanetLabel = document.getElementById('selected-planet-label');

const intensityKeys = [
    'colonizationIntensity',
    'urbanIntensity',
    'industrialIntensity',
    'terraformingIntensity',
    'militaryIntensity',
    'orbitalPresenceIntensity'
];

loadButton.addEventListener('click', loadVisualState);
renderModeInput.addEventListener('change', () => setStatus('Render mode changed. Reload or load a visual state.', false));

async function loadVisualState() {
    const id = idInput.value.trim();
    const mode = modeInput.value;

    if (!id) {
        setStatus('Identifier is required.', true);
        return;
    }

    const endpoint = mode === 'system'
        ? '/api/dev/solar-systems/' + encodeURIComponent(id) + '/visual-state'
        : '/api/dev/planets/' + encodeURIComponent(id) + '/visual-state';

    setStatus('Loading ' + mode + ' visual state...', false);

    try {
        const response = await fetch(endpoint, { headers: { Accept: 'application/json' } });
        const payload = await response.json();
        payloadOutput.textContent = JSON.stringify(payload, null, 2);

        if (!response.ok || !payload.succeeded) {
            setStatus((payload.errors || ['Request failed.']).join(' '), true);
            renderEmpty();
            return;
        }

        setStatus('Visual state loaded.', false);
        renderPayload(payload.visualState, mode);
    } catch (error) {
        setStatus('Unexpected error: ' + error.message, true);
        renderEmpty();
    }
}

function renderPayload(visualState, mode) {
    if (mode === 'system') {
        const planets = visualState.planets || [];
        if (planets.length === 0) {
            previewOutput.className = 'preview-empty';
            previewOutput.textContent = 'System has no planets.';
            renderIntensities(null);
            renderProfile(null);
            renderOverlays(visualState);
            return;
        }

        renderModeInput.value === 'pseudo3d' ? renderSystem(visualState) : renderCards(planets);
        renderIntensities(planets[0]);
        renderProfile(planets[0]);
        renderOverlays(visualState);
        selectedPlanetLabel.textContent = planets[0].planetName || 'Selected planet';
        return;
    }

    renderModeInput.value === 'pseudo3d' ? renderPlanet(visualState) : renderCards([visualState]);
    renderIntensities(visualState);
    renderProfile(visualState);
    renderOverlays(null);
    selectedPlanetLabel.textContent = visualState.planetName || 'Selected planet';
}

function renderPlanet(planet) {
    previewOutput.className = '';
    previewOutput.replaceChildren();
    const scene = document.createElement('div');
    const stage = document.createElement('div');
    const orb = document.createElement('div');
    const atmosphere = document.createElement('div');

    scene.className = 'pseudo-scene';
    stage.className = 'pseudo-planet-stage';
    orb.className = 'pseudo-planet';
    atmosphere.className = 'planet-atmosphere';
    applyOrbStyle(orb, planet);

    stage.appendChild(orb);
    stage.appendChild(atmosphere);
    scene.appendChild(stage);
    previewOutput.appendChild(scene);
}

function renderSystem(systemState) {
    const planets = systemState.planets || [];
    const layoutHints = systemState.layoutHints || [];
    const markers = systemState.orbitalGroupMarkers || [];
    const overlays = systemState.transferOverlays || [];
    const coordinates = createCoordinateLookup(layoutHints, planets.length);

    previewOutput.className = '';
    previewOutput.replaceChildren();
    const scene = document.createElement('div');
    const orbits = document.createElement('div');
    const star = document.createElement('div');

    scene.className = 'system-scene';
    orbits.className = 'system-orbits';
    star.className = 'system-star';
    orbits.appendChild(star);

    overlays.forEach(overlay => addTransferOverlay(orbits, overlay, coordinates));

    planets.forEach((planet, index) => {
        const node = document.createElement('button');
        const position = coordinates.get(planet.planetId) || createFallbackCoordinate(index, planets.length);
        node.type = 'button';
        node.className = 'system-planet-node';
        node.title = planet.planetName || '';
        node.style.width = Math.max(1.25, Math.min(2.5, planet.size / 52)) + 'rem';
        node.style.height = node.style.width;
        node.style.transform = 'translate(' + position.x + 'rem,' + position.y + 'rem) translate(-50%,-50%)';
        applyOrbStyle(node, planet);
        node.addEventListener('click', () => {
            renderIntensities(planet);
            renderProfile(planet);
            selectedPlanetLabel.textContent = planet.planetName || 'Selected planet';
        });
        orbits.appendChild(node);
    });

    markers.forEach(marker => addOrbitalGroupMarker(orbits, marker, coordinates));

    scene.appendChild(orbits);
    previewOutput.appendChild(scene);
}

function createCoordinateLookup(layoutHints, planetCount) {
    const map = new Map();
    layoutHints.forEach((hint, index) => {
        const radius = Number(hint.orbitRadius || (4.4 + index * 2.15));
        const angle = Number(hint.orbitAngleDegrees || (-145 + index * 38)) * Math.PI / 180;
        map.set(hint.planetId, {
            x: Math.cos(angle) * radius,
            y: Math.sin(angle) * radius * 0.56,
            scale: Number(hint.visualScale || 1)
        });
    });
    return map.size > 0 ? map : new Map(Array.from({ length: planetCount }, (_, index) => [String(index), createFallbackCoordinate(index, planetCount)]));
}

function createFallbackCoordinate(index, total) {
    const angle = (-145 + (total <= 1 ? 0 : index * (290 / Math.max(total - 1, 1)))) * Math.PI / 180;
    const radius = 4.4 + index * 2.15;
    return { x: Math.cos(angle) * radius, y: Math.sin(angle) * radius * 0.56, scale: 1 };
}

function addOrbitalGroupMarker(container, marker, coordinates) {
    const position = coordinates.get(marker.currentPlanetId);
    if (!position) return;

    const node = document.createElement('div');
    node.className = 'system-group-marker ' + getMarkerClass(marker.markerKind);
    node.title = marker.markerKind + ' · ' + marker.assetType + ' x' + marker.quantity;
    node.style.left = 'calc(50% + ' + (position.x + 1.1) + 'rem)';
    node.style.top = 'calc(50% + ' + (position.y - 1.1) + 'rem)';
    node.style.scale = String(Math.max(0.8, Math.min(2.2, Number(marker.markerScale || 1))));
    container.appendChild(node);
}

function addTransferOverlay(container, overlay, coordinates) {
    const start = coordinates.get(overlay.originPlanetId);
    const end = coordinates.get(overlay.destinationPlanetId);
    if (!start || !end) return;

    const dx = end.x - start.x;
    const dy = end.y - start.y;
    const length = Math.sqrt(dx * dx + dy * dy);
    const angle = Math.atan2(dy, dx) * 180 / Math.PI;
    const line = document.createElement('div');
    const progress = document.createElement('div');

    line.className = 'system-transfer-overlay';
    line.title = overlay.overlayKind + ' · ' + Math.round(Number(overlay.progress || 0) * 100) + '%';
    line.style.left = 'calc(50% + ' + start.x + 'rem)';
    line.style.top = 'calc(50% + ' + start.y + 'rem)';
    line.style.width = length + 'rem';
    line.style.transform = 'rotate(' + angle + 'deg)';
    progress.className = 'system-transfer-progress';
    progress.style.left = Math.max(0, Math.min(100, Number(overlay.progress || 0) * 100)) + '%';
    line.appendChild(progress);
    container.appendChild(line);
}

function getMarkerClass(markerKind) {
    if (markerKind === 'reserved_orbital_group') return 'reserved';
    if (markerKind === 'decommissioned_orbital_group') return 'decommissioned';
    return '';
}

function renderCards(planets) {
    previewOutput.className = '';
    previewOutput.replaceChildren();
    planets.forEach(planet => {
        const card = document.createElement('div');
        const orb = document.createElement('div');
        const meta = document.createElement('div');
        card.className = 'planet-card';
        orb.className = 'planet-orb';
        meta.className = 'planet-meta';
        applyOrbStyle(orb, planet);
        addText(meta, 'strong', planet.planetName || 'Unknown planet');
        addText(meta, 'span', planet.planetType + ' · size ' + planet.size);
        addText(meta, 'span', planet.colonizationStatus + ' · ' + (planet.isOwned ? 'Owned' : 'Unowned'));
        card.appendChild(orb);
        card.appendChild(meta);
        previewOutput.appendChild(card);
    });
}

function renderIntensities(planetState) {
    intensitiesOutput.replaceChildren();
    if (!planetState) {
        addText(intensitiesOutput, 'p', 'No planet selected.').className = 'summary';
        return;
    }

    intensityKeys.forEach(key => {
        const value = Math.max(0, Math.min(1, Number(planetState[key] || 0)));
        const percent = Math.round(value * 100);
        const row = document.createElement('div');
        const label = document.createElement('div');
        const track = document.createElement('div');
        const fill = document.createElement('div');
        row.className = 'intensity-row';
        label.className = 'intensity-label';
        track.className = 'intensity-track';
        fill.className = 'intensity-fill';
        addText(label, 'span', key.replace(/([A-Z])/g, ' $1'));
        addText(label, 'span', percent + '%');
        fill.style.width = percent + '%';
        track.appendChild(fill);
        row.appendChild(label);
        row.appendChild(track);
        intensitiesOutput.appendChild(row);
    });
}

function renderProfile(planetState) {
    profileOutput.replaceChildren();
    if (!planetState || !planetState.profile) {
        addText(profileOutput, 'p', 'No profile selected.').className = 'summary';
        return;
    }

    Object.entries(planetState.profile).forEach(([key, value]) => {
        const row = document.createElement('div');
        row.className = 'profile-row';
        addText(row, 'span', key + ': ' + value);
        profileOutput.appendChild(row);
    });
}

function renderOverlays(systemState) {
    overlaysOutput.replaceChildren();
    const markers = systemState?.orbitalGroupMarkers || [];
    const overlays = systemState?.transferOverlays || [];

    if (markers.length === 0 && overlays.length === 0) {
        addText(overlaysOutput, 'p', 'No system overlays.').className = 'summary';
        return;
    }

    markers.forEach(marker => addOverlayRow('Group', marker.markerKind, marker.assetType + ' x' + marker.quantity + ' @ ' + marker.currentPlanetId));
    overlays.forEach(overlay => addOverlayRow('Transfer', overlay.overlayKind, Math.round(Number(overlay.progress || 0) * 100) + '% · ' + overlay.originPlanetId + ' -> ' + overlay.destinationPlanetId));
}

function addOverlayRow(type, kind, value) {
    const row = document.createElement('div');
    const label = document.createElement('div');
    row.className = 'overlay-row';
    label.className = 'overlay-label';
    addText(label, 'strong', type);
    addText(label, 'span', kind);
    row.appendChild(label);
    addText(row, 'span', value).className = 'overlay-value';
    overlaysOutput.appendChild(row);
}

function applyOrbStyle(node, planet) {
    const colors = getPalette(planet.planetType);
    node.style.background = 'radial-gradient(circle at 35% 30%, ' + colors[0] + ', ' + colors[1] + ' 25%, ' + colors[2] + ' 60%, ' + colors[3] + ')';
}

function getPalette(planetType) {
    const palettes = {
        Terran: ['#ffffff', '#7bd7ff', '#2f7d4f', '#0b1f38'],
        Desert: ['#fff5d6', '#d7a85e', '#9c5f2d', '#201007'],
        Ice: ['#ffffff', '#b7ecff', '#6e93c9', '#101d36'],
        Volcanic: ['#ffe1ad', '#ff6f2c', '#5f1414', '#120708'],
        Oceanic: ['#d8ffff', '#2ea6ff', '#123f8c', '#05182f'],
        Barren: ['#f5f0df', '#a69f8f', '#60584f', '#171615'],
        GasGiant: ['#fff7d7', '#d7a35c', '#8a5a96', '#1a1023']
    };
    return palettes[planetType] || palettes.Barren;
}

function renderEmpty() {
    previewOutput.className = 'preview-empty';
    previewOutput.textContent = 'No visual state loaded.';
    intensitiesOutput.replaceChildren();
    profileOutput.replaceChildren();
    overlaysOutput.replaceChildren();
    selectedPlanetLabel.textContent = 'No selection';
}

function setStatus(message, isError) {
    statusOutput.textContent = message;
    statusOutput.style.color = isError ? '#ff9f9f' : '';
}

function addText(parent, tagName, value) {
    const node = document.createElement(tagName);
    node.textContent = String(value ?? '');
    parent.appendChild(node);
    return node;
}
