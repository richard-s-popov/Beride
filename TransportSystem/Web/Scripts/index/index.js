$(document).ready(function () {
    $('#chbxDriver').attr('checked', true);

    $('#startPoint').focus();
    
    $('.add-diapason-button').unbind().bind('click', function () {
        $('#additionalDate').slideDown(400);
        $('#dateAt').attr('placeholder', 'С какого');
        $('.date-hider').show();
        $(this).hide();
    });

    $('.date-hider').unbind().bind('click', function () {
        $('#additionalDate').slideUp(300);
        $('#dateAt').attr('placeholder', 'Дата');
        $(this).hide();
        $('.add-diapason-button').show();
    });
    
    $('#dateAt').datepicker($.datepicker.regional["ru"]);
    $('#dateAt').parent().click(function () {
        $('#dateAt').focus();
    });

    $('#dateTo').datepicker($.datepicker.regional["ru"]);
    $('#dateTo').parent().click(function () {
        $('#dateTo').focus();
    });

    // Восстанавливаем атрибуты с данными, если они не пустые
    Renewal();

    // Биндим автокомплит к инпутам с калссом location
    bindAutocomplate();
    
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

        window.location = document.SearchUrl
            + '?search=' + $(".user-type input[name='search']:checked").val()
            + '&startPointGid=' + $('#startPoint').attr('gid')
            + '&endPointGid=' + $('#endPoint').attr('gid')
            + '&dateAt=' + $('#dateAt').attr('date')
            + '&dateTo=' + $('#dateTo').attr('date')
            + '&startPointFullName=' + $('#startPoint').val()
            + '&endPointFullName=' + $('#endPoint').val();
    });
});

function Renewal() {
    if ($('#hdnStartPoint').val() != '') {
        $('#startPoint').attr('gid', $('#hdnStartPoint').val());
    }
    
    if ($('#hdnEndPoint').val() != '') {
        $('#endPoint').attr('gid', $('#hdnEndPoint').val());
    }
    
    if ($('#hdnDateAt').val() != '') {
        $('#dateAt').attr('date', $('#hdnDateAt').val());
    }
    
    if ($('#hdnDateTo').val() != '') {
        $('#dateTo').attr('date', $('#hdnDateTo').val());
    }
}