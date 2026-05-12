// =============================================================
// BASTION SHARED JS INTEROP — bastion-interop.js
// LocalStorage, Theme Engine, Chart.js helpers, Clipboard, Geolocation
// No audio — that lives in bastion-sound.js
// =============================================================

// ── LocalStorage Wrapper ──
window.bastionStorage = {
    get: function (key) {
        return localStorage.getItem(key);
    },
    set: function (key, value) {
        localStorage.setItem(key, value);
    },
    remove: function (key) {
        localStorage.removeItem(key);
    }
};

// ── Theme Engine ──
window.bastionTheme = {
    apply: function (isDark, accentColor) {
        const root = document.documentElement;
        const body = document.body;

        // Always apply accent color — both modes use var(--accent)
        if (accentColor) {
            root.style.setProperty('--accent', accentColor);
        }

        if (isDark) {
            root.setAttribute('data-theme', 'dark');
            body.classList.add('bastion-dark');
            body.classList.remove('bastion-light');
            root.style.setProperty('--bg-current', 'var(--bg-dark)');
            root.style.setProperty('--text-current', 'var(--text-primary)');
            root.style.setProperty('--card-current', 'var(--card-dark)');
            root.style.setProperty('--surface-current', 'var(--surface-dark)');
            root.style.setProperty('--border-current', 'var(--border-dark)');
            root.style.setProperty('--text-muted', 'var(--text-muted-dark)');
        } else {
            root.setAttribute('data-theme', 'light');
            body.classList.remove('bastion-dark');
            body.classList.add('bastion-light');
            root.style.setProperty('--bg-current', 'var(--bg-light)');
            root.style.setProperty('--text-current', 'var(--text-dark)');
            root.style.setProperty('--card-current', 'var(--card-light)');
            root.style.setProperty('--surface-current', 'var(--surface-light)');
            root.style.setProperty('--border-current', 'var(--border-light)');
            root.style.setProperty('--text-muted', 'var(--text-muted-light)');
        }
    }
};

// ── Clipboard Helper ──
window.bastionClipboard = {
    copy: async function (text) {
        try {
            await navigator.clipboard.writeText(text);
            return true;
        } catch {
            return false;
        }
    },
    read: async function () {
        try {
            return await navigator.clipboard.readText();
        } catch {
            return '';
        }
    }
};

// ── Animation Helpers ──
window.bastionAnimate = {
    countUp: function (elementId, target, duration) {
        const el = document.getElementById(elementId);
        if (!el) return;
        let start = 0;
        const step = target / (duration / 16);
        const timer = setInterval(() => {
            start += step;
            if (start >= target) {
                el.textContent = Math.round(target);
                clearInterval(timer);
            } else {
                el.textContent = Math.round(start);
            }
        }, 16);
    },

    fadeIn: function (elementId) {
        const el = document.getElementById(elementId);
        if (!el) return;
        el.style.opacity = '0';
        el.style.transition = 'opacity 0.5s ease';
        requestAnimationFrame(() => { el.style.opacity = '1'; });
    }
};

// ── Password Strength Helpers ──
window.bastionPassword = {
    calculateEntropy: function (password) {
        if (!password) return 0;
        let charsetSize = 0;
        if (/[a-z]/.test(password)) charsetSize += 26;
        if (/[A-Z]/.test(password)) charsetSize += 26;
        if (/[0-9]/.test(password)) charsetSize += 10;
        if (/[^a-zA-Z0-9]/.test(password)) charsetSize += 33;
        return Math.round(password.length * Math.log2(charsetSize || 1));
    }
};
