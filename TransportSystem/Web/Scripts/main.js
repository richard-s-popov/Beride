$(document).ready(function () {
    $.datepicker.regional['ru'] = { clearText: 'Очистить', clearStatus: '',
        minDate: "0",
        closeText: 'Закрыть', closeStatus: '',
        prevText: '<Пред', prevStatus: '',
        nextText: 'След>', nextStatus: '',
        currentText: 'Сегодня', currentStatus: '',
        monthNames: ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь',
            'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'],
        monthNamesShort: ['Янв', 'Фев', 'Мар', 'Апр', 'Май', 'Июн',
            'Июл', 'Авг', 'Сен', 'Окт', 'Ноя', 'Дек'],
        monthStatus: '', yearStatus: '',
        weekHeader: 'Не', weekStatus: '',
        dayNames: ['воскресенье', 'понедельник', 'вторник', 'среда', 'четверг', 'пятница', 'суббота'],
        dayNamesShort: ['вск', 'пнд', 'втр', 'срд', 'чтв', 'птн', 'сбт'],
        dayNamesMin: ['Вс', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'],
        dayStatus: 'DD', dateStatus: 'D, M d',
        dateFormat: 'dd.mm.yy', firstDay: 1,
        initStatus: '', isRTL: false,
        onSelect: function () {
            var day = $(this).datepicker('getDate').getDate();
            var month = $(this).datepicker('getDate').getMonth() + 1;
            var year = $(this).datepicker('getDate').getFullYear();
            var fullDate = month + "." + day + "." + year;
            $(this).attr('date', fullDate);
        }
    };

    $('.registration-link').click(function() {
        setTimeout(function() {
            $('#loginContainer').hide();
            $('.message').hide();
            $('#registrationContainer').show();
            $.fancybox.resize();
        }, 100);
    });

    $('.login-link').click(function() {
        $('#loginContainer').show();
        $('#registrationContainer').hide();
        $.fancybox.resize();
    });
});


function bindAutocomplate() {
    $('.input.location').autocomplete({
        source: function (request, response) {
            $.ajax({
                url: document.GetPlacesUrl,
                dataType: "json",
                // параметры запроса, передаваемые на сервер (последний - подстрока для поиска):
                data: {
                    geoName: request.term
                },
                // обработка успешного выполнения запроса
                success: function (data) {
                    var json = JSON.parse(data);
                    // приведем полученные данные к необходимому формату и передадим в предоставленную функцию response
                    response($.map(json.predictions, function (item) {
                        return {
                            label: item.description,
                            value: item.description,
                            id: item.id,
                            shortName: item.terms[0].value
                        };
                    }));
                }
            });
        },
        minLength: 3,
        select: function (event, ui) {
            $(this).attr('gid', ui.item.id);
            $(this).attr('shortName', ui.item.shortName);
            $(this).val(ui.item.value);
            $(this).trigger('change');
        },
        close: function (event, ui) {
            if (!$(this).attr('gid')) {
                $(this).val('');
            }
        },
        autoFocus: true,
        appendTo: "#results",
        open: function () {
            $(this).removeAttr('gid');
            $(this).removeAttr('shortName');

            var position = $(this).parent().position(),
                left = position.left, top = position.top;

            var width = $(this).parent().width();
            $("#results > ul").css('width', width - 3);
            $("#results > ul").css('font-size', '13px');
            $("#results > ul").css({
                left: (left + 10) + "px",
                top: (top + 40) + "px"
            });
        }
    });
}