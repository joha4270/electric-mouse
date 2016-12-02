$(".button-collapse").sideNav();
$(document).ready(function(){
    $('select').material_select();

    $('.datepicker').pickadate({
        selectMonths: true, // Creates a dropdown to control month
        selectYears: 15, // Creates a dropdown of 15 years to control year
        format: "dd-mm-yyyy",
        onStart: function()
        {
            var date = new Date();
            this.set('select', [date.getFullYear(), date.getMonth(), date.getDate()]);
        }
    });
});

function viewRoute(route_id)
{
	$("#loading").show();

	$.get("/Route/Details/" + route_id, function (data) {
    	$(".modal-content").html(data);
		$("#loading").hide();
	}).done(function () {

		history.pushState({
			 url: window.URL + "",
			 modalclose:true
		}, "Details", "/Route/Details/" + route_id);

		$('#modal1').openModal({
			complete: function () { history.back(-1); }
		});
  });
}

function togglemore()
{
    var element = $('#long_content');
    var size = element.css("max-height");
    if (size === "300px") {
        element.css('max-height', "3000px")
    } else {
        element.css('max-height', "300px")
    }
}

function langUpdate(id)
{
    $("#lang_id").val(id);
    $("#lang_id").closest('form').submit();
}

function initRouteTable()
{
    table = $('.table').DataTable({
        info: false,
        searching: true,
        lengthChange: false,
        pageLength: 50,
        paging: false,
        responsive: true,
        columnDefs: [
            { "searchable": false, "targets": 2},
            { "searchable": false, "targets": 3}
        ]
    });
}

function initDefaultTable()
{
	table = $('.table').DataTable({
		info: false,
		searching: false,
		lengthChange: false,
		pageLength: 50,
		paging: false,
		responsive: true,
		ordering: false
	});
}