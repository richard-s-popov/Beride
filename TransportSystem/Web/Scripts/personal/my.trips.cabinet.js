$(document).ready(function() {
    $('.trips .trip').unbind().bind('click', function () {
        $('.trips .trip').removeClass('selected');
        $(this).addClass('selected');
        
        $.fancybox.showActivity();

        $.ajax({
            type: "POST",
            url: document.GetRequestsByTripUrl,
            data: {
                tripId: $(this).attr('trip-id')
            },
            success: function (data) {
                $('.requests').html(data);
                $.fancybox.hideActivity();
            }
        });
    });
    
    $('.trips .trip:first').addClass('selected');
});