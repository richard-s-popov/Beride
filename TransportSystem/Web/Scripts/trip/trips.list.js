$(document).ready(function () {
    $('#showMyTripsPopup').fancybox({
        showCloseButton: false,
        scrolling: 'no',
        centerOnScroll: true,
        onClosed: function () {
        }
    });
    
    $('#dateAt').datepicker($.datepicker.regional["ru"]);
    $('#dateAt').parent().click(function () {
        $('#dateAt').focus();
    });

    $('#dateTo').datepicker($.datepicker.regional["ru"]);
    $('#dateTo').parent().click(function () {
        $('#dateTo').focus();
    });
    
    $('.add-diapason-button').unbind().bind('click', function () {
        $('#additionalDate').slideDown(100);
        $('#dateAt').attr('placeholder', 'С какого');
        $('.date-hider').show();
        $(this).hide();
    });

    $('.date-hider').unbind().bind('click', function () {
        $('#additionalDate').hide();
        $('#dateAt').attr('placeholder', 'Дата');
        $(this).hide();
        $('.add-diapason-button').show();
    });
    
    $('#search').bind('click', function () {
        $('#hdnStartPoint').val($('#startPoint').attr('gid'));
        $('#hdnEndPoint').val($('#endPoint').attr('gid'));
        $('#hdnDateAt').val($('#dateAt').attr('date'));
        $('#hdnDateTo').val($('#dateTo').attr('date'));

        if ($('#dateAt').attr('date') === undefined) {
            $('#dateAt').datepicker('setDate', new Date());

            var day = $('#dateAt').datepicker('getDate').getDate();
            var month = $('#dateAt').datepicker('getDate').getMonth() + 1;
            var year = $('#dateAt').datepicker('getDate').getFullYear();
            var fullDate = month + "." + day + "." + year;

            $('#dateAt').attr('date', fullDate);
        }

        $('.filters .input-date').each(function () {
            if ($(this).datepicker('getDate') != null) {
                var day = $(this).datepicker('getDate').getDate();
                var month = $(this).datepicker('getDate').getMonth() + 1;
                var year = $(this).datepicker('getDate').getFullYear();

                var fullDate = month + "." + day + "." + year;
                var fullDate2 = day + "." + month + "." + year;

                $(this).attr('date', fullDate);
                $(this).attr('date2', fullDate2);
            }
        });

        window.location = document.SearchUrl
            + '?search=' + $(".filters input[name='search']:checked").val()
            + '&startPointGid=' + $('#startPoint').attr('gid')
            + '&endPointGid=' + $('#endPoint').attr('gid')
            + '&dateAt=' + $('#dateAt').attr('date')
            + '&dateTo=' + $('#dateTo').attr('date')
            + '&startPointFullName=' + $('#startPoint').attr('full-name')
            + '&endPointFullName=' + $('#endPoint').attr('full-name');
    });

    bindAutocomplate({shortValue: true});
});

function Renewal() {
    if ($('#hdnStartPoint').val() != '') {
        $('#startPoint').attr('gid', $('#hdnStartPoint').val());
    }

    if ($('#hdnEndPoint').val() != '') {
        $('#endPoint').attr('gid', $('#hdnEndPoint').val());
    }
    
    if ($('#hdnStartPointFullName').val() != '') {
        $('#startPoint').attr('full-name', $('#hdnStartPointFullName').val());
    }

    if ($('#hdnEndPointFullName').val() != '') {
        $('#endPoint').attr('full-name', $('#hdnEndPointFullName').val());
    }

    if ($('#hdnDateAt').val() != '') {
        $('#dateAt').attr('date', $('#hdnDateAt').val());
    }

    if ($('#hdnDateTo').val() != '') {
        $('#dateTo').attr('date', $('#hdnDateTo').val());
    }
}

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