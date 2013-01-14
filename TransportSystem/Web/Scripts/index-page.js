$(document).ready(function () {
	$('#chbxDriver').attr('checked', true);
	
	$('.left-button.m-lr-10.driver > font').click(function () {
		$('#chbxDriver').parent().children('span').mousedown();
		$('#chbxDriver').parent().children('span').mouseup();
	});
	
	$('.right-button.m-lr-10.passenger > font').click(function () {
		$('#chbxPassenger').parent().children('span').mousedown();
		$('#chbxPassenger').parent().children('span').mouseup();
	});
	
	$('.add-diapason-button').unbind().bind('click', function () {
		$('#additionalDate').show(400);
	});
	
	$('.date-hider').unbind().bind('click', function () {
		$('#additionalDate').hide(300);
	});
	
	
});