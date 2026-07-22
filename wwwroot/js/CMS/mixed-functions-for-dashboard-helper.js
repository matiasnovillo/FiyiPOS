document.addEventListener('DOMContentLoaded', function () {
    let functionInterval;

    function verificarCargadoYEjecutar() {

        var body = document.getElementsByTagName('body')[0];
            clearInterval(functionInterval);

        document.addEventListener('click', function (event) {

            var fixedRightNavbar = document.querySelector('.fixed-plugin');

            //Hamburger in CMS system => Show/Hide left navbar
            if (event.target.matches('.sidenav-toggler-inner') || event.target.matches('.sidenav-toggler-line') ) {
                if (!body.classList.contains('g-sidenav-hidden')) {
                    body.classList.add('g-sidenav-hidden');
                    body.classList.remove('g-sidenav-show');
                } else {
                    body.classList.remove('g-sidenav-hidden');
                    body.classList.add('g-sidenav-show');
                }
            }

            //Gear icon in CMS system, top right=> Show fixed plugin
            if (event.target.matches('.fixed-plugin-button-nav')) {
                document.querySelector('.fixed-plugin').classList.add('show')
            }

            //Gear icon in CMS system, bottom right=> Show fixed plugin
            if (event.target.matches('.fixed-plugin-button')) {
                document.querySelector('.fixed-plugin').classList.add('show')
            }

            //Close button in CMS system, top right, inside fixed plugin => Close fixed plugin
            if (event.target.matches('.fixed-plugin-close-button') || event.target.closest('.fixed-plugin-close-button') ) {
                document.querySelector('.fixed-plugin').classList.remove('show')
            }

            //Nav item in CMS system, top right => Show dropdown //THIS DOESNT WORK
            var dropdownNotificationButton = document.querySelector('li.nav-item.dropdown.pe-2 > a.nav-link');
            if (dropdownNotificationButton && dropdownNotificationButton.contains(event.target) && event.target.parentNode == dropdownNotificationButton) {
                $('#notificationDropDown').toggleClass('show');
            } else {
                $('#notificationDropDown').removeClass('show');
            }

            //Hide fixed plugin if click anywhere else and ouside fixed plugin
            if (fixedRightNavbar && !fixedRightNavbar.contains(event.target) && event.target.id !== 'fixedRightNavbarButton2' && event.target.id !== 'closeRightNavbar' && event.target.id !== 'fixedRightNavbarButton') {
                $(fixedRightNavbar).removeClass('show');
            }

            //Footer modal //THIS IS WEIRD
            var btnModalSave = document.querySelector('.modal-footer .btn:nth-of-type(2)');
            if (event.target === btnModalSave) {
                $(document.querySelector('.modal-footer .btn:nth-of-type(1)')).trigger('click');
            }

        }, false);
    }

    // Verificar si está cargado boton y ejecutar cada 0.5 segundos
    functionInterval = setInterval(verificarCargadoYEjecutar, 500);
});
