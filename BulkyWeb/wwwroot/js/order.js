$(document).ready(function () {
    loadDataTable();
});

var dataTable;
function loadDataTable(){
    dataTable = $('#tblData').DataTable({
        "ajax": { 
            url: '/admin/order/getall',
            dataSrc: 'data' // Ensure 'data' property is being accessed correctly
        },
        "columns": [
            //column count here must match with Index page columns
            { data: 'id', "width": "10%" },
            { data: 'name', "width": "15%" },
            { data: 'phoneNumber', "width": "20%" },
            { data: 'applicationUser.email', "width": "15%" },
            { data: 'orderStatus', "width": "10%" },
            { data: 'orderTotal', "width": "10%" },
            {
                data: 'id',
                render: function (data) {
                    return `<div class="w-75 btn-group" role="group">
                              <a href="/admin/order/details?orderId=${data}" class="btn btn-primary mx-1"><i class="bi bi-pencil-square"></i></a>
                            </div>`
                },
                "width": "25%"
            }
        ]
    });
}
$.ajax({
    url: '/admin/product/getall',
    method: 'GET',
    success: function (response) {
        console.log(response);  // Log the response to ensure data structure is correct
    }
});

