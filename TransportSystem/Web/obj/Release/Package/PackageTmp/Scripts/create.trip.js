var countPoint = 0;
var oldRoute;
var points = [];

$(document).ready(function () {
    $('#addPoints').bind('click', function () {
        var html = '<div class="input-text" id="point-' + countPoint + '">' +
            '<input class="input-location between" type="text" placeholder="Через" />' +
                '<img src="/Beride/Content/img/map_point.png" />' +
                    '<img src="/Beride/Content/img/cross.png" class="delete-button" />' +
                        '</div>';
        $('#pointContainer .input-text:last').after(html);
        countPoint++;

        bindAutocomplate();
    });

    $('.delete-button').live('click', function () {
        $(this).parent().remove();

        points = [];
        points = [{ FullName: $.trim($('#startPoint').val()), ShortName: $('#startPoint').attr('ShortName'), Gid: $('#startPoint').attr('gid')}];

        if ($('.input-location.between').size() > 0) {
            $('.input-location.between').each(function () {
                points.push({ FullName: $.trim($(this).val()), ShortName: $(this).attr('ShortName'), Gid: $(this).attr('gid') });
            });
        }

        points.push({ FullName: $.trim($('#endPoint').val()), ShortName: $('#endPoint').attr('ShortName'), Gid: $('#endPoint').attr('gid') });

        buildRoute();
    });

    $('.input-location').live('change', function () {
        // Проверка на пустые поля
        if ($.trim($('#startPoint').val()) != '' && $.trim($('#endPoint').val()) != '') {
            points = [];
            points = [{ FullName: $.trim($('#startPoint').val()), ShortName: $('#startPoint').attr('ShortName'), Gid: $('#startPoint').attr('gid')}];

            // Проверка на пустые поля промужуточных точек
            if ($('.input-location.between').size() > 0) {
                var isEmpty = false;
                $('.input-location.between').each(function () {
                    if ($.trim($(this).val()) == '') {
                        isEmpty = true;
                    } else {
                        points.push({ FullName: $.trim($(this).val()), ShortName: $(this).attr('ShortName'), Gid: $(this).attr('gid') });
                    }
                });

                if (isEmpty) {
                    return false;
                }
            }

            points.push({ FullName: $.trim($('#endPoint').val()), ShortName: $('#endPoint').attr('ShortName'), Gid: $('#endPoint').attr('gid') });

            buildRoute();
        }
    });

    $('#sendData').bind('click', function () {
        sendData();
    });
});

function buildRoute() {
    var pointsForRoute = [];
    $.each(points, function (index, point) {
        pointsForRoute.push(point.FullName);
    });

    // Создаем маршрут
    ymaps.route(pointsForRoute, { mapStateAutoApply: true })
        .then(function (route) {
            if (oldRoute) {
                // удаляем старый, если есть таковой
                myMap.geoObjects.remove(oldRoute);
            }
            // отображаем новый
            myMap.geoObjects.add(route);
            oldRoute = route;

            var i = 0;
            $('#routesTable tbody').html('');
            route.getPaths().each(function (path) {
                var html = $('#routesTable tbody').html();
                var row = '<tr class="route">' +
                    '<td class="from">' + points[i].ShortName + '</td>' +
                        '<td class="arrow">-></td>' +
                            '<td class="to">' + points[i + 1].ShortName + '</td>' +
                                '<td class="equal">=</td>' +
                                    '<td class="cost">' + (path.getLength() / 1000 * 1.3).toFixed() + 'р</td>' +
                                        '<td class="distance">' + (path.getLength() / 1000).toFixed(1) + 'км</td>' +
                                            '</tr>';

                html = html + row;
                $('#routesTable tbody').html(html);

                i++;
            });
        },
        function (error) {
            alert('Возникла ошибка: ' + error.message);
        }
    );
}

function prepareDataToSent() {
    var tripData = {
        isDriver: $('#chbxDriver').checked,
        isFreeDriver: false,
        points: JSON.stringify(points)
    };

    return tripData;
}

function sendData() {
    $.ajax({
        type: "POST",
        url: document.SaveTripUrl,
        data: prepareDataToSent(),
        dataType: "json",
        traditional: true
    });
}