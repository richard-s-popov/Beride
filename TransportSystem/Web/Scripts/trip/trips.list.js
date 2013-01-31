$(document).ready(function () {

});

function sendRequestToTrip(tripId) {
    $.ajax({
        type: "POST",
        url: document.SaveTripUrl,
        data: prepareDataToSent(),
        dataType: "json",
        traditional: true
    });
}

function requestLogin() {
    $('#loginContainer').show();
    $('#registrationContainer').hide();
    $.fancybox.resize();
}