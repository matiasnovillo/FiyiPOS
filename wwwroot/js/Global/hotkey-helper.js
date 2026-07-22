window.navDropHelper = {
    getBounds(element) {
        var r = element.getBoundingClientRect();
        return { bottom: r.bottom, left: r.left };
    }
};

// Circuit loading is managed by Blazor state — no DOM manipulation needed
window.hideCircuitLoading = function () { };

window.hotkeyHelper = {
    _handler: null,

    // dotNetRef: DotNetObjectReference that exposes OnKeyPressed(key)
    register(dotNetRef) {
        if (this._handler) return; // guard: only one listener per circuit

        this._handler = (e) => {
            // Ctrl+F5 → hard reload
            if (e.ctrlKey && e.key === 'F5') {
                window.location.reload(true);
                return;
            }

            // Match bare F1–F12 only (ignore when Ctrl/Alt/Meta is held)
            if (e.ctrlKey || e.altKey || e.metaKey) return;
            if (!/^F([1-9]|1[0-2])$/.test(e.key)) return;

            // F5 = browser refresh — never intercept it
            if (e.key === 'F5') return;

            e.preventDefault(); // block browser defaults (help, devtools, menu bar, etc.)
            dotNetRef.invokeMethodAsync('OnKeyPressed', e.key);
        };

        document.addEventListener('keydown', this._handler);
    },

    dispose() {
        if (this._handler) {
            document.removeEventListener('keydown', this._handler);
            this._handler = null;
        }
    }
};
