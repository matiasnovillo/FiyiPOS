window.notificationHelper = {
    showDeletionNotification: function () {
        const toastEl = document.getElementById('delete-notification');
        if (toastEl) {
            const bsToast = new bootstrap.Toast(toastEl);
            bsToast.show();
        }
    }
};

document.addEventListener('DOMContentLoaded', function () {
    document.addEventListener('click', function (event) {
        //HIDE NOTIFICATION
        if (event.target.matches('.close-notification')) {
            $('#delete-notification').removeClass('show');
        }
    }, false);
});