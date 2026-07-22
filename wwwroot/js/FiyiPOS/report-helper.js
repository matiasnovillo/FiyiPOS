window.reportHelper = {
    openAndPrint: function (html) {
        const win = window.open('', '_blank', 'width=960,height=800,scrollbars=yes,resizable=yes');
        if (!win) {
            alert('Permitá las ventanas emergentes en el navegador para descargar el informe.');
            return;
        }
        win.document.open();
        win.document.write(html);
        win.document.close();
        win.onload = function () { win.print(); };
    }
};
