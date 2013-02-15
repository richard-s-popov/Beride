var countPoint = 0;
var oldRoute;
var points = [];
var betweenCount = 0;
var firstStepForm;
var secondStepForm;
var createAfterLogin = false;
var isDriver = true;

$(document).ready(function () {
    $(".user-type input[name='type']").change(function() {
        var selectedRadio = $(".user-type input[name='type']:checked").val();

        switch (selectedRadio) {
            case "driver":
                if (betweenCount < 5) {
                    $('#addPoints').slideDown(100);
                }
                $('#freePlaces').show();
                $('#needPlaces').hide();
                $('div[input-type="between-point"]').show();

                $('#showDateTo').hide();
                $('#dateToContainer').hide();
                $('#dateAt').attr('placeholder', 'Дата');
                $('#hideDateTo').click();
                
                isDriver = true;
                buildRoute();

                break;
            case "passenger":
                $('#addPoints').slideUp(100);
                $('div[input-type="between-point"]').hide();
                $('#freePlaces').hide();
                $('#needPlaces').show();
                
                $('#showDateTo').show();
                
                isDriver = false;
                buildRoute();

                break;
            default:
        }
    });
    
    jQuery("div#slider1").codaSlider();
    
    $('#dateAt').datepicker($.datepicker.regional["ru"]);
    $('#dateTo').datepicker($.datepicker.regional["ru"]);

    $('#showDateTo').click(function() {
        $('#dateToContainer').show(200);
        $('#dateAt').attr('placeholder', 'С какого');
        $(this).parent().hide();
    });
    
    $('#hideDateTo').click(function () {
        $('#dateToContainer').hide();
        $('#dateAt').attr('placeholder', 'Дата');
        $('#showDateTo').parent().show();
        $('.error.dates', firstStepForm).hide();
    });

    $('#nextStep').click(function () {
        firstStepForm = $('.panel.first-step');
        secondStepForm = $('.panel.second-step');

        jVal.errors = false;
        jVal.Type();
        jVal.Route();
        jVal.Dates();
        jVal.NextStep();
    });

    $('#prevStep').click(function() {
        $('#stripNavL0').children('a').trigger('click');
    });

    $('#createThis').click(function() {
        jVal.errors = false;
        jVal.Type();
        jVal.Route();
        jVal.Dates();
        jVal.sendIt();
    });

    $('#addPoints').bind('click', function () {
        var html = '<div class="input-text" input-type="between-point" id="point-' + countPoint + '">' +
            '<input class="input between location" type="text" placeholder="Через" />' +
                '<img src="../Content/img/map_point.png" />' +
                    '<img src="../Content/img/cross.png" class="delete-button" />' +
                        '</div>';
        $('#pointContainer .input-text:last').after(html);
        countPoint++;
        betweenCount++;
        
        if (betweenCount == 5) {
            $('#addPoints').hide(200);
        }

        bindAutocomplate();
    });

    $('.delete-button').live('click', function () {
        $(this).parent().remove();
        betweenCount--;
        
        if (betweenCount < 5) {
            $('#addPoints').show(200);
        }

        points = [];
        points = [{ FullName: $.trim($('#startPoint').val()), ShortName: $('#startPoint').attr('ShortName'), Gid: $('#startPoint').attr('gid')}];

        if ($('.input.location.between').size() > 0) {
            $('.input.location.between').each(function () {
                points.push({ FullName: $.trim($(this).val()), ShortName: $(this).attr('ShortName'), Gid: $(this).attr('gid') });
            });
        }

        points.push({ FullName: $.trim($('#endPoint').val()), ShortName: $('#endPoint').attr('ShortName'), Gid: $('#endPoint').attr('gid') });

        var isEmpty = false;
        for (var i = 0; i < points.length; i++) {
            if (points[i].FullName == '') {
                isEmpty = true;
            }
        }

        if (!isEmpty) {
            buildRoute();
        }
    });

    $('.input.location').live('change', function () {
        // Проверка на пустые поля
        if ($.trim($('#startPoint').val()) != '' && $.trim($('#endPoint').val()) != '') {
            points = [];
            points = [{ FullName: $.trim($('#startPoint').val()), ShortName: $('#startPoint').attr('ShortName'), Gid: $('#startPoint').attr('gid')}];

            // Проверка на пустые поля промужуточных точек
            if ($('.input.location.between').size() > 0) {
                var isEmpty = false;
                $('.input.location.between').each(function () {
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
    
    bindAutocomplate();
});

function buildRoute() {
    var pointsForRoute = [];
    
    if (points.length == 0) {
        return;
    }
    
    if (isDriver) {
        $.each(points, function (index, point) {
            pointsForRoute.push(point.FullName);
        });
    } else {
        pointsForRoute.push(points[0].FullName);
        pointsForRoute.push(points[points.length - 1].FullName);
    }

    // Создаем маршрут
    window.ymaps.route(pointsForRoute, { mapStateAutoApply: true })
        .then(function (route) {
            if (oldRoute) {
                // удаляем старый, если есть таковой
                myMap.geoObjects.remove(oldRoute);
            }
            // отображаем новый
            myMap.geoObjects.add(route);
            oldRoute = route;

            var i = 0;
            var allPath = 0;
            $('#routesList').html('');
            route.getPaths().each(function (path) {
                var html = $('#routesList').html();
                var row = '<div class="route">' +
                    '<span class="from">' + points[i].ShortName + '</span>' +
                        '<span class="arrow">&rarr;</span>' +
                            '<span class="to">' + points[i + 1].ShortName + '</span>' +
                                '<span class="equal">=</span>' +
                                    '<span class="cost"><input class="cost-spinner" name="value" value="' + (path.getLength() / 1000 * 1.3).toFixed() + '" /></span>' +
                                        '<span class="distance">' + (path.getLength() / 1000).toFixed(1) + 'км</span>' +
                                            '</div>';

                html = html + row;
                $('#routesList').html(html);

                i++;
                allPath += path.getLength() / 1000;
            });
            
            if (i > 1) {
                var mainRow = '<div class="route main">' +
                    '<span class="from">' + points[0].ShortName + '</span>' +
                        '<span class="arrow">&rarr;</span>' +
                            '<span class="to">' + points[points.length - 1].ShortName + '</span>' +
                                '<span class="equal">=</span>' +
                                    '<span id="summCost" class="cost"></span>' +
                                        '<span class="distance">' + allPath.toFixed(1) + 'км</span>' +
                                            '</div>';

                $('#routesList').html($('#routesList').html() + mainRow);
            }
            
            $('.cost-spinner').spinner({
                spin: calcSummCost,
                change: calcSummCost
            });

            calcSummCost();
            
            if (!isDriver) {
                $('.cost').hide();
                $('#costTitle').hide();
            } else {
                $('#costTitle').show();
            }
        },
        function (error) {
            alert('Возникла ошибка: ' + error.message);
        }
    );
}

function calcSummCost() {
    var summCost = 0;
    $('.cost-spinner').each(function () {
        summCost += parseInt($(this).val());
    });
    $('#summCost').html(summCost + ' руб');
}

function prepareDataToSent() {
    var costPerRoute = '';

    $('.cost-spinner').each(function() {
        costPerRoute += $(this).val() + ';';
    });

    costPerRoute = costPerRoute.substring(0, costPerRoute.length - 1);

    var tripData = {
        isDriver: $(".user-type input[name='type']:checked").val() == 'driver',
        isFreeDriver: false,
        points: JSON.stringify(points),
        dateAt: $('#dateAt').attr('date2'),
        dateTo: $('#dateTo').attr('date2'),
        SeatsNumber: $('#numberPlaces :selected').text(),
        costPerRoute: costPerRoute
    };

    return tripData;
}

function sendData() {
    $.getJSON(document.IsAuthenticatedUrl, function (result) {
        if (result) {
            $.ajax({
                type: "POST",
                url: document.SaveTripUrl,
                data: prepareDataToSent(),
                dataType: "json",
                traditional: true
            });
        } else {
            $('#signIn').click();
            createAfterLogin = true;
        }
    });
}

var jVal = {
    'Type': function () {
        var error = $('.error.who-im', firstStepForm);
        
        if ($(".user-type input[name='type']:checked").length == 0) {
            jVal.errors = true;
            error.show();
        } else {
            error.hide();
        }
    },
    'Route': function () {
        var elements = $('.input.location', firstStepForm);
        var error = $('.error.route-points', firstStepForm);
        var thisError = false;

        elements.each(function() {
            if ($(this).val() == '' || $(this).attr('gid') == '' || $(this).attr('shortname') == '') {
                jVal.errors = true;
                thisError = true;
                $(this).parent().css('border', '1px solid rgb(226, 79, 79)');
            } else {
                $(this).parent().css('border', '1px solid #CCCCCC');
            }
        });
        
        if (thisError) {
            error.show();
        } else {
            error.hide();
        }
    },
    'Dates': function () {
        var elements = $('.input.date', firstStepForm);
        var error = $('.error.dates', firstStepForm);
        var thisError = false;

        elements.each(function () {
            if ($(this).val() == '' && $(this).is(':visible')) {
                jVal.errors = true;
                thisError = true;
                error.html('Выберите дату');
                $(this).parent().css('border', '1px solid rgb(226, 79, 79)');
            } else if ($(this).datepicker("getDate") < (new Date()).setDate(new Date().getDate() - 1) && $(this).parent().parent().parent().is(':visible')) {
                jVal.errors = true;
                thisError = true;
                error.html('Вы ввели прошедшую дату');
                $(this).parent().css('border', '1px solid rgb(226, 79, 79)');
            } else {
                $(this).parent().css('border', '1px solid #CCCCCC');
            }
        });
        
        var dateAt = $('#dateAt', firstStepForm).datepicker("getDate");
        var dateTo = $('#dateTo', firstStepForm).datepicker("getDate");
        
        if (dateAt >= dateTo
            && $('#dateTo', firstStepForm).parent().parent().parent().is(':visible') && !thisError) {
            
            jVal.errors = true;
            thisError = true;
            error.html('Дата "С какого" больше даты "По какое"');
            $('#dateAt', firstStepForm).parent().css('border', '1px solid rgb(226, 79, 79)');
            $('#dateTo', firstStepForm).parent().css('border', '1px solid rgb(226, 79, 79)');
        }

        // Максимальный диапазон дат 7 дней
        if (dateTo != null && dateAt < dateTo.setDate(dateTo.getDate() - 7)
            && $('#dateTo', firstStepForm).parent().parent().parent().is(':visible') && !thisError) {
 
            jVal.errors = true;
            thisError = true;
            error.html('Максимальный период 7 дней');
            $('#dateAt', firstStepForm).parent().css('border', '1px solid rgb(226, 79, 79)');
            $('#dateTo', firstStepForm).parent().css('border', '1px solid rgb(226, 79, 79)');
        }
        
        if (thisError) {
            error.show();
        } else {
            error.hide();
        }
    },
    'NextStep': function () {
        if (!jVal.errors) {
            $('#stripNavR0').children('a').trigger('click');
        }
    },
    'sendIt': function () {
        if (!jVal.errors) {
            sendData();
        }
    }
};