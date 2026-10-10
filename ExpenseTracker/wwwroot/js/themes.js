/* =====================================================
   themes.js — Theme controller
   The ONE home for the theme list. Components never read
   a theme id to choose a colour; they consume tokens that
   the active [data-theme] block sets.
   Mirrors the pre-paint script in _Layout.cshtml — keep
   both in sync (see THEMES below vs. THEME_PREPAINT).
===================================================== */
(function (window, document) {
    'use strict';

    /* ---------- Registry (single source of truth) ---------- */
    var THEMES = {
        daylight: {
            id: 'daylight', label: 'Daylight', family: 'light',
            blurb: 'Crisp, cool surfaces with soft elevation',
            swatch: ['#eef2f8', '#0284c7'], themeColor: '#eef2f8'
        },
        midnight: {
            id: 'midnight', label: 'Midnight', family: 'dark',
            blurb: 'Deep navy dark with gentle glow',
            swatch: ['#0a101d', '#38bdf8'], themeColor: '#0a101d'
        },
        parchment: {
            id: 'parchment', label: 'Parchment', family: 'light',
            blurb: 'Warm sepia, flat & easy on the eyes',
            swatch: ['#f6f1e7', '#076b9e'], themeColor: '#f6f1e7'
        },
        onyx: {
            id: 'onyx', label: 'Onyx', family: 'dark',
            blurb: 'Maximum contrast, no shadows (AAA)',
            swatch: ['#000000', '#7dd3fc'], themeColor: '#000000'
        }
    };
    var THEME_IDS = Object.keys(THEMES);
    var DEFAULT_BY_FAMILY = { light: 'daylight', dark: 'midnight' };
    var STORAGE_KEY = 'theme';
    var LEGACY_MAP = { dark: 'midnight', light: 'daylight' }; // pre-existing user choices

    /* ---------- Storage (defensive) ---------- */
    function readStored() {
        try {
            var raw = localStorage.getItem(STORAGE_KEY);
            if (!raw) return null;
            if (THEME_IDS.indexOf(raw) !== -1) return raw;
            return LEGACY_MAP[raw] || null; // migrate legacy value
        } catch (e) { return null; }
    }
    function writeStored(id) { try { localStorage.setItem(STORAGE_KEY, id); } catch (e) {} }
    function clearStored() { try { localStorage.removeItem(STORAGE_KEY); } catch (e) {} }

    /* ---------- Media queries ---------- */
    var mqDark = window.matchMedia ? window.matchMedia('(prefers-color-scheme: dark)') : null;
    var mqContrast = window.matchMedia ? window.matchMedia('(prefers-contrast: more)') : null;
    var mqReduce = window.matchMedia ? window.matchMedia('(prefers-reduced-motion: reduce)') : null;

    function systemFamily() { return (mqDark && mqDark.matches) ? 'dark' : 'light'; }
    function systemTheme() {
        if (mqContrast && mqContrast.matches) return 'onyx';
        return DEFAULT_BY_FAMILY[systemFamily()];
    }

    /* ---------- Apply ---------- */
    function apply(id, silent) {
        var theme = THEMES[id] || THEMES.daylight;
        var root = document.documentElement;

        if (!silent && (!mqReduce || !mqReduce.matches)) {
            root.classList.add('theme-transition');
            window.clearTimeout(apply._t);
            apply._t = window.setTimeout(function () {
                root.classList.remove('theme-transition');
            }, 320);
        }

        root.dataset.theme = theme.id;                    // identity → token overrides
        root.classList.toggle('dark', theme.family === 'dark');  // family hook
        root.classList.toggle('light', theme.family === 'light');
        root.setAttribute('data-bs-theme', theme.family);  // Bootstrap colour mode

        var meta = document.querySelector('meta[name="theme-color"]');
        if (!meta) {
            meta = document.createElement('meta');
            meta.name = 'theme-color';
            document.head.appendChild(meta);
        }
        meta.setAttribute('content', theme.themeColor);
    }

    /* ---------- Public resolution ---------- */
    function currentId() {
        var s = readStored();
        return s || systemTheme();
    }
    function isExplicit() { return readStored() !== null; }

    function select(id) {
        if (THEME_IDS.indexOf(id) === -1) return;
        writeStored(id);
        apply(id);
        syncAll();
    }
    function useSystem() {
        clearStored();
        apply(systemTheme());
        syncAll();
    }

    /* ---------- Live OS tracking (only while no explicit choice) ---------- */
    function onSystemChange() {
        if (isExplicit()) return;
        apply(systemTheme());
        syncAll();
    }
    if (mqDark) { mqDark.addEventListener ? mqDark.addEventListener('change', onSystemChange) : mqDark.addListener(onSystemChange); }
    if (mqContrast) { mqContrast.addEventListener ? mqContrast.addEventListener('change', onSystemChange) : mqContrast.addListener(onSystemChange); }

    /* ---------- Picker UI (built from the registry, mounted in Profile settings) ---------- */
    function optionHtml(t, withCheck) {
        return '<span class="theme-swatch" style="--sw-a:' + t.swatch[0] + ';--sw-b:' + t.swatch[1] + '"></span>' +
            '<span class="theme-option-body">' +
            '  <span class="theme-option-label">' + t.label + '</span>' +
            '  <span class="theme-option-blurb">' + t.blurb + '</span>' +
            '</span>' +
            (withCheck ? '<i class="theme-check" data-lucide="check" aria-hidden="true"></i>' : '');
    }

    function buildPicker(container) {
        if (!container) return;
        container.classList.add('theme-grid');
        container.setAttribute('role', 'radiogroup');
        container.setAttribute('aria-label', 'Colour theme');
        container.innerHTML = '';

        THEME_IDS.forEach(function (id) {
            var t = THEMES[id];
            var b = document.createElement('button');
            b.type = 'button';
            b.className = 'theme-option';
            b.setAttribute('role', 'radio');
            b.dataset.themeId = id;
            b.innerHTML = optionHtml(t, true);
            b.addEventListener('click', function () { select(id); });
            container.appendChild(b);
        });

        var sys = document.createElement('button');
        sys.type = 'button';
        sys.className = 'theme-option theme-option--system';
        sys.setAttribute('role', 'radio');
        sys.dataset.action = 'system';
        sys.innerHTML =
            '<span class="theme-swatch theme-swatch--auto"><i data-lucide="monitor" aria-hidden="true"></i></span>' +
            '<span class="theme-option-body">' +
            '  <span class="theme-option-label">Use system setting</span>' +
            '  <span class="theme-option-blurb">Follow my device automatically</span>' +
            '</span>' +
            '<i class="theme-check" data-lucide="check" aria-hidden="true"></i>';
        sys.addEventListener('click', function () { useSystem(); });
        container.appendChild(sys);

        syncPicker();
        if (window.lucide) lucide.createIcons();
    }

    function syncPicker() {
        var active = currentId();
        var explicit = isExplicit();
        document.querySelectorAll('.theme-option[data-theme-id]').forEach(function (el) {
            var on = explicit && el.dataset.themeId === active;
            el.setAttribute('aria-checked', String(on));
        });
        var sys = document.querySelector('.theme-option[data-action="system"]');
        if (sys) sys.setAttribute('aria-checked', String(!explicit));
    }

    /* ---------- Collapsible theme switcher (auth / hero pages) ---------- */
    function setSwitcherOpen(el, open) {
        el.classList.toggle('open', open);
        var t = el.querySelector('.theme-switcher__trigger');
        if (t) t.setAttribute('aria-expanded', String(open));
    }
    function closeAllSwitchers(except) {
        document.querySelectorAll('.theme-switcher.open').forEach(function (el) {
            if (el !== except) setSwitcherOpen(el, false);
        });
    }

    function buildSwatchRow(container) {
        if (!container) return;
        container.classList.add('theme-switcher');
        container.innerHTML = '';

        var panel = document.createElement('div');
        panel.className = 'theme-switcher__panel';
        panel.setAttribute('role', 'group');
        panel.setAttribute('aria-label', 'Colour theme');

        THEME_IDS.forEach(function (id) {
            var t = THEMES[id];
            var b = document.createElement('button');
            b.type = 'button';
            b.className = 'theme-swatches__dot';
            b.dataset.themeId = id;
            b.setAttribute('aria-label', t.label);
            b.setAttribute('title', t.label);
            b.style.setProperty('--sw-a', t.swatch[0]);
            b.style.setProperty('--sw-b', t.swatch[1]);
            b.addEventListener('click', function () { select(id); setSwitcherOpen(container, false); });
            panel.appendChild(b);
        });

        var auto = document.createElement('button');
        auto.type = 'button';
        auto.className = 'theme-swatches__dot theme-swatches__auto';
        auto.dataset.action = 'system';
        auto.setAttribute('aria-label', 'Use system setting');
        auto.setAttribute('title', 'Use system setting');
        auto.innerHTML = '<i data-lucide="monitor" aria-hidden="true"></i>';
        auto.addEventListener('click', function () { useSystem(); setSwitcherOpen(container, false); });
        panel.appendChild(auto);

        var trigger = document.createElement('button');
        trigger.type = 'button';
        trigger.className = 'theme-switcher__trigger';
        trigger.setAttribute('aria-label', 'Choose theme');
        trigger.setAttribute('aria-expanded', 'false');
        trigger.innerHTML = '<i data-lucide="palette" aria-hidden="true"></i>';
        trigger.addEventListener('click', function (e) {
            e.stopPropagation();
            var open = !container.classList.contains('open');
            closeAllSwitchers(container);
            setSwitcherOpen(container, open);
        });

        // Panel first, trigger second → the row expands leftward (right-anchored).
        container.appendChild(panel);
        container.appendChild(trigger);

        syncSwatches();
        if (window.lucide) lucide.createIcons();
    }

    function syncSwatches() {
        var active = currentId();
        var explicit = isExplicit();
        document.querySelectorAll('.theme-swatches__dot[data-theme-id]').forEach(function (el) {
            el.setAttribute('aria-pressed', String(explicit && el.dataset.themeId === active));
        });
        var auto = document.querySelector('.theme-swatches__auto');
        if (auto) auto.setAttribute('aria-pressed', String(!explicit));
    }

    function syncAll() { syncPicker(); syncSwatches(); }

    /* ---------- Drift guard (dev only) ----------
       Confirms the pre-paint script set the same family the registry would. */
    function driftGuard() {
        try {
            var preloaded = document.documentElement.dataset.theme;
            if (preloaded && THEME_IDS.indexOf(preloaded) === -1) {
                console.warn('[themes] pre-paint set unknown theme "' + preloaded + '" — registry and _Layout THEME_PREPAINT are out of sync.');
            }
        } catch (e) {}
    }

    /* ---------- Init ---------- */
    function init() {
        driftGuard();
        buildPicker(document.getElementById('themePicker'));
        document.querySelectorAll('[data-theme-swatches]').forEach(buildSwatchRow);
        // Collapse any open switcher on outside click / Escape.
        document.addEventListener('click', function (e) {
            if (e.target.closest && e.target.closest('.theme-switcher')) return;
            closeAllSwitchers(null);
        });
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') closeAllSwitchers(null);
        });
        apply(currentId(), true); // reconcile Bootstrap attrs + meta (no first-paint fade)
    }

    window.ThemeController = {
        init: init,
        select: select,
        useSystem: useSystem,
        current: currentId,
        THEMES: THEMES,
        THEME_IDS: THEME_IDS
    };

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})(window, document);
