// =============================================================
// SETTINGS JS INTEROP — settings-interop.js
// Color picker, CSS variable injection, sound toggle
// =============================================================

window.bastionSettings = {
    _colorPickerInitialized: false,

    initColorPicker: function (elementId, dotNetHelper, currentColor) {
        const container = document.getElementById(elementId);
        if (!container) return;

        // Create a simple HSL color picker since we're not loading iro.js CDN
        container.innerHTML = '';

        const canvas = document.createElement('canvas');
        canvas.width = 220;
        canvas.height = 220;
        canvas.style.borderRadius = '50%';
        canvas.style.cursor = 'crosshair';
        container.appendChild(canvas);

        const ctx = canvas.getContext('2d');
        const centerX = canvas.width / 2;
        const centerY = canvas.height / 2;
        const radius = canvas.width / 2;

        // Draw color wheel
        for (let angle = 0; angle < 360; angle++) {
            const startAngle = (angle - 1) * Math.PI / 180;
            const endAngle = (angle + 1) * Math.PI / 180;

            ctx.beginPath();
            ctx.moveTo(centerX, centerY);
            ctx.arc(centerX, centerY, radius, startAngle, endAngle);
            ctx.closePath();

            const gradient = ctx.createRadialGradient(centerX, centerY, 0, centerX, centerY, radius);
            gradient.addColorStop(0, 'hsl(' + angle + ', 10%, 100%)');
            gradient.addColorStop(0.5, 'hsl(' + angle + ', 100%, 50%)');
            gradient.addColorStop(1, 'hsl(' + angle + ', 100%, 20%)');
            ctx.fillStyle = gradient;
            ctx.fill();
        }

        // Handle click
        canvas.addEventListener('click', function (e) {
            const rect = canvas.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;
            const pixel = ctx.getImageData(x, y, 1, 1).data;
            const hex = '#' + ((1 << 24) + (pixel[0] << 16) + (pixel[1] << 8) + pixel[2]).toString(16).slice(1);
            dotNetHelper.invokeMethodAsync('OnColorSelected', hex);
        });

        this._colorPickerInitialized = true;
    },

    setAccentPreview: function (hex) {
        document.documentElement.style.setProperty('--accent', hex);
    }
};
