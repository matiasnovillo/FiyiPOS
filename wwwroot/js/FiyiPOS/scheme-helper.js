window.schemeHelper = {
    applyVar: function (key, value) {
        var cssVar = '--' + key.replace(/_/g, '-');
        document.documentElement.style.setProperty(cssVar, value);
    },
    applyAll: function (vars) {
        for (var key in vars) {
            if (Object.prototype.hasOwnProperty.call(vars, key)) {
                var cssVar = '--' + key.replace(/_/g, '-');
                document.documentElement.style.setProperty(cssVar, vars[key]);
            }
        }
    }
};
