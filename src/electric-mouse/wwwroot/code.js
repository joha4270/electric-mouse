$(".button-collapse").sideNav();
$(document).ready(function(){
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
});

$('#search').keyup(function(){
      table.search($(this).val()).draw();
});

$('.nav-search i').click(function() {toggleSearchMenu();});

function toggleSearchMenu()
{
  if ($(".nav-content").is(":visible")) {
    $(".nav-content").hide();
    $(".nav-search").show();
    $("#search").focus().select();
  } else {
    $("#search").val('').blur();
    $(".nav-content").show();
    $(".nav-search").hide();
    table.search("").draw();
  }

}

$('tbody tr').click(function () {
  id = $(this).children('.app_id').text();
  viewRoute(id);
});

function viewRoute(route_id)
{
  $('.modal-content h4').html('Loading');
  $('#modal_content').html('');
  /*$.getJSON("api.php?route=" + route_id, function(result){
        var d = new Date(result['date']); // + ' - ' + d.getDate() + '/' + (d.getMonth()+1) + '/' + d.getFullYear()

        $('.modal-content h4').html('Route #' + route_id);

        $('#modal_content').append('<p><b>Section</b>: ' + result['section'] + '</p>');
        $('#modal_content').append('<p><b>Difficulty</b>: ' + result['difficulty'] + '</p>');
        $('#modal_content').append('<p><b>Grip</b>: ' + result['grip'] + '</p>');
        $('#modal_content').append('<p><b>Date</b>: ' + d.getDate() + '/' + (d.getMonth()+1) + '/' + d.getFullYear() + '</p>');
        // Users
        users = "";
          seperator = ", ";
          users += '<p><b>Users</b>: ';
          $.each(result['users'], function(i, field) {
            if (i >= $(result['users']).length - 1) {seperator = "";}
            users += '<a href="#">' + result['users'][i]['first_name'];
            if (!$.isEmptyObject(result['users'][i]['last_name'])) { users += ' ' + result['users'][i]['last_name']; }
            users += '</a>';
            users += seperator;
          });
          users += '</p>';
        $('#modal_content').append(users);
        $('#modal_content').append('<p><b>Note</b>: ' + result['note'] + '</p>');
    });*/
        $('#modal_content').html('API not implemented yet');
  $('#modal1').openModal();
}