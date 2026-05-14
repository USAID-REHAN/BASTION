// =============================================================
// LEXGUARD JS INTEROP — lexguard-interop.js
// File download helper, drag-drop visual feedback
// =============================================================

window.bastionLexGuard = {

    // Trigger file download from base64 data (for PDF reports)
    downloadFile: function (fileName, contentType, base64Data) {
        try {
            const byteCharacters = atob(base64Data);
            const byteNumbers = new Array(byteCharacters.length);
            for (let i = 0; i < byteCharacters.length; i++) {
                byteNumbers[i] = byteCharacters.charCodeAt(i);
            }
            const byteArray = new Uint8Array(byteNumbers);
            const blob = new Blob([byteArray], { type: contentType });
            const url = URL.createObjectURL(blob);

            const link = document.createElement('a');
            link.href = url;
            link.download = fileName;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);

            setTimeout(() => URL.revokeObjectURL(url), 5000);
            return true;
        } catch (e) {
            console.error('LexGuard download error:', e);
            return false;
        }
    },

    // Initialize drag-drop visual feedback on the upload zone
    initDragDrop: function (elementId) {
        const zone = document.getElementById(elementId);
        if (!zone) return;

        zone.addEventListener('dragover', (e) => {
            e.preventDefault();
            zone.classList.add('drag-over');
        });

        zone.addEventListener('dragleave', () => {
            zone.classList.remove('drag-over');
        });

        zone.addEventListener('drop', () => {
            zone.classList.remove('drag-over');
        });
    },

    // Scroll to element smoothly
    scrollToElement: function (elementId) {
        const el = document.getElementById(elementId);
        if (el) {
            el.scrollIntoView({ behavior: 'smooth', block: 'start' });
        }
    }
};
