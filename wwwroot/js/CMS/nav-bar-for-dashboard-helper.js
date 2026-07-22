//Function to show or hide g-sidenav
//boolAsString can be "true" or "false", string type
window.navBarForDashboardHelper = {
    showOrHideGSideNav: function (boolAsString) {

        var body = document.getElementsByTagName('body')[0];

        if (boolAsString === "true") {
            body.classList.remove('g-sidenav-hidden');
            body.classList.add('g-sidenav-show');
        }
        else {
            body.classList.add('g-sidenav-hidden');
            body.classList.remove('g-sidenav-show');
        }
    }
};