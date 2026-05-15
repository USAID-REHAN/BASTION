// =============================================================
// CYBERSHIELD JS INTEROP — cybershield-interop.js
// Geolocation map render, clipboard paste helper, chatbot voice/scroll, PDF generation
// =============================================================

window.bastionCyber = {
    scrollToBottom: function (elementId) {
        const el = document.getElementById(elementId);
        if (el) {
            el.scrollTop = el.scrollHeight;
        }
    },

    copyCommand: async function (text) {
        try {
            await navigator.clipboard.writeText(text);
            return true;
        } catch {
            return false;
        }
    },

    downloadReportPdf: function (sections) {
        // Simple client-side print trigger or PDF generation logic.
        // A full implementation would use a library like html2pdf.js or jsPDF.
        // For now, we format it for print.
        let printContent = '<h1>BASTION System Audit Report</h1>';
        sections.forEach(s => {
            printContent += `<h2>${s.title}</h2><p>${s.content.replace(/\n/g, '<br>')}</p>`;
        });
        
        const printWindow = window.open('', '_blank');
        printWindow.document.write(`<html><head><title>BASTION Security Report</title><style>body{font-family:sans-serif;}</style></head><body>${printContent}</body></html>`);
        printWindow.document.close();
        printWindow.print();
    },

    recognition: null,

    startVoiceInput: function (dotNetRef) {
        if (!('webkitSpeechRecognition' in window) && !('SpeechRecognition' in window)) {
            dotNetRef.invokeMethodAsync('OnVoiceError', 'no-speech');
            return false;
        }

        const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
        this.recognition = new SpeechRecognition();
        this.recognition.continuous = false;
        this.recognition.interimResults = false;
        this.recognition.lang = 'en-US';

        this.recognition.onresult = function (event) {
            const transcript = event.results[0][0].transcript;
            dotNetRef.invokeMethodAsync('OnVoiceResult', transcript);
        };

        this.recognition.onerror = function (event) {
            dotNetRef.invokeMethodAsync('OnVoiceError', event.error);
        };

        this.recognition.onend = function () {
            dotNetRef.invokeMethodAsync('OnVoiceEnd');
        };

        try {
            this.recognition.start();
            return true;
        } catch (e) {
            return false;
        }
    },

    stopVoiceInput: function () {
        if (this.recognition) {
            this.recognition.stop();
        }
    }
};
