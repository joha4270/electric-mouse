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

        $('#modal_content').append('<p><b>RouteSection</b>: ' + result['section'] + '</p>');
        $('#modal_content').append('<p><b>RouteDifficulty</b>: ' + result['difficulty'] + '</p>');
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