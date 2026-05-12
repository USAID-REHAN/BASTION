// =============================================================
// GATEKEEPER JS INTEROP — gatekeeper-interop.js
// Password strength animation, form effects
// =============================================================

window.bastionGateKeeper = {
    animateStrengthBar: function (elementId, percentage, color) {
        const bar = document.getElementById(elementId);
        if (!bar) return;
        bar.style.transition = 'width 0.4s ease, background-color 0.4s ease';
        bar.style.width = percentage + '%';
        bar.style.backgroundColor = color;
    },

    shakeElement: function (elementId) {
        const el = document.getElementById(elementId);
        if (!el) return;
        el.classList.add('shake-animation');
        setTimeout(() => el.classList.remove('shake-animation'), 600);
    },

    focusElement: function (elementId) {
        const el = document.getElementById(elementId);
        if (el) el.focus();
    }
};
