const modeInput = document.getElementById('visual-mode');
const idInput = document.getElementById('visual-id');
const loadButton = document.getElementById('load-visual-state');
const statusOutput = document.getElementById('visual-status');
const previewOutput = document.getElementById('visual-preview');
const intensitiesOutput = document.getElementById('visual-intensities');
const payloadOutput = document.getElementById('visual-payload');

const intensityKeys = [
    'colonizationIntensity',
    'urbanIntensity',
    'industrialIntensity',
    'terraformingIntensity',
    'militaryIntensity',
    'orbitalPresenceIntensity'
];

loadButton.addEventListener('click', loadVisualState);

async function loadVisualState() {
    const id = idInput.value.trim();
    const mode = modeInput.value;

    if (!id) {
        setStatus('Identifier is required.', true);
        return;
    }

    const endpoint = mode === 'system'
        ? `/api/dev/solar-systems/${encodeURIComponent(id)}/visual-state`
        : `/api/dev/planets/${encodeURIComponent(id)}/visual-state`;

    setStatus(`Loading ${mode} visual state...`, false);

    try {
        const response = await fetch(endpoint, { headers: { 'Accept': 'application/json' } });
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
        setStatus(`Unexpected error: ${error.message}`, true);
        renderEmpty();
    }
}

function renderPayload(visualState, mode) {
    if (mode === 'system') {
        renderSystem(visualState);
        renderIntensities(visualState.planets?.[0] || null);
        return;
    }

    renderPlanet(visualState);
    renderIntensities(visualState);
}

function renderSystem(systemState) {
    const planets = systemState.planets || [];

    if (planets.length === 0) {
        previewOutput.className = 'preview-empty';
        previewOutput.textContent = 'System has no planets.';
        return;
    }

    previewOutput.className = '';
    previewOutput.innerHTML = planets.map(createPlanetCard).join('');
}

function renderPlanet(planetState) {
    previewOutput.className = '';
    previewOutput.innerHTML = createPlanetCard(planetState);
}

function createPlanetCard(planet) {
    const profile = planet.profile || {};
    const ownedText = planet.isOwned ? `Owned by ${planet.civilizationId}` : 'Unowned';
    const orbStyle = getOrbStyle(planet);

    return `
        <div class="planet-card">
            <div class="planet-orb" style="${orbStyle}" aria-hidden="true"></div>
            <div class="planet-meta">
                <strong>${escapeHtml(planet.planetName || 'Unknown planet')}</strong>
                <span>${escapeHtml(planet.planetType)} · size ${planet.size}</span>
                <span>${escapeHtml(planet.colonizationStatus)} · ${escapeHtml(ownedText)}</span>
                <span>Surface: ${escapeHtml(profile.surfaceProfile || 'n/a')}</span>
                <span>Lights: ${escapeHtml(profile.lightDistributionMode || 'n/a')}</span>
            </div>
        </div>`;
}

function renderIntensities(planetState) {
    if (!planetState) {
        intensitiesOutput.innerHTML = '<p class="summary">No planet selected.</p>';
        return;
    }

    intensitiesOutput.innerHTML = intensityKeys
        .map(key => createIntensityRow(key, Number(planetState[key] || 0)))
        .join('');
}

function createIntensityRow(key, value) {
    const normalized = Math.max(0, Math.min(1, value));
    const percent = Math.round(normalized * 100);
    const label = key.replace(/([A-Z])/g, ' $1').replace(/^./, value => value.toUpperCase());

    return `
        <div class="intensity-row">
            <div class="intensity-label"><span>${escapeHtml(label)}</span><span>${percent}%</span></div>
            <div class="intensity-track"><div class="intensity-fill" style="width:${percent}%"></div></div>
        </div>`;
}

function getOrbStyle(planet) {
    const palette = {
        Terran: ['#ffffff', '#7bd7ff', '#2f7d4f', '#0b1f38'],
        Desert: ['#fff5d6', '#d7a85e', '#9c5f2d', '#201007'],
        Ice: ['#ffffff', '#b7ecff', '#6e93c9', '#101d36'],
        Volcanic: ['#ffe1ad', '#ff6f2c', '#5f1414', '#120708'],
        Oceanic: ['#d8ffff', '#2ea6ff', '#123f8c', '#05182f'],
        Barren: ['#f5f0df', '#a69f8f', '#60584f', '#171615'],
        GasGiant: ['#fff7d7', '#d7a35c', '#8a5a96', '#1a1023']
    };
    const colors = palette[planet.planetType] || palette.Barren;
    const orbitalGlow = Math.round((planet.orbitalPresenceIntensity || 0) * 60);

    return `background: radial-gradient(circle at 35% 30%, ${colors[0]} 0, ${colors[1]} 20%, ${colors[2]} 52%, ${colors[3]} 75%); box-shadow: inset -18px -22px 35px rgba(0,0,0,.55), 0 0 ${24 + orbitalGlow}px rgba(114,214,255,.24);`;
}

function renderEmpty() {
    previewOutput.className = 'preview-empty';
    previewOutput.textContent = 'No visual state loaded.';
    intensitiesOutput.innerHTML = '';
}

function setStatus(message, isError) {
    statusOutput.textContent = message;
    statusOutput.style.color = isError ? '#ff9f9f' : '';
}

function escapeHtml(value) {
    return String(value ?? '')
        .replaceAll('&', '&amp;')
        .replaceAll('<', '&lt;')
        .replaceAll('>', '&gt;')
        .replaceAll('"', '&quot;')
        .replaceAll("'", '&#039;');
}
