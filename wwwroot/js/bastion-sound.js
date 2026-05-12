// =============================================================
// BASTION SOUND — bastion-sound.js
// Audio ONLY file — HTML5 Audio API, completely isolated.
// Called ONLY by SoundService.cs. No other file references audio.
// =============================================================

window.bastionSound = {
    _audio: null,

    _getAudio: function () {
        if (!this._audio) {
            // Create a synthetic chime using Web Audio API since we don't have an mp3 yet
            this._useWebAudio = true;
        }
        return this._audio;
    },

    playChime: function () {
        try {
            const ctx = new (window.AudioContext || window.webkitAudioContext)();
            const oscillator = ctx.createOscillator();
            const gainNode = ctx.createGain();

            oscillator.connect(gainNode);
            gainNode.connect(ctx.destination);

            oscillator.frequency.setValueAtTime(880, ctx.currentTime);
            oscillator.frequency.setValueAtTime(1108.73, ctx.currentTime + 0.1);
            oscillator.frequency.setValueAtTime(1318.51, ctx.currentTime + 0.2);

            oscillator.type = 'sine';

            gainNode.gain.setValueAtTime(0.3, ctx.currentTime);
            gainNode.gain.exponentialRampToValueAtTime(0.01, ctx.currentTime + 0.5);

            oscillator.start(ctx.currentTime);
            oscillator.stop(ctx.currentTime + 0.5);
        } catch (e) {
            console.log('BASTION: Audio not available', e);
        }
    }
};
