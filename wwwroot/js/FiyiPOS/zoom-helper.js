window.zoomHelper = {
    _levels: [0.75, 0.80, 0.85, 0.90, 0.95, 1.00, 1.05, 1.10, 1.15, 1.20, 1.25],
    _idx: 5, // default = 100 %

    init: function () {
        const saved = localStorage.getItem('pos_zoom_idx');
        if (saved !== null) {
            const n = parseInt(saved, 10);
            if (!isNaN(n) && n >= 0 && n < this._levels.length)
                this._idx = n;
        }
        this._apply();
        return this._idx;
    },

    _apply: function () {
        document.documentElement.style.setProperty('--pos-zoom', this._levels[this._idx]);
    },

    zoomIn: function () {
        if (this._idx < this._levels.length - 1) {
            this._idx++;
            this._apply();
            localStorage.setItem('pos_zoom_idx', this._idx);
        }
        return this._idx;
    },

    zoomOut: function () {
        if (this._idx > 0) {
            this._idx--;
            this._apply();
            localStorage.setItem('pos_zoom_idx', this._idx);
        }
        return this._idx;
    }
};
