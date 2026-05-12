// =============================================================
// HOMESCREEN JS INTEROP — home-interop.js
// Score gauge animation, clock, activity ticker
// =============================================================

window.bastionHome = {
    animateGauge: function (elementId, score) {
        const el = document.getElementById(elementId);
        if (!el) return;

        const circumference = 2 * Math.PI * 54;
        const offset = circumference - (score / 100) * circumference;

        el.style.strokeDasharray = circumference;
        el.style.strokeDashoffset = circumference;
        el.style.transition = 'stroke-dashoffset 1.5s ease-in-out';

        requestAnimationFrame(() => {
            el.style.strokeDashoffset = offset;
        });
    },

    startClock: function (elementId) {
        const el = document.getElementById(elementId);
        if (!el) return;

        function update() {
            const now = new Date();
            el.textContent = now.toLocaleTimeString('en-US', {
                hour: '2-digit', minute: '2-digit', second: '2-digit', hour12: true
            });
        }
        update();
        setInterval(update, 1000);
    }
};
