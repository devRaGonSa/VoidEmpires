const modeInput = document.getElementById('visual-mode');
const renderModeInput = document.getElementById('render-mode');
const idInput = document.getElementById('visual-id');
const loadButton = document.getElementById('load-visual-state');
const statusOutput = document.getElementById('visual-status');
const previewOutput = document.getElementById('visual-preview');
const intensitiesOutput = document.getElementById('visual-intensities');
const profileOutput = document.getElementById('visual-profile');
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
            return;
        }

        renderModeInput.value === 'pseudo3d' ? renderSystem(planets) : renderCards(planets);
        renderIntensities(planets[0]);
        renderProfile(planets[0]);
        selectedPlanetLabel.textContent = planets[0].planetName || 'Selected planet';
        return;
    }

    renderModeInput.value === 'pseudo3d' ? renderPlanet(visualState) : renderCards([visualState]);
    renderIntensities(visualState);
    renderProfile(visualState);
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

function renderSystem(planets) {
    previewOutput.className = '';
    previewOutput.replaceChildren();
    const scene = document.createElement('div');
    const orbits = document.createElement('div');
    const star = document.createElement('div');

    scene.className = 'system-scene';
    orbits.className = 'system-orbits';
    star.className = 'system-star';
    orbits.appendChild(star);

    planets.forEach((planet, index) => {
        const node = document.createElement('button');
        node.type = 'button';
        node.className = 'system-planet-node';
        node.title = planet.planetName || '';
        node.style.width = Math.max(1.25, Math.min(2.5, planet.size / 52)) + 'rem';
        node.style.height = node.style.width;
        node.style.transform = 'rotate(' + (-145 + index * 38) + 'deg) translateX(' + (4.4 + index * 2.15) + 'rem) translate(-50%, -50%)';
        applyOrbStyle(node, planet);
        node.addEventListener('click', () => {
            renderIntensities(planet);
            renderProfile(planet);
            selectedPlanetLabel.textContent = planet.planetName || 'Selected planet';
        });
        orbits.appendChild(node);
    });

    scene.appendChild(orbits);
    previewOutput.appendChild(scene);
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
