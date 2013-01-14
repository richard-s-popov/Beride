$(document).ready(function () {
	$('#chbxFindDriver').attr('checked', true);
	
	$('.left-button.m-lr-10.driver > font').click(function () {
		$('#chbxFindDriver').parent().children('span').mousedown();
		$('#chbxFindDriver').parent().children('span').mouseup();
	});
	
	$('.right-button.m-lr-10.passenger > font').click(function () {
		$('#chbxFindPassenger').parent().children('span').mousedown();
		$('#chbxFindPassenger').parent().children('span').mouseup();
	});
	
	$('.add-diapason-button').unbind().bind('click', function () {
		$('#additionalDate').show(400);
	});
	
	$('.date-hider').unbind().bind('click', function () {
		$('#additionalDate').hide(300);
	});
	
	
});