$(document).ready(function () {
    $('#chbxDriver').attr('checked', true);
    
    $('.input-text').bind('click', function () {
        $(this).children('.input').focus();
    });
    
    $('.add-diapason-button').unbind().bind('click', function () {
        $('#additionalDate').show(400);
    });

    $('.date-hider').unbind().bind('click', function () {
        $('#additionalDate').hide(300);
    });
    
    $('#dateAt').datepicker($.datepicker.regional["ru"]);
    $('#dateAt').parent().click(function () {
        $('#dateAt').focus();
    });

    $('#dateTo').datepicker($.datepicker.regional["ru"]);
    $('#dateTo').parent().click(function () {
        $('#dateTo').focus();
    });

    bindAutocomplate();
    
    $('#search').bind('click', function () {
        window.location = document.SearchUrl
            + '?search=' + $(".user-type input[name='search']:checked").val()
            + '&startPointGid=' + $('#startPoint').attr('gid')
            + '&endPointGid=' + $('#endPoint').attr('gid')
            + '&dateAt=' + $('#dateAt').attr('date')
            + '&dateTo=' + $('#dateTo').attr('date');
    });

    $('#signIn').fancybox({
        showCloseButton: false,
        scrolling: 'no',
        onClosed: function () {
            $('.message').hide();
            $('#loginContainer').show();
            $('#registrationContainer').hide();
        }
    });
});