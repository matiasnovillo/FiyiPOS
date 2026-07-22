//Function to show toasts
//messageType allowed: success, info, warning and error

window.toastHelper = {
    showWithLimitedTime: function (title, message, messageType) {
        let MessageType = messageType;
        let Message = message;
        let Title = title;
        let $showDuration = "200";
        let $hideDuration = "700";
        let $timeOut = "3500";
        let $extendedTimeOut = "700";
        let $showEasing = "swing";
        let $hideEasing = "linear";
        let $showMethod = "fadeIn";
        let $hideMethod = "fadeOut";
        let addClear = false;

        toastr.options = {
            closeButton: false,
            debug: false,
            newestOnTop: false,
            progressBar: true,
            positionClass: 'toast-top-right',
            preventDuplicates: false,
            timeOut: 3500,
            extendedTimeOut: 700,
            onclick: null
        };

        let $toast = toastr[MessageType](Message, Title);

        if (typeof $toast === 'undefined') {
            return;
        }
    },
    showWithUnlimitedTime: function (title, message, messageType) {
        let MessageType = messageType;
        let Message = message;
        let Title = title;

        toastr.options = {
            closeButton: true,                // Mostrar bot�n de cerrar
            debug: false,
            newestOnTop: false,
            progressBar: false,               // Opcional: sac� la barra si no quer�s que parezca que "avanza"
            positionClass: 'toast-top-right',
            preventDuplicates: false,
            timeOut: 0,                       // 0 = no se cierra autom�ticamente
            extendedTimeOut: 0,               // Evita que se cierre tras hover
            showDuration: 300,
            hideDuration: 0,
            showEasing: "swing",
            hideEasing: "linear",
            showMethod: "fadeIn",
            hideMethod: "fadeOut",
            tapToDismiss: false               // Evita que se cierre al hacer clic en el cuerpo del toast
        };

        let $toast = toastr[MessageType](Message, Title);

        if (typeof $toast === 'undefined') {
            return;
        }
    }
};