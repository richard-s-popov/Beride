$(document).ready(function () {
    $('#showMyTripsPopup').fancybox({
        showCloseButton: false,
        scrolling: 'no',
        centerOnScroll: true,
        onClosed: function () {
        }
    });
});

function sendRequestToTrip(tripDateId, routeId) {
    $.getJSON(document.IsAuthenticatedUrl, function(result) {
        if (result) {
            $.ajax({
                type: "POST",
                url: document.SendRequestUrl,
                data: {
                    tripDateId: tripDateId,
                    routeId: routeId
                },
                success: function (data) {
                    $('#myTripsPopup').html(data);
                    $('#showMyTripsPopup').click();
                }
            });
        } else {
            $('#signIn').click();
        }
    });
}

function selectTrip(myTripId, ownerTripRouteId, ownerTripDateId) {
    $.ajax({
        type: "POST",
        url: document.SelectTripUrl,
        data: {
            myTripId: myTripId,
            ownerTripDateId: ownerTripDateId,
            ownerTripRouteId: ownerTripRouteId
        },
        success: function (data) {
            $('#myTripsPopup').html(data);
            $('#showMyTripsPopup').click();
        }
    });
}

function createNewTrip(ownerTripDateId, ownerTripRouteId) {
    $.ajax({
        type: "POST",
        url: document.CreateMyNewTripUrl,
        data: {
            ownerTripDateId: ownerTripDateId,
            ownerTripRouteId: ownerTripRouteId
        },
        success: function (data) {
            $('#myTripsPopup').html(data);
            $('#showMyTripsPopup').click();
        }
    });
}